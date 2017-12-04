open System

module Day1 =
    let solve inputStr =
        Seq.append inputStr [ Seq.head inputStr ] 
        |> Seq.pairwise 
        |> Seq.filter (fun (a, b) -> a = b) 
        |> Seq.sumBy (fun (a, b) -> int a - int '0')

let runSolver (argv : string array) = 
    match argv.[0] with
     | "Day1" -> printfn "%i" (Day1.solve argv.[1])
     | _ -> ()

[<EntryPoint>]
let main argv = 
    let result = runSolver argv
    Console.ReadKey() |> ignore
    0