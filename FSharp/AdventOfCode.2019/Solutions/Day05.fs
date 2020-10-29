module Year2019Day05

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.Intcode

let provideInput systemId =
    function
    | Input f -> f systemId
    | _ -> failwith "Expected an input"

let readOutput =
    function
    | Output (o, _) -> o
    | _ -> failwith "Expected an output"
    
let solve systemId =
    Computer.create
    >> run
    >> provideInput systemId
    >> readOutput
    >> List.last

let solver = { parse = parseIntCode; part1 = solve 1L; part2 = solve 5L }