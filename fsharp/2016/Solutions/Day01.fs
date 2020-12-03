module Year2016Day01

open AdventOfCode.FSharp.Common

type Dir = Left | Right

let parseDir (dir : string) =
    if dir.[0] = 'L' then
        Left, int dir.[1..]
    else
        Right, int dir.[1..]

let parse = parseFirstLine (splitBy ", " (Array.map parseDir)) 

let changeDir dir (dx, dy, px, py) =
    let ndx, ndy = 
        if dir = Left then
            (dy, -dx)
        else
            (-dy, dx)
    (ndx, ndy, px, py)

let move dist (dx, dy, px, py) =
    (dx, dy, px+dx*dist, py+dy*dist)

let goDir cur (dir, dist) =
    cur |> changeDir dir |> move dist

let solvePart1 data =
    let _, _, x, y = Seq.fold goDir (0, -1, 0, 0) data
    abs x + abs y

let solvePart2 data =
    let rec goDir' cur seen steps data =
        let _, _, px, py = cur
        if Set.contains (px, py) seen then
            abs px + abs py
        else
            if steps > 0 then
                goDir' (move 1 cur) (Set.add (px, py) seen) (steps - 1) data
            else
                match data with
                | (dir, dist) :: ds ->
                    let newPos = changeDir dir cur
                    goDir' newPos seen (dist) ds
                | [] -> abs px + abs py
    goDir' (0, -1, 0, 0) Set.empty 0 (data |> Array.toList)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }