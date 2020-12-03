module Year2015Day05

open AdventOfCode.FSharp.Common

let parse = parseEachLine asString

let isNice1 str =
    let counts = str |> Seq.groupBy id |> Map.ofSeq |> Map.map (fun k v -> Seq.length v)
    let vowels = "aeiou" |> Seq.sumBy (fun c -> Map.tryFind c counts |> Option.defaultValue 0)
    let doubles = str |> Seq.pairwise |> Seq.filter (fun (i, j) -> i = j)
    let hasDouble = doubles |> Seq.isEmpty |> not
    let consecPairs = str |> Seq.pairwise |> Seq.filter (fun (i, j) -> int j - int i = 1 && "acpx".Contains(i))
    let hasConsec = consecPairs |> Seq.isEmpty |> not
    vowels >= 3 && hasDouble && (not hasConsec)

let solvePart1 input =
    input
    |> Seq.where isNice1
    |> Seq.length

let isNice2 (str : string) =
    let triples = str |> Seq.windowed 3 |> Seq.where (fun cs -> cs.[0] = cs.[2])
    let pairsNoOverlap =
        str
        |> Seq.pairwise
        |> Seq.indexed
        |> Seq.groupBy snd
        |> Seq.filter (fun (_, pairs) ->
            let inds = pairs |> Seq.map fst |> Seq.toArray
            inds.Length > 2 || (inds.Length = 2 && abs (inds.[0] - inds.[1]) <> 1))
    
    let hasTriple = triples |> Seq.isEmpty |> not
    let hasPairs = pairsNoOverlap |> Seq.isEmpty |> not
    hasTriple && hasPairs

let solvePart2 input =
    input
    |> Seq.where isNice2
    |> Seq.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }