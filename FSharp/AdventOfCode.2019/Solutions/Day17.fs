module Year2019Day17

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.Intcode

let progOutputToGrid = List.map char >> charsToStr >> splitBy "\n" (Array.map Seq.toArray)

let getGrid intcode =
    match run (Computer.create intcode) with
    | Output (output, _) -> progOutputToGrid output
    | _ -> failwith "Expected an output"

let neighbours (x, y) = [| (x + 1, y); (x - 1, y); (x, y + 1); (x, y - 1) |]

let solvePart1 (intcode) =
    let g = getGrid intcode
    
    let getAt (x, y) = Array.tryItem y g |> Option.bind (Array.tryItem x) |> Option.defaultValue '.'
    let isScaffold (x, y) = getAt (x, y) = '#'
    let isIntersection pos =
        isScaffold pos && (neighbours pos |> Array.exists (isScaffold >> not) |> not)

    seq {
        for y = 1 to g.Length - 2 do
            for x = 1 to g.[0].Length - 2 do
                if isIntersection (x, y) then
                    x * y } |> Seq.sum

let charToDir = 
    function 
    | '^' -> Some (0, -1) | 'v' -> Some (0, 1) 
    | '<' -> Some (-1, 0) | '>' -> Some (1, 0) 
    | c -> None

type Action = Left | Right | Forward

let turn isLeft (dx, dy) = if isLeft then (dy, -dx) else (-dy, dx)
let move (dx, dy) (x, y) = (x + dx), (y + dy)

let getPath grid from dir =
    let getAt (x, y) = Array.tryItem y grid |> Option.bind (Array.tryItem x) |> Option.defaultValue '.'
    let getNextStep (from, dir) =
        if   getAt (move dir              from) = '#' then Some (Forward, (move dir from, dir))
        elif getAt (move (turn true  dir) from) = '#' then Some (Left,    (         from, turn true  dir))
        elif getAt (move (turn false dir) from) = '#' then Some (Right,   (         from, turn false dir))
        else None
    Array.unfold getNextStep (from, dir)

let actionLength actions =
    Array.fold (fun (acc, forward) c ->
        match c with
        | Left | Right -> (acc + 2, 0)
        | Forward when forward = 0 -> (acc + 2, 1)
        | Forward when forward % 10 = 0 -> (acc + 1, forward + 1)
        | Forward -> (acc, forward + 1)) (-1, 0) actions |> fst

let allValidPrefixes actions =
    Seq.init (Array.length actions) (fun i -> Array.splitAt (i + 1) actions) |> Seq.takeWhile (fun (a, _) -> actionLength a <= 20)

type Compression =
    { A : Action [] option
      B : Action [] option
      C : Action [] option
      Order : string
      Actions : Action [] }

let rec compress state : Compression option =
    seq {
        if state.Order.Length <= 20 then
            if state.Actions.Length = 0 then Some state
            else
                for (a, b) in (allValidPrefixes state.Actions |> Seq.rev) do
                    match state with
                    | { A = Some arr } when arr = a -> compress { state with Order = state.Order + ",A"; Actions = b }
                    | { B = Some arr } when arr = a -> compress { state with Order = state.Order + ",B"; Actions = b }
                    | { C = Some arr } when arr = a -> compress { state with Order = state.Order + ",C"; Actions = b }
                    | { A = None } -> compress { state with A = Some a; Order = "A"; Actions = b }
                    | { A = Some _; B = None } -> compress { state with B = Some a; Order = state.Order + ",B"; Actions = b }
                    | { A = Some _; B = Some _; C = None } -> compress { state with C = Some a; Order = state.Order + ",C"; Actions = b }
                    | _ -> ()
                } |> Seq.choose id |> Seq.tryHead

let providePath intcode input =
    let rec provideAllInput input state =
        match input, state with
        | (x :: xs), Input f -> provideAllInput xs (f x)
        | _ :: _, Output (o, s) ->
            provideAllInput input s
        | [], Output (o, Halted) -> o
        | _ -> failwith "Invalid State"
    Computer.create intcode
    |> Computer.set 0 2L
    |> run
    |> provideAllInput input

let pathToStr (path : Action []) =
    let s, f =
        Array.foldBack (fun action (acc, f) -> 
            match action with
            | Left when f = 0 -> ("L" :: acc, 0)
            | Left -> ("L" :: (string f) :: acc, 0)
            | Right when f = 0 -> ("R" :: acc, 0)
            | Right -> ("R" :: (string f) :: acc, 0)
            | Forward -> (acc, f + 1)) path ([], 0)

    let s = if f = 0 then s else (string f) :: s
    (String.concat "," s) + "\n"

let solvePart2 intcode =
    let grid = getGrid intcode
    let start, dir =
        seq {
            for y = 0 to grid.Length - 1 do
                for x = 0 to grid.[0].Length - 1 do
                    match charToDir grid.[y].[x] with
                    | Some dir -> (x, y), dir
                    | None -> () } |> Seq.head

    let path = getPath grid start dir
    let compressionResult = compress { A = None; B = None; C = None; Order = ""; Actions = path } |> Option.get

    let input =
        (compressionResult.Order + "\n") +
        (pathToStr compressionResult.A.Value) +
        (pathToStr compressionResult.B.Value) +
        (pathToStr compressionResult.C.Value) + "n\n"

    providePath intcode (input |> Seq.toList |> List.map int64)
    |> List.last

    
let solver = { parse = parseIntCode; part1 = solvePart1; part2 = solvePart2 }