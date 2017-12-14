open System
open System.Linq
open System.IO

let getLine = Seq.head
let split (str : string) = str.Split()
let splitOn (c : string) (str : string) = str.Split([| c |], StringSplitOptions.None)
let (><) f a b = f b a
let (%!) a b = (a % b + b) % b

type Day<'a, 'b, 'c> = {parse: string seq -> 'a; solvePart1: 'a -> 'b; solvePart2: 'a -> 'c}

module Day1 =
    let solve captcha windowSize = 
        Seq.append captcha captcha
        |> Seq.windowed windowSize
        |> Seq.take (Seq.length captcha)
        |> Seq.filter (fun w -> Seq.head w = Seq.last w)
        |> Seq.sumBy (Seq.head >> int >> (><) (-) (int '0'))

    let solvePart1 captcha = solve captcha 2
    let solvePart2 captcha = solve captcha ((Seq.length captcha / 2) + 1)
    let solver = { parse = getLine; solvePart1 = solvePart1; solvePart2 = solvePart2}

module Day2 = 
    let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
    let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
    let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

    let parse = Seq.map (split >> Seq.map int)
    let solvePart1 = Seq.sumBy getLargestDiff
    let solvePart2 = Seq.sumBy getDivisor
    let solver = { parse = parse; solvePart1 = solvePart1; solvePart2 = solvePart2}

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
    let solver = { parse = getLine >> int; solvePart1 = manhattanDistance; solvePart2 = solvePart2}

module Day4 = 
    let isUnique sequence = (sequence |> Seq.distinct |> Seq.length) = (sequence |> Seq.length)
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length
    let solver = { parse = Seq.map split; solvePart1 = solve id; solvePart2 = solve (Seq.map sortedString)}

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
    let solver = { parse = parse; solvePart1 = solvePart1; solvePart2 = solvePart2; }

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

    let parse = getLine >> split >> Array.map int
    let solvePart1 = solve Map.empty 0 >> fst >> Map.count
    let solvePart2 = solve Map.empty 0 >> (fun (seen, banks) -> (Map.count seen) - (Map.find (serialiseBanks banks) seen))
    let solver = { parse = parse; solvePart1 = solvePart1; solvePart2 = solvePart2; }

module Day7 =
    let parseLine (tokens : string array) = 
        (tokens.[0], (tokens.[1].Trim('(',')') |> int, if Array.length tokens = 2 then Array.empty else tokens.[3..] |> Array.map (fun c -> c.TrimEnd(','))))

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

    let parse = Seq.map (split >> parseLine)
    let solvePart1 tower = Seq.head tower |> fst |> findRoot tower
    let solvePart2 tower = Map.ofSeq tower |> getMissingWeight
    let solver = { parse = parse; solvePart1 = solvePart1; solvePart2 = solvePart2}

module Day8 =
    let parseIncOrDec = function | "inc" -> (+) | "dec" -> (-) | _ -> (fun _ x -> x)
    let parseOperator = function | ">" -> (>) | "<" -> (<) | ">=" -> (>=) | "<=" -> (<=) | "==" -> (=) | "!=" -> (<>) | _ -> (fun _ _ -> false)
    let parseLine (tokens : string array) = 
        (tokens.[0], (parseIncOrDec tokens.[1]) >< (int tokens.[2]), tokens.[4], (parseOperator tokens.[5]) >< (int tokens.[6]))

    let getOrZero map key = if Map.containsKey key map then Map.find key map else 0
    let simulate (var1, incOrDec, var2, passesComparisonCheck) vars = 
        let val1, val2 = getOrZero vars var1, getOrZero vars var2
        if passesComparisonCheck val2 then Map.add var1 (incOrDec val1) vars else vars
    let maxVar = Map.toSeq >> Seq.map snd >> Seq.max

    let parse = Seq.map (split >> parseLine)
    let solve folder program = Seq.fold (fun (vars, acc) insn -> simulate insn vars |> (folder acc)) (Map.empty, 0) program |> snd
    let solvePart1 = solve (fun _ vars -> (vars, maxVar vars))
    let solvePart2 = solve (fun acc vars -> (vars, max (maxVar vars) acc))
    let solver = { parse = parse; solvePart1 = solvePart1; solvePart2 = solvePart2; }

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
    let solver = { parse = getLine; solvePart1 = solvePart1; solvePart2 = solvePart2; }

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
    
    let solvePart1 = splitOn "," >> Array.map int >> sparseHash 256 1 >> (fun s -> s.[0] * s.[1])
    let solvePart2 = Seq.map int >> Seq.toArray >> Array.append >< [|17;31;73;47;23|] >> sparseHash 256 64 >> denseHash
    let solver = { parse = getLine; solvePart1 = solvePart1; solvePart2 = solvePart2 >> toHexStr}

module Day11 = 
    let dist (x, y) = (abs(x) + abs(y)) / 2
    let getDir = function | "n" -> (0, 2) | "s" -> (0, -2) | "ne" -> (1, 1) | "nw" -> (-1, 1) | "se" -> (1, -1) | "sw" -> (-1, -1) | _  -> (0, 0)
    let addDir (x1,y1) (x2,y2) = (x1+x2,y1+y2)
    let step (coords, maxDist) = getDir >> addDir coords >> (fun c -> (c, max maxDist (dist c)))
    let solve = Array.fold step ((0, 0), 0)
    let solver = { parse = getLine >> splitOn ","; solvePart1 = solve >> fst >> dist; solvePart2 = solve >> snd}

module Day12 = 
    let arrayMinusSet s = Array.filter ((Set.contains >< s) >> not)
    let rec getConnectedComponent (graph : int[][]) connComp = function
        | [| |] -> connComp
        | xs when (Set.contains xs.[0] connComp) -> getConnectedComponent graph connComp (Array.skip 1 xs)
        | xs -> getConnectedComponent graph (connComp.Add(xs.[0])) (graph.[xs.[0]] |> arrayMinusSet connComp |> Array.append (Array.skip 1 xs))

    let getComponentContaining graph = Array.singleton >> getConnectedComponent graph Set.empty
    let rec getAllComponents graph components = function
        | [| |] -> components
        | remaining ->
            let c = getComponentContaining graph (Seq.head remaining)
            getAllComponents graph (c :: components) (arrayMinusSet c remaining)

    let parse = Seq.map (splitOn " <-> " >> (fun t -> splitOn ", " t.[1] |> Array.map int)) >> Seq.toArray
    let solvePart2 graph = getAllComponents graph List.empty [|0..(Array.length graph - 1)|] |> List.length
    let solver = { parse = parse; solvePart1 = getComponentContaining >< 0 >> Set.count; solvePart2 = solvePart2 }

module Day13 = 
    let collides delay (layer, length) = (delay + layer) % (2 * (length - 1)) = 0
    let getScore = Seq.filter (collides 0) >> (Seq.sumBy (fun l -> fst l * snd l))
    let rec findValid delay layers = if (Seq.exists (collides delay) layers) then findValid (delay + 1) layers else delay
    let parse = Seq.map (splitOn ": " >> (fun l -> (int l.[0], int l.[1]))) >> Seq.toArray
    let solver = { parse = parse; solvePart1 = getScore; solvePart2 = findValid 0 }

module Day14 = 
    let toBinStr (i : int) = Convert.ToString(i, 2).PadLeft(8, '0')
    let getHash key i = Day10.solvePart2 (sprintf "%s-%i" key i) |> Array.fold (fun h i -> h + toBinStr i) "" 
    let hashToCoords i = Seq.mapi (fun j h -> ((i, j), h)) >> Seq.filter (snd >> ((=) '1')) >> Seq.map fst >> Set.ofSeq
    let getActiveCoords key = Seq.map (getHash key) [0..127] |> Seq.mapi hashToCoords |> Set.unionMany
    let addSurroundingNodes (i, j) xs = (i-1, j) :: (i+1, j) :: (i, j-1) :: (i, j+1) :: xs
    let rec getComponentCount seen unseen count = function
        | [] when Set.isEmpty unseen -> count
        | [] -> getComponentCount seen unseen (count + 1) [Seq.head unseen]
        | x :: xs when Set.contains x seen || not (Set.contains x unseen) -> getComponentCount seen unseen count xs
        | x :: xs -> getComponentCount (Set.add x seen) (Set.remove x unseen) count (addSurroundingNodes x xs)
    let solvePart2 key = getComponentCount Set.empty (getActiveCoords key) 0 []
    let solver = { parse = getLine; solvePart1 = getActiveCoords >> Set.count ; solvePart2 = solvePart2 }

let runSolver = 
    let run day input = 
        let sw = System.Diagnostics.Stopwatch.StartNew()
        printfn "Part 1: %A (%fms)" (day.solvePart1 (day.parse input)) sw.Elapsed.TotalMilliseconds
        sw.Restart()
        printfn "Part 2: %A (%fms)" (day.solvePart2 (day.parse input)) sw.Elapsed.TotalMilliseconds
    function
    | "Day1"  -> run Day1.solver  | "Day2"  -> run Day2.solver  | "Day3"  -> run Day3.solver  | "Day4"  -> run Day4.solver
    | "Day5"  -> run Day5.solver  | "Day6"  -> run Day6.solver  | "Day7"  -> run Day7.solver  | "Day8"  -> run Day8.solver
    | "Day9"  -> run Day9.solver  | "Day10" -> run Day10.solver | "Day11" -> run Day11.solver | "Day12" -> run Day12.solver
    | "Day13" -> run Day13.solver | "Day14" -> run Day14.solver
    | name -> (fun _ -> printfn "Invalid Problem: %s" name)

[<EntryPoint>]
let main argv =
    let result = argv.[1] |> File.ReadLines |> runSolver argv.[0]
    Console.ReadKey() |> ignore
    0