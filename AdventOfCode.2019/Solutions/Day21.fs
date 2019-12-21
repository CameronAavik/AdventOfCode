module Year2019Day21

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.Intcode

let debug = false

let solve program =
    let printAscii = Seq.map char >> charsToStr
    let rec runProg input comp =
        match input, comp with
        | x :: xs, Input f -> f x |> runProg xs
        | x :: xs, Output (o, s) ->
            if debug then printf "%s" (printAscii o)
            s |> runProg input
        | [], Output (o, s) ->
            if debug then printf "%s" (printAscii o)
            o |> List.last
        | _, _ -> failwith "ERR"

    let inputAsStr = ((String.concat "\n" program) + "\n" |> Seq.map int64 |> Seq.toList)
    Computer.create >> run >> runProg inputAsStr

(*
    If A is a hole we must jump or we fall into it
    If C is a hole and D is not a hole we must jump or we fail on ####.#..###
    This can be represented by !A || (!C && D)
*)
let part1 =
    [ "NOT C J" // J = !C
      "AND D J" // J = !C && D
      "NOT A T" // T = !A
      "OR T J" // J = !A || (!C && D)
      "WALK" ]

(*
    Similar to part 1, except changing (!C && D) to (!(B && C) && D && (E || H))
    The (E || H) part ensures that when we jump to D, that a floor exists a step or a jump after it.
    The !C is changed to !(B && C) since we want to jump immediately on ##.##...#
*)
let part2 =
    [ "OR H J" // J = H
      "OR E J" // J = E || H
      "AND D J" // J = D && (E || H)
      "OR C T" // T = C
      "AND B T" // T = C && B
      "NOT T T" // T = !(C && B)
      "AND T J" // J = !(B && C) && D && (E || H)
      "NOT A T" // T = !A
      "OR T J" // J = !A || (!(B && C) && D && (E || H))
      "RUN" ]

let solver = { parse = parseIntCode; part1 = solve part1; part2 = solve part2 }