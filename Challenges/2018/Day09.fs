module Year2018Day09

open CameronAavik.AdventOfCode.Common

let asMarblesGame = splitBy " " (fun s -> (int s.[0], int s.[6]))
        
// circular zipper thing, not sure of a faster way to do this
type Circle<'t> = {before: 't list; cur: 't; after: 't list}
let insert n circle = {circle with before=circle.cur::circle.before; cur=n}
let rotateClockwise {before=before; cur=cur; after=after} =
    match after with
    | [] ->
        let newAfter = List.rev (cur::before)
        {before=[]; cur=List.head newAfter; after=List.tail newAfter}
    | x :: xs -> {before=cur::before; cur=x; after=xs}
let rotateAntiClockwise {before=before; cur=cur; after=after} =
    match before with
    | [] ->
        let newBefore = List.rev (cur::after)
        {before=List.tail newBefore; cur=List.head newBefore; after=[]}
    | x :: xs -> {before=xs; cur=x; after=cur::after}
let rec rotate n circle =
    match n with
    | 0 -> circle
    | _ when n > 0 -> circle |> rotateClockwise     |> rotate (n - 1)
    | _            -> circle |> rotateAntiClockwise |> rotate (n + 1)
let removeCurrent {before=before; after=after} =
    match after with
    | [] -> {before=List.tail before; cur=List.head before; after=after} |> rotate 1
    | x :: xs -> {before=before; cur=x; after=xs}
        
let solve (players, marbles) =
    let rec play i player circle scores =
        if i = (int64 marbles) then
            scores |> Map.toSeq |> Seq.map snd |> Seq.max
        elif i % 23L = 0L then
            let curScore = getOrDefault player scores 0L
            let rotated = circle |> rotate (-7)
            let newScores = Map.add player (curScore + rotated.cur + i) scores
            let circleWithCurrentRemoved = rotated |> removeCurrent
            play (i + 1L) ((player + 1) % players) circleWithCurrentRemoved newScores
        else
            play (i + 1L) ((player + 1) % players) (circle |> (rotate 1) |> (insert i)) scores
    play 1L 1 {before=[]; cur=0L; after=[]} Map.empty

let solver = {parse = parseFirstLine asMarblesGame; part1 = solve; part2 = (fun (p, m) -> solve (p, 100 * m))}