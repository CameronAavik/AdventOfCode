open System
open System.Linq
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
    let doesIntersect (a : 'a seq) (b : 'a seq) = a.Intersect(b).Any()
    let isValidDivisor ints i = 
        ints 
        |> Seq.map ((*) i)
        |> doesIntersect ints

    let getDivisor ints = 
        [2 .. (Seq.max ints)]
        |> Seq.filter (isValidDivisor ints)
        |> Seq.head

    let solve input computeLineResult = input |> Seq.map parseLine |> Seq.sumBy computeLineResult 
    let solvePart1 input = solve input getLargestDiff
    let solvePart2 input = solve input getDivisor

module Day3 = 
    let solve (input : string seq) = 
        let target = int (Seq.head input)
        let ringNumber = (target |> float |> Math.Sqrt |> Math.Ceiling |> int) / 2
        let ringEnd = (int (Math.Pow(float ringNumber * 2.0, 2.0))) + 1
        let centers = [1..2..9] |> Seq.map (fun i -> ringEnd - i * ringNumber)
        let minDistanceFromCenter = centers |> Seq.map (fun c -> Math.Abs(target - c)) |> Seq.min
        minDistanceFromCenter + ringNumber

let getSolver problemName = 
    match problemName with
        | "Day1.1" -> Some Day1.solvePart1
        | "Day1.2" -> Some Day1.solvePart2
        | "Day2.1" -> Some Day2.solvePart1
        | "Day2.2" -> Some Day2.solvePart2
        | "Day3.1" -> Some Day3.solve
        | _ -> None

let runSolver (problem: string) (input: string seq) = 
    getSolver problem 
    |> Option.map (fun solver -> printfn "%i" (solver input))

[<EntryPoint>]
let main argv = 
    let result = runSolver argv.[0] (File.ReadLines argv.[1])
    Console.ReadKey() |> ignore
    0