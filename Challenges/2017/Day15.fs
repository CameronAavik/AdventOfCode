module Year2017Day15

open CameronAavik.AdventOfCode.Common

let lcg g x = g * x % 2147483647UL
let rec lcg2 mask g x = match lcg g x with | next when (next &&& mask = 0UL) -> next | next -> lcg2 mask g next

let asSeed = splitBy "with " (Array.item 1 >> uint64)
let solve genA genB iterations seeds = 
    let seedA, seedB = Seq.head seeds, Seq.tail seeds |> Seq.head
    let rec solve' a b count = function
    | 0 -> count
    | n -> 
        let a, b = genA 16807UL a, genB 48271UL b
        let i = if (a &&& 0xFFFFUL) = (b &&& 0xFFFFUL) then 1 else 0
        solve' a b (count + i) (n - 1)
    solve' seedA seedB 0 iterations
let solver = {parse = parseEachLine asSeed; part1 = solve lcg lcg 40_000_000; part2 = solve (lcg2 3UL) (lcg2 7UL) 5_000_000}