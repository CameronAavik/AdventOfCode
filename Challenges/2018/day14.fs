module Year2018Day14

open CameronAavik.AdventOfCode.Common
open System

// infinite zipper with a queue of items to append once it reaches the end
type ScoreZipper = {l: int list; c: int; r: int list; scoresToAdd: int list}

let shift1 {l=l; c=c; r=r; scoresToAdd=scoresToAdd} =
    match r with
    | x :: xs ->
        {l=c::l; c=x; r=xs; scoresToAdd=scoresToAdd}
    | [] -> // if we reach the end
        match scoresToAdd with
        | [] -> // we need to move back to the start
            let rev = List.rev (c::l)
            {l=[]; c=List.head rev; r=List.tail rev; scoresToAdd=[]}
        | _ -> // add the items from the queue
            let rev = List.rev scoresToAdd
            {l=c::l; c=List.head rev; r=List.tail rev; scoresToAdd=[]}

// does not support negative movements
let rec move =
    function
    | 0 -> id
    | n -> shift1 >> move (n - 1)

let rec addNewRecipes newRecipes zipper =
    match newRecipes with
    | [] -> zipper
    | r :: rs -> {zipper with scoresToAdd = r::zipper.scoresToAdd} |> addNewRecipes rs

let pickNewRecipe zipper = move (zipper.c + 1) zipper

type Elves = {elf1: ScoreZipper; elf2: ScoreZipper; numScores: int}

let mapElves f {elf1=elf1; elf2=elf2; numScores=numScores} =
    {elf1=f elf1; elf2=f elf2; numScores=numScores}

let getNewRecipes elves =
    let s1 = elves.elf1.c
    let s2 = elves.elf2.c
    let total = s1 + s2
    let recipeList = [total % 10]
    if total >= 10 then
        (total / 10) :: recipeList
    else
        recipeList

let updateRecipes elves = 
    let newRecipes = getNewRecipes elves
    {elves with numScores=elves.numScores + (List.length newRecipes)}
    |> mapElves (addNewRecipes newRecipes)

let moveElves = mapElves pickNewRecipe
let step = updateRecipes >> moveElves

let initState =
    let elf1 = {l=[]; c=3; r=[7]; scoresToAdd=[]}
    let elf2 = elf1 |> move 1
    {elf1=elf1; elf2=elf2; numScores=2}

let solvePart1 targetStr =
    let target = int targetStr
    let rec getNRecipes elves =
        if elves.numScores > target + 10 then
            let e1 = elves.elf1
            let e1Index = List.length e1.l
            let e1AtTarget =
                if e1Index < target then
                    e1 |> move (target - e1Index)
                else
                    // i may have been too lazy to implement a negative movement for this
                    // so it just cycles the entire list
                    e1 |> move ((List.length e1.r) + (List.length e1.scoresToAdd) + 1 + target)
            let chars =
                Seq.init 10 id
                |> Seq.mapFold (fun elf _ -> (char (elf.c + int '0')), (move 1 elf)) e1AtTarget
                |> fst
            new String(chars |> Seq.toArray)
        else
            elves |> step |> getNRecipes
    getNRecipes initState

let rec foundTarget target recipes =
    match target, recipes with
    | (x :: xs), (y :: ys) when x = y -> foundTarget xs ys
    | [], _ -> true
    | _ -> false

type TargetBuffer = int * int * int * int * int * int
let pushChar (_, b2, b3, b4, b5, b6) ch =
    (b2, b3, b4, b5, b6, ch)

let solvePart2 targetStr =
    let target = targetStr |> Seq.map (fun c -> int c - int '0') |> Seq.fold pushChar (0, 0, 0, 0, 0, 0)
    let rec findPattern elves recipeBuffer bufferCount =
        let newRecipes = getNewRecipes elves
        let rec applyRecipes newRecipes bufferCount recipes =
            match newRecipes with
            | [] -> (recipes, bufferCount, false)
            | x :: xs ->
                let newBuffer = pushChar recipes x
                if newBuffer = target then
                    (newBuffer, bufferCount + 1, true)
                else
                    applyRecipes xs (bufferCount + 1) newBuffer
        let appliedRecipes, newBufferCount, solnFound = applyRecipes newRecipes bufferCount recipeBuffer
        if solnFound then
            newBufferCount
        else
            findPattern (step elves) appliedRecipes newBufferCount
    findPattern initState (0, 0, 0, 0, 3, 7) (-4)

let solver = {parse = parseFirstLine asString; part1 = solvePart1; part2 = solvePart2}