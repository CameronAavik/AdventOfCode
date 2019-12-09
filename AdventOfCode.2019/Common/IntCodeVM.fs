namespace CameronAavik.AdventOfCode.Y2019.Common

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections

module IntCodeVM =
    let parseIntCodeFromFile = 
        parseFirstLine (splitBy "," asInt64Array)
        >> Array.mapi (fun i v -> (int64 i, v))
        >> Map.ofArray

    type ProgramState = { PC : int64; Memory : Map<int64, int64>; Input : Queue<int64>; Output : Queue<int64>; IsHalted : bool; RelativeBase : int64 }
    let bootProgram intCode = { PC = 0L; Memory = intCode; Input = Queue.empty; Output = Queue.empty; IsHalted = false; RelativeBase = 0L }
    
    type Param = Position of int64 | Immediate of int64 | Relative of int64
    let parsePosition mode = 
        match mode with
        | 0 -> Position
        | 1 -> Immediate
        | 2 -> Relative
        | m -> failwithf "Invalid Mode: %i" m
    
    let rec getVal param state = 
        match param with
        | Position i -> Map.tryFind i state.Memory |> Option.defaultValue 0L
        | Relative i -> getVal (Position (i + state.RelativeBase)) state
        | Immediate i -> i
        
    let apply2 f param1 param2 state = 
        f (getVal param1 state) (getVal param2 state)
    
    let rec setVal param x state =
        match param with
        | Position i -> { state with Memory = Map.add i x state.Memory }
        | Relative i -> setVal (Position (i + state.RelativeBase)) x state
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
        | AdjustBase of Param
        | Halt

    let instructionLength =
        function
        | Add _ | Mult _ | SetIfLessThan _ | SetIfEqual _ -> 4
        | JumpIfTrue _ | JumpIfFalse _ -> 3
        | Input _ | Output _ | AdjustBase _ -> 2
        | Halt -> 1

    let setPC i state = { state with PC = (getVal i state) }
    let nextPC op state = { state with PC = state.PC + int64 (instructionLength op) }
    let adjustRelativeBase amount state = { state with RelativeBase = state.RelativeBase + amount }
    
    let getOperation state =
        let instruction = getVal (Position state.PC) state
        let opCode = instruction % 100L
        let paramModes = [| 100L; 1000L; 10000L |] |> Array.map (fun d -> parsePosition (int (instruction / d) % 10))
    
        let param i = getVal (Position (state.PC + int64 (i + 1))) state |> paramModes.[i]
        let with1Param () = param 0
        let with2Params () = (param 0, param 1)
        let with3Params () = (param 0, param 1, param 2)
    
        match opCode with
        | 1L -> Add (with3Params ())
        | 2L -> Mult (with3Params ())
        | 3L -> Input (with1Param ())
        | 4L -> Output (with1Param ())
        | 5L -> JumpIfTrue (with2Params ())
        | 6L -> JumpIfFalse (with2Params ())
        | 7L -> SetIfLessThan (with3Params ())
        | 8L -> SetIfEqual (with3Params ())
        | 9L -> AdjustBase (with1Param ())
        | 99L -> Halt
        | _ -> failwithf "Invalid Opcode: %i" instruction
    
    let executeInstruction state =
        let op = getOperation state
        match op with
        | Add  (p1, p2, dst) -> state |> setVal dst (apply2 (+) p1 p2 state) |> nextPC op
        | Mult (p1, p2, dst) -> state |> setVal dst (apply2 (*) p1 p2 state) |> nextPC op
        | Input  addr -> state |> readFromInput addr |> nextPC op
        | Output addr -> state |> writeToOutput addr |> nextPC op
        | JumpIfTrue  (t, dst) -> state |> if getVal t state <> 0L then setPC dst else nextPC op
        | JumpIfFalse (t, dst) -> state |> if getVal t state =  0L then setPC dst else nextPC op
        | SetIfLessThan (p1, p2, dst) -> state |> setVal dst (if (apply2 (<) p1 p2 state) then 1L else 0L) |> nextPC op
        | SetIfEqual    (p1, p2, dst) -> state |> setVal dst (if (apply2 (=) p1 p2 state) then 1L else 0L) |> nextPC op
        | AdjustBase addr -> state |> adjustRelativeBase (getVal addr state) |> nextPC op
        | Halt -> { state with IsHalted = true }
        
    let rec runUntilHalt s =
        if s.IsHalted then s
        else executeInstruction s |> runUntilHalt

    let rec runUntilOutput s =
        if (s.Output |> Queue.isEmpty |> not) || s.IsHalted then s
        else executeInstruction s |> runUntilOutput