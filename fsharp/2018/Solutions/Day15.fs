module Year2018Day15

open AdventOfCode.FSharp.Common

type UnitType = Goblin | Elf

type Unit = {unitType: UnitType; pos: int * int; hp: int; damage: int}
let newUnit unitType x y damage = {unitType=unitType; pos=(x, y); hp=200; damage=damage}

type CaveCell = Wall | Open

let extractUnits elfDamage cave =
    let grid = cave |> array2D
    let units = seq {
        for y = 0 to (grid.GetLength 0) - 1 do
            for x = 0 to (grid.GetLength 1) - 1 do
                match grid.[y,x] with
                | 'G' -> yield newUnit Goblin x y 3
                | 'E' -> yield newUnit Elf x y elfDamage
                | _ -> ()}
    let charToCell = 
        function
        | 'G' | 'E' | '.' -> Open
        | '#' -> Wall
        | c -> failwithf "Invalid char %c" c
    let extractedGrid = grid |> Array2D.map charToCell
    extractedGrid, units

// return neighbours in reading order
let neighbours (x, y) = [(x, y-1); (x-1, y); (x + 1, y); (x, y + 1)]
let openNeighbours (grid : CaveCell [,]) = neighbours >> List.filter (fun (x, y) -> grid.[y, x] = Open)

let readingOrder (x, y) = (y, x)

let stepToClosestTarget grid src targets excludedNodes =
    // - seen is a set of (x, y) coords that shouldn't be visited again
    // - nextLevel are the coords that are all equal shortest distance away from
    //   src. It is a set of ((sx, sy), (x, y)) coords where (sx, sy) is the
    //   first step taken after src to get here and (x, y) is the coord.
    //   Using this we are able to find the shortest path with the first step
    //   that comes first in reading order
    let rec bfs seen nextLevel =
        let nextLevelNodes = nextLevel |> Set.map snd
        // see if we have found any targets in this level
        let intersection = Set.intersect targets nextLevelNodes
        if Set.isEmpty intersection then
            let newSeen = Set.union seen nextLevelNodes
            // take a node and add any unseen neighbours to the next levl
            let processNode nextLevel (start, node) =
                openNeighbours grid node
                |> List.filter (fun p -> Set.contains p newSeen |> not)
                |> List.fold (fun ls l -> (start, l) :: ls) nextLevel // start is propagated to next level
            let nextLevel =
                nextLevel
                |> Set.toList
                |> List.fold processNode []
                |> Set.ofList
            if Set.isEmpty nextLevel then
                src
            else
                bfs newSeen nextLevel
        else
            let closestTarget = intersection |> Seq.minBy readingOrder
            nextLevel
            |> Seq.filter (fun t -> snd t = closestTarget) // find all items in next level at the closest target
            |> Seq.map fst // get the starting steps that took them there
            |> Seq.minBy readingOrder // get the starting step first in the reading order
    let initSeen = excludedNodes |> Set.add src
    let firstLevel =
        Set.difference (openNeighbours grid src |> Set.ofList) excludedNodes
        |> Set.map (fun p -> (p, p)) // first step source will be itself
    bfs initSeen firstLevel

type CaveState = {
    locations: Map<int * int, int>; // maps (x, y) coords to unit index
    units: Map<int, Unit>; // maps unit index to the unit
    endedAtEndOfRound: bool; // if true, then it did not end mid-round (needed for outcome calculation)
    ended: bool; // indicates if all of either the elves or goblins are dead
    elfDied: bool; // indicates if an elf has died
    round: int; // the round number, increments after the round is finished
}

let createState units =
    let unitIndexes = Seq.mapi (fun i u -> (i, u)) units
    let unitMap = unitIndexes |> Map.ofSeq
    let locations = unitIndexes |> Seq.map (fun (i, u) -> (u.pos, i)) |> Map.ofSeq
    {locations=locations; units=unitMap; endedAtEndOfRound=false; ended=false; elfDied=false; round=0}

let invalidateEndedAtEndOfRound caveState = {caveState with endedAtEndOfRound=false}

let updateRound caveState =
    if not caveState.ended || caveState.endedAtEndOfRound then
        {caveState with round=caveState.round+1}
    else
        caveState

