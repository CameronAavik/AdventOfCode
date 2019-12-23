module Year2019Day22

open CameronAavik.AdventOfCode.Common
open System.Numerics

// modulo that handles negative numbers properly
let (%!) n m = ((n % m) + m) % m

// modular inversion
let modInv x n = BigInteger.ModPow(x, n - 2I, n)

type Shuffle =
    | Deal
    | Cut of int
    | Incr of int

let asShuffle line =
    let parts = splitBy " " id line
    match parts.[0], parts.[1] with
    | "cut", amount -> Cut (int amount)
    | "deal", "into" -> Deal
    | "deal", "with" -> Incr (int parts.[3])
    | _ -> failwithf "Invalid shuffle: %s" line

type Affine =
    { Scale : bigint
      Shift : bigint
      Mod : bigint }

    static member combine aff1 aff2 =
        { Scale = (aff1.Scale * aff2.Scale) %! aff1.Mod
          Shift = (aff2.Scale * aff1.Shift + aff2.Shift) %! aff1.Mod
          Mod = aff1.Mod }

    static member inverse aff =
        let invScale = modInv aff.Scale aff.Mod
        { aff with Scale = invScale; Shift = -(invScale * aff.Shift) %! aff.Mod }

    static member nTimes n aff =
        if n < 0I then Affine.nTimes -n (Affine.inverse aff)
        else
            let newScale = BigInteger.ModPow(aff.Scale, n, aff.Mod)
            let newShift = aff.Shift * (newScale - 1I) * (modInv (aff.Scale - 1I) aff.Mod)
            { aff with Scale = newScale; Shift = newShift }

    static member apply x aff = (aff.Scale * x + aff.Shift) %! aff.Mod

let shuffleToAffine deckSize =
    function
    | Deal ->   { Mod = deckSize; Scale = -1I;      Shift = deckSize - 1I }
    | Cut i ->  { Mod = deckSize; Scale = 1I;       Shift = deckSize - (bigint i) }
    | Incr i -> { Mod = deckSize; Scale = bigint i; Shift = 0I }

let solve deckSize iterations position =
    Seq.map (shuffleToAffine deckSize)
    >> Seq.reduce Affine.combine
    >> Affine.nTimes iterations
    >> Affine.apply position

let solvePart1 = solve 10007I 1I 2019I
let solvePart2 = solve 119315717514047I -101741582076661I 2020I

let solver = { parse = parseEachLine asShuffle; part1 = solvePart1; part2 = solvePart2 }