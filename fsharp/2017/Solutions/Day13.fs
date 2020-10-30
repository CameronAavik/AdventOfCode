module Year2017Day13

open AdventOfCode.FSharp.Common

let collides delay (layer, length) =
    (delay + layer) % (2 * (length - 1)) = 0

let getScore =
    List.filter (collides 0) 
    >> (List.sumBy (fun l -> fst l * snd l))

let rec findValid delay layers =
    if List.exists (collides delay) layers then
        findValid (delay + 1) layers
    else
        delay

let asLayer = splitBy ": " (fun l -> (int l.[0], int l.[1]))

let solver = { parse = parseEachLine asLayer >> Seq.toList; part1 = getScore; part2 = findValid 0 }