module Year2019Day09

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let solve boostMode =
    bootProgram (QueueIO.create)
    >> writeInputToQueue boostMode
    >> run
    >> readOutputFromQueue
    >> fst
    >> Option.get

let solver = { parse = parseIntCodeFromFile; part1 = solve 1L; part2 = solve 2L }