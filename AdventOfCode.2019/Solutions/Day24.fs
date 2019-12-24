module Year2019Day24

open CameronAavik.AdventOfCode.Common
open System.Numerics

let lineToCells = Seq.map (fun c -> if c = '#' then 1 else 0)
let gridToBits = Seq.indexed >> Seq.sumBy (fun (i, v) -> v * pown 2 i)
let parse = parseEachLine lineToCells >> Seq.collect id >> gridToBits

let inline getBit n b = (b >>> n) &&& 1
let inline getBitAt (x, y) n = getBit (y * 5 + x) n

type Dir = Left | Right | Up | Down
let neighbours (x, y) =
    [| (x - 1, y), Left
       (x + 1, y), Right
       (x, y - 1), Up
       (x, y + 1), Down |]

let nextState state getBugCountAt =
    let rec nextState' c acc =
        if c = 25 then acc
        else
            let cur = getBit c state
            let x, y = c % 5, c / 5
            let neighbourBugs = neighbours (x, y) |> Array.sumBy (fun (pos, dir) -> getBugCountAt pos dir)
            let bit = if (neighbourBugs = 1) || (cur = 0 && neighbourBugs = 2) then 1 else 0
            nextState' (c + 1) ((acc <<< 1) + bit)
    nextState' 0 0

let solvePart1 (bugs) =
    let getBugsAt n (x, y) _ =
        if x >= 0 && x < 5 && y >= 0 && y < 5 then
            getBitAt (x, y) n
        else 0
        
    let rec stepUntilRepeat seen cells =
        if Set.contains cells seen then cells
        else nextState cells (getBugsAt cells) |> stepUntilRepeat (Set.add cells seen)

    stepUntilRepeat Set.empty bugs

let stepLayer layer (layers : int []) =
    let prevLayer = if layer = 0 then 0 else layers.[layer - 1]
    let nextLayer = if layer = layers.Length - 1 then 0 else layers.[layer + 1]
    let curLayer = layers.[layer]

    if curLayer = 0 && prevLayer = 0 && nextLayer = 0 then 0
    else

    let getPositionsOfSide =
        function
        | Left  -> 0b10000_10000_10000_10000_10000
        | Right -> 0b00001_00001_00001_00001_00001
        | Up    -> 0b11111_00000_00000_00000_00000
        | Down  -> 0b00000_00000_00000_00000_11111

    let getBugsAt (x, y) dir =
        if (x, y) = (2, 2) then BitOperations.PopCount(getPositionsOfSide dir &&& nextLayer |> uint64)
        elif x < 0 then getBitAt (1, 2) prevLayer
        elif x >= 5 then getBitAt (3, 2) prevLayer
        elif y < 0 then getBitAt (2, 1) prevLayer
        elif y >= 5 then getBitAt (2, 3) prevLayer
        else getBitAt (x, y) curLayer

    nextState curLayer getBugsAt

let stepAllLayers layers = Array.Parallel.mapi (fun k _ -> stepLayer k layers) layers

let getBugCount bits = BitOperations.PopCount(uint32 (bits &&& ~~~(1 <<< 12)))

let solvePart2 (cells) =
    let rec repeatUntilTime time layers =
        if time = 0 then layers
        else stepAllLayers layers |> repeatUntilTime (time - 1)

    Array.init 201 (fun i -> if i = 100 then cells else 0)
    |> repeatUntilTime 200
    |> Array.sumBy getBugCount

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }