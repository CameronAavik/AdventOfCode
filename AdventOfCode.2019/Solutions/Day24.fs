module Year2019Day24

open CameronAavik.AdventOfCode.Common
open System.Numerics

let lineToCells = Seq.map (fun c -> if c = '#' then 1 else 0)
let gridToBits = Seq.indexed >> Seq.sumBy (fun (i, v) -> v * pown 2 i)
let parse = parseEachLine lineToCells >> Seq.collect id >> gridToBits

let getBit n b = (b >>> n) &&& 1
let getBitAt (x, y) n = getBit (y * 5 + x) n

type Dir = Left | Right | Up | Down
let neighbours (x, y) =
    [| (x - 1, y), Left
       (x + 1, y), Right
       (x, y - 1), Up
       (x, y + 1), Down |]

let nextState state getBugCountAt =
    [| 0 .. 24 |]
    |> Array.map (fun c ->
        let cur = getBit c state
        let x, y = c % 5, c / 5
        let neighbourBugs = neighbours (x, y) |> Array.sumBy (fun (pos, dir) -> getBugCountAt pos dir)
        if (neighbourBugs = 1) || (cur = 0 && neighbourBugs = 2) then 1 else 0)
    |> gridToBits

let solvePart1 (bugs) =
    let getBugsAt n (x, y) _ =
        if x >= 0 && x < 5 && y >= 0 && y < 5 then
            getBitAt (x, y) n
        else 0
        
    let rec stepUntilRepeat seen cells =
        if Set.contains cells seen then cells
        else nextState cells (getBugsAt cells) |> stepUntilRepeat (Set.add cells seen)

    stepUntilRepeat Set.empty bugs

let stepLayer layer layers =
    let prevLayer = Map.tryFind (layer - 1) layers |> Option.defaultValue 0
    let nextLayer = Map.tryFind (layer + 1) layers |> Option.defaultValue 0
    let curLayer = layers.[layer]

    let getPositionsOfSide =
        function
        | Left ->  [| for y in 0 .. 4 -> (4, y)|]
        | Right -> [| for y in 0 .. 4 -> (0, y)|]
        | Up ->    [| for x in 0 .. 4 -> (x, 4)|]
        | Down ->  [| for x in 0 .. 4 -> (x, 0)|]

    let getBugsAt (x, y) dir =
        if (x, y) = (2, 2) then getPositionsOfSide dir |> Array.sumBy (fun pos -> getBitAt pos nextLayer)
        elif x < 0 then getBitAt (1, 2) prevLayer
        elif x >= 5 then getBitAt (3, 2) prevLayer
        elif y < 0 then getBitAt (2, 1) prevLayer
        elif y >= 5 then getBitAt (2, 3) prevLayer
        else getBitAt (x, y) curLayer

    nextState curLayer getBugsAt

let stepAllLayers atTime layers =
    let layers' =
        layers
        |> Map.add (-atTime - 1) 0
        |> Map.add (atTime + 1) 0
    Map.map (fun k _ -> stepLayer k layers') layers'

let getBugCount bits = BitOperations.PopCount(uint32 (bits &&& ~~~(1 <<< 12)))

let solvePart2 (cells) =
    let layers = Map.empty |> Map.add 0 cells
    let iterations = 200

    let rec repeatUntilTime time layers =
        if time = iterations then layers
        else stepAllLayers time layers |> repeatUntilTime (time + 1)

    repeatUntilTime 0 layers 
    |> Map.toSeq
    |> Seq.sumBy (snd >> getBugCount)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }