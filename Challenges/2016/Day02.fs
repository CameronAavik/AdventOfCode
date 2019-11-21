module Year2016Day02

open CameronAavik.AdventOfCode.Common

type Move = Up | Left | Down | Right
let parseMove = function
    | 'L' -> Left
    | 'R' -> Right
    | 'U' -> Up
    | _ -> Down

let asInstructions = Seq.map parseMove >> Seq.toArray

let parse = parseEachLine asInstructions

let changeIndex (x, y) move =
    match move with
    | Left -> (max 0 (x-1), y)
    | Right -> (min 2 (x + 1), y)
    | Down -> (x, (min 2 (y+1)))
    | Up -> (x, (max 0 (y-1)))

let changeIndex2 (x, y) move =
    match move with
    | Left -> ((max (abs (2-y)) (x-1)), y)
    | Right -> (min (4-(abs (2-y))) (x + 1), y)
    | Down -> (x, (min (4-(abs (2-x))) (y+1)))
    | Up -> (x, (max (abs (2-x)) (y-1)))

let code1 = [|
    [|'1'; '2'; '3'|]
    [|'4'; '5'; '6'|]
    [|'7'; '8'; '9'|]
|]

let code2 = [|
    [|'0'; '0'; '1'; '0'; '0' |]
    [|'0'; '2'; '3'; '4'; '0' |]
    [|'5'; '6'; '7'; '8'; '9' |]
    [|'0'; 'A'; 'B'; 'C'; '0' |]
    [|'0'; '0'; 'E'; '0'; '0' |]
|]

let solve changeI (code : char [] []) start data =
    let applyMoves sequence start =
        Array.fold changeI start sequence
    let apply (state, pass) seq =
        let x, y = applyMoves seq state
        (x, y), (pass + string code.[y].[x])
    Seq.fold apply (start, "") data |> snd

let solvePart1 = solve changeIndex code1 (1, 1)
let solvePart2 = solve changeIndex2 code2 (0, 2)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }