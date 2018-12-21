module Year2018Day21

open CameronAavik.AdventOfCode.Common

let getSeed =
    // after comparing many inputs, the only lines that was different was line 8 
    Seq.item 8 >> splitBy " " (fun p -> int p.[1])

let rec getNextTerminatingValue innerVal acc =
    let innerVal =
        acc &&& 0xFF
        |> (+) innerVal
        |> (&&&) 0xFFFFFF
        |> (*) 65899
        |> (&&&) 0xFFFFFF
    if acc < 256 then
        innerVal
    else
        getNextTerminatingValue innerVal (acc >>> 8)

let solvePart1 seed = getNextTerminatingValue seed 0x10000
let solvePart2 seed =
    let rec findLastUnique prev seen =
        let next = getNextTerminatingValue seed (prev ||| 0x10000)
        if Set.contains next seen then
            prev
        else
            findLastUnique next (Set.add next seen)
    findLastUnique 0 Set.empty

let solver = {parse = getSeed; part1 = solvePart1; part2 = solvePart2}