module Year2019Day02

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let solve noun verb =
    bootProgram NoIO.create
    >> setVal (Position 1L) noun
    >> setVal (Position 2L) verb
    >> run
    >> getVal (Position 0L)

let solvePart2 intCode = 
    seq {
        for noun = 0L to 99L do
            for verb = 0L to 99L do
                if solve noun verb intCode = 19690720L then
                    100L * noun + verb } |> Seq.head

let solver = { parse = parseIntCodeFromFile; part1 = solve 12L 2L; part2 = solvePart2 }