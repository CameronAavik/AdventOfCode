module Year2019Day02

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let solve noun verb =
    bootProgram
    >> setVal (Position 1) noun
    >> setVal (Position 2) verb
    >> runUntilHalt
    >> getVal (Position 0)

let solvePart2 intCode = 
    seq {
        for noun = 0 to 99 do
            for verb = 0 to 99 do
                if solve noun verb intCode = 19690720 then
                    100 * noun + verb } |> Seq.head

let solver = { parse = parseIntCodeFromFile; part1 = solve 12 2; part2 = solvePart2 }