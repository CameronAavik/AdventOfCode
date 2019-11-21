module Year2016Day03

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine extractInts

let isValid sides =
    let sorted = sides |> Array.sort
    sorted.[0] + sorted.[1] > sorted.[2]

let solvePart1 lines =
    lines |> Seq.filter (isValid) |> Seq.length

let transpose (s : int [] seq) =
    let r = s |> Seq.toArray
    Seq.init 3 (fun i -> [| r.[0].[i]; r.[1].[i]; r.[2].[i] |])

let solvePart2 lines =  
    lines 
    |> Seq.chunkBySize 3
    |> Seq.map transpose
    |> Seq.concat
    |> Seq.filter (isValid)
    |> Seq.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }