module Year2015Day12

open AdventOfCode.FSharp.Common
open Newtonsoft.Json.Linq

let parse = parseFirstLine asString

let solvePart1 input = extractInts input |> Seq.sum

let solvePart2 input =
    let rec getTotals (obj : JToken) =
        match obj.Type with
        | JTokenType.Integer -> int obj
        | JTokenType.Array -> (obj :?> JArray) |> Seq.sumBy getTotals
        | JTokenType.Object ->
            let values : JToken seq = (obj :?> JObject) |> Seq.map (fun kvp -> (kvp :?> JProperty).Value)
            let hasRed =
                values
                |> Seq.exists (fun t -> t.Type = JTokenType.String && (string t) = "red")
            if hasRed then 0
            else values |> Seq.sumBy getTotals
        | _ -> 0

    let asJson = JObject.Parse ("{\"data\": " + input + "}")
    getTotals asJson

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }
