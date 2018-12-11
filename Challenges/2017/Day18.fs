module Year2017Day18

open CameronAavik.AdventOfCode.Common
open System

let getOrDefault key map ``default``= 
    match Map.tryFind key map with
    | Some v -> v
    | None -> ``default``

type Computer = { code: (string * (string array)) list; pc : int; registers: Map<string, int64>; buffer: int64 list }
let defaultComp code = {code = Seq.toList code; pc = 0; registers = Map.empty; buffer = []}

// computer helper functions
let getVal comp (value : string) =
    Int64.TryParse(value) 
    |> (fun (isInt, i) -> if isInt then i else getOrDefault value comp.registers 0L)
let jump offset comp = {comp with pc = comp.pc + offset}
let updateRegister register value comp = {comp with registers = Map.add register value comp.registers}
let queue comp = function | None -> comp | Some x -> {comp with buffer = comp.buffer @ [x]}
let dequeue comp = {comp with buffer = List.tail comp.buffer}
let getCurrentInsn comp = List.item comp.pc comp.code
let isLocked comp = fst (getCurrentInsn comp) = "rcv" && comp.buffer = []
// returns function which makes the relevant changes to the computer and the value being sent given instruction and rcv buffer
let applyInsn handleRcvEmptyBuffer get = function
    | ("snd", [| x |]), _ -> jump 1, Some (get x)
    | ("set", [| x; y |]), _ -> updateRegister x (get y) >> jump 1, None
    | ("add", [| x; y |]), _ -> updateRegister x (get x + get y) >> jump 1, None
    | ("mul", [| x; y |]), _ -> updateRegister x (get x * get y) >> jump 1, None
    | ("mod", [| x; y |]), _ -> updateRegister x (get x % get y) >> jump 1, None
    | ("rcv", _), [] -> handleRcvEmptyBuffer, None 
    | ("rcv", [| x |]), x' :: _ ->  updateRegister x x' >> dequeue >> jump 1, None
    | ("jgz", [| x; y |]), _ -> jump (if get x > 0L then int (get y) else 1), None
    | _ -> id, None
// simulates one clock tick of the computer
let step onRCV comp = applyInsn onRCV (getVal comp) (getCurrentInsn comp, comp.buffer) |> (fun (app, sent) -> (app comp, sent))
let step1, step2 = step (jump 1), step id
// parses a string of the file into an instruction
let asInstruction = splitBy " " (fun tokens -> (tokens.[0], tokens.[1..]))
// recursively ticks the clock until a valid recover is found, then returns last sound value
let rec findRecover comp lastSound = 
    match getCurrentInsn comp with
    | ("rcv", [| x |]) when getVal comp x <> 0L -> lastSound
    | _ -> step1 comp |> (fun (c, s) -> findRecover c (if s.IsSome then s.Value else lastSound))
// recursively ticks the clock until a deadlock is found, then returns number of messages sent from p2
let rec findDeadlock p1 p2 c = 
    if isLocked p1 && isLocked p2 then c
    else (step2 p1, step2 p2) |> (fun ((c1, s1), (c2, s2)) -> findDeadlock (queue c1 s2) (queue c2 s1) (c + if s2.IsSome then 1 else 0))
// setups up and calls the recursive methods
let part1 = defaultComp >> (fun comp -> findRecover comp 0L)
let part2 = defaultComp >> (fun comp -> findDeadlock comp (updateRegister "p" 1L comp) 0)
let solver = {parse = parseEachLine asInstruction; part1 = part1; part2 = part2}