module Year2016Day11

open AdventOfCode.FSharp.Common
open FSharpx.Collections

type Element = Promethium | Cobalt | Curium | Ruthenium | Plutonium | Elerium | Dilithium
type Floor = {floor: int; generators: Set<Element>; microchips: Set<Element>}

let parse lines =
    let floor1 = {floor=0; generators=set [Promethium]; microchips=set [Promethium]}
    let floor2 = {floor=1; generators=set [Cobalt; Curium; Ruthenium; Plutonium]; microchips=Set.empty}
    let floor3 = {floor=2; generators=set []; microchips=set [Cobalt; Curium; Ruthenium; Plutonium]}
    let floor4 = {floor=3; generators=set []; microchips=set []}
    [|floor1; floor2; floor3; floor4|]

type Item = Generator of Element | Microchip of Element
type Action = {fromFloor: int; toFloor: int; generators: Set<Element>; microchips: Set<Element>}

let isValidCombination generators microchips =
    let unmatchedMicrochips = Set.difference microchips generators
    Set.count generators = 0 || Set.count unmatchedMicrochips = 0

let isValidFloor (floor : Floor) = isValidCombination floor.generators floor.microchips

type State = {floors: Floor []; curFloor: int}

let applyAction state action =
    let fromF = state.floors.[action.fromFloor]
    let toF = state.floors.[action.toFloor]
    let removedFrom = 
        {fromF with 
            generators=Set.difference fromF.generators action.generators;
            microchips=Set.difference fromF.microchips action.microchips}
    let addedTo = 
        {toF with 
            generators=Set.union toF.generators action.generators;
            microchips=Set.union toF.microchips action.microchips}
    let newFloors =
        Array.mapi (fun i v -> 
            if i = action.fromFloor then
                removedFrom
            elif i = action.toFloor then
                addedTo
            else
                v) state.floors
    {curFloor=action.toFloor; floors=newFloors}

let isValidAction state action =
    let result = (applyAction state action).floors
    let fromFloor = result.[action.fromFloor]
    let toFloor = result.[action.toFloor]
    isValidFloor fromFloor && isValidFloor toFloor && isValidCombination action.generators action.microchips

let getActions state =
    let floors = state.floors
    let curFloor = floors.[state.curFloor]
    let curFloorNum = state.curFloor
    let possibleFloors = 
        match curFloorNum with
        | 0 -> [1]
        | 1 -> [0; 2]
        | 2 -> [1; 3]
        | _ -> [2]
    
    seq {
        for f in possibleFloors do
            for g1 in curFloor.generators do
                yield {fromFloor=curFloorNum; toFloor=f; generators=set [g1]; microchips=set[]}
                for g2 in curFloor.generators do
                    if g1 <> g2 then
                        yield {fromFloor=curFloorNum; toFloor=f; generators=set [g1; g2]; microchips=set[]}
                for m2 in curFloor.microchips do
                    yield {fromFloor=curFloorNum; toFloor=f; generators=set [g1]; microchips=set[m2]}
            for m1 in curFloor.microchips do
                yield {fromFloor=curFloorNum; toFloor=f; generators=set []; microchips=set[m1]}
                for m2 in curFloor.microchips do
                    if m1 <> m2 then
                        yield {fromFloor=curFloorNum; toFloor=f; generators=set []; microchips=set[m1; m2]}
    } |> Seq.filter (isValidAction state)

let heuristic src =
    let f = src.floors
    let itemsFirstFloor = Set.count f.[0].generators + Set.count f.[0].microchips
    let itemsSecondFloor = Set.count f.[1].generators + Set.count f.[1].microchips
    let itemsThirdFloor = Set.count f.[2].generators + Set.count f.[2].microchips

    let tripsFirstFloor = itemsFirstFloor / 2
    let tripsSndFloor = itemsSecondFloor / 2
    let tripsThirdFloor = itemsThirdFloor / 2

    (tripsFirstFloor) * 3 + 
    (tripsSndFloor) * 2 + 
    (tripsThirdFloor)

type Prioritised<'t> = {priority: int; dist: int; value: 't}
let toPrio dist state = {priority=heuristic state + dist; dist=dist; value=state}

