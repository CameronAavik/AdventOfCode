module Year2015Day18

open CameronAavik.AdventOfCode.Common

let parseChar c = match c with | '#' -> true | '.' -> false | _ -> failwithf "%A" c

let parse = parseEachLine (Seq.map parseChar >> Seq.toArray) >> Seq.toArray

let step1 (lights : bool [] []) =
    let h = lights.Length   
    let w = lights.[0].Length
    lights
    |> Array.mapi (fun y row ->
        row
        |> Array.mapi (fun x cell ->
            let cur = lights.[y].[x]
            let mutable c = 0
            for y2 = (max 0 (y-1)) to (min (h - 1) (y + 1)) do
                for x2 = (max 0 (x-1)) to (min (w - 1) (x + 1)) do
                    if ((x2 = x && y = y2) |> not) && (lights.[y2].[x2]) then
                        c <- c + 1
            ((c = 2 || c = 3)&& cur) || (not cur && (c = 3))))

let solvePart1 input =
    let rec stepN lights n =
        if n = 0 then lights
        else stepN (step1 lights) (n - 1)
    stepN input 100
    |> Seq.sumBy (fun r -> Seq.filter id r |> Seq.length)

let step2 (lights : bool [] []) =
    let h = lights.Length   
    let w = lights.[0].Length
    lights
    |> Array.mapi (fun y row ->
        row
        |> Array.mapi (fun x cell ->
            if x = 0 && y = 0 then true
            elif x = 0 && y = h - 1 then true
            elif x = w - 1 && y = 0 then true
            elif x = w - 1 && y = h - 1 then true
            else
            let cur = lights.[y].[x]
            let mutable c = 0
            for y2 = (max 0 (y-1)) to (min (h - 1) (y + 1)) do
                for x2 = (max 0 (x-1)) to (min (w - 1) (x + 1)) do
                    if ((x2 = x && y = y2) |> not) && (lights.[y2].[x2]) then
                        c <- c + 1
            ((c = 2 || c = 3)&& cur) || (not cur && (c = 3))))

let solvePart2 input =
    let rec stepN lights n =
        if n = 0 then lights
        else stepN (step2 lights) (n - 1)
    stepN input 100
    |> Seq.sumBy (fun r -> Seq.filter id r |> Seq.length)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }