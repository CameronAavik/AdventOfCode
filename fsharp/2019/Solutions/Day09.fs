module Year2019Day09

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

let provideInput systemId =
    function
    | Input f -> f systemId
    | _ -> failwith "Expected an input"

let readOutput =
    function
    | Output (o, _) -> o
    | _ -> failwith "Expected an output"

let solve boostMode =
    Computer.create
    >> run
    >> provideInput boostMode
    >> readOutput
    >> List.head

let solver = { parse = parseIntCode; part1 = solve 1L; part2 = solve 2L }