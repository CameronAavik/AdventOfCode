open System
open System.Diagnostics
open System.Linq
open System.IO

module Utils =
    // helper methods for parsing
    let asString, asInt, asStringArray, asIntArray = id, int, Array.map id, Array.map int
    let splitBy (c : string) f (str : string) = str.Split([| c |], StringSplitOptions.None) |> f
    let parseFirstLine f = Seq.head >> f
    let parseEachLine f = Seq.map f

    // this is an infix flip, for example you can replace (fun a -> a / 2) with (><) (/) 2
    let (><) f a b = f b a
    // this is a modulo operator that handles negative numbers correctly
    let (%!) a b = (a % b + b) % b

    type Day<'a, 'b, 'c> = {parse: string seq -> 'a; solvePart1: 'a -> 'b; solvePart2: 'a -> 'c}

    module Graph =
        let getConnectedComponent getVerts rootNode =
            let rec getConnectedComponent' comp = function
                | [] -> comp
                | x :: xs when Set.contains x comp -> getConnectedComponent' comp xs
                | x :: xs -> getConnectedComponent' (Set.add x comp) (getVerts x @ xs)
            getConnectedComponent' Set.empty [rootNode]

        let getConnectedComponents getVerts nodes =
            let rec getConnectedComponents' seen unseen components =
                if Set.isEmpty unseen then components
                else
                    let newComp = getConnectedComponent getVerts (Seq.head unseen)
                    getConnectedComponents' (Set.union seen newComp) (Set.difference unseen newComp) (newComp :: components)
            getConnectedComponents' Set.empty nodes List.empty

open Utils

module Day1 =
    let charToDigit c = int c - int '0'
    let solve windowSize captcha = 
        Seq.append captcha captcha
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length captcha)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (Seq.head >> charToDigit)

    let solvePart2 captcha = solve ((Seq.length captcha / 2) + 1) captcha
    let solver = { parse = parseFirstLine asString; solvePart1 = solve 2; solvePart2 = solvePart2}

module Day2 = 
    let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
    let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
    let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

    let solvePart1 = Seq.sumBy getLargestDiff
    let solvePart2 = Seq.sumBy getDivisor
    let solver = { parse = parseEachLine (splitBy "\t" asIntArray); solvePart1 = solvePart1; solvePart2 = solvePart2}

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

    let solvePart2 target = buildGrid List.empty target Map.empty (0, 0)
    let solver = { parse = parseFirstLine asInt; solvePart1 = manhattanDistance; solvePart2 = solvePart2}

module Day4 = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length
    let solver = { parse = parseEachLine (splitBy " " asStringArray); solvePart1 = solve id; solvePart2 = solve (Seq.map sortedString)}

module Day5 = 
    let solve modifyOffset offsets =
        let maze = Seq.toArray offsets
        let rec solve' currentPosition total = 
            if currentPosition < 0 || currentPosition >= Array.length maze then total
            else
                let currentOffset = Array.get maze currentPosition
                maze.[currentPosition] <- modifyOffset currentOffset
                solve' (currentOffset + currentPosition) (total + 1)
        solve' 0 0

    let solvePart1 = solve ((+) 1)
    let solvePart2 = solve (fun x -> if x >= 3 then (x - 1) else (x + 1))
    let solver = { parse = parseEachLine asInt; solvePart1 = solvePart1; solvePart2 = solvePart2; }

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

    let solvePart1 = solve Map.empty 0 >> fst >> Map.count
    let solvePart2 = solve Map.empty 0 >> (fun (seen, banks) -> (Map.count seen) - (Map.find (serialiseBanks banks) seen))
    let solver = { parse = parseFirstLine (splitBy "\t" asIntArray); solvePart1 = solvePart1; solvePart2 = solvePart2; }

module Day7 =
    let getSubTowers (tokens : string array) = if Array.length tokens = 2 then Array.empty else tokens.[3..] |> Array.map (fun c -> c.TrimEnd(','))
    let asProgram = splitBy " " (fun tokens -> (tokens.[0], (tokens.[1].Trim('(',')') |> int, getSubTowers tokens)))

    let rec findRoot tower current = 
        match tower |> Seq.tryFind (fun (_, (_, children)) -> Array.contains current children) with
        | None -> current
        | Some (node, _) -> findRoot tower node

    let rec getWeight tower node = 
        let data = Map.find node tower
        (fst data) + (Array.sumBy (getWeight tower) (snd data))

    let getChildrenWeights tower = 
        Seq.map (fun c -> (getWeight tower c, Map.find c tower |> fst)) 
        >> Seq.groupBy fst 
        >> Seq.sortByDescending (fun (k, g) -> Seq.length g) 
        >> Seq.toArray

    let getMissingWeight tower = 
        let weightMap = tower |> Map.map (fun k v -> getChildrenWeights tower (snd v))
        let programsWithTwoWeights = weightMap |> Map.filter (fun k v -> (Array.length v) = 2)
        let adjustments = programsWithTwoWeights |> Map.map (fun k v -> (snd v.[1] |> Seq.head |> snd) + (fst v.[0]) - (fst v.[1]))
        adjustments |> Seq.sortBy (fun kv -> kv.Value) |> Seq.head |> (fun kv -> kv.Value)

    let solvePart1 tower = Seq.head tower |> fst |> findRoot tower
    let solvePart2 tower = Map.ofSeq tower |> getMissingWeight
    let solver = { parse = parseEachLine asProgram; solvePart1 = solvePart1; solvePart2 = solvePart2}

module Day8 =
    let parseIncOrDec = function | "inc" -> (+) | "dec" -> (-) | _ -> (fun _ x -> x)
    let parseOperator = function | ">" -> (>) | "<" -> (<) | ">=" -> (>=) | "<=" -> (<=) | "==" -> (=) | "!=" -> (<>) | _ -> (fun _ _ -> false)
    let asInstruction = splitBy " " (fun tokens -> (tokens.[0], (parseIncOrDec tokens.[1]) >< (int tokens.[2]), tokens.[4], (parseOperator tokens.[5]) >< (int tokens.[6])))

    let getOrZero map key = if Map.containsKey key map then Map.find key map else 0
    let simulate (var1, incOrDec, var2, passesComparisonCheck) vars = 
        let val1, val2 = getOrZero vars var1, getOrZero vars var2
        if passesComparisonCheck val2 then Map.add var1 (incOrDec val1) vars else vars
    let maxVar = Map.toSeq >> Seq.map snd >> Seq.max

    let solve folder program = Seq.fold (fun (vars, acc) insn -> simulate insn vars |> (folder acc)) (Map.empty, 0) program |> snd
    let solvePart1 = solve (fun _ vars -> (vars, maxVar vars))
    let solvePart2 = solve (fun acc vars -> (vars, max (maxVar vars) acc))
    let solver = { parse = parseEachLine asInstruction; solvePart1 = solvePart1; solvePart2 = solvePart2; }

module Day9 = 
    type GarbageState = NotGarbage | Garbage | Cancelled
    type State = {level: int; state: GarbageState; score: int; garbage: int }

    let step current nextChar =
        match (current.state, nextChar) with
        | (Garbage, '!') -> {current with state = Cancelled}
        | (Garbage, '>') -> {current with state = NotGarbage} 
        | (Garbage, _)   -> {current with garbage = current.garbage + 1}
        | (Cancelled, _) | (NotGarbage, '<') -> {current with state = Garbage}
        | (NotGarbage, '{') -> {current with level = current.level + 1}
        | (NotGarbage, '}') -> {current with level = current.level - 1; score = current.score + current.level}
        | _ -> current;

    let solve = Seq.fold step {level=0; state=NotGarbage; score=0; garbage=0}
    let solvePart1 = solve >> (fun state -> state.score)
    let solvePart2 = solve >> (fun state -> state.garbage)
    let solver = { parse = parseFirstLine asString; solvePart1 = solvePart1; solvePart2 = solvePart2; }

