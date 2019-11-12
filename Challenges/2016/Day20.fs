module Year2016Day20

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine (splitBy "-" (fun i -> int64 i.[0], int64 i.[1])) >> Seq.toArray

let getValidIpRanges lines =
    let joins = lines |> Array.map (fun (s, _) -> (s, 1))
    let leaves = lines |> Array.map (fun (_, e) -> (e, -1))

    let together =
        Array.append joins leaves
        |> Array.sort

    let step count (value, diff) =
        let newCount = count + diff
        (value, newCount), newCount

    let temps, _ = Array.mapFold step 0 together

    temps
    |> Seq.pairwise
    |> Seq.where (fun ((v1, c1), (v2, c2)) -> c1 = 0 && c2 = 1 && v2 - v1 > 1L)
    |> Seq.map (fun ((v1, _), (v2, _)) -> (v1 + 1L, v2 - 1L))

let solvePart1 lines = getValidIpRanges lines |> Seq.head |> fst
let solvePart2 lines = getValidIpRanges lines |> Seq.sumBy (fun (s, e) -> e - s + 1L)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }