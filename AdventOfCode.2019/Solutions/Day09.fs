module Year2019Day09

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let solve boostMode =
    bootProgram
    >> writeToInput boostMode
    >> runUntilHalt
    >> readFromOutput
    >> fst

let solver = { parse = parseIntCodeFromFile; part1 = solve 1L; part2 = solve 2L }