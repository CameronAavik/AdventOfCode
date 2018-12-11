open System
open System.Diagnostics
open System.IO
open CameronAavik.AdventOfCode.Common

let runSolver day year =
    let run (solver : Day<'a, 'b, 'c>) fileName =
        let time f x = Stopwatch.StartNew() |> (fun sw -> (f x, sw.Elapsed.TotalMilliseconds))
        let run part solve =
            let (result, t) = time solve (fileName |> File.ReadLines |> solver.parse)
            printfn "Year %i Day %02i-%i %7.2fms %O" year day part t result
        run 1 solver.part1
        run 2 solver.part2
    match year with
    | 2017 ->
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
        | day -> (fun _ -> printfn "Invalid Day: %i (Year %i)" day year)
    | 2018 ->
        match day with
        | 1  -> run Year2018Day01.solver | 2  -> run Year2018Day02.solver | 3  -> run Year2018Day03.solver
        | 4  -> run Year2018Day04.solver | 5  -> run Year2018Day05.solver | 6  -> run Year2018Day06.solver
        | 7  -> run Year2018Day07.solver | 8  -> run Year2018Day08.solver | 9  -> run Year2018Day09.solver
        | 10 -> run Year2018Day10.solver | 11 -> run Year2018Day11.solver
        | day -> (fun _ -> printfn "Invalid Day: %i (Year %i)" day year)
    | year -> (fun _ -> printfn "Invalid Year: %i" year)

[<EntryPoint>]
let main argv =
    let runDay day year = runSolver day year (sprintf "input_files\\%i\\day%i.txt" year day)
    match argv.[0] with
        | "ALL" ->
            for i in 1..25 do runDay i 2017
            for i in 1..11 do runDay i 2018
        | x -> // 2018.1 for Day 1 2018
            let parts = x.Split('.')
            runDay (int parts.[1]) (int parts.[0])
    Console.ReadKey() |> ignore
    0

// Note: I used to use this file as the file that had all my solutions. I have
// since restructured my projects, see the Challenges directory for solutions
// if you found your way here through one of my links somewhere.