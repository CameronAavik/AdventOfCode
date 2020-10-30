module Year2016Day24

open AdventOfCode.FSharp.Common
open GraphFS.Graph
open GraphFS.Algorithms.ShortestPath

type Space = Wall | Open | Location of int
let parseSpace c =
    match c with
    | '#' -> Wall
    | '.' -> Open
    | c when c >= '0' && c <= '9' -> Location (int c - int '0')
    | c -> failwithf "Invalid Char: %A" c

let isOpen space = match space with | Wall -> false | _ -> true

let parse (filename : string) =
    parseEachLine id filename
    |> Seq.map (Seq.map parseSpace >> Seq.toArray)
    |> Seq.toArray

let getMazeGraph (grid : Space [] []) =
    let height = grid.Length
    let width = grid.[0].Length
    seq {
        for y = 0 to (height - 1) do
            for x = 0 to (width - 1) do
                if x > 0 then (x, y), (x - 1, y)
                if x < (width - 1) then (x, y), (x + 1, y)
                if y > 0 then (x, y), (x, y - 1)
                if y < (height - 1) then (x, y), (x, y + 1)
    }
    |> Seq.filter (fun ((x1, y1), (x2, y2)) -> 
        let from = grid.[y1].[x1]
        let to' = grid.[y2].[x2]
        isOpen from && isOpen to')
    |> Graph.fromEdges

let getLocations grid =
    grid
    |> Array.mapi (fun y row ->
        row
        |> Array.mapi (fun x cell -> match cell with | Location l -> Some (l, (x, y)) | _ -> None)
        |> Array.choose id)
    |> Array.concat
    |> Map.ofArray

let getShortestPathGraph mazeGraph (locations : Map<int, int * int>) =
    let locationCount = locations.Count

    seq {
        for from = 0 to (locationCount - 1) do
            for to' = (from + 1) to (locationCount - 1) do
                let fromLoc = locations.[from]
                let toLoc = locations.[to']
                let shortestPath = (dijkstra fromLoc toLoc mazeGraph).Value
                (from, to'), shortestPath
                (to', from), shortestPath } |> Graph.fromEdgesWithData

let solvePart1 (grid : Space [] []) =
    let G = getMazeGraph grid
    let locations = getLocations grid
    let shortestPathGraph = getShortestPathGraph G locations

    let rec shortestPath atNode choices =
        if Set.isEmpty choices then 0
        else
            choices
            |> Seq.map (fun c -> 
                let dist = Graph.getEdgeData (atNode, c) shortestPathGraph
                dist + shortestPath c (Set.remove c choices))
            |> Seq.min

    let nextChoices = [| 1 .. (locations.Count - 1) |] |> Set.ofArray
    shortestPath 0 nextChoices

let solvePart2 grid = 
    let G = getMazeGraph grid
    let locations = getLocations grid
    let shortestPathGraph = getShortestPathGraph G locations

    let rec shortestPath atNode choices =
        if Set.isEmpty choices then Graph.getEdgeData (atNode, 0) shortestPathGraph
        else
            choices
            |> Seq.map (fun c -> 
                let dist = Graph.getEdgeData (atNode, c) shortestPathGraph
                dist + shortestPath c (Set.remove c choices))
            |> Seq.min

    let nextChoices = [| 1 .. (locations.Count - 1) |] |> Set.ofArray
    shortestPath 0 nextChoices

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }