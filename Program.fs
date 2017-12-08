open System
open System.Linq
open System.IO

let split (str : string) = str.Split()
let (><) f a b = f b a

module Day1 =
    let solve captcha windowSize = 
        Seq.append captcha captcha
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length captcha)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (Seq.head >> int >> (><) (-) (int '0'))
        
    let parse = Seq.head
    let solvePart1 captcha = solve captcha 2
    let solvePart2 captcha = solve captcha ((Seq.length captcha / 2) + 1)

module Day2 = 
    let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
    let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
    let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

    let parse = Seq.map (split >> Seq.map int)
    let solvePart1 = Seq.sumBy getLargestDiff
    let solvePart2 = Seq.sumBy getDivisor

module Day3 = 
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
            |> Seq.sumBy ((Map.find >< posMap) >> (List.item >< grid))

    let getNextPos (x, y) = 
        if (y <= 0) && (x <= -y) && (y <= x) then (x + 1, y)
        elif (x > 0) && (y < x) then (x, y + 1)
        elif (y > 0) && (-x < y) then (x - 1, y)
        else (x, y - 1)
            
    let rec buildGrid grid maxDepth posMap newPos = 
        let newValue = getNextValue grid posMap newPos
        if newValue > maxDepth then newValue 
        else buildGrid (grid @ [ newValue ]) maxDepth (Map.add newPos grid.Length posMap) (getNextPos newPos)

    let parse = Seq.head >> int
    let solvePart1 = manhattanDistance
    let solvePart2 target = buildGrid List.empty target Map.empty (0, 0)

module Day4 = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length

    let parse = Seq.map split
    let solvePart1 = solve id
    let solvePart2 = solve (Seq.map sortedString)

module Day5 = 
    let rec solve' currentPosition modifyOffset maze total = 
        if currentPosition < 0 || currentPosition >= Array.length maze then total
        else
            let currentOffset = Array.get maze currentPosition
            maze.[currentPosition] <- modifyOffset currentOffset
            solve' (currentOffset + currentPosition) modifyOffset maze (total + 1)
    let solve modifyOffset offsets = solve' 0 modifyOffset offsets 0

    let parse = Seq.map int >> Seq.toArray
    let solvePart1 = solve ((+) 1)
    let solvePart2 = solve (fun x -> if x >= 3 then (x - 1) else (x + 1))

module Day6 = 
    let serialiseBanks = Array.map (fun i -> i.ToString()) >> String.concat ","

    let distribute' numBanks maxV maxI i v =
        let doesOverlap = ((if i <= maxI then numBanks else 0) + i) <= (maxI + (maxV % numBanks))
        (if i = maxI then 0 else v) + (maxV / numBanks) + (if doesOverlap then 1 else 0)

    let rec solve seen c banks= 
        if (Map.containsKey (serialiseBanks banks) seen) then (seen, banks)
        else
        let maxV = Array.max banks
        let maxI = Array.findIndex ((=) maxV) banks
        solve (Map.add (serialiseBanks banks) c seen) (c + 1) (Array.mapi (distribute' (Seq.length banks) maxV maxI) banks)

    let parse = Seq.head >> split >> Array.map int
    let solvePart1 = solve Map.empty 0 >> fst >> Map.count
    let solvePart2 = solve Map.empty 0 >> (fun (seen, banks) -> (Map.count seen) - (Map.find (serialiseBanks banks) seen))

let runSolver' parse solvePart1 solvePart2 input = 
    let sw = System.Diagnostics.Stopwatch.StartNew()
    printfn "Part 1: %A (%fms)" (solvePart1 (parse input)) sw.Elapsed.TotalMilliseconds
    sw.Restart()
    printfn "Part 2: %A (%fms)" (solvePart2 (parse input)) sw.Elapsed.TotalMilliseconds

let runSolver problemName = 
    match problemName with
        | "Day1" -> runSolver' Day1.parse Day1.solvePart1 Day1.solvePart2
        | "Day2" -> runSolver' Day2.parse Day2.solvePart1 Day2.solvePart2
        | "Day3" -> runSolver' Day3.parse Day3.solvePart1 Day3.solvePart2
        | "Day4" -> runSolver' Day4.parse Day4.solvePart1 Day4.solvePart2
        | "Day5" -> runSolver' Day5.parse Day5.solvePart1 Day5.solvePart2
        | "Day6" -> runSolver' Day6.parse Day6.solvePart1 Day6.solvePart2
        | _ -> (fun _ -> printfn "Invalid Problem: %s" problemName)

[<EntryPoint>]
let main argv = 
    let run = argv.[0] |> runSolver
    let result = argv.[1] |> File.ReadLines |> run
    Console.ReadKey() |> ignore
    0