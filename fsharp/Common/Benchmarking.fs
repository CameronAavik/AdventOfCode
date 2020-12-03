namespace AdventOfCode.FSharp

open BenchmarkDotNet.Reports
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Columns
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Running
open Perfolizer.Horology
open System.Globalization

module Benchmarking =

    [<AbstractClass>]
    type Bench() =
        let mutable solverFunc : unit -> unit = fun _ -> ()

        abstract member GetSolverFunc : int -> int -> unit -> unit
    
        [<Params (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25)>]
        member val public Day = 0 with get, set

        [<Params (1, 2)>]
        member val public Part = 0 with get, set

        [<GlobalSetup>]
        member self.GlobalSetupData() =
            solverFunc <- self.GetSolverFunc self.Day self.Part

        [<Benchmark>]
        member __.RunPart () = solverFunc ()

    let runBenchmarks<'T when 'T :> Bench> () =
        let benchmarkJob = 
            Job.Default
                .WithWarmupCount(1)
                .WithIterationTime(TimeInterval.FromMilliseconds(250.))
                .WithMinIterationCount(10)
                .WithMaxIterationCount(15)
                .DontEnforcePowerPlan()
        
        let summaryStyle = new SummaryStyle(CultureInfo.InvariantCulture, true, SizeUnit.B, TimeUnit.Nanosecond, false)
        
        let benchmarkConfig =
            DefaultConfig.Instance
                .AddJob(benchmarkJob.AsDefault())
                .WithSummaryStyle(summaryStyle)

        BenchmarkRunner.Run<'T>(benchmarkConfig) |> ignore