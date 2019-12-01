namespace CameronAavik.AdventOfCode.Y2016

open CameronAavik.AdventOfCode
open CameronAavik.AdventOfCode.Common

module Program =
    let getSolver day part printAnswer =
        let run (solver : Day<_, _, _>) =
            Runner.run printAnswer 2016 day part solver
        match day with
        | 1  -> run Year2016Day01.solver | 2  -> run Year2016Day02.solver | 3  -> run Year2016Day03.solver
        | 4  -> run Year2016Day04.solver | 5  -> run Year2016Day05.solver | 6  -> run Year2016Day06.solver
        | 7  -> run Year2016Day07.solver | 8  -> run Year2016Day08.solver | 9  -> run Year2016Day09.solver
        | 10 -> run Year2016Day10.solver | 11 -> run Year2016Day11.solver | 12 -> run Year2016Day12.solver
        | 13 -> run Year2016Day13.solver | 14 -> run Year2016Day14.solver | 15 -> run Year2016Day15.solver
        | 16 -> run Year2016Day16.solver | 17 -> run Year2016Day17.solver | 18 -> run Year2016Day18.solver
        | 19 -> run Year2016Day19.solver | 20 -> run Year2016Day20.solver | 21 -> run Year2016Day21.solver
        | 22 -> run Year2016Day22.solver | 23 -> run Year2016Day23.solver | 24 -> run Year2016Day24.solver
        | 25 -> run Year2016Day25.solver
        | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day 2016

    [<EntryPoint>]
    let main argv =
        let runPart day part = getSolver day part true ()
        let runDay day = for part in 1..2 do runPart day part
        match argv.[0] with
            | "BENCH" -> Benchmarking.runBenchmarks getSolver
            | "ALL" -> for day in 1..25 do runDay day
            | x ->
                let parts = x.Split('.') |> Array.map int
                match parts.Length with
                | 1 -> runDay parts.[0]
                | 2 -> runPart parts.[0] parts.[1]
                | _ -> ()
        0

// Note: I used to use this file as the file that had all my solutions. I have
// since restructured my projects, see the Challenges directory for solutions
// if you found your way here through one of my links somewhere.