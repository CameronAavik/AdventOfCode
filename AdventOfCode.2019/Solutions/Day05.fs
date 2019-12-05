module Year2019Day05

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let solve systemId =
    bootProgram
    >> writeToInput systemId
    >> runUntilHalt
    >> readAllOutput
    >> fst
    >> Seq.last

let solver = { parse = parseIntCodeFromFile; part1 = solve 1; part2 = solve 5 }