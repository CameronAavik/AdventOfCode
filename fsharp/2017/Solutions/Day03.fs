module Year2017Day03

open AdventOfCode.FSharp.Common

let manhattanDistance target = 
    let ringNumber = target |> float |> sqrt |> ceil |> int |> (fun r -> r / 2)
    let ringEnd = ringNumber * 2 |> (fun r -> pown r 2) |> (+) 1
    [1 ; 3 ; 5 ; 7] 
    |> Seq.map (fun i -> abs (target - (ringEnd - i * ringNumber)) + ringNumber) 
    |> Seq.min

let getNextValue grid posMap (newX, newY) = 
    if Map.isEmpty posMap then 1
    else
        [(-1, -1); (-1, 0); (-1, 1); (0, -1); (0, 1); (1, -1); (1, 0); (1, 1)]
        |> Seq.map (fun (x, y) -> (newX + x, newY + y))
        |> Seq.filter (fun pos -> Map.containsKey pos posMap)
        |> Seq.sumBy ((fun pos -> Map.find pos posMap) >> (fun pos -> List.item pos grid))

// this somehow manages to calculate the next x and y position given the current position in the spiral. I forget how it works.
let getNextPos (x, y) = 
    if (y <= 0) && (x <= -y) && (y <= x) then (x + 1, y)
    elif (x > 0) && (y < x) then (x, y + 1)
    elif (y > 0) && (-x < y) then (x - 1, y)
    else (x, y - 1)
            
let part2 target = 
    let rec solve grid posMap newPos = 
        let newValue = getNextValue grid posMap newPos
        if newValue > target then newValue 
        else solve (grid @ [ newValue ]) (Map.add newPos grid.Length posMap) (getNextPos newPos)
    solve List.empty Map.empty (0, 0)

let solver = {parse = parseFirstLine asInt; part1 = manhattanDistance; part2 = part2}