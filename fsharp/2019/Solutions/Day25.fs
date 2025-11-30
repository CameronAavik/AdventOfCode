module Year2019Day25

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode
open System
open System.Text.RegularExpressions

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern, RegexOptions.Singleline ||| RegexOptions.ExplicitCapture)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let outputToAscii = Seq.map char >> charsToStr
let asciiToOutput str = str |> Seq.map int64 |> List.ofSeq

let rec provideInput inputs =
    match inputs with
    | x :: xs ->
        function
        | Input f -> f x |> provideInput xs
        | Output (o1, s) -> 
            match provideInput inputs s with
            | Output (o2, s') -> Output (o1 @ o2, s')
            | s' -> Output (o1, s')
        | Halted -> Halted
    | [] -> id

let rec playGameInteractive prog =
    match prog with
    | Output (o, s) ->
        printfn "%s\n" (outputToAscii o)
        match Console.ReadLine() with
        | null -> "Program Ended"
        | inp ->
            s |> provideInput (asciiToOutput (inp + "\n")) |> playGameInteractive
    | _ -> "Program Ended"

type Direction = North | South | West | East
let parseDir = function | "north" -> Some North | "south" -> Some South | "west" -> Some West | "east" -> Some East | s -> None
let dirToStr = function | North -> "north" | South -> "south" | West -> "west" | East -> "east"
let oppositeDir = function | North -> South | South -> North | West -> East | East -> West

type ExploringState =
    { Items : Set<string>
      Path : Direction list
      Unexplored : Set<Direction> list
      PathToSensor : (Direction list) option
      IsBacktracking : bool }

    static member create = { Items = Set.empty; Path = []; Unexplored = []; PathToSensor = None; IsBacktracking = false }

type PassingSensorState =
    { Items : string []
      Dir : Direction
      PrevGrayCode : int 
      GrayCodeIndex : int }

    static member create items dir = { Items = items; Dir = dir; PrevGrayCode = 0; GrayCodeIndex = 0 }

type GameState =
    | Exploring of ExploringState
    | PassingSensor of PassingSensorState
    | Solved of int

let deadlyItems = set ["photons"; "infinite loop"; "escape pod"; "molten lava"; "giant electromagnet"]

let getMoveStr dir = (sprintf "%s\n" (dirToStr dir))

let backtrack state =
    match state.Unexplored, state.Path with
    | (_ :: us), (p :: ps) -> (oppositeDir p), { state with IsBacktracking = true; Path = ps; Unexplored = us }
    | _ -> failwith "Can't backtrack from current state"

let handleExploring (response : string) (state : ExploringState) : string * GameState =
    if state.IsBacktracking then
        match state.Unexplored with
        | [s] when state.Path.IsEmpty && s.IsEmpty ->
            match state.PathToSensor with
            | Some path ->
                let commands =
                    [ for dir in path do
                          getMoveStr dir ]
                String.concat "" commands, PassingSensor (PassingSensorState.create (Set.toArray state.Items) (List.last path))
            | None -> failwith "Finished searching but did not find the sensors"
        | x :: xs ->
            let dir, state =
                if Set.isEmpty x then backtrack state
                else
                    let newDir = Set.minElement x
                    newDir, { state with Path = newDir :: state.Path; Unexplored = (Set.remove newDir x) :: xs; IsBacktracking = false }
            (getMoveStr dir), Exploring state
        | [] -> failwith "Backtracking when no more paths left to explore"
    else
        let extractListData n = splitBy "\n" id n |> Array.choose (fun s -> if s.StartsWith("- ") then Some s.[2..] else None) 
        match response with
        | Regex "== Security Checkpoint ==.+?(Doors here lead:\n(?<dirs>.+)\n\n)" [dirs] ->
            let dirs = extractListData dirs |> Array.choose parseDir |> Set.ofArray |> Set.remove (oppositeDir (List.head state.Path))
            let sensorPath = List.rev (dirs.MinimumElement :: state.Path)
            let dir, state' = backtrack { state with Unexplored = Set.empty :: state.Unexplored }
            (getMoveStr dir), Exploring { state' with PathToSensor = Some sensorPath }
        | Regex "^.+?(Doors here lead:\n(?<dirs>.+?)\n\n|)(Items here:\n(?<items>.+)\n\n|)Command\?\n$" [dirs; items] ->
            let items = extractListData items |> Set.ofArray |> Set.filter (fun i -> not (Set.contains i deadlyItems))
            let dirs  = extractListData dirs |> Array.choose parseDir |> Set.ofArray

            // remove the direction that we just came from
            let dirs' = 
                match state.Path with
                | x :: _ -> dirs |> Set.remove (oppositeDir x)
                | [] -> dirs

            // add all the new items to the state
            let state' = { state with Items = Set.union items state.Items }

            let dir, state' =
                if dirs'.Count = 0 then backtrack { state' with Unexplored = Set.empty :: state'.Unexplored }
                else
                    let newDir = Set.minElement dirs'
                    newDir, { state' with Path = newDir :: state.Path; Unexplored = (Set.remove newDir dirs') :: state.Unexplored }

            let commands =
                [ for item in items do
                      sprintf "take %s\n" item 
                  getMoveStr dir ]

            String.concat "" commands, Exploring state'
        | _ -> failwithf "Invalid response: %s" response

let handlePassing response sensorState =
    match response with
    | Regex "to get in by typing (?<solution>\d+) on the keypad" [solution] -> "", Solved (int solution)
    | _ ->
        let i = sensorState.GrayCodeIndex
        let grayCode = i ^^^ (i >>> 1)
        let commands =
            [ if i = 0 then
                for item in sensorState.Items do
                    sprintf "drop %s\n" item

              for item = 0 to sensorState.Items.Length - 1 do
                let bit = (grayCode >>> item) &&& 1
                let prevBit = (sensorState.PrevGrayCode >>> item) &&& 1
                if bit <> prevBit then
                    if bit = 1 then
                        sprintf "take %s\n" sensorState.Items.[item]
                    else
                        sprintf "drop %s\n" sensorState.Items.[item]
              getMoveStr sensorState.Dir ] |> String.concat ""
        commands, PassingSensor { sensorState with GrayCodeIndex = i + 1; PrevGrayCode = grayCode }

let handleResponse response state =
    match state with
    | Exploring expState -> handleExploring response expState
    | PassingSensor sensorState -> handlePassing response sensorState
    | Solved _ -> failwith "Should not have asked for response when solved"

let rec playGameAutomated state prog =
    match prog with
    | Output (o, s) ->
        let action, state' = handleResponse (outputToAscii o) state
        match state' with
        | Solved i -> i
        | _ -> s |> provideInput (asciiToOutput action) |> playGameAutomated state'
    | _ -> failwith "Invalid State"

let solve (intcode) =
    let comp = Computer.create intcode
    playGameAutomated (Exploring ExploringState.create) (run comp)

let solver = { parse = parseIntCode; part1 = solve; part2 = (fun _ -> "Advent of Code Finished!") }
