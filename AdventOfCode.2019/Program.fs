namespace CameronAavik.AdventOfCode.Y2019

open CameronAavik.AdventOfCode
open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Benchmarking

module Program =
    let getSolver day part printAnswer =
        let run (solver : Day<_, _, _>) =
            Runner.run printAnswer 2019 day part solver
        match day with
        | 1  -> run Year2019Day01.solver | 2  -> run Year2019Day02.solver | 3  -> run Year2019Day03.solver
        | 4  -> run Year2019Day04.solver | 5  -> run Year2019Day05.solver | 6  -> run Year2019Day06.solver
        | 7  -> run Year2019Day07.solver | 8  -> run Year2019Day08.solver | 9  -> run Year2019Day09.solver
        //| 10 -> run Year2019Day10.solver | 11 -> run Year2019Day11.solver | 12 -> run Year2019Day12.solver
        //| 13 -> run Year2019Day13.solver | 14 -> run Year2019Day14.solver | 15 -> run Year2019Day15.solver
        //| 16 -> run Year2019Day16.solver | 17 -> run Year2019Day17.solver | 18 -> run Year2019Day18.solver
        //| 19 -> run Year2019Day19.solver | 20 -> run Year2019Day20.solver | 21 -> run Year2019Day21.solver
        //| 22 -> run Year2019Day22.solver | 23 -> run Year2019Day23.solver | 24 -> run Year2019Day24.solver
        //| 25 -> run Year2019Day25.solver
        | day -> fun () -> if printAnswer then printfn "Invalid Day: %i (Year %i)" day 2019

    type Bench2019() =
        inherit Bench() with
            override _.GetSolverFunc day part () =
                getSolver day part false ()

    [<EntryPoint>]
    let main argv =
        let runPart day part = getSolver day part true ()
        let runDay day = for part in 1..2 do runPart day part
        match argv.[0] with
            | "BENCH" -> Benchmarking.runBenchmarks<Bench2019>()
            | "ALL" -> for day in 1..25 do runDay day
            | x ->
                let parts = x.Split('.') |> Array.map int
                match parts.Length with
                | 1 -> runDay parts.[0]
                | 2 -> runPart parts.[0] parts.[1]
                | _ -> ()
        0
