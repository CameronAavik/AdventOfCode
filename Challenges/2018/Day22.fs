module Year2018Day22

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections
open System.IO

type Maze = {depth: int; tx: int; ty: int}
let getDepthAndTarget lines =
    let lineArr = lines |> Seq.toArray
    let depth = splitBy " " (fun p -> int p.[1]) lineArr.[0]
    let target = splitBy " " (fun p -> p.[1]) lineArr.[1]
    let tx, ty = splitBy "," (fun p -> int p.[0], int p.[1]) target
    {depth=depth; tx=tx; ty=ty}

let genGrid maze newWidth newHeight =
    let getErosionLevel prevValue (x, y, upperValue) =
        let geologicIndex = 
            match x, y with
            | _ when x = maze.tx && y = maze.ty -> 0
            | 0, _ -> y * 48271
            | _, 0 -> x * 16807
            | _ -> prevValue * upperValue
        (geologicIndex + maze.depth) % 20183

    let getErosionLevelRow (prevRow : int []) y =
        let firstCell = getErosionLevel 0 (0, y, 0)
        Array.init (newWidth - 1) (fun x -> (x + 1, y, prevRow.[x+1]))
        |> Array.scan getErosionLevel firstCell

    let firstRow = Array.init newWidth (fun x -> getErosionLevel 0 (x, 0, 0))
    Array.init (newHeight - 1) ((+)1)
    |> Array.scan getErosionLevelRow firstRow
    |> array2D

let solvePart1 maze =
    genGrid maze maze.tx maze.ty 
    |> Seq.cast<int> 
    |> Seq.sumBy (fun i -> i % 3)

let rec getAndUpdateFromGrid maze (grid : int [,]) x y =
    let inXBounds = x < Array2D.length2 grid
    let inYBounds = y < Array2D.length1 grid
    if inXBounds && inYBounds then
        grid.[y, x], grid
    else
        let newWidth = Array2D.length2 grid + (if inXBounds then 0 else 30)
        let newHeight = Array2D.length1 grid + (if inYBounds then 0 else 30)
        let newGrid = genGrid maze newWidth newHeight
        getAndUpdateFromGrid maze newGrid x y

type RegionType = Rocky | Narrow | Wet
let erosionToRegionType erosionlevel = 
    match erosionlevel % 3 with
    | 0 -> Rocky
    | 1 -> Narrow
    | _ -> Wet

type Tool = Nothing | Gear | Torch
type Vertex = {x: int; y: int; tool: Tool}
type Edge = {dest: Vertex; weight: int}
let possibleEdges vertex = 
    [|
        {dest={vertex with x=vertex.x-1}; weight=1}
        {dest={vertex with x=vertex.x+1}; weight=1}
        {dest={vertex with y=vertex.y-1}; weight=1}
        {dest={vertex with y=vertex.y+1}; weight=1}
        {dest={vertex with tool=Nothing}; weight=7}
        {dest={vertex with tool=Gear}; weight=7}
        {dest={vertex with tool=Torch}; weight=7}
    |]

let canUseTool regionType tool =
    match regionType with
    | Rocky -> tool <> Nothing
    | Narrow -> tool <> Torch
    | Wet -> tool <> Gear

let getEdges maze grid vertex =
    let getRegionType grid edge =
        let erosionLevel, grid' = getAndUpdateFromGrid maze grid edge.dest.x edge.dest.y
        (edge, erosionToRegionType erosionLevel), grid'
    let edgeWithDestRegions, grid' =
        possibleEdges vertex
        |> Array.filter (fun v -> v.dest.x >= 0 && v.dest.y >= 0 && v.dest <> vertex)
        |> Array.mapFold getRegionType grid
    let filteredEdges =
        edgeWithDestRegions
        |> Array.filter (fun (edge, regionType) -> canUseTool regionType edge.dest.tool )
        |> Array.map fst
    filteredEdges, grid'

let heuristic src dst =
    abs (src.x - dst.x) + abs (src.y - dst.y) + (if src.tool <> dst.tool then 7 else 0)

let putEdgesOnQueue dst distance seen edges pQueue =
    let getQueueItem edge =
        let dist = distance + edge.weight
        let h = heuristic edge.dest dst
        (dist + h, dist, edge.dest)
    let unseenEdges =
        edges
        |> Array.filter (fun e -> Set.contains e.dest seen |> not)
        |> Array.map getQueueItem
    Array.fold (fun pQueue elem -> PriorityQueue.insert elem pQueue) pQueue unseenEdges

let shortestPathLength maze grid src dst =
    let rec shortestPath' grid seen pQueue =
        let (_, distance, v), remainingQueue = PriorityQueue.pop pQueue
        if v = dst then
            distance
        else
            if Set.contains v seen then
                shortestPath' grid seen remainingQueue
            else
                let edges, grid' = getEdges maze grid v
                let updatedQueue = putEdgesOnQueue dst distance seen edges pQueue
                shortestPath' grid' (Set.add v seen) updatedQueue
    let initQueue = PriorityQueue.empty false |> PriorityQueue.insert (0, 0, src)
    shortestPath' grid Set.empty initQueue
    
let solvePart2 maze =
    let grid = genGrid maze (maze.tx + 1) (maze.ty + 1)
    shortestPathLength maze grid {x=0; y=0; tool=Torch} {x=maze.tx; y=maze.ty; tool=Torch}

let solver = {parse = File.ReadLines >> getDepthAndTarget; part1 = solvePart1; part2 = solvePart2}