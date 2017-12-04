open System

module Day1 =
    let solve inputStr windowSize = 
        Seq.append inputStr inputStr
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length inputStr)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (fun w -> int (Seq.head w) - int '0')

    let solvePart1 inputStr = solve inputStr 2
    let solvePart2 inputStr = solve inputStr (((Seq.length inputStr) / 2) + 1)

let runSolver (argv : string array) = 
    match argv.[0] with
     | "Day1.1" -> printfn "%i" (Day1.solvePart1 argv.[1])
     | "Day1.2" -> printfn "%i" (Day1.solvePart2 argv.[1])
     | _ -> ()

[<EntryPoint>]
let main argv = 
    let result = runSolver argv
    Console.ReadKey() |> ignore
    0