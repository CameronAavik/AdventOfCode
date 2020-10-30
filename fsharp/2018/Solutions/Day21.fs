module Year2018Day21

open AdventOfCode.FSharp.Common
open System.IO

let getSeed =
    // after comparing many inputs, the only lines that was different was line 8 
    Seq.item 8 >> splitBy " " (fun p -> int p.[1])

let getNextTerminatingValue seed prev =
    let applyByte value =
        (+) value
        >> (&&&) 0xFFFFFF
        >> (*) 65899
        >> (&&&) 0xFFFFFF
    
    let b = prev ||| 0x10000
    [|0; 8; 16|]
    |> Array.map (fun shift -> b >>> shift &&& 0xFF)
    |> Array.fold applyByte seed

let solvePart1 seed = getNextTerminatingValue seed 0
let solvePart2 seed =
    let rec findLastUnique prev seen =
        let next = getNextTerminatingValue seed prev
        if Set.contains next seen then
            prev
        else
            findLastUnique next (Set.add next seen)
    findLastUnique 0 Set.empty

let solver = {parse = File.ReadLines >> getSeed; part1 = solvePart1; part2 = solvePart2}