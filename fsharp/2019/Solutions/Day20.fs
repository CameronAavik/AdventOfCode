module Year2019Day20

open AdventOfCode.FSharp.Common
open System.Collections.Generic

type Cell = Wall | Open | Outside of char
let parseCell =
    function
    | '#' -> Wall
    | '.' -> Open
    | c -> Outside c

type Maze =
    { Grid : Cell [] []
      Entrance : int * int
      Exit : int * int
      Portals: Map<int * int, int * int>;
      UnmatchedPortals : Map<string, int * int> }

    static member create grid = { Grid = grid; Entrance = (-1, -1); Exit = (-1, -1); Portals = Map.empty; UnmatchedPortals = Map.empty }

let portalNameLocations (x, y) =
    [|
        (x, y - 2), (x, y - 1)
        (x, y + 1), (x, y + 2)
        (x - 2, y), (x - 1, y)
        (x + 1, y), (x + 2, y)
    |]

let parseMaze (grid : Cell [] []) =
    let width = grid.[0].Length
    let height = grid.Length

    let cellAt (x, y) = grid.[y].[x]
    let tryGetPortalName =
        portalNameLocations
        >> Array.choose (fun (p1, p2) ->
            match cellAt p1, cellAt p2 with
            | Outside a, Outside b -> charsToStr [a; b] |> Some
            | _ -> None)
        >> Array.tryHead

    (Maze.create grid, seq { for y in 0 .. height - 1 do for x in 0 .. width - 1 -> (x, y) })
    ||> Seq.fold (fun m pos ->
        match cellAt pos with
        | Open ->
            match tryGetPortalName pos with
            | Some "AA" -> { m with Entrance = pos }
            | Some "ZZ" -> { m with Exit = pos }
            | Some s ->
                match Map.tryFind s m.UnmatchedPortals with
                | Some dest -> { m with Portals = m.Portals |> Map.add pos dest |> Map.add dest pos }
                | None -> { m with UnmatchedPortals = Map.add s pos m.UnmatchedPortals }
            | None -> m
        | _ -> m)

let parse = parseEachLine (Seq.map parseCell >> Seq.toArray) >> Seq.toArray >> parseMaze

let astar start dest heuristic getEdges =
    let seen = new HashSet<_>()
    let rec astar' fringe =
        if Set.isEmpty fringe then None
        else
            let (_, negDist : int, vertex) as minElem = Set.minElement fringe
            let fringe' = Set.remove minElem fringe
            let dist = -negDist
            if seen.Contains(vertex) then astar' fringe'
            elif vertex = dest then Some dist
            else
                seen.Add(vertex) |> ignore
                getEdges vertex
                |> Array.map (fun v ->
                    let dist = dist + 1
                    (heuristic v) + dist, -dist, v)
                |> Set.ofArray
                |> Set.union fringe'
                |> astar'
    astar' (Set.ofList [0, 0, start])

let bfs start dest getEdges = astar start dest (fun _ -> 0) getEdges

let neighbours (x, y) = [| (x + 1, y); (x - 1, y); (x, y + 1); (x, y - 1) |]

let solvePart1 maze =
    let cellAt (x, y) = maze.Grid.[y].[x]
    let getEdges pos =
        neighbours pos
        |> Array.choose (fun neighbour ->
            match cellAt neighbour with
            | Open -> Some neighbour
            | Outside _ -> Map.tryFind pos maze.Portals
            | Wall -> None)

    bfs maze.Entrance maze.Exit getEdges
    |> Option.get
    
let solvePart2 maze = 
    let width = maze.Grid.[0].Length
    let height = maze.Grid.Length
    let isOuterEdge (x, y) = 
        x <= 2 || x >= (width - 3) || y <= 2 || y >= (height - 3)

    let cellAt (x, y) = maze.Grid.[y].[x]
    
    let heuristic (level, _) = level
    let getEdges (level, pos) =
        neighbours pos
        |> Array.choose (fun neighbour ->
            match cellAt neighbour with
            | Open -> Some (level, neighbour)
            | Outside _ ->
                match Map.tryFind pos maze.Portals with
                | Some portalDest when isOuterEdge portalDest -> Some (level + 1, portalDest)
                | Some portalDest when level > 0 -> Some (level - 1, portalDest)
                | _ -> None
            | Wall -> None)

    astar (0, maze.Entrance) (0, maze.Exit) heuristic getEdges
    |> Option.get

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }