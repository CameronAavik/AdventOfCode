module Year2015Day09

open CameronAavik.AdventOfCode.Common
open GraphFS.Graph

type Edge =
    { P1 : string; P2 : string; Dist : int }

let parseEdge line =
    let toks = splitBy " " id line
    { P1 = toks.[0]; P2 = toks.[2]; Dist = int toks.[4]}

let parse = parseEachLine parseEdge

let solvePart1 input =
    let G =
        input
        |> Seq.map (fun e -> [(e.P1, e.P2), e.Dist; (e.P2, e.P1), e.Dist])
        |> Seq.collect id
        |> Graph.fromEdgesWithData

    let locations = Graph.verts G |> Set.ofSeq

    let rec getShortest current remainingLocs =
        if Set.isEmpty remainingLocs then 0
        else
            remainingLocs
            |> Seq.map (fun l ->
                let dist =  Graph.getEdgeData (current, l) G
                let remPath = getShortest l (Set.remove l remainingLocs)
                dist + remPath)
            |> Seq.min

    locations
    |> Seq.map (fun l -> getShortest l (Set.remove l locations))
    |> Seq.min

let solvePart2 input =
    let G =
        input
        |> Seq.map (fun e -> [(e.P1, e.P2), e.Dist; (e.P2, e.P1), e.Dist])
        |> Seq.collect id
        |> Graph.fromEdgesWithData

    let locations = Graph.verts G |> Set.ofSeq

    let rec getShortest current remainingLocs =
        if Set.isEmpty remainingLocs then 0
        else
            remainingLocs
            |> Seq.map (fun l ->
                let dist =  Graph.getEdgeData (current, l) G
                let remPath = getShortest l (Set.remove l remainingLocs)
                dist + remPath)
            |> Seq.max

    locations
    |> Seq.map (fun l -> getShortest l (Set.remove l locations))
    |> Seq.max

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }