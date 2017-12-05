open System
open System.Linq
open System.IO

module Utils = 
    let split (str : string) = str.Split()

module Day1 =
    let solve' input windowSize = 
        Seq.append input input
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length input)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (Seq.head >> (fun c -> int c - int '0'))
    let solve input = solve' (Seq.head input)

    let solvePart1 input = solve input 2
    let getInputLength = Seq.head >> Seq.length
    let solvePart2 input = solve input ((getInputLength input / 2) + 1)

module Day2 = 
    let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
    let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
    let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

    let solve computeLineResult = Seq.sumBy (Utils.split >> Seq.map int >> computeLineResult)
    let solvePart1 = solve getLargestDiff
    let solvePart2 = solve getDivisor

module Day3 = 
    let solvePart1 input = 
        let target = input |> Seq.head |> int
        let ringNumber = target |> float |> sqrt |> ceil |> int |> (fun x -> x / 2)
        let ringEnd = ringNumber * 2 |> (fun x -> x * x) |> (+) 1
        [1 ; 3 ; 5 ; 7] 
        |> Seq.map (fun i -> abs (target - (ringEnd - i * ringNumber)) + ringNumber) 
        |> Seq.min 
        
    let getNextValue grid posMap (newX, newY) =
        [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]
        |> Seq.map (fun (x, y) -> (newX + x, newY + y))
        |> Seq.filter (fun pos -> Map.containsKey pos posMap) 
        |> Seq.sumBy (fun pos -> List.item (Map.find pos posMap) grid)

    let getNextPos (x, y) = 
        if (y <= 0) && (x <= -y) && (y <= x) then (x + 1, y)
        elif (x > 0) && (y < x) then (x, y + 1)
        elif (y > 0) && (-x < y) then (x - 1, y)
        else (x, y - 1)
            
    let rec buildGrid grid maxDepth posMap newPos = 
        let newValue = getNextValue grid posMap newPos
        if newValue > maxDepth then newValue 
        else buildGrid (grid @ [ newValue ]) maxDepth (posMap.Add(newPos, grid.Length)) (getNextPos newPos)

    let solvePart2 input = buildGrid [ 1 ] (input |> Seq.head |> int) (Map.empty.Add((0, 0), 0)) (1, 0)

module Day4 = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.filter (Utils.split >> mapper >> isUnique) >> Seq.length
    let solvePart1 = solve id
    let solvePart2 = solve (Seq.map sortedString)

module Day5 = 
    let rec solve' currentPosition modifyOffset maze total = 
        if currentPosition < 0 || currentPosition >= (Array.length maze) then total
        else
            let currentOffset = Array.get maze currentPosition
            maze.[currentPosition] <- modifyOffset currentOffset
            solve' (currentOffset + currentPosition) modifyOffset maze (total + 1)

    let solve modifyOffset input = solve' 0 modifyOffset (Seq.map int input |> Seq.toArray) 0
    let solvePart1 = solve ((+) 1)
    let solvePart2 = solve (fun x -> if x >= 3 then (x - 1) else (x + 1))

let getSolver problemName = 
    match problemName with
        | "Day1.1" -> Some Day1.solvePart1
        | "Day1.2" -> Some Day1.solvePart2
        | "Day2.1" -> Some Day2.solvePart1
        | "Day2.2" -> Some Day2.solvePart2
        | "Day3.1" -> Some Day3.solvePart1
        | "Day3.2" -> Some Day3.solvePart2
        | "Day4.1" -> Some Day4.solvePart1
        | "Day4.2" -> Some Day4.solvePart2
        | "Day5.1" -> Some Day5.solvePart1
        | "Day5.2" -> Some Day5.solvePart2
        | _ -> None

let runSolver (problem: string) (input: string seq) = 
    getSolver problem 
    |> Option.map (fun solver -> printfn "%i" (solver input))

[<EntryPoint>]
let main argv = 
    let result = runSolver argv.[0] (File.ReadLines argv.[1])
    Console.ReadKey() |> ignore
    0