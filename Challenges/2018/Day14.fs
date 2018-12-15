module Year2018Day14

open CameronAavik.AdventOfCode.Common
open System

// We maintain the scores as a lazy array. If we wish to add new scores to the
// end, then we add it to scoresToAdd. Only when we query the scores past the 
// length of scores do we append the scoresToAdd. elf1 and elf2 are indexes 
// into scores showing what recipe they are currently using
type Elves = {elf1: int; elf2: int; scores: int []; scoresToAdd: int list; numScores: int}
let getElfIndex elf elves = if elf = 1 then elves.elf1 else elves.elf2
let setElfIndex elf newIndex elves =
    if elf = 1 then
        {elves with elf1=newIndex}
    else
        {elves with elf2=newIndex}
let getScore elf elves = elves.scores.[getElfIndex elf elves]

// Return a list of new recipes based on where the elves currently are
let getNewScores elves =
    let s1 = getScore 1 elves
    let s2 = getScore 2 elves
    let total = s1 + s2
    let scoreList = [total % 10]
    if total >= 10 then
        (total / 10) :: scoreList
    else
        scoreList

// Handle adding/queueing the new scores based on the current elf recipes
let updateScores elves =
    let newScores = getNewScores elves
    let appendedScores = List.fold (fun scoreList x -> x :: scoreList) elves.scoresToAdd newScores
    let newScoreCount = elves.numScores + (List.length newScores)
    {elves with scoresToAdd=appendedScores; numScores=newScoreCount}

// Takes the queued scores and adds them to the scores array
let addQueuedScores elves =
    let newScores = elves.scoresToAdd |> List.toArray |> Array.rev
    {elves with scores = Array.append elves.scores newScores; scoresToAdd=[]}

// Move an elf to their new index. If it causes the elf to go beyond the end of
// the scores array, it will queue any scores and cycle back to start if needed
let rec moveElf elf elves =
    let numScores = Array.length elves.scores
    let score = getElfIndex elf elves + getScore elf elves + 1
    if score < numScores || elves.scoresToAdd = [] then
        elves |> setElfIndex elf (score % numScores)
    else
        elves |> addQueuedScores |> moveElf elf

// Updates new recipes, then moves both elves
let step = updateScores >> moveElf 1 >> moveElf 2

// Initial status of the elves at the beginning of the program
let initElves = {elf1=0; elf2=1; scores=[| 3; 7 |]; scoresToAdd=[]; numScores=2}

// Simulate steps until we hit target + 10 recipes
let solvePart1 targetStr =
    let target = int targetStr
    let rec getNScores elves =
        if elves.numScores > target + 10 then
            let appended = addQueuedScores elves
            let chars =
                Array.sub appended.scores target 10
                |> Array.map (fun c -> char (c + int '0'))
            new String(chars)
        else
            elves |> step |> getNScores
    getNScores initElves

// Fixed size buffer of 6 integers (the length of the target)
type Buffer = int * int * int * int * int * int
let push (_, b2, b3, b4, b5, b6) ch = (b2, b3, b4, b5, b6, ch)
let emptyBuffer = (0, 0, 0, 0, 0, 0)

// Maintains state over the most recently added scores, how many scores have
// been seen before, and whether or not we have found a match
type ScoreBuffer = {target: Buffer; scores: Buffer; beforeCount: int; matchFound: bool}

// Handles adding a new score and updating the ScoreBuffer appropriately
let addScore scoreBuffer newScore =
    if scoreBuffer.matchFound then
        scoreBuffer
    else
        // Push the new score and compare with the target
        let newBuffer = push scoreBuffer.scores newScore
        let bufferIsTarget = scoreBuffer.target = newBuffer
        {scoreBuffer with scores=newBuffer; beforeCount=scoreBuffer.beforeCount+1; matchFound=bufferIsTarget}

let initScoreBuffer target initScores =
    List.fold addScore {target=target; scores=emptyBuffer; beforeCount=(-6); matchFound=false} initScores

let targetStrToBuffer = 
    Seq.map (fun c -> int c - int '0') // Convert chars to ints/digits
    >> Seq.fold push emptyBuffer

let solvePart2 targetStr =
    let rec findPattern elves scoreBuffer =
        let newScoreBuffer =
            getNewScores elves // Get the scores that will be added
            |> List.fold addScore scoreBuffer // Add the new scores to the buffer
        if newScoreBuffer.matchFound then
            newScoreBuffer.beforeCount
        else
            findPattern (step elves) newScoreBuffer
    let target = targetStrToBuffer targetStr
    let scoreBuffer = initScoreBuffer target [3 ; 7]
    findPattern initElves scoreBuffer

let solver = {parse = parseFirstLine asString; part1 = solvePart1; part2 = solvePart2}