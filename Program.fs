namespace CameronAavik.AdventOfCode

open System
open System.Linq

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
    // curry and uncurry for making working with tuples easier
    let curry f a b = f (a,b)
    let uncurry f (a,b) = f a b

    // every day has a corresponding Day record which defines how to parse the file, then two functions for soving each part respectively
    type Day<'a, 'b, 'c> = {parse: string seq -> 'a; part1: 'a -> 'b; part2: 'a -> 'c}

    // we had two problems which involved counting the number of connected components, so I abstracted it out
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

module Year2017 =
    module Day1 =
        let charToDigit c = int c - int '0'
        let solve windowSize captcha = 
            Seq.append captcha captcha
            |> Seq.windowed windowSize
            |> Seq.take (Seq.length captcha)
            |> Seq.filter (fun w -> Seq.head w = Seq.last w)
            |> Seq.sumBy (Seq.head >> charToDigit)

        let part2 captcha = solve ((Seq.length captcha / 2) + 1) captcha
        let solver = {parse = parseFirstLine asString; part1 = solve 2; part2 = part2}

    module Day2 = 
        let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
        let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
        let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

        let part1 = Seq.sumBy getLargestDiff
        let part2 = Seq.sumBy getDivisor
        let solver = {parse = parseEachLine (splitBy "\t" asIntArray); part1 = part1; part2 = part2}

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

        // this somehow manages to calculate the next x and y position given the current position in the spiral. I forget how it works.
        let getNextPos (x, y) = 
            if (y <= 0) && (x <= -y) && (y <= x) then (x + 1, y)
            elif (x > 0) && (y < x) then (x, y + 1)
            elif (y > 0) && (-x < y) then (x - 1, y)
            else (x, y - 1)
            
        let part2 target = 
            let rec solve grid posMap newPos = 
                let newValue = getNextValue grid posMap newPos
                if newValue > target then newValue 
                else solve (grid @ [ newValue ]) (Map.add newPos grid.Length posMap) (getNextPos newPos)
            solve List.empty Map.empty (0, 0)

        let solver = {parse = parseFirstLine asInt; part1 = manhattanDistance; part2 = part2}

    module Day4 = 
        let isUnique lst =
            let rec isUnique' seen = function 
                | [] -> true 
                | x :: xs -> if Set.contains x seen then false else isUnique' (Set.add x seen) xs
            isUnique' Set.empty lst
        let sortedString (str : string) = str |> Seq.sort |> String.Concat
        let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length
        let solver = {parse = parseEachLine (splitBy " " asStringArray >> Array.toList); part1 = solve id; part2 = solve (List.map sortedString)}

    module Day5 = 
        let solve modifyOffset offsets = 
            // I took inspiration from https://github.com/mstksg/advent-of-code-2017/blob/master/src/AOC2017/Day05.hs and used a zipper
            let rec solve' total ls x rs n = 
                if   n = 0 then solve' (total + 1) ls (modifyOffset x) rs x
                elif n < 0 then match ls with | l :: ls' -> solve' total ls' l (x :: rs) (n + 1) | [] -> total
                else            match rs with | r :: rs' -> solve' total (x :: ls) r rs' (n - 1) | [] -> total
            solve' 0 [] (Seq.head offsets) (Seq.tail offsets |> Seq.toList) 0
    
        let modifyPart2 x = if x >= 3 then (x - 1) else (x + 1)
        let solver = {parse = parseEachLine asInt; part1 = solve ((+) 1); part2 = solve modifyPart2}

    module Day6 = 
        let banksToStr = Array.map (fun i -> i.ToString()) >> String.concat ","

        let distribute numBanks maxV maxI i v =
            let doesOverlap = ((if i <= maxI then numBanks else 0) + i) <= (maxI + (maxV % numBanks))
            (if i = maxI then 0 else v) + (maxV / numBanks) + (if doesOverlap then 1 else 0)
    
        let solve = 
            let rec solve' seen c banks = 
                if (Map.containsKey (banksToStr banks) seen) then (seen, banks)
                else
                    let maxV = Array.max banks
                    let maxI = Array.findIndex ((=) maxV) banks
                    solve' (Map.add (banksToStr banks) c seen) (c + 1) (Array.mapi (distribute (Seq.length banks) maxV maxI) banks)
            solve' Map.empty 0

        let part1 = solve >> fst >> Map.count
        let part2 = solve >> (fun (seen, banks) -> (Map.count seen) - (Map.find (banksToStr banks) seen))
        let solver = {parse = parseFirstLine (splitBy "\t" asIntArray); part1 = part1; part2 = part2}

    module Day7 =
        let getSubTowers (tokens : string array) = if Array.length tokens = 2 then Array.empty else tokens.[3..] |> Array.map (fun c -> c.TrimEnd(','))
        let asProgram = splitBy " " (fun tokens -> (tokens.[0], (tokens.[1].Trim('(',')') |> int, getSubTowers tokens)))

        let rec findRoot tower currentProgram = 
            match Map.tryFindKey (fun _ (_, children) -> Array.contains currentProgram children) tower with
            | None -> currentProgram
            | Some parent -> findRoot tower parent

        let rec getWeight tower node = 
            let weight, children = Map.find node tower
            weight + (Array.sumBy (getWeight tower) children)

        let getChildrenWeights tower = 
            Seq.map (fun c -> (getWeight tower c, Map.find c tower |> fst)) 
            >> Seq.groupBy fst 
            >> Seq.sortByDescending (fun (k, g) -> Seq.length g) 
            >> Seq.toArray

        let getMissingWeight tower = 
            tower 
            |> Map.map (fun _ (_, children) -> getChildrenWeights tower children)
            |> Map.toSeq
            |> Seq.filter (fun (_, v) -> (Array.length v) = 2)
            |> Seq.map (fun (_, v) -> (snd v.[1] |> Seq.head |> snd) + (fst v.[0]) - (fst v.[1]))
            |> Seq.min

        let part1 tower = findRoot tower (Seq.head tower).Key
        let solver = {parse = parseEachLine asProgram >> Map.ofSeq; part1 = part1; part2 = getMissingWeight}

    module Day8 =
        let parseIncOrDec = function "inc" -> (+) | "dec" -> (-) | _ -> (fun _ x -> x)
        let parseOperator = function ">" -> (>) | "<" -> (<) | ">=" -> (>=) | "<=" -> (<=) | "==" -> (=) | "!=" -> (<>) | _ -> (fun _ _ -> false)
        let asInstruction = splitBy " " (fun t -> (t.[0], (parseIncOrDec t.[1]) >< (int t.[2]), t.[4], (parseOperator t.[5]) >< (int t.[6])))

        let simulate (var1, incOrDec, var2, passesComparisonCheck) vars = 
            let val1, val2 = getOrDefault var1 vars 0, getOrDefault var2 vars 0
            if passesComparisonCheck val2 then Map.add var1 (incOrDec val1) vars else vars
        let maxVar = Map.toSeq >> Seq.map snd >> Seq.max
    
        let solve = Seq.mapFold (fun vars insn -> simulate insn vars |> (fun v -> (maxVar v, v))) Map.empty >> fst
        let solver = {parse = parseEachLine asInstruction; part1 = solve >> Seq.last; part2 = solve >> Seq.max}

    module Day9 = 
        type GarbageState = NotGarbage | Garbage | Cancelled
        type State = {level: int; state: GarbageState; score: int; garbage: int }

        let step current nextChar =
            match (current.state, nextChar) with
            | (Garbage, '!') -> {current with state = Cancelled}
            | (Garbage, '>') -> {current with state = NotGarbage} 
            | (Garbage, _)   -> {current with garbage = current.garbage + 1}
            | (Cancelled, _) -> {current with state = Garbage}
            | (NotGarbage, '{') -> {current with level = current.level + 1}
            | (NotGarbage, '}') -> {current with level = current.level - 1; score = current.score + current.level}
            | (NotGarbage, '<') -> {current with state = Garbage}
            | _ -> current;

        let solve = Seq.fold step {level=0; state=NotGarbage; score=0; garbage=0}
        let solver = {parse = parseFirstLine asString; part1 = solve >> (fun state -> state.score); part2 = solve >> (fun state -> state.garbage)}

    module Day10 = 
        let repeat n folder init = Seq.fold (fun s _ -> folder s) init [0..(n-1)]

        type State = {hash: int array; skip: int; start: int}
        let revN n s = Array.append (Array.take n s |> Array.rev) (Array.skip n s)
        let shift n s = Array.append (Array.skip n s) (Array.take n s)
        let step listLen state length = 
            let shiftLen = ((length + state.skip) % listLen)
            {state with hash = state.hash |> revN length |> shift shiftLen; skip = state.skip + 1; start = (state.start - shiftLen) %! listLen}
    
        let solveRound listLen init = Array.fold (step listLen) init
        let sparseHash listLen n lengths = 
            repeat n (solveRound listLen >< lengths) {hash=[|0..(listLen-1)|]; skip=0; start=0} 
            |> (fun s -> shift s.start s.hash)
        let denseHash = Array.chunkBySize 16 >> Array.map (Array.fold (^^^) 0)
        let strToDenseHash = Seq.map int >> Seq.toArray >> Array.append >< [|17;31;73;47;23|] >> sparseHash 256 64 >> denseHash

        let part1 = splitBy "," asIntArray >> sparseHash 256 1 >> (fun s -> s.[0] * s.[1])
        let toHexStr = Array.fold (fun h i -> h + sprintf "%02x" i) ""
        let solver = {parse = parseFirstLine asString; part1 = part1; part2 = strToDenseHash >> toHexStr}

    module Day11 = 
        let dist (x, y) = (abs(x) + abs(y)) / 2
        let getDir = function | "n" -> (0, 2) | "s" -> (0, -2) | "ne" -> (1, 1) | "nw" -> (-1, 1) | "se" -> (1, -1) | "sw" -> (-1, -1) | _  -> (0, 0)
        let addDir (x1,y1) (x2,y2) = (x1+x2,y1+y2)
        let step coords = getDir >> addDir coords >> (fun c -> (dist c, c))
        let solve = Array.mapFold step (0, 0) >> fst
        let solver = {parse = parseFirstLine (splitBy "," asStringArray); part1 = solve >> Array.last; part2 = solve >> Array.max}

    module Day12 = 
        let asConnections = splitBy ", " asIntArray >> Array.toList
        let asPipe = splitBy " <-> " (Array.item 1 >> asConnections)
        let part1 graph = Graph.getConnectedComponent (List.item >< graph) 0 |> Set.count
        let part2 graph = Graph.getConnectedComponents (List.item >< graph) (set [0..(List.length graph - 1)]) |> List.length
        let solver = {parse = parseEachLine asPipe >> Seq.toList; part1 = part1; part2 = part2}

    module Day13 = 
        let collides delay (layer, length) = (delay + layer) % (2 * (length - 1)) = 0
        let getScore = List.filter (collides 0) >> (List.sumBy (fun l -> fst l * snd l))
        let rec findValid delay layers = if (List.exists (collides delay) layers) then findValid (delay + 1) layers else delay
        let asLayer = splitBy ": " (fun l -> (int l.[0], int l.[1]))
        let solver = {parse = parseEachLine asLayer >> Seq.toList; part1 = getScore; part2 = findValid 0}

    module Day14 = 
        let toBinStr (i : int) = Convert.ToString(i, 2).PadLeft(8, '0')
        let getHash key i = Day10.strToDenseHash (sprintf "%s-%i" key i) |> Array.fold (fun h i -> h + toBinStr i) "" 
        let hashToCoords i = Seq.mapi (fun j h -> ((i, j), h)) >> Seq.filter (snd >> ((=) '1')) >> Seq.map fst >> Set.ofSeq
        let getActiveCoords key = Seq.map (getHash key) [0..127] |> Seq.mapi hashToCoords |> Set.unionMany
        let getSurroundingNodes activeCoords (i, j) = [(i-1, j); (i+1, j); (i, j-1); (i, j+1)] |> List.filter (Set.contains >< activeCoords)
        let part2 = getActiveCoords >> (fun coords -> Graph.getConnectedComponents (getSurroundingNodes coords) coords)
        let solver = {parse = parseFirstLine asString; part1 = getActiveCoords >> Set.count; part2 = part2 >> List.length}

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
        let solver = {parse = parseEachLine asSeed; part1 = solve lcg lcg 40_000_000; part2 = solve (lcg2 3UL) (lcg2 7UL) 5_000_000}

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
    
        let solver = {parse = parseFirstLine (splitBy "," (Array.map asMove)); part1 = performNDances 1; part2 = performNDances 1_000_000_000}

    module Day17 = 
        let getInsertPositions i skip = List.fold (fun l n -> (((List.head l) + skip) % n + 1) :: l) [0] (List.init i ((+)1))
        let rec findTarget target = function
            | [] -> 0
            | x :: xs when x = target -> List.length xs
            | x :: xs -> findTarget (target + if x < target then - 1 else 0) xs 
        let part1 = getInsertPositions 2017 >> (fun pos -> findTarget ((List.head pos) + 1) pos)

        let rec part2 afterZero i n skip = 
            if n = 50000001 then afterZero 
            else (i + skip) % n |> (fun next -> part2 (if next = 0 then n else afterZero) (next + 1) (n + 1) skip)
        let solver = {parse = parseFirstLine asInt; part1 = part1; part2 = part2 0 0 1}

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
        let part1 = defaultComp >> findRecover >< 0L
        let part2 = defaultComp >> (fun comp -> findDeadlock comp (updateRegister "p" 1L comp) 0)
        let solver = {parse = parseEachLine asInstruction; part1 = part1; part2 = part2}

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
        let solver = {parse = parseEachLine asString >> Seq.toList; part1 = solve >> (fun s -> s.letters); part2 = solve >> (fun s -> s.steps)}

    module Day20 =
        let asVector (s : string) = splitBy "," (Array.map int64) s.[3..(s.Length-2)] |> (fun v -> (v.[0],v.[1],v.[2]))
        let asParticle = splitBy ", " (Array.map asVector >> (fun vecs -> (vecs.[0],vecs.[1],vecs.[2])))
    
        let addVec (x1,y1,z1) (x2,y2,z2) = (x1 + x2, y1 + y2, z1 + z2)
        let tickParticle = (fun (p, v, a) -> (p, addVec v a, a)) >> (fun (p, v, a) -> (addVec p v, v, a))
        let dist (x, y, z) = abs x + abs y + abs z
    
        let ticksToSimulate = 200
        let solve transformList = 
            let tickAll = List.map (fun (pos, p) -> (pos, tickParticle p)) >> transformList
            let rec tick t particles = if t = ticksToSimulate then particles else tick (t + 1) (tickAll particles)
            List.mapi (fun i v -> i, v) >> tick 0
    
        let filterColliding = List.groupBy (fun (_, (p, _, _)) -> p) >> List.filter (fun (_, l) -> List.tail l = []) >> List.collect snd
        let part1 = solve id >> List.minBy (fun (_, (p, v, a)) -> (dist a, dist v, dist p)) >> fst
        let part2 = solve filterColliding >> List.length

        let solver = {parse = parseEachLine asParticle >> Seq.toList; part1 = part1; part2 = part2}

    module Day21 =
        let maxIndex p = Array2D.length1 p - 1
        let flatten grid = seq {for x in [0..maxIndex grid] do
                                    for y in [0..maxIndex grid] do 
                                        yield grid.[x, y]}
        let asRule = splitBy " => " (Array.map (splitBy "/" array2D) >> (fun arr -> (arr.[0], arr.[1])))
        let gridToStr = flatten >> Seq.fold (fun str x -> str + x.ToString()) ""
        let genPerms (pattern, out) =
            let flip (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[x, maxIndex p - y]) p
            let rotate (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[maxIndex p - y, x]) p
            let rec gen p = seq { yield p; yield (flip p); yield! gen (rotate p)}
            gen pattern |> Seq.take 8 |> Seq.map (fun p -> (gridToStr p, out))
    
        let iterate grid (rules : Map<string, char[,]>) =
            let currentGridSize = maxIndex grid + 1
            let subSize = if (currentGridSize % 2) = 0 then 2 else 3
            let size = currentGridSize / subSize
            let subgrids = seq {for x in [0..size-1] do 
                                    for y in [0..size-1] do 
                                        yield grid.[subSize*x .. subSize*(x+1)-1, subSize*y .. subSize*(y+1)-1]}
            let enhancedSubgrids = subgrids |> Seq.map gridToStr |> Seq.map (Map.find >< rules) |> Seq.toArray
            let subSize' = subSize + 1
            Array2D.init (subSize' * size) (subSize' * size) (fun x y -> enhancedSubgrids.[(x/subSize' * size) + (y/subSize')].[x%subSize',y%subSize'])
            
        let getActiveCount = flatten >> Seq.filter (fun c -> c = '#') >> Seq.length
        let solve iterations rules =
            let rec getIterations grid = seq { yield grid; yield! getIterations (iterate grid rules)}
            getIterations (array2D [".#.";"..#";"###"]) |> Seq.item iterations |> getActiveCount
        
        let solver = {parse = parseEachLine asRule >> Seq.collect genPerms >> Map.ofSeq; part1 = solve 5; part2 = solve 18}

    module Day22 =
        open System.Collections.Generic
        // tuples are slow when used as a map/dictionary key for some reason, convert the coord to a long instead. Value determined by n^2+n=Int64.MaxValue
        let toHash (x, y) = x + 3037000500L * y
        let toGridMap lines = 
            // Map was too slow, we use a mutable Dictionary instead
            let grid = new Dictionary<int64, int>();
            let center = (String.length (Seq.head lines)) / 2
            let defaultEntries = 
                lines 
                |> Seq.mapi (fun i r -> Seq.mapi (fun j c -> ((j-center, i-center), if c = '#' then 2 else 0)) r) 
                |> Seq.collect id
            for (x, y), v in defaultEntries do
                grid.[toHash (int64 x, int64 y)] <- v
            grid
    
        let move (x, y) = function 0 -> (x, y - 1L) | 1 -> (x + 1L, y) | 2 -> (x, y + 1L) | 3 -> (x - 1L, y) | _ -> (x, y)
        let solve jump iterations (grid : Dictionary<int64, int>) = 
            let rec step pos dir infected = function
                | 0 -> infected
                | n ->
                    let node = grid.GetValueOrDefault(toHash pos, 0)
                    let dir', node' = (dir + node + 3) % 4, (node + jump) % 4
                    grid.[toHash pos] <- node'
                    step (move pos dir') dir' (infected + if node' = 2 then 1 else 0) (n - 1)
            step (0L, 0L) 0 0 iterations

        let solver = {parse = parseEachLine asString >> toGridMap; part1 = solve 2 10000; part2 = solve 1 10000000}

    module Day23 =
        let rec checkPrimes limit n d = if d = limit then true elif n % d = 0 then false else checkPrimes limit n (d + 1)
        let isPrime n = if n < 2 then false else checkPrimes (float n |> sqrt |> ceil |> int) n 2
        let parse = parseFirstLine (splitBy " " asStringArray >> Array.item 2 >> int)
        let part2 x = 
            (x + 1000) * 100 
            |> (fun n -> [n..17..n+17000]) 
            |> List.filter (isPrime >> not) 
            |> List.length
        let solver = {parse = parse; part1 = (fun n -> (n - 2) * (n - 2)); part2 = part2}

    module Day24 =
        let asComponent = splitBy "/" asIntArray >> (fun a -> a.[0], a.[1])
        let strength = List.sumBy (fun c -> fst c + snd c)
        let rec build bridge next components =
            seq { yield bridge
                  // if we have a component which bridges to itself that can connect, then always use this. This cuts down a lot of branches in the DFS
                  if Set.contains (next, next) components then yield! build ((next, next) :: bridge) next (Set.remove (next, next) components)
                  else
                      let bridgeable = Set.filter (fun c -> fst c = next || snd c = next) components
                      for comp in bridgeable do
                          let next' = if snd comp = next then fst comp else snd comp
                          yield! build (comp :: bridge) next' (Set.remove comp components) }
        let solve maximiser = set >> build [] 0 >> Seq.maxBy maximiser >> strength
        let solver = {parse = parseEachLine asComponent; part1 = solve strength; part2 = solve (fun c -> (List.length c, strength c))}

    module Day25 =
        // I frequently needed the last word without the last letter (unnecessary punctuation)
        let lastWord line = splitBy " " Array.last line |> (fun word -> word.Remove(word.Length - 1))
        let parseInstruction (lines : string list) = 
            let getAction i = (lastWord lines.[i] |> int, lastWord lines.[i + 1] = "left", lastWord lines.[i + 2])
            (lastWord lines.[0], (getAction 2, getAction 6))
        let parseInstructions lines = lines |> Seq.skip 3 |> Seq.chunkBySize 10 |> Seq.map (Seq.toList >> parseInstruction) |> Map.ofSeq
        let parseBlueprint lines =
            let initState = lastWord (Seq.head lines)
            let steps = Seq.item 1 lines |> splitBy " " (fun words -> int words.[5])
            (initState, steps, parseInstructions lines)
    
        // more zipper stuff, for weird reasons I could not figure out how to abstract out the zipper logic without taking a performance hit
        let solve (initState, steps, instructions) =
            let rec step ls x rs state = function
                | 0 -> Seq.sum ls + x + Seq.sum rs
                | n ->
                    let instruction = Map.find state instructions
                    let (newValue, isLeft, newState) = if x = 0 then fst instruction else snd instruction
                    let newLs, newX, newRs = 
                        if isLeft then match ls with l :: ls' -> (ls', l, newValue :: rs) | [] -> ([], 0, newValue :: rs)
                        else           match rs with r :: rs' -> (newValue :: ls, r, rs') | [] -> (newValue :: ls, 0, [])
                    step newLs newX newRs newState (n - 1)
            step [] 0 [] initState steps

        let solver = {parse = parseBlueprint; part1 = solve; part2 = (fun _ -> "Advent of Code Finished!")}

module Year2018 =
    module Day1 =
        let solvePart2 changes =
            let shift = Seq.sum changes
            let getDiff ((xi, xf), (yi, yf)) = if shift > 0 then (yf - xf, xi, yf) else (yf - xf, yi, xf)
            Seq.toArray changes
            |> Array.scan (+) 0
            |> Array.take (Seq.length changes)
            |> Array.mapi (curry id)
            |> Array.groupBy (fun x -> (snd x) %! shift)
            |> Array.map(fun g -> snd g |> Array.sort |> Array.pairwise |> Array.map getDiff)
            |> Array.concat
            |> Array.min

        let solver = {parse = parseEachLine asInt; part1 = Seq.sum; part2 = solvePart2 >> (fun (_, _, f) -> f)}