let nearbyEnemies grid unit caveState =
    openNeighbours grid unit.pos
    |> List.choose (fun p -> Map.tryFind p caveState.locations)
    |> List.choose (fun i -> Map.tryFind i caveState.units)
    |> List.filter (fun u -> u.unitType <> unit.unitType)

let moveToClosestTarget grid i caveState unit =
    let otherUnits =
        caveState.units
        |> Map.toSeq
        |> Seq.filter (fun (idx, _) -> idx <> i)
        |> Seq.map snd
    let excludedNodes =
        otherUnits
        |> Seq.map (fun u -> u.pos)
        |> Set.ofSeq
    let targets =
        otherUnits
        |> Seq.filter (fun u -> u.unitType <> unit.unitType)
        |> Seq.collect(fun u -> openNeighbours grid u.pos)
        |> Set.ofSeq
    let filteredTargets = Set.difference targets excludedNodes
    let nextPos = stepToClosestTarget grid unit.pos filteredTargets excludedNodes
    let newUnits = caveState.units |> Map.add i {unit with pos=nextPos}
    let newLocations =
        caveState.locations
        |> Map.remove unit.pos
        |> Map.add nextPos i
    {caveState with locations=newLocations; units=newUnits}

let tryMoveToClosestTarget grid i caveState =
    match Map.tryFind i caveState.units with
    | Some unit ->
        let shouldMove = List.isEmpty (nearbyEnemies grid unit caveState)
        if shouldMove then
            moveToClosestTarget grid i caveState unit
        else
            caveState
    | None -> caveState

let updateEndedState caveState =
    let remainingUnitTypes =
        caveState.units
        |> Map.toSeq 
        |> Seq.map (fun (_, u) -> u.unitType)
        |> Set.ofSeq
    if Set.count remainingUnitTypes = 1 then
        {caveState with endedAtEndOfRound=true; ended=true}
    else
        caveState

let attackWeakestEnemy caveState unit enemiesInRange =
    let weakest = List.minBy (fun e -> e.hp, readingOrder e.pos) enemiesInRange
    let weakestIndex = Map.find weakest.pos caveState.locations
    let newHP = weakest.hp - unit.damage
    if newHP <= 0 then
        let newLocations = caveState.locations |> Map.remove weakest.pos
        let newUnits = caveState.units |> Map.remove weakestIndex
        let elfDied = caveState.elfDied || weakest.unitType = Elf
        {caveState with locations=newLocations; units=newUnits; elfDied=elfDied}
        |> updateEndedState
    else
        let damagedWeakest = {weakest with hp=weakest.hp-unit.damage}
        let newUnits = caveState.units |> Map.add weakestIndex damagedWeakest
        {caveState with units=newUnits}

let tryAttackWeakestEnemy grid i caveState =
    match Map.tryFind i caveState.units with
    | Some unit ->
        let enemies = nearbyEnemies grid unit caveState
        if List.isEmpty enemies then
            caveState
        else
            attackWeakestEnemy caveState unit enemies
    | None -> caveState

let performAction grid i =
    invalidateEndedAtEndOfRound // if we are performing an action, then we did not end at the end
    >> tryMoveToClosestTarget grid i
    >> tryAttackWeakestEnemy grid i

let performRound grid caveState =
    caveState.units
    |> Map.toSeq
    |> Seq.sortBy (fun (_, unit) -> readingOrder unit.pos)
    |> Seq.fold (fun s (i, _) -> performAction grid i s) caveState
    |> updateRound

let getOutcome caveState =
    let hpSum = caveState.units |> Map.toSeq |> Seq.sumBy (fun (_, u) -> u.hp)
    caveState.round * hpSum

let getScore elfDamage stopAtElfDeath gridLines =
    let grid, units = extractUnits elfDamage gridLines
    let rec loopUntilEnded caveState =
        let nextState = performRound grid caveState
        if nextState.elfDied && stopAtElfDeath then
            false, 0
        elif nextState.ended then
            true, getOutcome nextState
        else
            loopUntilEnded nextState
    createState units |> loopUntilEnded

let solvePart1 = getScore 3 false >> snd
   
let solvePart2 gridLines =
    Seq.initInfinite (fun i -> getScore (i + 4) true gridLines)
    |> Seq.skipWhile (fun (elvesSurvived, _) -> not elvesSurvived)
    |> Seq.head
    |> snd

let solver = {parse = parseEachLine asString; part1 = solvePart1; part2 = solvePart2}