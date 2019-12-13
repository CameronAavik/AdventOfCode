namespace CameronAavik.AdventOfCode.Y2019.Common

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections

type IntCodeState =
    | Running
    | Debugging of IntCodeDebugData
    | WaitingOnInput
    | Halted
    | Error of string

and IntCodeDebugData =
    { PCFrequencies : Map<int64, int>
      WrittenAddresses : Set<int64> 
      ExecutionTime : int64
      Breakpoints : Set<int64>
      ConditionalBreakpoints : (IntCodeVM -> bool) list }

and IntCodeVM = 
    { PC : int64
      Memory : Map<int64, int64>
      Input : Queue<int64>
      Output : Queue<int64>
      State : IntCodeState
      RelativeBase : int64;
      Overrides : Map<int64, IntCodeVM -> IntCodeVM> }

module IntCodeVM =
    let parseIntCodeFromFile = 
        parseFirstLine (splitBy "," asInt64Array)
        >> Array.mapi (fun i v -> (int64 i, v))
        >> Map.ofArray

    let createDebugData = 
        { PCFrequencies = Map.empty
          WrittenAddresses = Set.empty
          ExecutionTime = 0L
          Breakpoints = Set.empty
          ConditionalBreakpoints = List.empty }

    let bootProgram intCode = { PC = 0L; Memory = intCode; Input = Queue.empty; Output = Queue.empty; State = Running; RelativeBase = 0L; Overrides = Map.empty }
    let debugProgram intCode = { bootProgram intCode with State = Debugging createDebugData }

    let addNewPC pc debug =
        let curCount = Map.tryFind pc debug.PCFrequencies |> Option.defaultValue 0
        { debug with PCFrequencies = Map.add pc (curCount + 1) debug.PCFrequencies }

    let updateExecutionTime debug =
        { debug with ExecutionTime = debug.ExecutionTime + 1L }

    let addNewWrittenAddress addr debug =
        { debug with WrittenAddresses = Set.add addr debug.WrittenAddresses }

    let applyIfDebug f state =
        match state.State with
        | Debugging d -> { state with State = Debugging (f d) }
        | _ -> state

    let addBreakpoint pc state =
        match state.State with
        | Debugging d -> { state with State = Debugging { d with Breakpoints = Set.add pc d.Breakpoints } }
        | _ -> state

    let addStateCondBreakpoint bp state =
        match state.State with
        | Debugging d -> { state with State = Debugging { d with ConditionalBreakpoints = bp :: d.ConditionalBreakpoints } }
        | _ -> state

    let addDebugCondBreakpoint bp state =
        let debugBp s =
            match s.State with
            | Debugging d -> bp d
            | _ -> false
        addStateCondBreakpoint debugBp state
    
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
        | Position i ->
            let state = applyIfDebug (addNewWrittenAddress i) state
            { state with Memory = Map.add i x state.Memory }
        | Relative i -> setVal (Position (i + state.RelativeBase)) x state
        | Immediate i -> { state with State = Error (sprintf "Tried to set value at address %i via immediate mode" i) }

    let readFromInput addr state =
        match state.Input with
        | Queue.Cons (i, is) -> setVal addr i { state with Input = is }
        | Queue.Nil -> { state with State = WaitingOnInput; PC = state.PC - 2L }
    
    let writeToOutput addr state = { state with Output = Queue.conj (getVal addr state) state.Output }

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
    
    let getOperationAtPC pc state =
        let instruction = getVal (Position pc) state
        let opCode = instruction % 100L
        let paramModes = [| 100L; 1000L; 10000L |] |> Array.map (fun d -> parsePosition (int (instruction / d) % 10))
    
        let param i = getVal (Position (pc + int64 (i + 1))) state |> paramModes.[i]
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

    let getOperation state = getOperationAtPC state.PC state
    
    let executeInstruction state =
        let state = applyIfDebug (addNewPC state.PC >> updateExecutionTime) state
        match Map.tryFind state.PC state.Overrides with
        | Some f -> f state
        | None ->
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
            | Halt -> { state with State = Halted }

    let printInstruction pc state debug =
        let isMutated addr = Set.contains addr debug.WrittenAddresses
        let printParam param offset =
            let mutMarker = if isMutated (pc + int64 offset) then "!" else ""
            match param with
            | Position i -> sprintf "pos:%i%s" i mutMarker
            | Immediate i -> sprintf "imm:%i%s" i mutMarker
            | Relative i -> sprintf "rel:%i%s" i mutMarker
        let printInstr instr =
            match instr with
            | Add (p1, p2, dst) -> sprintf "%s <- %s + %s" (printParam dst 3) (printParam p1 1) (printParam p2 2)
            | Mult (p1, p2, dst) -> sprintf "%s <- %s * %s" (printParam dst 3) (printParam p1 1) (printParam p2 2)
            | Input addr -> sprintf "%s <- readInput" (printParam addr 1)
            | Output addr -> sprintf "print %s" (printParam addr 1)
            | JumpIfTrue (t, dst) -> sprintf "if %s != 0 then jump to %s" (printParam t 1) (printParam dst 2)
            | JumpIfFalse (t, dst) -> sprintf "if %s = 0 then jump to %s" (printParam t 1) (printParam dst 2)
            | SetIfLessThan (p1, p2, dst) -> sprintf "%s <- %s < %s" (printParam dst 3) (printParam p1 1) (printParam p2 2)
            | SetIfEqual (p1, p2, dst) -> sprintf "%s <- %s = %s" (printParam dst 3) (printParam p1 1) (printParam p2 2)
            | AdjustBase addr -> sprintf "adjust base by %s" (printParam addr 1)
            | Halt -> "halt"

        let op = getOperationAtPC pc state
        let count = Map.tryFind pc debug.PCFrequencies |> Option.defaultValue 0
        let mutationMarker = if isMutated pc then " (MUTATED INSTR)" else ""
        sprintf "%4d (%4d): %s%s" pc count (printInstr op) mutationMarker

    let printAllInstructions state debug =
        debug.PCFrequencies
        |> Map.toArray
        |> Array.map (fun (pc, _) -> printInstruction pc state debug)

    let testBreakAndTracepoints state =
        match state.State with
        | Debugging debug ->
            if Set.contains state.PC debug.Breakpoints then
                let instrData = printAllInstructions state debug
                if false then
                    String.concat "\n" instrData |> printfn "%s"
                printfn "Hit breakpoint at PC=%i; N=%i" state.PC debug.ExecutionTime

            for (i, bp) in List.indexed debug.ConditionalBreakpoints do
                if bp state then
                    let instrData = printAllInstructions state debug
                    if true then
                        String.concat "\n" instrData |> printfn "%s"
                    printfn "Hit tracepoint %i at PC=%i; N=%i" i state.PC debug.ExecutionTime
        | _ -> ()
        state

    let rec runUntilHalt s =
        match s.State with
        | Running | Debugging _ -> executeInstruction s |> testBreakAndTracepoints  |> runUntilHalt
        | _ -> s

    let rec runUntilOutputOrHalt s =
        match s.State with
        | Running | Debugging _ when s.Output.IsEmpty -> executeInstruction s |> testBreakAndTracepoints |> runUntilOutputOrHalt
        | _ -> s

    let rec runUntilInputRequired s =
        match s.State with
        | Running | Debugging _ -> executeInstruction s |> testBreakAndTracepoints |> runUntilInputRequired
        | WaitingOnInput -> { s with State = Running }
        | _ -> s

    let writeToInput value state = { state with Input = Queue.conj value state.Input }

    let readFromOutput state =
        let state = runUntilOutputOrHalt state
        match Queue.tryUncons state.Output with
        | Some (i, is) -> Some i, { state with Output = is }
        | None -> None, state

    let readAllOutput state = 
        Queue.toSeq state.Output, { state with Output = Queue.empty }