module Day10 = 
    let repeat n folder init = (Seq.fold (fun s i -> folder s) init [0..(n-1)])

    type State = { l: int array; skip: int; start: int}
    let revN n s = Array.append (Array.take n s |> Array.rev) (Array.skip n s)
    let shift n s = Array.append (Array.skip n s) (Array.take n s)
    let step listLen state length = 
        let shiftLen = ((length + state.skip) % listLen)
        {state with l = state.l |> revN length |> shift shiftLen; skip = state.skip + 1; start = (state.start - shiftLen) %! listLen}
    
    let solveRound listLen init = Array.fold (step listLen) init
    let sparseHash listLen n lengths = 
        repeat n (solveRound listLen >< lengths) {l=[|0..(listLen-1)|]; skip=0; start=0} |> (fun s -> shift s.start s.l)
    let denseHash = Array.chunkBySize 16 >> Array.map (Array.fold (^^^) 0)
    let toHexStr = Array.fold (fun h i -> h + sprintf "%02x" i) ""
    
    let solvePart1 = splitBy "," asIntArray >> sparseHash 256 1 >> (fun s -> s.[0] * s.[1])
    let solvePart2 = Seq.map int >> Seq.toArray >> Array.append >< [|17;31;73;47;23|] >> sparseHash 256 64 >> denseHash
    let solver = { parse = parseFirstLine asString; solvePart1 = solvePart1; solvePart2 = solvePart2 >> toHexStr}

module Day11 = 
    let dist (x, y) = (abs(x) + abs(y)) / 2
    let getDir = function | "n" -> (0, 2) | "s" -> (0, -2) | "ne" -> (1, 1) | "nw" -> (-1, 1) | "se" -> (1, -1) | "sw" -> (-1, -1) | _  -> (0, 0)
    let addDir (x1,y1) (x2,y2) = (x1+x2,y1+y2)
    let step (coords, maxDist) = getDir >> addDir coords >> (fun c -> (c, max maxDist (dist c)))
    let solve = Array.fold step ((0, 0), 0)
    let solver = { parse = parseFirstLine (splitBy "," asStringArray); solvePart1 = solve >> fst >> dist; solvePart2 = solve >> snd}

module Day12 = 
    let asConnections = splitBy ", " asIntArray >> Array.toList
    let asPipe = splitBy " <-> " (Array.item 1 >> asConnections)
    let solvePart1 graph = Graph.getConnectedComponent (List.item >< graph) 0 |> Set.count
    let solvePart2 graph = Graph.getConnectedComponents (List.item >< graph) (Set.ofList [0..(List.length graph - 1)]) |> List.length
    let solver = { parse = parseEachLine asPipe >> Seq.toList; solvePart1 = solvePart1; solvePart2 = solvePart2 }

module Day13 = 
    let collides delay (layer, length) = (delay + layer) % (2 * (length - 1)) = 0
    let getScore = List.filter (collides 0) >> (List.sumBy (fun l -> fst l * snd l))
    let rec findValid delay layers = if (List.exists (collides delay) layers) then findValid (delay + 1) layers else delay
    let asLayer = splitBy ": " (fun l -> (int l.[0], int l.[1]))
    let solver = { parse = parseEachLine asLayer >> Seq.toList; solvePart1 = getScore; solvePart2 = findValid 0 }

module Day14 = 
    let toBinStr (i : int) = Convert.ToString(i, 2).PadLeft(8, '0')
    let getHash key i = Day10.solvePart2 (sprintf "%s-%i" key i) |> Array.fold (fun h i -> h + toBinStr i) "" 
    let hashToCoords i = Seq.mapi (fun j h -> ((i, j), h)) >> Seq.filter (snd >> ((=) '1')) >> Seq.map fst >> Set.ofSeq
    let getActiveCoords key = Seq.map (getHash key) [0..127] |> Seq.mapi hashToCoords |> Set.unionMany
    let getSurroundingNodes activeCoords (i, j) = [(i-1, j); (i+1, j); (i, j-1); (i, j+1)] |> List.filter (Set.contains >< activeCoords)
    let solvePart2 = getActiveCoords >> (fun coords -> Graph.getConnectedComponents (getSurroundingNodes coords) coords)
    let solver = { parse = parseFirstLine asString; solvePart1 = getActiveCoords >> Set.count ; solvePart2 = solvePart2 >> List.length}

module Day15 = 
    let lcg g x = g * x % 2147483647UL
    let rec lcg2 mask g x = match lcg g x with | next when (next &&& mask = 0UL) -> next | next -> lcg2 mask g next

    let asSeed = splitBy "with " (Array.item 1 >> uint64)
    let solve genA genB iterations seeds = 
        let seedA, seedB = Seq.head seeds, Seq.tail seeds |> Seq.head
        let rec solve' a b count = function
        | 0 -> count
        | n -> 
            let a, b = genA 16807UL a, genB 48271UL b
            let i = if (a &&& 0xFFFFUL) = (b &&& 0xFFFFUL) then 1 else 0
            solve' a b (count + i) (n - 1)
        solve' seedA seedB 0 iterations
    let solver = { parse = parseEachLine asSeed; solvePart1 = solve lcg lcg 40_000_000; solvePart2 = solve (lcg2 3UL) (lcg2 7UL) 5_000_000 }

module Day16 = 
    type DanceMove =
        | Spin of int
        | Exchange of int * int
        | Partner of int * int
    
    let asMove (move : string) = 
        match move.[0] with
        | 's' -> Spin (move.[1..] |> int)
        | 'x' -> Exchange (move.[1..] |> splitBy "/" (fun x -> (int x.[0], int x.[1])))
        | 'p' -> Partner (int move.[1] - int 'a', int move.[3] - int 'a')
        | _ -> Spin 0

    let swap (i, j) (arr : 'a []) = 
        let valI = arr.[i]
        arr.[i] <- arr.[j]
        arr.[j] <- valI
        arr

    let performMove order = function
        | Spin i -> Array.append (Array.skip (16 - i) order) (Array.take (16-i) order)
        | Exchange (a, b) -> swap (a, b) order
        | Partner (a, b) -> swap ((Array.findIndex ((=) a) order), (Array.findIndex ((=)b) order)) order
    
    let orderToStr = Array.map ((+) (int 'a') >> char) >> String.Concat
    let performNDances n moves =
        let performDance order = Array.fold performMove order moves
        let rec performNDances' dances order = function
            | 0 -> orderToStr order
            | x when List.contains (orderToStr order) dances -> List.item (n % (n - x)) dances
            | x -> performNDances' (dances @ [orderToStr order]) (performDance order) (x - 1)
        performNDances' List.empty [|0..15|] n
    
    let solver = { parse = parseFirstLine (splitBy "," (Array.map asMove)); solvePart1 = performNDances 1; solvePart2 = performNDances 1_000_000_000 }

let runSolver day =
    let run solver fileName =
        let time f x = Stopwatch.StartNew() |> (fun sw -> (f x, sw.Elapsed.TotalMilliseconds))
        let timePart part solve =
            let (_, t) = time solve (fileName |> File.ReadLines |> solver.parse)
            printfn "Day %02i-%i %7.2fms" day part t
        let runPart part solve = 
            printfn "Day %02i-%i %A" day part (fileName |> File.ReadLines |> solver.parse |> solve)
        runPart 1 solver.solvePart1
        runPart 2 solver.solvePart2
    match day with
    | 1  -> run Day1.solver  | 2  -> run Day2.solver  | 3  -> run Day3.solver  | 4  -> run Day4.solver
    | 5  -> run Day5.solver  | 6  -> run Day6.solver  | 7  -> run Day7.solver  | 8  -> run Day8.solver
    | 9  -> run Day9.solver  | 10 -> run Day10.solver | 11 -> run Day11.solver | 12 -> run Day12.solver
    | 13 -> run Day13.solver | 14 -> run Day14.solver | 15 -> run Day15.solver | 16 -> run Day16.solver
    | day -> (fun _ -> printfn "Invalid Problem: %i" day)

[<EntryPoint>]
let main argv =
    let runDay day = runSolver day (sprintf "input_files\\day%i.txt" day)
    match argv.[0] with
        | "ALL" -> for i in 1..16 do runDay i
        | x -> runDay (int x)
    Console.ReadKey() |> ignore
    0