module Year2015Day21

open AdventOfCode.FSharp.Common

type Item = { Cost : int; Damage : int; Armor : int }

let weapons = [|
    { Cost = 8; Damage = 4; Armor = 0 }
    { Cost = 10; Damage = 5; Armor = 0 }
    { Cost = 25; Damage = 6; Armor = 0 }
    { Cost = 40; Damage = 7; Armor = 0 }
    { Cost = 74; Damage = 8; Armor = 0 }
|]

let armors  = [|
    { Cost = 13; Damage = 0; Armor = 1 }
    { Cost = 31; Damage = 0; Armor = 2 }
    { Cost = 53; Damage = 0; Armor = 3 }
    { Cost = 75; Damage = 0; Armor = 4 }
    { Cost = 102; Damage = 0; Armor = 5 }
|]

let rings = [|
    { Cost = 25; Damage = 1; Armor = 0 }
    { Cost = 50; Damage = 2; Armor = 0 }
    { Cost = 100; Damage = 3; Armor = 0 }
    { Cost = 20; Damage = 0; Armor = 1 }
    { Cost = 40; Damage = 0; Armor = 2 }
    { Cost = 80; Damage = 0; Armor = 3 }
|]

type Player = { HP : int; Damage : int; Armor : int }

type GameState = { You : Player; Enemy : Player; Winner : bool option }

let getAttackDamage player1 player2 =
    max 1 (player1.Damage - player2.Armor)

let takePlayerTurn isYou state =
    match state.Winner with
    | None ->
        let attacker = if isYou then state.You else state.Enemy
        let attackee = if isYou then state.Enemy else state.You
        let damage = getAttackDamage attacker attackee
        let damagedAttackee = { attackee with HP = max 0 (attackee.HP - damage) }
        let winner = if damagedAttackee.HP = 0 then Some isYou else None
        if isYou then { state with Enemy = damagedAttackee; Winner = winner}
        else { state with You = damagedAttackee; Winner = winner}
    | Some _ -> state

let takeTurn state = 
    state
    |> takePlayerTurn true
    |> takePlayerTurn false

let simulateGame player enemy =
    let rec simulate state =
        match state.Winner with
        | Some w -> w
        | None -> takeTurn state |> simulate
    let initState = { You = player; Enemy = enemy; Winner = None }
    simulate initState

type ChosenItems = { Weapon : Item; Armor : Item option; Ring1 : Item option; Ring2 : Item option; Ring3 : Item option }
let asItemList items = [| Some items.Weapon; items.Armor; items.Ring1; items.Ring2; items.Ring3 |] |> Array.choose id
let summary items =
    items
    |> asItemList
    |> Array.reduce (fun { Cost = c1; Damage = d1; Armor = a1 } { Cost = c2; Damage = d2; Armor = a2 } -> { Cost = c1 + c2; Damage = d1 + d2; Armor = a1 + a2 })

let toPlayer (itemSummary : Item) =
    { HP = 100; Damage = itemSummary.Damage; Armor = itemSummary.Armor }

// yes, I know this is disgusting
let allItemsCombos =
    seq { for w in weapons do
            for a in armors do
                for r1 in rings do
                    for r2 in rings do
                        for r3 in rings do
                            { Weapon = w; Armor = Some a; Ring1 = Some r1; Ring2 = Some r2; Ring3 = Some r3 }
                        { Weapon = w; Armor = Some a; Ring1 = Some r1; Ring2 = Some r2; Ring3 = None }
                    { Weapon = w; Armor = Some a; Ring1 = Some r1; Ring2 = None; Ring3 = None }
                { Weapon = w; Armor = Some a; Ring1 = None; Ring2 = None; Ring3 = None }
            for r1 in rings do
                for r2 in rings do
                    for r3 in rings do
                        { Weapon = w; Armor = None; Ring1 = Some r1; Ring2 = Some r2; Ring3 = Some r3 }
                    { Weapon = w; Armor = None; Ring1 = Some r1; Ring2 = Some r2; Ring3 = None }
                { Weapon = w; Armor = None; Ring1 = Some r1; Ring2 = None; Ring3 = None }
            { Weapon = w; Armor = None; Ring1 = None; Ring2 = None; Ring3 = None }} 
            |> Seq.filter (fun i -> i.Ring1 <> i.Ring2 && i.Ring2 <> i.Ring3 && i.Ring1 <> i.Ring3)
            |> Seq.map summary
            |> Seq.toArray
            |> Array.sortBy (fun i -> i.Cost)
            |> Array.map (fun i -> (i.Cost, toPlayer i))

let solvePart1 boss =
    allItemsCombos
    |> Array.find (fun (_, p) -> simulateGame p boss)
    |> fst

let solvePart2 boss =
    allItemsCombos
    |> Array.findBack (fun (c, p) -> simulateGame p boss |> not)
    |> fst

let parseBoss (lines : (int []) seq) =
    let lineArr = lines |> Seq.toArray
    { HP = lineArr.[0].[0]; Damage = lineArr.[1].[0]; Armor = lineArr.[2].[0] }

let parse = parseEachLine extractInts >> parseBoss

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }