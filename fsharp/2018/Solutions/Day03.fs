module Year2018Day03

open AdventOfCode.FSharp.Common

type Claim = {_id: int; left: int; top: int; width: int; height: int}
let asClaim (str : string) =
    let parts = str.Split(" ,x:#".ToCharArray())
    {_id = int parts.[1]; left = int parts.[3]; top = int parts.[4]; width = int parts.[6]; height = int parts.[7]}

type Interval<'a> = {start: int; stop: int; data: 'a}
type Boundary<'a> = {cursor: int; isAdding: bool; data: 'a}
let sweep processBoundary startState =
    let intervalToBoundaries i = 
        [{cursor=i.start; isAdding=true; data=i.data}; {cursor=i.stop; isAdding=false; data=i.data}]
    Seq.map intervalToBoundaries >> Seq.concat >> Seq.sortBy (fun b -> b.cursor) >> Seq.fold processBoundary startState
        
let getArea1D =
    let processBoundary (prevX, count, total) {cursor=cursor; isAdding=isAdding} =
        (cursor, count + (if isAdding then 1 else -1), total + (if count > 1 then 1L else 0L) * int64 (cursor-prevX))
    sweep processBoundary (0, 0, 0L) >> (fun (_, _, total) -> total)

let getArea2D =
    let processBoundary (prevY, claimSet, total) {cursor=cursor; isAdding=isAdding; data=(top, height)} =
        let newSet = (if isAdding then Set.add else Set.remove) {start=top; stop=top+height; data=()} claimSet
        let area1d = if cursor = prevY then 0L else getArea1D claimSet
        (cursor, newSet, total + area1d * int64 (cursor-prevY))
    Seq.map (fun c -> {start=c.left; stop=c.left+c.width; data=(c.top, c.height)})
    >> sweep processBoundary (0, Set.empty, 0L)
    >> (fun (_, _, total) -> total)
        
let getOverlapping1D =
    let processBoundary (currentClaims, overlap, prevY) {cursor=cursor; isAdding=isAdding; data=_id} =
        let newClaims = (if isAdding then Set.add else Set.remove) _id currentClaims
        let newOverlap = if (Set.count currentClaims > 1 && cursor <> prevY) then (Set.union currentClaims overlap) 
                            else overlap
        (newClaims, newOverlap, cursor)
    sweep processBoundary (Set.empty, Set.empty, 0) >> (fun (_, overlap, _) -> overlap)

let filterOverlapping2D claims =
    let ids = claims |> Seq.map (fun c -> c._id) |> Set.ofSeq
    let processBoundary (noOverlap, found, claimSet, prevX) {cursor=cursor; isAdding=isAdding; data=(top, height, _id)} =
        if found then (noOverlap, true, claimSet, cursor)
        else
            let newSet = (if isAdding then Set.add else Set.remove) {start=top; stop=top+height; data=_id} claimSet
            let overlapping = if prevX = cursor then Set.empty else getOverlapping1D claimSet
            let newNoOverlap = Set.difference noOverlap overlapping
            // a little bit of short circuiting
            if not isAdding && Set.contains _id newNoOverlap then (Set.singleton _id, true, claimSet, cursor)
            else (newNoOverlap, false, newSet, cursor)
    claims
    |> Seq.map (fun c -> {start=c.left; stop=c.left+c.width; data=(c.top, c.height, c._id)})
    |> sweep processBoundary (ids, false, Set.empty, 0)
    |> fun (noOverlap, _, _, _) -> Seq.head noOverlap

let solver = {parse = parseEachLine asClaim; part1 = getArea2D; part2 = filterOverlapping2D}