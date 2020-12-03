module Year2018Day11

open AdventOfCode.FSharp.Common

let getPrefixSums serial size =
    let indexArray = Array.init size ((+)1)
    let getPowerLevel x y =
        let rackId = x + 10
        (((rackId * y + serial) * rackId / 100) % 10) - 5
    let getRow (prevRow : int []) row =
        let getPrefixSum prevSum x = 
            let ownValue = getPowerLevel x row
            let overlap = if x > 0 then prevRow.[x - 1] else 0
            prevSum + ownValue + prevRow.[x] - overlap
        indexArray |> Array.scan getPrefixSum 0
    indexArray |> Array.scan getRow (Array.zeroCreate (size + 1)) |> array2D

let getBestSumForSize size s (sums : int [,]) =
    let subGridSum x y =
        sums.[y + s, x + s] - sums.[y + s, x] - sums.[y, x + s] + sums.[y, x]
    seq { for y in 0..size-s do
            for x in 0..size-s do
                yield subGridSum x y, (x, y) } |> Seq.max

let solvePart1 size subsize serial =
    let _, (x, y) =
        getPrefixSums serial size
        |> getBestSumForSize size subsize
    sprintf "%i,%i" (x + 1) (y + 1)

let solvePart2 size serial =
    let sums = getPrefixSums serial size
    let (_, (x, y)), subsize =
        Array.init 300 ((+)1) // for each subsize
        |> Array.Parallel.map (fun i -> (getBestSumForSize size i sums, i))
        |> Array.max
    sprintf "%i,%i,%i" (x + 1) (y + 1) subsize

let solver = {parse = parseFirstLine asInt; part1 = solvePart1 300 3; part2 = solvePart2 300}