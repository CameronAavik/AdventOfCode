module Year2016Day13

open AdventOfCode.FSharp.Common
open GraphFS.Core.VertexSet
open GraphFS.Core.EdgeSet
open System.Numerics
open GraphFS.Graph
open GraphFS.Algorithms.ShortestPath

let parse = parseFirstLine asInt >> uint64

let isOpenSpace input (x, y) =
    let z = x*x + 3UL*x + 2UL*x*y + y + y*y
    let withInput = z + input
    let bits = BitOperations.PopCount withInput
    bits % 2 = 0

let manhattan (p1 : uint64 * uint64) (p2 : uint64 * uint64) =
    let (x1, y1), (x2, y2) = p1, p2
    let xDiff, yDiff = int64 x2 - int64 x1, int64 y2 - int64 y1
    abs xDiff + abs yDiff |> int

let getGraph input =
    let vertexSet = { new IVertexSet<uint64 * uint64> with member __.HasVert v = isOpenSpace input v }

    let edgeSet = 
        { new IEdgeSet<uint64 * uint64> with
            member __.HasEdge edge =
                let p1, p2 = edge
                manhattan p1 p2 = 1
            member __.Neighbours p =
                let x, y = p
                seq {
                    if x <> 0UL then (x-1UL, y)
                    if y <> 0UL then (x, y-1UL)
                    (x+1UL, y)
                    (x, y+1UL) } 
                    |> Seq.filter (isOpenSpace input) }

    Graph.fromSets vertexSet edgeSet

let solvePart1 input =
    let graph = getGraph input
    let shortestPath = astar (1UL, 1UL) (31UL, 39UL) manhattan graph
    shortestPath.Value

let solvePart2 input = 
    let graph = getGraph input
    let source = (1UL, 1UL)

    let rec bfs steps seen fringe =
        if Set.isEmpty fringe || steps < 0 then seen
        else
            let seen = Set.union seen fringe
            let fringe' = 
                fringe
                |> Seq.collect (fun vertex -> Graph.neighbours vertex graph)
                |> Set.ofSeq
            Set.difference fringe' seen
            |> bfs (steps - 1) seen
    
    (set [source] |> bfs 50 Set.empty).Count

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }