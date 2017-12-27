namespace CameronAavik.AdventOfCode

open System
open System.Diagnostics
open System.IO
open Utils

module Main =
    let runSolver day =
        let run solver fileName =
            let time f x = Stopwatch.StartNew() |> (fun sw -> (f x, sw.Elapsed.TotalMilliseconds))
            let run part solve =
                let (result, t) = time solve (fileName |> File.ReadLines |> solver.parse)
                printfn "Day %02i-%i %7.2fms %O" day part t result
            run 1 solver.solvePart1
            run 2 solver.solvePart2
        match day with
        | 1  -> run Day1.solver  | 2  -> run Day2.solver  | 3  -> run Day3.solver  | 4  -> run Day4.solver
        | 5  -> run Day5.solver  | 6  -> run Day6.solver  | 7  -> run Day7.solver  | 8  -> run Day8.solver
        | 9  -> run Day9.solver  | 10 -> run Day10.solver | 11 -> run Day11.solver | 12 -> run Day12.solver
        | 13 -> run Day13.solver | 14 -> run Day14.solver | 15 -> run Day15.solver | 16 -> run Day16.solver
        | 17 -> run Day17.solver | 18 -> run Day18.solver | 19 -> run Day19.solver | 20 -> run Day20.solver
        | 21 -> run Day21.solver | 22 -> run Day22.solver | 23 -> run Day23.solver | 24 -> run Day24.solver
        | 25 -> run Day25.solver
        | day -> (fun _ -> printfn "Invalid Problem: %i" day)

    [<EntryPoint>]
    let main argv =
        let runDay day = runSolver day (sprintf "input_files\\day%i.txt" day)
        match argv.[0] with
            | "ALL" -> for i in 1..25 do runDay i
            | x -> runDay (int x)
        Console.ReadKey() |> ignore
        0