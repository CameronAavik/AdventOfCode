module Year2019Day02

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

let rec run comp =
    match runInstruction comp with
    | InstructionResult.Complete c -> run c
    | InstructionResult.Halted -> comp
    | _ -> failwith "Should not have entered another state for day 2"

let solve noun verb =
    Computer.create
    >> Computer.set 1 noun
    >> Computer.set 2 verb
    >> run
    >> Computer.get 0

let solvePart2 intCode = 
    seq {
        for noun = 0L to 99L do
            for verb = 0L to 99L do
                if solve noun verb intCode = 19690720L then
                    100L * noun + verb } |> Seq.head

let solver = { parse = parseIntCode; part1 = solve 12L 2L; part2 = solvePart2 }