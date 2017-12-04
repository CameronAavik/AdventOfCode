open System
open System.IO

module Day1 =
    let solve input windowSize = 
        let inputStr = Seq.head input
        Seq.append inputStr inputStr
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length inputStr)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (fun w -> int (Seq.head w) - int '0')

    let solvePart1 input = solve input 2
    let solvePart2 input = 
        let length = Seq.head input |> Seq.length
        solve input ((length / 2) + 1)

module Day2 = 
    let parseLine (line : string) = line.Split() |> Seq.map int
    let getLargestDiff ints = ((Seq.max ints) - (Seq.min ints));

    let solve input = 
        input 
        |> Seq.map parseLine 
        |> Seq.map getLargestDiff 
        |> Seq.sum

let getSolver problemName = 
    match problemName with
        | "Day1.1" -> Some Day1.solvePart1
        | "Day1.2" -> Some Day1.solvePart2
        | "Day2.1" -> Some Day2.solve
        | _ -> None

let runSolver (problem: string) (input: string seq) = 
    getSolver problem 
    |> Option.map (fun solver -> printfn "%i" (solver input))

[<EntryPoint>]
let main argv = 
    let result = runSolver argv.[0] (File.ReadLines argv.[1])
    Console.ReadKey() |> ignore
    0