module Year2017Day09

open AdventOfCode.FSharp.Common

type GarbageState = NotGarbage | Garbage | Cancelled
type State = {level: int; state: GarbageState; score: int; garbage: int }

let step current nextChar =
    match (current.state, nextChar) with
    | (Garbage, '!') -> {current with state = Cancelled}
    | (Garbage, '>') -> {current with state = NotGarbage} 
    | (Garbage, _)   -> {current with garbage = current.garbage + 1}
    | (Cancelled, _) -> {current with state = Garbage}
    | (NotGarbage, '{') -> {current with level = current.level + 1}
    | (NotGarbage, '}') -> {current with level = current.level - 1; score = current.score + current.level}
    | (NotGarbage, '<') -> {current with state = Garbage}
    | _ -> current;

let solve = Seq.fold step {level=0; state=NotGarbage; score=0; garbage=0}
let solver = {parse = parseFirstLine asString; part1 = solve >> (fun state -> state.score); part2 = solve >> (fun state -> state.garbage)}