module Year2018Day02

open CameronAavik.AdventOfCode.Common
open System

let solvePart1 gifts =
    let containsCount i = Seq.groupBy id >> Seq.exists (fun (_, k) -> Seq.length k = i)
    let countContains i = Seq.map (containsCount i) >> Seq.filter id >> Seq.length
    (countContains 2 gifts) * (countContains 3 gifts)

let solvePart2 gifts =
    let replaceNth str n = String.mapi (fun i x -> if i = n then '_' else x) str
    gifts
    |> Seq.map (fun str -> Seq.init (String.length str) (replaceNth str))
    |> Seq.concat
    |> Seq.groupBy id
    |> Seq.find (fun (_, s) -> Seq.length s = 2)
    |> (fun (s, _) -> Seq.filter ((<>)'_') s |> String.Concat)

let solver = {parse = parseEachLine asString; part1 = solvePart1; part2 = solvePart2}