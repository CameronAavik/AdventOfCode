module Year2017Day01

open CameronAavik.AdventOfCode.Common

let charToDigit c = int c - int '0'
let solve windowSize captcha = 
    Seq.append captcha captcha
    |> Seq.windowed windowSize
    |> Seq.take (Seq.length captcha)
    |> Seq.filter (fun w -> Seq.head w = Seq.last w)
    |> Seq.sumBy (Seq.head >> charToDigit)

let part2 captcha = solve ((Seq.length captcha / 2) + 1) captcha
let solver = { parse = parseFirstLine asString; part1 = solve 2; part2 = part2 }