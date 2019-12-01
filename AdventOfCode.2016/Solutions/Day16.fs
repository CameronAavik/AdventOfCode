module Year2016Day16

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine asString

let expand (str : string) =
    let a = str |> Seq.toArray
    let b = a |> Array.rev |> Array.map (function | '0' -> '1' | _ -> '0')
    let newBytes = Array.concat [a; [| '0' |]; b]
    newBytes |> charsToStr

let rec expandFully length (str : string) =
    if str.Length >= length then str.Substring(0, length)
    else expandFully length (expand str)

let rec checksum (str : string) =
    if str.Length % 2 = 0 then
        str
        |> Seq.chunkBySize 2
        |> Seq.map (fun i -> if i.[0] = i.[1] then '1' else '0')
        |> charsToStr
        |> checksum
    else str

let solve length input =
    expandFully length input
    |> checksum

let solvePart1 input = solve 272 input

let solvePart2 input = solve 35651584 input

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }