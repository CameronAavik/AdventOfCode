module Year2016Day25

open AdventOfCode.FSharp.Common

let parse = parseEachLine asString >> Seq.toArray

let solvePart1 (instrs : string []) =
    let val1 = (instrs.[1].Split " ").[1] |> int
    let val2 = (instrs.[2].Split " ").[1] |> int
    let minVal = val1 * val2
    let rec findVal val' =
        if val' > minVal then val' - minVal
        else findVal ((val' <<< 2) + 2)
    findVal 2

let solver = { parse = parse; part1 = solvePart1; part2 = (fun _ -> "Advent of Code Finished!") }