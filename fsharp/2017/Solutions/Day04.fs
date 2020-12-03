module Year2017Day04

open AdventOfCode.FSharp.Common
open System

let isUnique lst =
    let rec isUnique' seen = function 
        | [] -> true 
        | x :: xs -> if Set.contains x seen then false else isUnique' (Set.add x seen) xs
    isUnique' Set.empty lst
let sortedString (str : string) = str |> Seq.sort |> String.Concat
let solve mapper = Seq.map mapper >> Seq.filter isUnique >> Seq.length
let solver = {parse = parseEachLine (splitBy " " asStringArray >> Array.toList); part1 = solve id; part2 = solve (List.map sortedString)}