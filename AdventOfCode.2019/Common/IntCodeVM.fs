namespace CameronAavik.AdventOfCode.Y2019.Common

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections

module IntCodeVM =
    let parseIntCodeFromFile = 
        parseFirstLine (splitBy "," asIntArray)
        >> Array.indexed
        >> Map.ofArray

    type ProgramState = { PC : int; Memory : Map<int, int>; Input : Queue<int>; Output : Queue<int>; IsHalted : bool }
    let bootProgram intCode = { PC = 0; Memory = intCode; Input = Queue.empty; Output = Queue.empty; IsHalted = false }
    
    type Param = Position of int | Immediate of int
    let parsePosition mode = 
        match mode with
        | 0 -> Position
        | 1 -> Immediate
        | m -> failwithf "Invalid Mode: %i" m
    
    let getVal param state = 
        match param with
        | Position i -> state.Memory.[i]
        | Immediate i -> i
        
    let apply2 f param1 param2 state = 
        f (getVal param1 state) (getVal param2 state)
    
    let setVal param x state =
        match param with
        | Position i -> { state with Memory = Map.add i x state.Memory }
        | Immediate i -> failwithf "Tried to set value at address %i via immediate mode" i

    let writeToInput value state = { state with Input = Queue.conj value state.Input }
    let readFromInput addr state =
        match state.Input with
        | Queue.Cons (i, is) -> setVal addr i { state with Input = is }
        | Queue.Nil -> failwith "Tried to read input from empty buffer"
    
    let writeToOutput addr state = { state with Output = Queue.conj (getVal addr state) state.Output }
    let readFromOutput state =
        match Queue.tryUncons state.Output with
        | Some (i, is) -> i, { state with Output = is }
        | None -> failwith "Tried to read output from empty buffer"

    let tryReadFromOutput state =
        match Queue.tryUncons state.Output with
        | Some (i, is) -> Some i, { state with Output = is }
        | None -> None, state

    let readAllOutput state = Queue.toSeq state.Output
    
    type Operation =
        | Add of Param * Param * Param
        | Mult of Param * Param * Param
        | Input of Param
        | Output of Param
        | JumpIfTrue of Param * Param
        | JumpIfFalse of Param * Param
        | SetIfLessThan of Param * Param * Param
        | SetIfEqual of Param * Param * Param
        | Halt

    let instructionLength =
        function
        | Add _ | Mult _ | SetIfLessThan _ | SetIfEqual _ -> 4
        | JumpIfTrue _ | JumpIfFalse _ -> 3
        | Input _ | Output _ -> 2
        | Halt -> 1

    let setPC i state = { state with PC = (getVal i state) }
    let nextPC op state = { state with PC = state.PC + instructionLength op }
    
    let getOperation state =
        let instruction = getVal (Position state.PC) state
        let opCode = instruction % 100
        let paramModes = [| 100; 1000; 10000 |] |> Array.map (fun d -> parsePosition (instruction / d % 10))
    
        let param i = getVal (Position (state.PC + i + 1)) state |> paramModes.[i]
        let with1Param () = param 0
        let with2Params () = (param 0, param 1)
        let with3Params () = (param 0, param 1, param 2)
    
        match opCode with
        | 1 -> Add (with3Params ())
        | 2 -> Mult (with3Params ())
        | 3 -> Input (with1Param ())
        | 4 -> Output (with1Param ())
        | 5 -> JumpIfTrue (with2Params ())
        | 6 -> JumpIfFalse (with2Params ())
        | 7 -> SetIfLessThan (with3Params ())
        | 8 -> SetIfEqual (with3Params ())
        | 99 -> Halt
        | _ -> failwithf "Invalid Opcode: %i" instruction
    
    let executeInstruction state =
        let op = getOperation state
        match op with
        | Add  (p1, p2, dst) -> state |> setVal dst (apply2 (+) p1 p2 state) |> nextPC op
        | Mult (p1, p2, dst) -> state |> setVal dst (apply2 (*) p1 p2 state) |> nextPC op
        | Input  addr -> state |> readFromInput addr |> nextPC op
        | Output addr -> state |> writeToOutput addr |> nextPC op
        | JumpIfTrue  (t, dst) -> state |> if getVal t state <> 0 then setPC dst else nextPC op
        | JumpIfFalse (t, dst) -> state |> if getVal t state =  0 then setPC dst else nextPC op
        | SetIfLessThan (p1, p2, dst) -> state |> setVal dst (if (apply2 (<) p1 p2 state) then 1 else 0) |> nextPC op
        | SetIfEqual    (p1, p2, dst) -> state |> setVal dst (if (apply2 (=) p1 p2 state) then 1 else 0) |> nextPC op
        | Halt -> { state with IsHalted = true }
        
    let rec runUntilHalt s =
        if s.IsHalted then s
        else executeInstruction s |> runUntilHalt

    let rec runUntilOutput s =
        if (s.Output |> Queue.isEmpty |> not) || s.IsHalted then s
        else executeInstruction s |> runUntilOutput