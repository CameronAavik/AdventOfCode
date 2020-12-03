module Year2019Day04

open AdventOfCode.FSharp.Common

let asRange (ints : int []) = ints.[0], ints.[1]
let parse = parseFirstLine (splitBy "-" asIntArray) >> asRange

let isMonotonic str = str |> Seq.pairwise |> Seq.exists (fun (a, b) -> a > b) |> not
let countChars str = str |> Seq.countBy id
let hasPair str = countChars str |> Seq.exists (fun (_, c) -> c >= 2)
let hasStrictPair str = countChars str |> Seq.exists (fun (_, c) -> c = 2)

let solvePart1 (low, high) =
    [low .. high]
    |> Seq.map string
    |> Seq.filter isMonotonic
    |> Seq.filter hasPair
    |> Seq.length
    
let solvePart2 (low, high) =
    [low .. high]
    |> Seq.map string
    |> Seq.filter isMonotonic
    |> Seq.filter hasStrictPair
    |> Seq.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }