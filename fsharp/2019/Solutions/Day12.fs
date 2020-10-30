module Year2019Day12

open AdventOfCode.FSharp.Common

let toDimensions moons =
    let xDim = moons |> Array.map (fun m -> Array.item 0 m, 0)
    let yDim = moons |> Array.map (fun m -> Array.item 1 m, 0)
    let zDim = moons |> Array.map (fun m -> Array.item 2 m, 0)
    [| xDim; yDim; zDim |]

let parse = parseEachLine extractInts >> Seq.toArray >> toDimensions

let getAccel x y =
    if x < y then 1
    elif x > y then -1
    else 0

let step moons =
    moons
    |> Array.map (fun p -> Array.fold (fun (p1, v1) (p2, _) -> (p1, v1 + getAccel p1 p2)) p moons)
    |> Array.map (fun (p, v) -> (p + v, v))
    
let rec stepN n dim =
    if n = 0 then dim
    else step dim |> stepN (n - 1)

let totalEnergy dims =
    let totalEnergyForMoon moon =
        let potential = dims |> Array.sumBy (Array.item moon >> fst >> abs)
        let kinetic = dims |> Array.sumBy (Array.item moon >> snd >> abs)
        potential * kinetic
    let m = Array.length (Array.head dims)
    [| 0 .. m - 1|] |> Array.sumBy totalEnergyForMoon

let solvePart1 dims =
    dims
    |> Array.map (stepN 1000)
    |> totalEnergy
    
let stepsUntilEqual dim =
    let rec stepsUntilEqual' n dim' =
        if dim = dim' && n > 0L then n
        else step dim' |> stepsUntilEqual' (n + 1L)
    stepsUntilEqual' 0L dim

let rec gcd a b =
    if b = 0L then abs a
    else gcd b (a % b)

let solvePart2 dims = 
    dims
    |> Array.map stepsUntilEqual
    |> Array.reduce (fun a b -> a * b / (gcd a b))

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }