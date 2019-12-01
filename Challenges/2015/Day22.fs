module Year2015Day22

open CameronAavik.AdventOfCode.Common

type Entity = { HP : int; Damage : int; Armor : int; Mana : int; IsBoss : bool }
type Spell = { ManaCost : int; InstantDamage : int; Heal : int; ArmorBoost : int; ExtraBossDamage : int; ManaRegen : int; Duration : int }
let newSpell cost duration = { ManaCost = cost; Duration = duration; InstantDamage = 0; Heal = 0; ArmorBoost = 0; ExtraBossDamage = 0; ManaRegen = 0 }

let spells = [
    { newSpell 53 0 with InstantDamage = 4 }
    { newSpell 73 0 with InstantDamage = 2; Heal = 2 }
    { newSpell 113 6 with ArmorBoost = 7 }
    { newSpell 173 3 with ExtraBossDamage = 3 }
    { newSpell 229 5 with ManaRegen = 101 }
]

let solvePart1 input =
    input

let solvePart2 input =
    input

let parseBoss (lines : (int []) seq) =
    let lineArr = lines |> Seq.toArray
    { HP = lineArr.[0].[0]; Damage = lineArr.[1].[0]; Armor = 0; Mana = 0; IsBoss = true }

let parse = parseEachLine extractInts >> parseBoss

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }