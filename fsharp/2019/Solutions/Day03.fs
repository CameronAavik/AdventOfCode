module Year2019Day03

open AdventOfCode.FSharp.Common

type Dir = L | R | D | U
let parseDir = 
    function 
    | 'L' -> L | 'R' -> R | 'D' -> D | 'U' -> U 
    | c -> failwithf "Invalid Dir: %c" c

type Stride = { Dir : Dir; Length : int }

let parseStride (item : string) =
    { Dir = parseDir item.[0]; Length = int item.[1..] }

let asTwoWires wires =
    Seq.item 0 wires, Seq.item 1 wires

let parse = parseEachLine (splitBy "," (Array.map parseStride)) >> asTwoWires

type LineAxis = X of int | Y of int

type Line =
    { Axis : LineAxis 
      Start : int
      End : int }

    static member fromStrideAtPos { Dir = d; Length = l } (x, y) =
        match d with
        | L -> { Axis = Y y; Start = x; End = x - l }
        | R -> { Axis = Y y; Start = x; End = x + l }
        | U -> { Axis = X x; Start = y; End = y - l }
        | D -> { Axis = X x; Start = y; End = y + l }

    static member getStartPos line =
        match line.Axis with
        | Y y -> (line.Start, y)
        | X x -> (x, line.Start)

    static member getEndPos line =
        match line.Axis with
        | Y y -> (line.End, y)
        | X x -> (x, line.End)

type WireSegment = 
    { Line : Line 
      Delay : int
      IsFirstWire : bool  }

    static member getDelayAt p { Line = { Start = p1; End = p2 }; Delay = d } =
        if   p1 <= p && p <= p2 then d + (p - p1)
        elif p1 >= p && p >= p2 then d + (p1 - p)
        else failwithf "Point is not on wire segment: %i" p

let stridesToLines isFirstWire strides =
    (((0, 0), 0), strides)
    ||> Array.mapFold (fun (pos, del) stride ->
        let wireSegment = { Line = Line.fromStrideAtPos stride pos; Delay = del; IsFirstWire = isFirstWire }
        wireSegment, (Line.getEndPos wireSegment.Line, del + stride.Length))
    |> fst

type Intersection =
    { X : int
      Y : int
      TotalDelay : int }

type SweepAction =
    | EnterHorz of y: int * WireSegment
    | ExitHorz of y: int * WireSegment
    | Vert of x: int * WireSegment

type SweepState =
    { Segments : Map<int * bool, WireSegment> }

    static member newSegment y seg { Segments = segs } =
        { Segments = Map.add (y, seg.IsFirstWire) seg segs }

    static member removeSegment y seg { Segments = segs } =
        { Segments = Map.remove (y, seg.IsFirstWire) segs }

    static member getSegmentsBetween minY maxY { Segments = segs } =
        segs
        |> Map.filter (fun (y, _) _ -> minY <= y && y <= maxY)
        |> Map.toSeq
        |> Set.ofSeq

let findIntersections wireSegments =
    let sweepActions =
        wireSegments
        |> Array.collect (fun w ->
            match w.Line.Axis with
            | Y y -> [| EnterHorz (y, w); ExitHorz (y, w) |]
            | X x -> [| Vert (x, w) |])
        |> Array.sortBy (fun action ->
            match action with
            | EnterHorz (_, { Line = l }) -> min l.Start l.End, 0
            | Vert (x, _) -> x, 1
            | ExitHorz (_, { Line = l }) -> max l.Start l.End, 2)

    ({ Segments = Map.empty }, sweepActions)
    ||> Array.mapFold (fun state action ->
        match action with
        | EnterHorz (y, seg) -> Array.empty, SweepState.newSegment y seg state
        | ExitHorz (y, seg) -> Array.empty, SweepState.removeSegment y seg state
        | Vert (x, seg) ->
            let y1, y2 = seg.Line.Start, seg.Line.End
            let minY, maxY = min y1 y2, max y1 y2
            let intersections = 
                SweepState.getSegmentsBetween minY maxY state
                |> Set.filter (fun ((y, _), w) -> w.IsFirstWire <> seg.IsFirstWire && (x, y) <> (0, 0))
                |> Set.map (fun ((y, _), w) -> { X = x; Y = y; TotalDelay = WireSegment.getDelayAt y seg + WireSegment.getDelayAt x w })
            Set.toArray intersections, state)
    |> fst
    |> Array.collect id

let solvePart1 (wire1, wire2) =
    let segments = Array.append (stridesToLines true wire1) (stridesToLines false wire2)
    let intersections = findIntersections segments
    intersections
    |> Array.map (fun { X = x; Y = y } -> abs x + abs y)
    |> Array.min
    
let solvePart2 (wire1, wire2) = 
    let segments = Array.append (stridesToLines true wire1) (stridesToLines false wire2)
    let intersections = findIntersections segments
    intersections
    |> Array.map (fun { TotalDelay = d } -> d)
    |> Array.min

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }