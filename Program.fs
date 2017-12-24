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
    // option coalescing operator
    let (|?) (a: 'a option) b = if a.IsSome then a.Value else b
    // this is a get or default for map
    let getOrDefault key map ``default`` = if Map.containsKey key map then Map.find key map else ``default``

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
    let rec isUnique seen = function [] -> true | x :: xs -> if Set.contains x seen then false else isUnique (Set.add x seen) xs
    let sortedString (str : string) = str |> Seq.sort |> String.Concat
    let solve mapper = Seq.map mapper >> Seq.filter (isUnique Set.empty) >> Seq.length
    let solver = { parse = parseEachLine (splitBy " " asStringArray >> Array.toList); solvePart1 = solve id; solvePart2 = solve (List.map sortedString)}

module Day5 = 
    let solve modifyOffset offsets = 
        let rec solve' total ls x rs n = 
            if   n = 0 then solve' (total + 1) ls (modifyOffset x) rs x
            elif n < 0 then match ls with | l :: ls' -> solve' total ls' l (x :: rs) (n + 1) | [] -> total
            else            match rs with | r :: rs' -> solve' total (x :: ls) r rs' (n - 1) | [] -> total
        solve' 0 [] (Seq.head offsets) (Seq.tail offsets |> Seq.toList) 0

    let solvePart2 = solve (fun x -> if x >= 3 then (x - 1) else (x + 1))
    let solver = {parse = parseEachLine asInt; solvePart1 = solve ((+) 1); solvePart2 = solvePart2}

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

    let simulate (var1, incOrDec, var2, passesComparisonCheck) vars = 
        let val1, val2 = getOrDefault var1 vars 0, getOrDefault var2 vars 0
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
    let solvePart2 graph = Graph.getConnectedComponents (List.item >< graph) (set [0..(List.length graph - 1)]) |> List.length
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
    type DanceMove = Spin of int | Exchange of int * int | Partner of int * int
    
    let asMove (move : string) = 
        match move.[0] with
        | 's' -> Spin (move.[1..] |> int)
        | 'x' -> Exchange (move.[1..] |> splitBy "/" (fun x -> (int x.[0], int x.[1])))
        | 'p' -> Partner (int move.[1] - int 'a', int move.[3] - int 'a')
        | _ -> Spin 0

    let swap (i, j) list = list |> List.mapi (fun k h -> if k = i then list.[j] elif k = j then list.[i] else h)
    let performMove order = function
        | Spin i -> (List.skip (16 - i) order) @ (List.take (16-i) order)
        | Exchange (a, b) -> swap (a, b) order
        | Partner (a, b) -> swap ((List.findIndex ((=) a) order), (List.findIndex ((=)b) order)) order
    
    let orderToStr = List.map ((+) (int 'a') >> char) >> String.Concat
    let performNDances n moves =
        let performDance order = Array.fold performMove order moves
        let rec performNDances' dances order = function
            | 0 -> orderToStr order
            | x when List.contains (orderToStr order) dances -> List.item (n % (n - x)) dances
            | x -> performNDances' (dances @ [orderToStr order]) (performDance order) (x - 1)
        performNDances' List.empty [0..15] n
    
    let solver = { parse = parseFirstLine (splitBy "," (Array.map asMove)); solvePart1 = performNDances 1; solvePart2 = performNDances 1_000_000_000 }

module Day17 = 
    let getInsertPositions i skip = List.fold (fun l n -> (((List.head l) + skip) % n + 1) :: l) [0] (List.init i ((+)1))
    let rec findTarget target = function
        | [] -> 0
        | x :: xs when x = target -> List.length xs
        | x :: xs -> findTarget (target + if x < target then - 1 else 0) xs 
    let solvePart1 = getInsertPositions 2017 >> (fun pos -> findTarget ((List.head pos) + 1) pos)

    let rec solvePart2 skip afterZero i n = 
        if n = 50000001 then afterZero 
        else (i + skip) % n |> (fun next -> solvePart2 skip (if next = 0 then n else afterZero) (next + 1) (n + 1))
    let solver = { parse = parseFirstLine asInt; solvePart1 = solvePart1; solvePart2 = (fun skip -> solvePart2 skip 0 0 1)}

module Day18 =
    type Computer = { code: (string * (string array)) list; pc : int; registers: Map<string, int64>; buffer: int64 list }
    let defaultComp code = {code = Seq.toList code; pc = 0; registers = Map.empty; buffer = []}
    // computer helper functions
    let getVal comp value = Int64.TryParse(value) |> (fun (isInt, i) -> if isInt then i else getOrDefault value comp.registers 0L)
    let jump offset comp = {comp with pc = comp.pc + offset}
    let updateRegister register value comp = {comp with registers = Map.add register value comp.registers}
    let queue comp = function | None -> comp | Some x -> {comp with buffer = comp.buffer @ [x]}
    let dequeue comp = {comp with buffer = List.tail comp.buffer}
    let getCurrentInsn comp = List.item comp.pc comp.code
    let isLocked comp = fst (getCurrentInsn comp) = "rcv" && comp.buffer = []
    // returns function which makes the relevant changes to the computer and the value being sent given instruction and rcv buffer
    let applyInsn handleRcvEmptyBuffer get = function
        | ("snd", [| x |]), _ -> jump 1, Some (get x)
        | ("set", [| x; y |]), _ -> updateRegister x (get y) >> jump 1, None
        | ("add", [| x; y |]), _ -> updateRegister x (get x + get y) >> jump 1, None
        | ("mul", [| x; y |]), _ -> updateRegister x (get x * get y) >> jump 1, None
        | ("mod", [| x; y |]), _ -> updateRegister x (get x % get y) >> jump 1, None
        | ("rcv", _), [] -> handleRcvEmptyBuffer, None 
        | ("rcv", [| x |]), x' :: _ ->  updateRegister x x' >> dequeue >> jump 1, None
        | ("jgz", [| x; y |]), _ -> jump (if get x > 0L then int (get y) else 1), None
        | _ -> id, None
    // simulates one clock tick of the computer
    let step onRCV comp = applyInsn onRCV (getVal comp) (getCurrentInsn comp, comp.buffer) |> (fun (app, sent) -> (app comp, sent))
    let step1, step2 = step (jump 1), step id
    // parses a string of the file into an instruction
    let asInstruction = splitBy " " (fun tokens -> (tokens.[0], tokens.[1..]))
    // recursively ticks the clock until a valid recover is found, then returns last sound value
    let rec findRecover comp lastSound = 
        match getCurrentInsn comp with
        | ("rcv", [| x |]) when getVal comp x <> 0L -> lastSound
        | _ -> step1 comp |> (fun (c, s) -> findRecover c (s |? lastSound))
    // recursively ticks the clock until a deadlock is found, then returns number of messages sent from p2
    let rec findDeadlock p1 p2 c = 
        if isLocked p1 && isLocked p2 then c
        else (step2 p1, step2 p2) |> (fun ((c1, s1), (c2, s2)) -> findDeadlock (queue c1 s2) (queue c2 s1) (c + if s2.IsSome then 1 else 0))
    // setups up and calls the recursive methods
    let solvePart1 = defaultComp >> findRecover >< 0L
    let solvePart2 = defaultComp >> (fun comp -> findDeadlock comp (updateRegister "p" 1L comp) 0)
    let solver = {parse = parseEachLine asInstruction; solvePart1 = solvePart1; solvePart2 = solvePart2}

module Day19 =
    type State = {x: int; y: int; dx: int; dy: int; steps: int; letters: string}
    let step state = {state with x=state.x+state.dx; y=state.y+state.dy; steps=state.steps+1}
    let solve (diagram : string list) = 
        let rec move state = 
            match diagram.[state.y].[state.x], state.dx with
            | ' ', _ -> state
            | '+', 0 -> move ({state with dx = (if diagram.[state.y].[state.x-1] = ' ' then 1 else -1); dy = 0} |> step)
            | '+', _ -> move ({state with dy = (if diagram.[state.y-1].[state.x] = ' ' then 1 else -1); dx = 0} |> step)
            | '-', _ | '|', _ -> move (step state)
            | x, _ -> move ({state with letters = state.letters + x.ToString()} |> step)
        move {x=diagram.[0].IndexOf('|'); y=0; dx=0; dy=1; steps=0; letters=""}
    let solver = {parse = parseEachLine asString >> Seq.toList; solvePart1 = solve >> (fun s -> s.letters); solvePart2 = solve >> (fun s -> s.steps)}

module Day20 =
    let asVector (s : string) = splitBy "," (Array.map int64) s.[3..(s.Length-2)] |> (fun v -> (v.[0],v.[1],v.[2]))
    let asParticle = splitBy ", " (Array.map asVector >> (fun vecs -> (vecs.[0],vecs.[1],vecs.[2])))
    
    let addVec (x1,y1,z1) (x2,y2,z2) = (x1 + x2, y1 + y2, z1 + z2)
    let tickParticle = (fun (p, v, a) -> (p, addVec v a, a)) >> (fun (p, v, a) -> (addVec p v, v, a))
    let dist (x, y, z) = abs x + abs y + abs z
    
    let solve transformList = 
        let tickAll = List.map (fun (pos, p) -> (pos, tickParticle p)) >> transformList
        let rec tick t particles = if t = 200 then particles else tick (t + 1) (tickAll particles)
        List.mapi (fun i v -> i, v) >> tick 0
    
    let filterColliding = List.groupBy (fun (_, (p, _, _)) -> p) >> List.filter (fun (_,l) -> List.tail l = []) >> List.collect snd
    let solvePart1 = solve id >> List.minBy (fun (_, (p, v, a)) -> (dist a, dist v, dist p)) >> fst
    let solvePart2 = solve filterColliding >> List.length

    let solver = {parse = parseEachLine asParticle >> Seq.toList; solvePart1 = solvePart1; solvePart2 = solvePart2}

module Day21 =
    let maxIndex p = Array2D.length1 p - 1
    let asRule = splitBy " => " (Array.map (splitBy "/" array2D) >> (fun arr -> (arr.[0], arr.[1])))
    let genPerms (pattern, out) =
        let f (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[x, maxIndex p - y]) p // flips
        let r (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[maxIndex p - y, x]) p // rotates
        let rec gen p = seq { yield p; yield (f p); yield! gen (r p)}
        gen pattern |> Seq.take 8 |> Seq.map (fun p -> (p, out))
    
    let gridToSubgrids grid =
        let s1 = maxIndex grid + 1
        let s2 = if (s1 % 2) = 0 then 2 else 3
        Array2D.init (s1 / s2) (s1 / s2) (fun x y -> grid.[s2*x .. s2*(x+1)-1, s2*y .. s2*(y+1)-1])
    
    let gridMatchesRule grid (rule, _) =
        let sg, sr = maxIndex grid, maxIndex rule
        if sg = sr then List.forall (fun x -> List.forall (fun y -> grid.[x, y] = rule.[x, y]) [0..sg]) [0..sg] else false

    let combineSubgrids (subgrids : 'a [,] [,]) = 
        let s1 = Array2D.length1 subgrids.[0, 0]
        let s2 = s1 * Array2D.length1 subgrids
        Array2D.init s2 s2 (fun x y -> subgrids.[x/s1,y/s1].[x%s1,y%s1])
    
    let getActiveCount grid = seq {for x in [0..maxIndex grid] do for y in [0..maxIndex grid] do yield if grid.[x, y] = '#' then 1 else 0} |> Seq.sum
    let solve iterations rules =
        let enhanceSubgrid grid = List.find (gridMatchesRule grid) rules |> snd
        let iterate = gridToSubgrids >> Array2D.map enhanceSubgrid >> combineSubgrids
        let rec getIterations grid = seq { yield grid; yield! getIterations (iterate grid)}
        getIterations (array2D [".#.";"..#";"###"]) |> Seq.item iterations |> getActiveCount
        
    let solver = {parse = parseEachLine asRule >> Seq.collect genPerms >> Seq.toList; solvePart1 = solve 5; solvePart2 = solve 18}

module Day22 = 
    type Coord = {x: int; y: int}
    let toGridMap grid = 
        let center = (String.length (Seq.head grid)) / 2
        grid |> Seq.mapi (fun i r -> Seq.mapi (fun j c -> ({x=j-center;y= i-center}, if c = '#' then 2 else 0)) r) |> Seq.collect id |> Map.ofSeq

    let move p = function 0 -> {p with y=p.y-1} | 1 -> {p with x=p.x+1} | 2 -> {p with y=p.y+1} | 3 -> {p with x=p.x-1} | _ -> p
    let solve jump iterations initialGrid = 
        let rec step pos dir grid infected = function
            | 0 -> infected
            | n ->
                let node = getOrDefault pos grid 0
                let newDir = (dir + node + 3) % 4
                let newState = (node + jump) % 4
                step (move pos newDir) newDir (Map.add pos newState grid) (infected + if newState = 2 then 1 else 0) (n - 1)
        step {x=0; y=0} 0 initialGrid 0 iterations

    let solver = {parse = parseEachLine asString >> toGridMap; solvePart1 = solve 2 10000; solvePart2 = solve 1 10000000}

let runSolver day =
    let run solver fileName =
        let time f x = Stopwatch.StartNew() |> (fun sw -> (f x, sw.Elapsed.TotalMilliseconds))
        let timePart part solve =
            let (_, t) = time solve (fileName |> File.ReadLines |> solver.parse)
            printfn "Day %02i-%i %8.2fms" day part t
        let runPart part solve = 
            printfn "Day %02i-%i %O" day part (fileName |> File.ReadLines |> solver.parse |> solve)
        runPart 1 solver.solvePart1
        runPart 2 solver.solvePart2
    match day with
    | 1  -> run Day1.solver  | 2  -> run Day2.solver  | 3  -> run Day3.solver  | 4  -> run Day4.solver
    | 5  -> run Day5.solver  | 6  -> run Day6.solver  | 7  -> run Day7.solver  | 8  -> run Day8.solver
    | 9  -> run Day9.solver  | 10 -> run Day10.solver | 11 -> run Day11.solver | 12 -> run Day12.solver
    | 13 -> run Day13.solver | 14 -> run Day14.solver | 15 -> run Day15.solver | 16 -> run Day16.solver
    | 17 -> run Day17.solver | 18 -> run Day18.solver | 19 -> run Day19.solver | 20 -> run Day20.solver
    | 21 -> run Day21.solver | 22 -> run Day22.solver
    | day -> (fun _ -> printfn "Invalid Problem: %i" day)

[<EntryPoint>]
let main argv =
    let runDay day = runSolver day (sprintf "input_files\\day%i.txt" day)
    match argv.[0] with
        | "ALL" -> for i in 1..22 do runDay i
        | x -> runDay (int x)
    Console.ReadKey() |> ignore
    0