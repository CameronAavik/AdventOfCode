module Year2019Day03

open CameronAavik.AdventOfCode.Common

type Dir = L | R | D | U
let parseDir = 
    function 
    | 'L' -> L | 'R' -> R | 'D' -> D | 'U' -> U 
    | c -> failwithf "Invalid Dir: %c" c

let moveDir (x, y) = 
    function
    | U -> (x, y + 1)
    | D -> (x, y - 1)
    | R -> (x + 1, y)
    | L -> (x - 1, y)

let parseStride (item : string) =
    let dir = parseDir item.[0]
    let length = int item.[1..]
    dir, length

let asTwoWires wires = 
    Seq.item 0 wires, Seq.item 1 wires

let parse = parseEachLine (splitBy "," (Array.map parseStride)) >> asTwoWires

type WireSignalState = { Pos : int * int; Points : Map<int * int, int>; Delay : int }

let updatePoints state =
    let setIfNotExists k v m = if Map.containsKey k m then m else Map.add k v m
    { state with Points = setIfNotExists state.Pos state.Delay state.Points }

let rec moveByStride (dir, length) state =
    if length = 0 then state
    else
        { state with Pos = moveDir state.Pos dir; Delay = state.Delay + 1 }
        |> updatePoints
        |> moveByStride (dir, length - 1)

let getAllPoints wireStrides =
    let initialState = { Pos = 0, 0; Points = Map.empty; Delay = 0 }
    let finalState = wireStrides |> Array.fold (fun state stride -> moveByStride stride state) initialState
    finalState.Points
            
let mapKeys m = m |> Map.toSeq |> Seq.map fst |> set

let solvePart1 (wire1, wire2) =
    let points1 = getAllPoints wire1
    let points2 = getAllPoints wire2
    Set.intersect (mapKeys points1) (mapKeys points2)
    |> Seq.map (fun (x, y) -> abs x + abs y)
    |> Seq.min
    
let solvePart2 (wire1, wire2) = 
    let points1 = getAllPoints wire1
    let points2 = getAllPoints wire2
    Set.intersect (mapKeys points1) (mapKeys points2)
    |> Seq.map (fun pos -> points1.[pos] + points2.[pos])
    |> Seq.min

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }