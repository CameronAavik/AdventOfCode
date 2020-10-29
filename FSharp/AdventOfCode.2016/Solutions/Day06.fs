module Year2016Day06

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine asString

let toInds = Seq.mapi (fun i v -> (i, v))

let getMostCommon isDescending =    
    let sortFn = if isDescending then Seq.sortByDescending else Seq.sortBy
    Seq.groupBy id
    >> sortFn (fun (k1, v1) -> Seq.length v1)
    >> Seq.head
    >> fst

let solve isDescending =
    Seq.map toInds
    >> Seq.concat
    >> Seq.groupBy fst
    >> Seq.map (snd >> getMostCommon isDescending)
    >> Seq.sortBy fst
    >> Seq.map snd
    >> charsToStr

let solvePart1 = solve true
let solvePart2 = solve false

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }