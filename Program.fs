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
    let solvePart1 input = 
        let target = int (Seq.head input)
        let ringNumber = (target |> float |> Math.Sqrt |> Math.Ceiling |> int) / 2
        let ringEnd = (int (Math.Pow(float ringNumber * 2.0, 2.0))) + 1
        let centers = [1..2..9] |> Seq.map (fun i -> ringEnd - i * ringNumber)
        let minDistanceFromCenter = centers |> Seq.map (fun c -> Math.Abs(target - c)) |> Seq.min
        minDistanceFromCenter + ringNumber
        
    let getNextGrid grid posMap (newX, newY) =
        let newValue = 
            [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]
            |> Seq.map (fun (x, y) -> (newX + x, newY + y))
            |> Seq.filter (fun pos -> Map.containsKey pos posMap) 
            |> Seq.map (fun pos -> List.item (Map.find pos posMap) grid)
            |> Seq.sum
        List.append grid [newValue]

    let getNextPos (x, y) = 
        if (y < 0) && (x <= -y) && (y <= x) then (x + 1, y)
        elif (x > 0) && (y < x) then (x, y + 1)
        elif (y > 0) && (-x < y) then (x - 1, y)
        else (x, y - 1)
            

    let rec buildGrid grid maxDepth posMap newPos = 
        let newGrid = getNextGrid grid posMap newPos
        let newElement = (List.last newGrid)
        if newElement > maxDepth then newElement 
        else
            let newPosMap = posMap.Add(newPos, newGrid.Length - 1)
            let nextPos = getNextPos newPos
            buildGrid newGrid maxDepth newPosMap nextPos

    let solvePart2 input = 
        let target = int (Seq.head input)
        let posMap = Map.empty.Add((0, 0), 0).Add((1, 0), 1)
        buildGrid [ 1; 1 ] target posMap (1, 1)

module Day4 = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat

    let solve mapper input = 
        input 
        |> Seq.map (fun (line : string) -> line.Split())
        |> Seq.map mapper
        |> Seq.filter isUnique 
        |> Seq.length

    let solvePart1 = solve id
    let solvePart2 = solve (Seq.map sortedString)
        

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
        | _ -> None

let runSolver (problem: string) (input: string seq) = 
    getSolver problem 
    |> Option.map (fun solver -> printfn "%i" (solver input))

[<EntryPoint>]
let main argv = 
    let result = runSolver argv.[0] (File.ReadLines argv.[1])
    Console.ReadKey() |> ignore
    0