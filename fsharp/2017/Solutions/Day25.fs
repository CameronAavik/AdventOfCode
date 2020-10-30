module Year2017Day25

open AdventOfCode.FSharp.Common
open System.IO

// I frequently needed the last word without the last letter (unnecessary punctuation)
let lastWord line = splitBy " " Array.last line |> (fun word -> word.Remove(word.Length - 1))
let parseInstruction (lines : string list) = 
    let getAction i = (lastWord lines.[i] |> int, lastWord lines.[i + 1] = "left", lastWord lines.[i + 2])
    (lastWord lines.[0], (getAction 2, getAction 6))
let parseInstructions lines = lines |> Seq.skip 3 |> Seq.chunkBySize 10 |> Seq.map (Seq.toList >> parseInstruction) |> Map.ofSeq
let parseBlueprint lines =
    let initState = lastWord (Seq.head lines)
    let steps = Seq.item 1 lines |> splitBy " " (fun words -> int words.[5])
    (initState, steps, parseInstructions lines)
    
// more zipper stuff, for weird reasons I could not figure out how to abstract out the zipper logic without taking a performance hit
let solve (initState, steps, instructions) =
    let rec step ls x rs state = function
        | 0 -> Seq.sum ls + x + Seq.sum rs
        | n ->
            let instruction = Map.find state instructions
            let (newValue, isLeft, newState) = if x = 0 then fst instruction else snd instruction
            let newLs, newX, newRs = 
                if isLeft then match ls with l :: ls' -> (ls', l, newValue :: rs) | [] -> ([], 0, newValue :: rs)
                else           match rs with r :: rs' -> (newValue :: ls, r, rs') | [] -> (newValue :: ls, 0, [])
            step newLs newX newRs newState (n - 1)
    step [] 0 [] initState steps

let solver = {parse = id; part1 = File.ReadLines >> parseBlueprint >> solve; part2 = (fun _ -> "Advent of Code Finished!")}