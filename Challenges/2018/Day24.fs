module Year2018Day24

open CameronAavik.AdventOfCode.Common
open System.Text.RegularExpressions

type DamageType = Slashing | Radiation | Fire | Bludgeoning | Cold
type Team = ImmuneSystem | Infection
type Group = 
    { initiative: int;
      units: int;
      hp: int;
      weaknesses: Set<DamageType>;
      immunities: Set<DamageType>;
      damage: int;
      damageType: DamageType;
      team: Team }

let parseDamageType = function
    | "slashing" -> Slashing
    | "radiation" -> Radiation
    | "fire" -> Fire 
    | "bludgeoning" -> Bludgeoning
    | "cold" -> Cold
    | invalidType -> failwithf "Invalid damage type: %s" invalidType

let parseWeaknessesAndImmunities str =
    if str = "" then
        Set.empty, Set.empty
    else
        let parseSection (section : string) =
            let isWeakness = section.StartsWith("weak")
            let typeOffset = if isWeakness then 8 else 10
            isWeakness, section.[typeOffset..] |> splitBy ", " (Array.map parseDamageType)
        let sections = str.Trim([| '('; ')'; ' ' |]) |> splitBy "; " (Array.map parseSection)
        let updateSets (weaknesses, immunities) (isWeakness, types) =
            let typeSet = Set.ofArray types
            if isWeakness then (typeSet, immunities)
            else (weaknesses, typeSet)
        Array.fold updateSets (Set.empty, Set.empty) sections

let parseGroup team line =
    let groupRegex = @"(\d+) units each with (\d+) hit points (\(.*\) )?with an attack that does (\d+) (\w+) damage at initiative (\d+)"
    let m = Regex.Match(line, groupRegex)
    let gs = [| for g in m.Groups -> g.Value |]
    let units = int gs.[1]
    let hp = int gs.[2]
    let weaknesses, immunities = parseWeaknessesAndImmunities gs.[3]
    let damage = int gs.[4]
    let damageType = parseDamageType gs.[5]
    let initiative = int gs.[6]
    {units=units; hp=hp; weaknesses=weaknesses; immunities=immunities; damage=damage; damageType=damageType; initiative=initiative; team=team}

let parseGroups lines =
    let lineArr = lines |> Seq.toArray
    let immuneEnd = lineArr |> Array.findIndex (fun l -> l = "")
    let immuneSystem = lineArr.[1..immuneEnd-1] |> Array.map (parseGroup ImmuneSystem)
    let infection = lineArr.[immuneEnd+2..] |> Array.map (parseGroup Infection)
    Array.append immuneSystem infection

let effectivePower group = group.units * group.damage

let getDamageDealt attacker attackee =
    let damage = effectivePower attacker
    let damageType = attacker.damageType
    let mul =
        if   Set.contains damageType attackee.immunities then 0
        elif Set.contains damageType attackee.weaknesses then 2
        else 1
    mul * damage

let chooseTarget group choices =
    let enemies = choices |> Set.toArray |> Array.filter (fun g -> g.team <> group.team)
    if enemies.Length = 0 then
        None
    else
        let bestChoice = Array.maxBy (fun g -> (getDamageDealt group g, effectivePower g, g.initiative)) enemies
        if getDamageDealt group bestChoice = 0 then
            None
        else
            Some bestChoice

let chooseTargets groupMap =
    let choose (targetMap, unchosen) group =
        match chooseTarget group unchosen with
        | Some target -> Map.add group.initiative (Some target.initiative) targetMap, Set.remove target unchosen
        | None -> Map.add group.initiative None targetMap, unchosen
    let groupArray = groupMap |> Map.toArray |> Array.map snd
    groupArray
    |> Array.sortByDescending (fun g -> (effectivePower g, g.initiative))
    |> Array.fold choose (Map.empty, Set.ofArray groupArray)
    |> fst

let attackBy attacker attackee =
    let damage = getDamageDealt attacker attackee
    let killedUnits = damage / attackee.hp
    let newUnitCount = max 0 (attackee.units - killedUnits)
    {attackee with units=newUnitCount}

let attackTargets groupMap targetMap =
    let attack groupMap attackerId =
        match Map.find attackerId targetMap with
        | Some targetId ->
            let attacker = Map.find attackerId groupMap
            let target = Map.find targetId groupMap
            let attacked = attackBy attacker target
            Map.add targetId attacked groupMap
        | None -> groupMap
    groupMap
    |> Map.toArray
    |> Array.map fst
    |> Array.sortDescending
    |> Array.fold attack groupMap

let removeDead = Map.filter (fun _ g -> g.units > 0)
let fight groupMap = chooseTargets groupMap |> attackTargets groupMap |> removeDead
let score = Map.toArray >> Array.sumBy (fun (_, g) -> g.units)

let simulate boost groups =
    let applyBoost g = if g.team = ImmuneSystem then {g with damage=g.damage+boost} else g
    let groupMap = groups |> Array.map applyBoost |> Array.map (fun g -> (g.initiative, g)) |> Map.ofArray
    let rec nextRound groupMap =
        let nextMap = fight groupMap
        let remainingTeams = nextMap |> Map.toArray |> Array.map (fun (_, g) -> g.team) |> Set.ofArray
        if nextMap = groupMap then
            None, score nextMap
        elif Set.count remainingTeams = 1 then
            Some (remainingTeams |> Set.toList |> List.head), score nextMap
        else
            nextRound nextMap
    nextRound groupMap

let solvePart1 = simulate 0 >> snd
let solvePart2 groups =
    Seq.initInfinite (fun boost -> simulate boost groups)
    |> Seq.find (fun (winner, _) -> winner = Some ImmuneSystem)
    |> snd

let solver = {parse = parseGroups; part1 = solvePart1; part2 = solvePart2}