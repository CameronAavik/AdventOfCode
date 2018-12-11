module Year2017Day05

open CameronAavik.AdventOfCode.Common

let solve modifyOffset offsets = 
    // I took inspiration from https://github.com/mstksg/advent-of-code-2017/blob/master/src/AOC2017/Day05.hs and used a zipper
    let rec solve' total ls x rs n = 
        if   n = 0 then solve' (total + 1) ls (modifyOffset x) rs x
        elif n < 0 then match ls with | l :: ls' -> solve' total ls' l (x :: rs) (n + 1) | [] -> total
        else            match rs with | r :: rs' -> solve' total (x :: ls) r rs' (n - 1) | [] -> total
    solve' 0 [] (Seq.head offsets) (Seq.tail offsets |> Seq.toList) 0
    
let modifyPart2 x = if x >= 3 then (x - 1) else (x + 1)
let solver = {parse = parseEachLine asInt; part1 = solve ((+) 1); part2 = solve modifyPart2}