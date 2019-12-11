module Year2019Day10

open System
open CameronAavik.AdventOfCode.Common

let parseLine = Seq.map (function | '#' -> true | _ -> false) >> Seq.toArray
let parse = parseEachLine parseLine >> Seq.toArray

// returns all rotations (in the form of an x, y delta) that can point to another point on the grid
// will not return two rotations that are colinear.
let getAllPossibleRotations (x, y) (width, height) =
    let rec gcd x y =
        if y = 0 then x
        else gcd y (x % y)

    seq { 
        for dx = -x to (width - x - 1) do
            for dy = -y to (height - y - 1) do
                if (dx, dy) <> (0, 0) && (abs (gcd dx dy) = 1) then
                    (dx, dy) }

let dimensions asteroids =
    let height = Array.length asteroids
    let width = Array.length asteroids.[0]
    width, height

let getAsteroid asteroids (x, y) =
    Array.tryItem y asteroids
    |> Option.bind (Array.tryItem x)

let rec asteroidsInRange asteroids (x, y) (dx, dy) =
    let x', y' = (x + dx, y + dy)
    match getAsteroid asteroids (x', y') with
    | Some true -> (x', y') :: (asteroidsInRange asteroids (x', y') (dx, dy))
    | Some false -> asteroidsInRange asteroids (x', y') (dx, dy) 
    | None -> []

let getVisibleAsteroids asteroids (x, y) =
    getAllPossibleRotations (x, y) (dimensions asteroids)
    |> Seq.map (asteroidsInRange asteroids (x, y))
    |> Seq.choose List.tryHead
    |> Set.ofSeq
    |> Set.count

let getBestAsteroid asteroids =
    let width, height = dimensions asteroids
    let asteroidVisibilities =
        seq {
            for y = 0 to height - 1 do
                let row = asteroids.[y]
                for x = 0 to width - 1 do
                    if row.[x] then
                        (x, y), getVisibleAsteroids asteroids (x, y) }
    Seq.maxBy snd asteroidVisibilities

let solvePart1 asteroids =
    snd (getBestAsteroid asteroids)

let angleFromVertical (dx, dy) =
    Math.Atan2(float (dx), float(-dy)) + (if dx < 0 then 100. else 0.)

let solvePart2 asteroids = 
    let x, y = fst (getBestAsteroid asteroids)

    let ax, ay =
        getAllPossibleRotations (x, y) (dimensions asteroids)
        |> Seq.sortBy angleFromVertical
        |> Seq.map (asteroidsInRange asteroids (x, y))
        |> Seq.filter (List.isEmpty >> not)
        |> Seq.item 199
        |> List.head

    ax * 100 + ay

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }