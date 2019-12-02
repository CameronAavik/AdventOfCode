module Year2015Day24

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine asInt >> Seq.map int64 >> Seq.toArray

let getCombinations (target : int64) (weights : int64 []) =
    let rec getCombinations' target (weightIndex : int) =
        match Array.tryItem weightIndex weights with
        | None -> Seq.empty
        | Some w ->
            if target = w then seq { (2 <<< int32 w), 1, w }
            elif target < w then Seq.empty
            else seq {
                yield! getCombinations' (target - w) (weightIndex + 1) |> Seq.map (fun (c, ct, qe) -> (c ||| (2 <<< int32 w)), ct + 1, w * qe) 
                yield! getCombinations' target (weightIndex + 1) }
    getCombinations' target 0

let solvePart1 input =
    let totalWeight = input |> Array.sum
    let target = totalWeight / 3L
    let allCombinations = getCombinations target (Array.sort input) |> Seq.toArray
    let _, _, qe =
        allCombinations
        // The below two lines are necessary if the minimal value does not divide evenly.
        //|> Array.sortBy (fun (c, ct, qe) -> ct, qe)
        //|> Array.find (fun (c1, _, _) -> Array.exists (fun (c2, _, _) -> c1 &&& c2 = 0) allCombinations)
        |> Array.minBy (fun (_, ct, qe) -> ct, qe)
    qe

let solvePart2 input =
    let totalWeight = input |> Array.sum
    let target = totalWeight / 4L
    let allCombinations = getCombinations target (Array.sort input) |> Seq.toArray
    // The commented out lines are necessary if the minimal value does not divide evenly.
    //let allCombinationPairs = getCombinations (target * 2L) (Array.sort input) |> Seq.toArray
    let _, _, qe =
        allCombinations
        //|> Array.sortBy (fun (_, ct, qe) -> ct, qe)
        //|> Array.find (fun (c1, _, _) -> Array.exists (fun (c2, _, _) -> c1 &&& c2 = 0) allCombinationPairs)
        |> Array.minBy (fun (_, ct, qe) -> ct, qe)
    qe

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }