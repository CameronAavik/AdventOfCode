module Year2019Day24

open AdventOfCode.FSharp.Common
open System.Numerics

let lineToCells = Seq.map (fun c -> if c = '#' then 1 else 0)
let parse = parseEachLine lineToCells >> Seq.collect id >> Seq.rev >> Seq.reduce (fun i j -> (i <<< 1) + j)

// Gets the number of 1s in the binary representation of x
let inline popcount x = BitOperations.PopCount(uint32 x)

let neighbours (x, y) =
    [| (x - 1, y)
       (x + 1, y)
       (x, y - 1)
       (x, y + 1) |]

// pre-compute the bitmasks to get the neighbours for a given cell
let neighbourBitmasks =
    [| for i = 0 to 24 do
        neighbours (i % 5, i / 5)
        |> Array.filter (fun (x, y) -> x >= 0 && x < 5 && y >= 0 && y < 5)
        |> Array.sumBy (fun (x, y) -> 1 <<< (y * 5 + x)) |]

// loops over the bits of the bugs and map them to their new state using a callback to get the neighbouring bug counts
let inline nextState getNeighbourBugCount bugs =
    let rec nextState' c newBugs =
        if c < 0 then newBugs
        else
            let cur = (bugs >>> c) &&& 1
            let neighbourBugs = getNeighbourBugCount c
            let bit = if (neighbourBugs = 1) || (cur = 0 && neighbourBugs = 2) then 1 else 0
            nextState' (c - 1) ((newBugs <<< 1) ||| bit)
    nextState' 24 0

let solvePart1 =
    let getNeighbourBugCount bugs c = popcount (neighbourBitmasks.[c] &&& bugs)
    let step bugs = nextState (getNeighbourBugCount bugs) bugs
        
    repeatUntilDuplicate step

// pre-computes the bitmasks for getting the neighbours in the outer layer
let outerLayerBitmasks =
    [| for i = 0 to 24 do
        let x, y = i % 5, i / 5
        let prevLayerXMask =
            match x with
            | 0 -> 1 <<< (5*2 + 1)
            | 4 -> 1 <<< (5*2 + 3)
            | _ -> 0
        let prevLayerYMask =
            match y with
            | 0 -> 1 <<< (5*1 + 2)
            | 4 -> 1 <<< (5*3 + 2)
            | _ -> 0
        prevLayerXMask ||| prevLayerYMask |]

// steps a single layer at the given layerIndex
let stepLayer layers layerIndex layer =
    // Get the inner and outer layer if they exist
    let innerLayer = if layerIndex = Array.length layers - 1 then 0 else layers.[layerIndex + 1]
    let outerLayer = if layerIndex = 0 then 0 else layers.[layerIndex - 1]

    // If all 3 are zero, then the current layer will remain 0
    if layer = 0 && outerLayer = 0 && innerLayer = 0 then 0
    else

    // Callback for getting the neighbouring bug count
    let getNeighbourBugCount c =
        // by handling (2, 2) with 0, we ensure that we are never putting a bug in the middle where the inner layer is
        if c = 12 (* x=2, y=2 *) then 0
        else
        let curLayerCount = popcount (neighbourBitmasks.[c] &&& layer)
        let outerLayerCount = popcount (outerLayerBitmasks.[c] &&& outerLayer)

        // For the 4 positions which look at the inner layer, use a bitmask to get the number of neighbouring bugs
        let innerLayerCount =
            match c with
            | 13 (*x=3, y=2*) -> popcount (0b10000_10000_10000_10000_10000 &&& innerLayer)
            | 11 (*x=1, y=2*) -> popcount (0b00001_00001_00001_00001_00001 &&& innerLayer)
            | 7  (*x=2, y=1*) -> popcount (0b00000_00000_00000_00000_11111 &&& innerLayer)
            | 17 (*x=2, y=3*) -> popcount (0b11111_00000_00000_00000_00000 &&& innerLayer)
            | _ -> 0
        curLayerCount + innerLayerCount + outerLayerCount
    nextState getNeighbourBugCount layer

let solvePart2 bugs =
    let inline step layers = Array.Parallel.mapi (stepLayer layers) layers
    let N = 200

    // We create an array of size N + 1 because it takes 2 minutes for bugs to reach a new layer
    // Therefore after N iterations we will N/2 layers on either side of the original layer
    // Since our arrays are zero-indexed, we place layer 0 at index N/2
    Array.init (N + 1) (fun i -> if i = (N / 2) then bugs else 0)
    |> repeatN N step
    |> Array.sumBy popcount

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }