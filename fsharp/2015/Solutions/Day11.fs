module Year2015Day11

open AdventOfCode.FSharp.Common

let bumpInvalidLetters (str : string) =
    str.Replace('i', 'j').Replace('o', 'p').Replace('l', 'm')

let parse = parseFirstLine (bumpInvalidLetters)

let nextChar c =
    match c with
    | 'z' -> 'a'
    | 'h' -> 'j'
    | 'n' -> 'p'
    | 'k' -> 'm'
    | c -> (int c) + 1 |> char

let rec nextStr (str : string) =
    let len = String.length str
    if len = 1 then string (nextChar str.[0])
    else
        match str.[len - 1] with
        | 'z' -> nextStr (str.Substring(0, len - 1)) + "a"
        | c -> str.Substring(0, len - 1) + (nextChar c |> string)

let isValid str =
    let hasTriple = str |> Seq.map int |> Seq.windowed 3 |> Seq.exists (fun w -> w.[1] = w.[0] + 1 && w.[2] = w.[1] + 1)
    let doubleIndexes =
        str
        |> Seq.pairwise
        |> Seq.indexed
        |> Seq.filter (fun (i, (a, b)) -> a = b)
        |> Seq.map fst
        |> Seq.sort
        |> Seq.toArray

    let hasDouble = 
        match doubleIndexes.Length with
        | 0 | 1 -> false
        | 2 -> doubleIndexes.[1] - doubleIndexes.[0] > 1
        | _ -> true

    hasTriple && hasDouble

let solvePart1 input =
    let rec findNextMatch str =
        let n = nextStr str
        if isValid n then n
        else findNextMatch n

    findNextMatch (bumpInvalidLetters input)

let solvePart2 input =
    let rec findNextMatch str =
        let n = nextStr str
        if isValid n then n
        else findNextMatch n

    findNextMatch (bumpInvalidLetters input)
    |> findNextMatch

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }