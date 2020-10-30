namespace AdventOfCode.FSharp.Y2018

open AdventOfCode.FSharp
open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Benchmarking

module Program =
    let getSolver day part printAnswer =
        let run (solver : Day<_, _, _>) =
            Runner.run printAnswer 2018 day part solver
        match day with
        | 1  -> run Year2018Day01.solver | 2  -> run Year2018Day02.solver | 3  -> run Year2018Day03.solver
        | 4  -> run Year2018Day04.solver | 5  -> run Year2018Day05.solver | 6  -> run Year2018Day06.solver
        | 7  -> run Year2018Day07.solver | 8  -> run Year2018Day08.solver | 9  -> run Year2018Day09.solver
        | 10 -> run Year2018Day10.solver | 11 -> run Year2018Day11.solver | 12 -> run Year2018Day12.solver
        | 13 -> run Year2018Day13.solver | 14 -> run Year2018Day14.solver | 15 -> run Year2018Day15.solver
        | 16 -> run Year2018Day16.solver | 17 -> run Year2018Day17.solver | 18 -> run Year2018Day18.solver
        | 19 -> run Year2018Day19.solver | 20 -> run Year2018Day20.solver | 21 -> run Year2018Day21.solver
        | 22 -> run Year2018Day22.solver | 23 -> run Year2018Day23.solver | 24 -> run Year2018Day24.solver
        | 25 -> run Year2018Day25.solver
        | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day 2018

    type Bench2018() =
        inherit Bench() with
            override _.GetSolverFunc day part () =
                getSolver day part false ()

    [<EntryPoint>]
    let main argv =
        let runPart day part = getSolver day part true ()
        let runDay day = for part in 1..2 do runPart day part
        match argv.[0] with
            | "BENCH" -> Benchmarking.runBenchmarks<Bench2018>()
            | "ALL" -> for day in 1..25 do runDay day
            | x ->
                let parts = x.Split('.') |> Array.map int
                match parts.Length with
                | 1 -> runDay parts.[0]
                | 2 -> runPart parts.[0] parts.[1]
                | _ -> ()
        0
