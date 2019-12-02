module Year2018Day05

open CameronAavik.AdventOfCode.Common

let remainingPolymer =
    let processUnit polymer ch =
        match polymer with
        | x :: xs when abs (ch - x) = 32 -> xs
        | xs -> ch :: xs
    Seq.fold processUnit []
let filterChars str ch = Seq.filter (fun c -> c <> ch && (c - 32) <> ch) str
let solvePart1 = Seq.map int >> remainingPolymer >> Seq.length
let solvePart2 str =
    let reducedStr = str |> Seq.map int |> remainingPolymer
    Seq.init 26 ((+)65 >> filterChars reducedStr >> remainingPolymer >> Seq.length) |> Seq.min
let solver = {parse = parseFirstLine asString; part1 = solvePart1; part2 = solvePart2}