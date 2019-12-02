module Year2019Day01

open CameronAavik.AdventOfCode.Common

let getFuel mass = mass / 3 - 2

let solvePart1 input = input |> Seq.sumBy getFuel
  
let getFuel2 mass =
    let rec getFuel2' mass =
        if mass <= 0 then 0
        else mass + getFuel2' (getFuel mass)
    getFuel2' (getFuel mass)

let solvePart2 input = input |> Seq.sumBy getFuel2

let solver = { parse = parseEachLine asInt; part1 = solvePart1; part2 = solvePart2 }