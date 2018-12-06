namespace CameronAavik.AdventOfCode

open System
open System.Diagnostics
open System.IO
open Utils

module Main =
    let runSolver day year =
        let run solver fileName =
            let time f x = Stopwatch.StartNew() |> (fun sw -> (f x, sw.Elapsed.TotalMilliseconds))
            let run part solve =
                let (result, t) = time solve (fileName |> File.ReadLines |> solver.parse)
                printfn "Year %i Day %02i-%i %7.2fms %O" year day part t result
            run 1 solver.part1
            run 2 solver.part2
        match year with
        | 2017 ->
            match day with
            | 1  -> run Year2017.Day1.solver  | 2  -> run Year2017.Day2.solver  | 3  -> run Year2017.Day3.solver 
            | 4  -> run Year2017.Day4.solver  | 5  -> run Year2017.Day5.solver  | 6  -> run Year2017.Day6.solver
            | 7  -> run Year2017.Day7.solver  | 8  -> run Year2017.Day8.solver  | 9  -> run Year2017.Day9.solver 
            | 10 -> run Year2017.Day10.solver | 11 -> run Year2017.Day11.solver | 12 -> run Year2017.Day12.solver
            | 13 -> run Year2017.Day13.solver | 14 -> run Year2017.Day14.solver | 15 -> run Year2017.Day15.solver
            | 16 -> run Year2017.Day16.solver | 17 -> run Year2017.Day17.solver | 18 -> run Year2017.Day18.solver
            | 19 -> run Year2017.Day19.solver | 20 -> run Year2017.Day20.solver | 21 -> run Year2017.Day21.solver
            | 22 -> run Year2017.Day22.solver | 23 -> run Year2017.Day23.solver | 24 -> run Year2017.Day24.solver
            | 25 -> run Year2017.Day25.solver
            | day -> (fun _ -> printfn "Invalid Day: %i (Year %i)" day year)
        | 2018 ->
            match day with
            | 1  -> run Year2018.Day1.solver  | 2  -> run Year2018.Day2.solver  | 3  -> run Year2018.Day3.solver
            | 4  -> run Year2018.Day4.solver  | 5  -> run Year2018.Day5.solver  | 6  -> run Year2018.Day6.solver
            | day -> (fun _ -> printfn "Invalid Day: %i (Year %i)" day year)
        | year -> (fun _ -> printfn "Invalid Year: %i" year)

    [<EntryPoint>]
    let main argv =
        let runDay day year = runSolver day year (sprintf "input_files\\%i\\day%i.txt" year day)
        match argv.[0] with
            | "ALL" ->
                for i in 1..25 do runDay i 2017
                for i in 1..6 do runDay i 2018
            | x -> // 2018.1 for Day 1 2018
                let parts = x.Split('.')
                runDay (int parts.[1]) (int parts.[0])
        Console.ReadKey() |> ignore
        0