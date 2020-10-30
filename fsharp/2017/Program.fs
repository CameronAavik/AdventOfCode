namespace AdventOfCode.FSharp.Y2017

open AdventOfCode.FSharp
open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Benchmarking

module Program =
    let getSolver day part printAnswer =
        let run (solver : Day<_, _, _>) =
            Runner.run printAnswer 2017 day part solver
        match day with
        | 1  -> run Year2017Day01.solver | 2  -> run Year2017Day02.solver | 3  -> run Year2017Day03.solver
        | 4  -> run Year2017Day04.solver | 5  -> run Year2017Day05.solver | 6  -> run Year2017Day06.solver
        | 7  -> run Year2017Day07.solver | 8  -> run Year2017Day08.solver | 9  -> run Year2017Day09.solver
        | 10 -> run Year2017Day10.solver | 11 -> run Year2017Day11.solver | 12 -> run Year2017Day12.solver
        | 13 -> run Year2017Day13.solver | 14 -> run Year2017Day14.solver | 15 -> run Year2017Day15.solver
        | 16 -> run Year2017Day16.solver | 17 -> run Year2017Day17.solver | 18 -> run Year2017Day18.solver
        | 19 -> run Year2017Day19.solver | 20 -> run Year2017Day20.solver | 21 -> run Year2017Day21.solver
        | 22 -> run Year2017Day22.solver | 23 -> run Year2017Day23.solver | 24 -> run Year2017Day24.solver
        | 25 -> run Year2017Day25.solver
        | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day 2017

    type Bench2017() =
        inherit Bench() with
            override _.GetSolverFunc day part () =
                getSolver day part false ()

    [<EntryPoint>]
    let main argv =
        let runPart day part = getSolver day part true ()
        let runDay day = for part in 1..2 do runPart day part
        match argv.[0] with
            | "BENCH" -> Benchmarking.runBenchmarks<Bench2017>()
            | "ALL" -> for day in 1..25 do runDay day
            | x ->
                let parts = x.Split('.') |> Array.map int
                match parts.Length with
                | 1 -> runDay parts.[0]
                | 2 -> runPart parts.[0] parts.[1]
                | _ -> ()
        0