type IntFloor = {floor: int; gens: Set<int>; chips: Set<int>}

let serialise state =
    let floors = state.floors
    let order = 
        floors
        |> Array.map (fun f -> Array.append (Set.toArray f.generators) (Set.toArray f.microchips))
        |> Array.concat
        |> Array.distinct
        |> Array.mapi (fun i v -> (v, i))
        |> Map.ofArray
    
    let toIntFloor (floor : Floor) =
        let gens = floor.generators |> Set.map (fun v -> Map.find v order)
        let chips = floor.microchips |> Set.map (fun v -> Map.find v order)
        {floor=floor.floor; gens=gens; chips=chips}
    
    (state.curFloor, floors |> Array.map toIntFloor)
    

let aStar initState dest =
    let rec aStar' pQ seen =
        let {value=state; dist=dist}, pQ = PriorityQueue.pop pQ
        let ser = serialise state
        if state = dest then
            dist
        else
            if Set.contains ser seen then
                aStar' pQ seen
            else
                let seen = Set.add ser seen
                let newPQ = 
                    state
                    |> getActions
                    |> Seq.map (applyAction state)
                    |> Seq.filter (fun s -> Set.contains (serialise s) seen |> not)
                    |> Seq.map (toPrio (dist + 1))
                    |> Seq.fold (fun p s -> PriorityQueue.insert s p) pQ
                aStar' newPQ seen
    let pQ =
        PriorityQueue.empty false
        |> PriorityQueue.insert (toPrio 0 initState)
    aStar' pQ Set.empty


let bfs initState dest =
    let rec bfs' queue seen steps =
        let newSeen = Set.union seen queue
        let buildNextStates nextStates state =
            let actions = getActions state
            Set.union nextStates (set (Seq.map (applyAction state) actions))
        let nextLevel = Set.fold buildNextStates Set.empty queue
        let nextLevel = Set.difference nextLevel newSeen
        if Set.contains dest nextLevel then
            steps + 1
        else
            bfs' nextLevel newSeen (steps + 1)
    bfs' (set [initState]) Set.empty 0
    
let testFloors =
    let floor1 = {floor=0; generators=set []; microchips=set [Cobalt; Curium]}
    let floor2 = {floor=1; generators=set [Cobalt]; microchips=set []}
    let floor3 = {floor=2; generators=set [Curium]; microchips=set []}
    let floor4 = {floor=3; generators=set []; microchips=set []}
    [|floor1; floor2; floor3; floor4|]

let solvePart1 floors =
    let initState = {curFloor=0; floors=floors}
    let emptyFloor i = {floor=i; generators=set []; microchips=set []}
    let finalDestFloor = {floor=3; generators = set [Cobalt; Curium; Ruthenium; Plutonium; Promethium]; microchips=set [Cobalt; Curium; Ruthenium; Plutonium; Promethium]}
    let destState = 
        {curFloor=3; 
         floors = [|emptyFloor 0; emptyFloor 1; emptyFloor 2; finalDestFloor|]}
    aStar initState destState

let floorsPart2 =
    let floor1 = {floor=0; generators=set [Promethium; Elerium; Dilithium]; microchips=set [Promethium; Elerium; Dilithium]}
    let floor2 = {floor=1; generators=set [Cobalt; Curium; Ruthenium; Plutonium]; microchips=Set.empty}
    let floor3 = {floor=2; generators=set []; microchips=set [Cobalt; Curium; Ruthenium; Plutonium]}
    let floor4 = {floor=3; generators=set []; microchips=set []}
    [|floor1; floor2; floor3; floor4|]

let solvePart2 floors = 
    let floors = floorsPart2
    let initState = {curFloor=0; floors=floors}
    let emptyFloor i = {floor=i; generators=set []; microchips=set []}
    let finalDestFloor = {floor=3; generators = set [Cobalt; Curium; Ruthenium; Plutonium; Promethium; Elerium; Dilithium]; microchips=set [Cobalt; Curium; Ruthenium; Plutonium; Promethium; Elerium; Dilithium]}
    let destState = 
        {curFloor=3; 
         floors = [|emptyFloor 0; emptyFloor 1; emptyFloor 2; finalDestFloor|]}
    aStar initState destState

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }