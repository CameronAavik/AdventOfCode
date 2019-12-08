module Year2019Day08

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine (Seq.map (fun c -> int c - int '0')) >> Seq.toArray

let solvePart1 input =
    let getAnswer (counts : Map<int, int>)= counts.[1] * counts.[2]

    input
    |> Array.chunkBySize (25 * 6)
    |> Array.map (Array.countBy id >> Map.ofArray)
    |> Array.minBy (Map.find 0)
    |> getAnswer
    
let solvePart2 input = 
    let combineLayers layer1 layer2 =
        Array.zip layer1 layer2
        |> Array.map (fun (p1, p2) -> if p1 = 2 then p2 else p1)

    let transparentLayer = Array.create (25 * 6) 2

    let output =
        input
        |> Array.chunkBySize (25 * 6)
        |> Array.fold combineLayers transparentLayer
        |> Array.map (fun i -> if i = 1 then '#' else ' ')
        |> Array.chunkBySize 25
        |> Array.map charsToStr
        |> String.concat "\n"

    // just so it's nicer to read for the runner
    "\n" + output

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }