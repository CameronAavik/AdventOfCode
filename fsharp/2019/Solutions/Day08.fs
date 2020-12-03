module Year2019Day08

open AdventOfCode.FSharp.Common

let solvePart1 input =
    let counts =
        input
        |> Array.chunkBySize (25 * 6)
        |> Array.map (Array.countBy id >> Map.ofArray)
        |> Array.minBy (Map.find '0')

    counts.['1'] * counts.['2']
    
let solvePart2 input = 
    let output =
        input
        |> Array.chunkBySize (25 * 6)
        |> Array.reduce (Array.map2 (fun p1 p2 -> if p1 = '2' then p2 else p1))
        |> Array.chunkBySize 25
        |> Array.map (System.String.Concat >> String.map (function '1' -> '█' | _ -> ' '))
        |> String.concat "\n"

    // just so it's nicer to read for the runner
    "\n" + output

let solver = { parse = parseFirstLine asCharArray; part1 = solvePart1; part2 = solvePart2 }