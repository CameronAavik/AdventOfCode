module Year2017Day19

open AdventOfCode.FSharp.Common

type State = {x: int; y: int; dx: int; dy: int; steps: int; letters: string}
let step state = {state with x=state.x+state.dx; y=state.y+state.dy; steps=state.steps+1}
let solve (diagram : string list) = 
    let rec move state = 
        match diagram.[state.y].[state.x], state.dx with
        | ' ', _ -> state
        | '+', 0 -> move ({state with dx = (if diagram.[state.y].[state.x-1] = ' ' then 1 else -1); dy = 0} |> step)
        | '+', _ -> move ({state with dy = (if diagram.[state.y-1].[state.x] = ' ' then 1 else -1); dx = 0} |> step)
        | '-', _ | '|', _ -> move (step state)
        | x, _ -> move ({state with letters = state.letters + x.ToString()} |> step)
    move {x=diagram.[0].IndexOf('|'); y=0; dx=0; dy=1; steps=0; letters=""}
let solver = {parse = parseEachLine asString >> Seq.toList; part1 = solve >> (fun s -> s.letters); part2 = solve >> (fun s -> s.steps)}