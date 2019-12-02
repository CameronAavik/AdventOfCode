module Year2015Day15

open CameronAavik.AdventOfCode.Common

type Ingredient = { Capacity : int; Durability : int; Flavor : int; Texture : int; Calories : int }

let props ingr = [| ingr.Capacity; ingr.Durability; ingr.Flavor; ingr.Texture |]

let parseIngredient line =
    let ints = extractInts line
    { Capacity = ints.[0]; Durability = ints.[1]; Flavor = ints.[2]; Texture = ints.[3]; Calories = ints.[4] }

let parse = parseEachLine parseIngredient

let solvePart1 input =
    let ings = input |> Seq.toArray
    seq {
        for i = 0 to 100 do
            for j = 0 to (100 - i) do
                for k = 0 to (100 - i - j) do
                    for l = 0 to (100 - i - j - k) do
                        let mults = [|i; j; k; l|]
                        [ 0 .. 3 ]
                        |> Seq.map (fun prop ->
                            [0 .. 3]
                            |> Seq.sumBy (fun ingI -> (props ings.[ingI]).[prop] * mults.[ingI])
                            |> max 0)
                        |> Seq.reduce (*) } |> Seq.max

let solvePart2 input =
    let ings = input |> Seq.toArray
    seq {
        for i = 0 to 100 do
            for j = 0 to (100 - i) do
                for k = 0 to (100 - i - j) do
                    let l = 100 - i - j - k
                    let mults = [|i; j; k; l|]
                    let calories = [0 .. 3] |> Seq.sumBy (fun ingI -> ings.[ingI].Calories * mults.[ingI])
                    if calories = 500 then
                        [ 0 .. 3 ]
                        |> Seq.map (fun prop ->
                            [0 .. 3]
                            |> Seq.sumBy (fun ingI -> (props ings.[ingI]).[prop] * mults.[ingI])
                            |> max 0)
                        |> Seq.reduce (*) } |> Seq.max

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }