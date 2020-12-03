module Year2015Day16

open AdventOfCode.FSharp.Common

type Sue = { Number : int; Items : Map<string, int> }

let parseSue line =
    let toks = splitBy " " id line
    let num = toks.[1].TrimEnd(':') |> int

    let items =
        toks
        |> Seq.skip 2
        |> Seq.chunkBySize 2
        |> Seq.map (fun (entry : string []) ->
            let item = entry.[0].TrimEnd(':')
            let amount = int (entry.[1].TrimEnd(','))
            item, amount)
        |> Map.ofSeq

    { Number = num; Items = items }

let parse = parseEachLine parseSue

let amounts =
    [
        3, "children"
        7, "cats"
        2, "samoyeds"
        3, "pomeranians"
        0, "akitas"
        0, "vizslas"
        5, "goldfish"
        3, "trees"
        2, "cars"
        1, "perfumes"
    ]

let scoreSue1 sue =
    let tryGet item =
        if Map.containsKey item sue.Items then Some sue.Items.[item]
        else None

    amounts
    |> Seq.exists (fun (a, i) ->
        match tryGet i with
        | Some x -> a <> x
        | None -> false)
    |> not

let solvePart1 sues =
    sues
    |> Seq.filter scoreSue1
    |> Seq.head
    |> (fun s -> s.Number)

let scoreSue2 sue =
    let tryGet item =
        if Map.containsKey item sue.Items then Some sue.Items.[item]
        else None

    amounts
    |> Seq.exists (fun (a, i) ->
        match tryGet i with
        | Some x -> 
            match i with
            | "cats" | "trees" -> x <= a
            | "pomeranians" | "goldfish" -> x >= a
            | _ -> x <> a
        | None -> false)
    |> not

let solvePart2 sues =
    sues
    |> Seq.filter scoreSue2
    |> Seq.head
    |> (fun s -> s.Number)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }