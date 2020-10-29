namespace CameronAavik.AdventOfCode

open System.IO
open Common

module Runner =
    let run printResult year day part (solver : Day<_, _, _>) =
        let run part solve =
            let fileName = Path.Combine("InputFiles", (sprintf "day%02i.txt" day))
            fun _ ->
                let result = fileName |> solver.parse |> solve
                if printResult then
                    printfn "Year %i Day %02i-%i %O" year day part result
        match part with
        | 1 -> run 1 solver.part1
        | 2 -> run 2 solver.part2
        | _ -> fun _ -> ()