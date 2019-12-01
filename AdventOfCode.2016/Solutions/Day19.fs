module Year2016Day19

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine asInt

let josephus n =
    let rec josephus' n i acc =
        if (i > n) then acc + 1
        else josephus' n (i + 1) ((acc + 2) % i)
    josephus' n 1 0

let solvePart1 elves = josephus elves

let solvePart2 elves =
    // round down to nearest power of 3
    let p = pown 3 (System.Math.Log(float elves, 3.) |> int)
    elves - p + (max (elves - 2 * p) 0)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }