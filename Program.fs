open System
open System.IO
open CameronAavik.AdventOfCode.Common
open BenchmarkDotNet.Running
open BenchmarkDotNet.Attributes

let getSolver year day part printResult =
    let run (solver : Day<'a, 'b, 'c>) =
        let run part solve =
            let fileName = (sprintf "InputFiles\\%i\\day%02i.txt" year day)
            fun _ ->
                let result = fileName |> File.ReadLines |> solver.parse |> solve
                if printResult then
                    printfn "Year %i Day %02i-%i %O" year day part result
        match part with
        | 1 -> run 1 solver.part1
        | 2 -> run 2 solver.part2
        | _ -> fun _ -> ()
    run Year2016Day11.solver
    //match year with
    //| 2016 ->
    //    match day with
    //    | 1  -> run Year2016Day01.solver | 2  -> run Year2016Day02.solver | 3  -> run Year2016Day03.solver 
    //    | 4  -> run Year2016Day04.solver | 5  -> run Year2016Day05.solver | 6  -> run Year2016Day06.solver
    //    | 7  -> run Year2016Day07.solver | 8  -> run Year2016Day08.solver | 9  -> run Year2016Day09.solver 
    //    | 10 -> run Year2016Day10.solver | 11 -> run Year2016Day11.solver | 12 -> run Year2016Day12.solver
    //    | 13 -> run Year2016Day13.solver | 14 -> run Year2016Day14.solver | 15 -> run Year2016Day15.solver
    //    | 16 -> run Year2016Day16.solver | 17 -> run Year2016Day17.solver | 18 -> run Year2016Day18.solver
    //    | 19 -> run Year2016Day19.solver | 20 -> run Year2016Day20.solver | 21 -> run Year2016Day21.solver
    //    | 22 -> run Year2016Day22.solver | 23 -> run Year2016Day23.solver | 24 -> run Year2016Day24.solver
    //    | 25 -> run Year2016Day25.solver
    //    | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day year
    //| 2017 ->
    //    match day with
    //    | 1  -> run Year2017Day01.solver | 2  -> run Year2017Day02.solver | 3  -> run Year2017Day03.solver 
    //    | 4  -> run Year2017Day04.solver | 5  -> run Year2017Day05.solver | 6  -> run Year2017Day06.solver
    //    | 7  -> run Year2017Day07.solver | 8  -> run Year2017Day08.solver | 9  -> run Year2017Day09.solver 
    //    | 10 -> run Year2017Day10.solver | 11 -> run Year2017Day11.solver | 12 -> run Year2017Day12.solver
    //    | 13 -> run Year2017Day13.solver | 14 -> run Year2017Day14.solver | 15 -> run Year2017Day15.solver
    //    | 16 -> run Year2017Day16.solver | 17 -> run Year2017Day17.solver | 18 -> run Year2017Day18.solver
    //    | 19 -> run Year2017Day19.solver | 20 -> run Year2017Day20.solver | 21 -> run Year2017Day21.solver
    //    | 22 -> run Year2017Day22.solver | 23 -> run Year2017Day23.solver | 24 -> run Year2017Day24.solver
    //    | 25 -> run Year2017Day25.solver
    //    | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day year
    //| 2018 ->
    //    match day with
    //    | 1  -> run Year2018Day01.solver | 2  -> run Year2018Day02.solver | 3  -> run Year2018Day03.solver
    //    | 4  -> run Year2018Day04.solver | 5  -> run Year2018Day05.solver | 6  -> run Year2018Day06.solver
    //    | 7  -> run Year2018Day07.solver | 8  -> run Year2018Day08.solver | 9  -> run Year2018Day09.solver
    //    | 10 -> run Year2018Day10.solver | 11 -> run Year2018Day11.solver | 12 -> run Year2018Day12.solver
    //    | 13 -> run Year2018Day13.solver | 14 -> run Year2018Day14.solver | 15 -> run Year2018Day15.solver
    //    | 16 -> run Year2018Day16.solver | 17 -> run Year2018Day17.solver | 18 -> run Year2018Day18.solver
    //    | 19 -> run Year2018Day19.solver | 20 -> run Year2018Day20.solver | 21 -> run Year2018Day21.solver
    //    | 22 -> run Year2018Day22.solver | 23 -> run Year2018Day23.solver | 24 -> run Year2018Day24.solver
    //    | 25 -> run Year2018Day25.solver
    //    | day -> fun _ -> printfn "Invalid Day: %i (Year %i)" day year
    //| year -> fun _ -> printfn "Invalid Year: %i" year

type Bench() =
    let mutable solverFunc : unit -> unit = fun _ -> ()
    
    [<Params (2016, 2017, 2018)>]
    member val public Year = 0 with get, set

    [<Params (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25)>]
    member val public Day = 0 with get, set

    [<Params (1, 2)>]
    member val public Part = 0 with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        solverFunc <- getSolver self.Year self.Day self.Part false

    [<Benchmark>]
    member self.RunPart () = solverFunc ()


[<EntryPoint>]
let main argv =
    let runPart year day part = getSolver year day part true ()
    let runDay year day = for part in 1..2 do runPart year day part
    let runYear year = for day in 1..25 do runDay year day
    match argv.[0] with
        | "BENCH" -> BenchmarkRunner.Run<Bench>() |> ignore
        | "ALL" -> for year in 2016..2018 do runYear year
        | x ->
            let parts = x.Split('.') |> Array.map int
            match parts.Length with
            | 1 -> runYear parts.[0]
            | 2 -> runDay parts.[0] parts.[1]
            | 3 -> runPart parts.[0] parts.[1] parts.[2]
            | _ -> ()
    Console.ReadKey() |> ignore
    0

// Note: I used to use this file as the file that had all my solutions. I have
// since restructured my projects, see the Challenges directory for solutions
// if you found your way here through one of my links somewhere.