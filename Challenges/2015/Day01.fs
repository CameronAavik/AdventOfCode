module Year2015Day01

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine asString

let solvePart1 input =
    input
    |> Seq.fold (fun i c -> 
        if c = '(' then i + 1
        else i - 1) 0

let solvePart2 input =
    input
    |> Seq.indexed
    |> Seq.mapFold (fun i (idx, c) -> 
        if c = '(' then (idx + 1, i + 1), i + 1
        else (idx + 1, i - 1), i - 1) 0
    |> fst
    |> Seq.find (fun (i, l) -> l < 0)
    |> fst

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }