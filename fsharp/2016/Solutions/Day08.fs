module Year2016Day08

open AdventOfCode.FSharp.Common

let mo a b = ((a % b) + b) % b

type RC = Row | Column
type SeqType = Rect of int * int | Rotate of RC * int * int

let parseRect (parts : string[]) =
    let dims = splitBy "x" asIntArray parts.[1]
    let w = dims.[0]
    let h = dims.[1]
    Rect (w, h)

let parseRotate (parts : string[]) =
    let t = if parts.[1] = "row" then Row else Column
    let v = (extractInts parts.[2]).[0]
    let by = int parts.[4] |> string |> int
    Rotate (t, v, by)

let parseDir line =
    let parts = splitBy " " asStringArray line
    if parts.[0] = "rect" then
        parseRect parts
    else
        parseRotate parts
        
let parse = parseEachLine parseDir

let strip = Array2D.zeroCreate<int> 6 50

let applyDir grid =
    let W = Array2D.length2 grid
    let H = Array2D.length1 grid
    function
    | Rect (w, h) ->
        Array2D.mapi (fun y x v -> if y < h && x < w then 1 else v) grid
    | Rotate (typ, idx, by) ->
        let getValue y x v =
            if typ = Column && x = idx then
                grid.[mo (y - by) 6, x]
            elif typ = Row && y = idx then
                grid.[y, mo (x - by) 50]
            else
                v
        Array2D.mapi getValue grid

let solvePart1 lines =
    let grid = Seq.fold applyDir strip lines
    grid |> Seq.cast<int> |> Seq.sum

let solvePart2 lines =
    let grid = Seq.fold applyDir strip lines
    seq {
        for row in 0..5 do
            '\n'
            for col in 0..49 do
                (if grid.[row, col] = 1 then '#' else ' ') }
        |> charsToStr

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }