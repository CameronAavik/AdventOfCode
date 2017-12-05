open System
open System.Linq
open System.IO

let split (str : string) = str.Split()
let (><) f a b = f b a

type Day<'T> = 
    abstract member processInput : string seq -> 'T
    abstract member solvePart1 : 'T -> int
    abstract member solvePart2 : 'T -> int

type Day1() =
    let solve captcha windowSize = 
        Seq.append captcha captcha
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length captcha)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (Seq.head >> int >> (-) (int '0'))

    interface Day<string> with
        member this.processInput(seq) = Seq.head seq
        member this.solvePart1(captcha) = solve captcha 2
        member this.solvePart2(captcha) = solve captcha ((Seq.length captcha / 2) + 1)

type Day2() = 
    let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
    let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
    let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

    interface Day<int seq seq> with
        member this.processInput(seq) = Seq.map (split >> Seq.map int) seq
        member this.solvePart1(spreadsheet) = Seq.sumBy getLargestDiff spreadsheet
        member this.solvePart2(spreadsheet) = Seq.sumBy getDivisor spreadsheet

type Day3() = 
    let manhattanDistance target = 
        let ringNumber = target |> float |> sqrt |> ceil |> int |> ((><) (/) 2)
        let ringEnd = ringNumber * 2 |> (pown >< 2) |> (+) 1
        [1 ; 3 ; 5 ; 7] 
        |> Seq.map (fun i -> abs (target - (ringEnd - i * ringNumber)) + ringNumber) 
        |> Seq.min

    let getNextValue grid posMap (newX, newY) = 
        if Map.isEmpty posMap then 1
        else
            [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]
            |> Seq.map (fun (x, y) -> (newX + x, newY + y))
            |> Seq.filter (Map.containsKey >< posMap)
            |> Seq.sumBy (Map.find >< posMap) |> (List.item >< grid)

    let getNextPos (x, y) = 
        if (y <= 0) && (x <= -y) && (y <= x) then (x + 1, y)
        elif (x > 0) && (y < x) then (x, y + 1)
        elif (y > 0) && (-x < y) then (x - 1, y)
        else (x, y - 1)
            
    let rec buildGrid grid maxDepth posMap newPos = 
        let newValue = getNextValue grid posMap newPos
        if newValue > maxDepth then newValue 
        else buildGrid (grid @ [ newValue ]) maxDepth (posMap.Add(newPos, grid.Length)) (getNextPos newPos)

    interface Day<int> with
        member this.processInput(seq) = seq |> Seq.head |> int
        member this.solvePart1(target) = manhattanDistance target
        member this.solvePart2(target) = buildGrid List.empty target Map.empty (0, 0)

type Day4() = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length

    interface Day<string[] seq> with
        member this.processInput(seq) = seq |> Seq.map split
        member this.solvePart1(passphrases) = solve id passphrases
        member this.solvePart2(passphrases) = solve (Seq.map sortedString) passphrases

type Day5() = 
    let rec solve' currentPosition modifyOffset maze total = 
        if currentPosition < 0 || currentPosition >= Array.length maze then total
        else
            let currentOffset = Array.get maze currentPosition
            maze.[currentPosition] <- modifyOffset currentOffset
            solve' (currentOffset + currentPosition) modifyOffset maze (total + 1)
    let solve modifyOffset offsets = solve' 0 modifyOffset offsets 0

    interface Day<int[]> with
        member this.processInput(seq) = seq |> Seq.map int |> Seq.toArray
        member this.solvePart1(offsets) = solve ((+) 1) offsets
        member this.solvePart2(offsets) = solve (fun x -> if x >= 3 then (x - 1) else (x + 1)) offsets

let runSolver' (solver : Day<'T>) input = 
    let processedInput = solver.processInput(input)
    let output1 = solver.solvePart1(processedInput)
    let output2 = solver.solvePart2(processedInput)
    printfn "Part 1: %i" output1
    printfn "Part 2: %i" output2

let runSolver problemName input = 
    match problemName with
        | "Day1" -> runSolver' (new Day1()) input
        | "Day2" -> runSolver' (new Day2()) input
        | "Day3" -> runSolver' (new Day3()) input
        | "Day4" -> runSolver' (new Day4()) input
        | "Day5" -> runSolver' (new Day5()) input
        | _ -> ()

[<EntryPoint>]
let main argv = 
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let run = argv.[0] |> runSolver
    let result = argv.[1] |> File.ReadLines |> run
    printfn "Time to run: %f" sw.Elapsed.TotalMilliseconds
    Console.ReadKey() |> ignore
    0