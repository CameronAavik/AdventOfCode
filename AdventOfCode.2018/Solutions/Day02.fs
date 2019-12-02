module Year2018Day02

open CameronAavik.AdventOfCode.Common
open System

let solvePart1 gifts =
    let containsCount i = Seq.countBy id >> Seq.map snd >> Seq.contains i
    let countContains i = Seq.map (containsCount i) >> Seq.filter id >> Seq.length
    (countContains 2 gifts) * (countContains 3 gifts)

let solvePart2 gifts =
    let replaceNth str n = String.mapi (fun i x -> if i = n then '_' else x) str
    let removeUnderscore s = Seq.filter ((<>)'_') s |> String.Concat
    let duplicate, _ =
        gifts
        |> Seq.map (fun str -> Seq.init (String.length str) (replaceNth str))
        |> Seq.concat
        |> Seq.countBy id
        |> Seq.find (fun (_, s) -> s = 2)
    removeUnderscore duplicate

let solver = {parse = parseEachLine asString; part1 = solvePart1; part2 = solvePart2}