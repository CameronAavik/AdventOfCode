module Year2015Day13

open CameronAavik.AdventOfCode.Common
open GraphFS.Graph

type Action = { Person : string; IsGain : bool; Units : int; NextTo : string }

let asAction line =
    let toks = splitBy " " id line
    { Person = toks.[0]; IsGain = toks.[2] = "gain"; Units = int toks.[3]; NextTo = toks.[10].Trim('.')}

let parse = parseEachLine asAction

let solvePart1 input =
    let G = 
        input
        |> Seq.map (fun a -> (a.Person, a.NextTo), if a.IsGain then a.Units else -a.Units)
        |> Graph.fromEdgesWithData

    let people = Graph.verts G |> Set.ofSeq

    let rec findBestOrder first recent remaining =  
        if Set.isEmpty remaining then
            let h1 = Graph.getEdgeData (recent, first) G
            let h2 = Graph.getEdgeData (first, recent) G
            h1 + h2
        else
            remaining
            |> Seq.map (fun p ->
                let h1 = Graph.getEdgeData (recent, p) G
                let h2 = Graph.getEdgeData (p, recent) G
                let h3 = findBestOrder first p (Set.remove p remaining)
                h1 + h2 + h3)
            |> Seq.max

    people
    |> Seq.map (fun p -> findBestOrder p p (Set.remove p people))
    |> Seq.max

let solvePart2 input =
    let G = 
        input
        |> Seq.map (fun a -> (a.Person, a.NextTo), if a.IsGain then a.Units else -a.Units)
        |> Graph.fromEdgesWithData

    let people = Graph.verts G |> Set.ofSeq

    let edgesToAdd =
        people
        |> Seq.map (fun p -> [(p, "ME"), 0; ("ME", p), 0])
        |> Seq.collect id

    let G = Graph.addEdgesWithData edgesToAdd G

    let rec findBestOrder first recent remaining =  
        if Set.isEmpty remaining then
            let h1 = Graph.getEdgeData (recent, first) G
            let h2 = Graph.getEdgeData (first, recent) G
            h1 + h2
        else
            remaining
            |> Seq.map (fun p ->
                let h1 = Graph.getEdgeData (recent, p) G
                let h2 = Graph.getEdgeData (p, recent) G
                let h3 = findBestOrder first p (Set.remove p remaining)
                h1 + h2 + h3)
            |> Seq.max

    findBestOrder "ME" "ME" people

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }