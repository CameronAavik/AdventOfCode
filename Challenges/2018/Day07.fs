module Year2018Day07

open CameronAavik.AdventOfCode.Common

let asEdge = splitBy " " (fun x -> (x.[1], x.[7]))

let getOrDefault key map ``default``= 
    match Map.tryFind key map with
    | Some v -> v
    | None -> ``default``

let toGraph key value = Seq.groupBy key >> Seq.map (fun (k, v) -> (k, Seq.map value v)) >> Map.ofSeq
let getKeySet = Map.toSeq >> Seq.map fst >> Set.ofSeq
let solve workers edges =
    let initialSuccessors = toGraph fst snd edges
    let initialPredecessors = toGraph snd fst edges
    let nodes = Set.union (getKeySet initialSuccessors) (getKeySet initialPredecessors)
            
    let getTaskTime task = int (char task) - 4
    let getNextTask predecessors =
        let hasNoPredecessors task = getOrDefault task predecessors Set.empty |> Set.isEmpty
        Seq.sort >> Seq.tryFind hasNoPredecessors
            
    let shiftTimeAndRemoveFinished minTime =
        List.filter (fst >> (<>) minTime) // remove elves that have minTime left
        >> List.map (fun (time, task) -> (time - minTime, task)) // subtract minTime remaining

    let removeFromPredecessors predecessors toRemove =
        let newPredecessors s =  Set.difference (Map.find s predecessors) toRemove
        toRemove
        |> Seq.collect (fun t -> Map.find t initialSuccessors) // find successors
        |> Seq.map (fun s -> (s, newPredecessors s)) // generate new predecessor set for each successor
        |> Seq.fold (fun m (k, v) -> Map.add k v m) predecessors // update the predecessor map

    let rec step t tasks elves predecessors order =
        if Set.isEmpty tasks then
            (order |> Seq.rev |> String.concat ""), ((List.max elves |> fst) + t)
        else
            match getNextTask predecessors tasks with
            | Some x when List.length elves < workers ->
                step t (Set.remove x tasks) ((getTaskTime x, x)::elves) predecessors (x::order)
            | _ -> 
                let minTime = elves |> List.map fst |> List.min
                let newElves = shiftTimeAndRemoveFinished minTime elves
                let newPredecessors =
                    elves
                    |> List.filter (fst >> (=)minTime) // if the task was finished
                    |> List.map snd // get the task
                    |> Set.ofList
                    |> removeFromPredecessors predecessors // remove task from predecessor map
                step (t + minTime) tasks newElves newPredecessors order

    let predecessorMap = initialPredecessors |> Map.map (fun _ v -> Set.ofSeq v)
    step 0 nodes [] predecessorMap []  

let solver = {parse = parseEachLine asEdge; part1 = solve 1 >> fst; part2 = solve 5 >> snd}