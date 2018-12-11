module Year2017Day23

open CameronAavik.AdventOfCode.Common

let rec checkPrimes limit n d = if d = limit then true elif n % d = 0 then false else checkPrimes limit n (d + 1)
let isPrime n = if n < 2 then false else checkPrimes (float n |> sqrt |> ceil |> int) n 2
let parse = parseFirstLine (splitBy " " asStringArray >> Array.item 2 >> int)
let part2 x = 
    (x + 1000) * 100 
    |> (fun n -> [n..17..n+17000]) 
    |> List.filter (isPrime >> not) 
    |> List.length
let solver = {parse = parse; part1 = (fun n -> (n - 2) * (n - 2)); part2 = part2}