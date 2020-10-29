module Year2017Day02

open CameronAavik.AdventOfCode.Common
open System.Linq

let getLargestDiff ints = (Seq.max ints) - (Seq.min ints)
let isValidDivisor ints i = (Seq.map ((*) i) ints).Intersect(ints).Any()
let getDivisor ints = [2 .. (Seq.max ints)] |> Seq.find (isValidDivisor ints)

let part1 = Seq.sumBy getLargestDiff
let part2 = Seq.sumBy getDivisor
let solver = {parse = parseEachLine (splitBy "\t" asIntArray); part1 = part1; part2 = part2}