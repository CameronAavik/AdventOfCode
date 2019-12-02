module Year2017Day11

open CameronAavik.AdventOfCode.Common

let dist (x, y) = (abs(x) + abs(y)) / 2
let getDir = function | "n" -> (0, 2) | "s" -> (0, -2) | "ne" -> (1, 1) | "nw" -> (-1, 1) | "se" -> (1, -1) | "sw" -> (-1, -1) | _  -> (0, 0)
let addDir (x1,y1) (x2,y2) = (x1+x2,y1+y2)
let step coords = getDir >> addDir coords >> (fun c -> (dist c, c))
let solve = Array.mapFold step (0, 0) >> fst
let solver = {parse = parseFirstLine (splitBy "," asStringArray); part1 = solve >> Array.last; part2 = solve >> Array.max}