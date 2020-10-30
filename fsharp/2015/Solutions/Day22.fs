module Year2015Day22

open AdventOfCode.FSharp.Common

type Spell = MagicMissile | Drain | Shield | Poison | Recharge
let spells = set [| MagicMissile; Drain; Shield; Poison; Recharge |]

let spellCost = function | MagicMissile -> 53 | Drain -> 73 | Shield -> 113 | Poison -> 173  | Recharge -> 229
let spellDuration = function | MagicMissile -> 0| Drain -> 0 | Shield -> 6 | Poison -> 6 | Recharge -> 5

type GameData = { BossDamage : int; PlayerTurnDamage : int }
type State = { BossHP : int; PlayerHP : int; PlayerMana : int; Spells : Map<Spell, int> }

let addSpell state spell =
    let state' =
        match spell with
        | MagicMissile -> { state with State.BossHP = max 0 (state.BossHP - 4) }
        | Drain -> { state with BossHP = max 0 (state.BossHP - 2); PlayerHP = state.PlayerHP + 2 }
        | _ -> { state with Spells = Map.add spell (spellDuration spell) state.Spells }
    { state' with PlayerMana = state'.PlayerMana - (spellCost spell); }

let applyPlayerTurnDamage gameData state =
    { state with PlayerHP = state.PlayerHP - gameData.PlayerTurnDamage }

let applyPoisonIfExists state =
    if Map.containsKey Poison state.Spells then { state with BossHP = state.BossHP - 3 }
    else state

let applyRechargeIfExists state =
    if Map.containsKey Recharge state.Spells then { state with PlayerMana = state.PlayerMana + 101 }
    else state

let decreaseAllSpellDurations state =
    let newSpells = Map.fold (fun m s c -> if c > 1 then Map.add s (c - 1) m else m) Map.empty state.Spells
    { state with Spells = newSpells }

let makePlayerTurn gameData state =
    state
    |> applyPlayerTurnDamage gameData
    |> applyPoisonIfExists
    |> applyRechargeIfExists
    |> decreaseAllSpellDurations

let applyBossDamage gameData state =
    if state.BossHP > 0 then
        let shield = if Map.containsKey Shield state.Spells then 7 else 0
        { state with PlayerHP = state.PlayerHP - (gameData.BossDamage - shield) }
    else state

let makeBossTurn gameData state =
    state
    |> applyPoisonIfExists
    |> applyRechargeIfExists
    |> applyBossDamage gameData
    |> decreaseAllSpellDurations

let tryAllPossibleSpells gameData state =
    let state' = makePlayerTurn gameData state
    spells
    |> Set.filter (fun spell -> not (Map.containsKey spell state'.Spells)) // spell must not be active
    |> Seq.map (fun spell -> addSpell state' spell, spellCost spell)       // use spell and capture the mana cost
    |> Seq.filter (fun (p, _) -> p.PlayerMana > 0)                         // player's mana should be above 0
    |> Seq.map (fun (s, c) -> makeBossTurn gameData s, c)                  // make the boss's turn
    |> Seq.filter (fun (p, _) -> p.PlayerHP > 0)                           // make sure the player still has HP after the boss's turn

let dijkstra start isFinished gameData =
    let rec dijkstra' fringe =
        if Set.isEmpty fringe then None
        else
            let (dist, vertex) as minElem = Set.minElement fringe
            let fringe' = Set.remove minElem fringe
            if isFinished vertex then Some dist
            else
                tryAllPossibleSpells gameData vertex
                |> Seq.map (fun (v, d) -> (dist + d, v))
                |> Set.ofSeq
                |> Set.union fringe'
                |> dijkstra'
    dijkstra' (set [0, start])

type Input = { BossHP : int; BossDamage : int }
let parseBoss (lines : (int []) seq) =
    let lineArr = lines |> Seq.toArray
    { BossHP = lineArr.[0].[0]; BossDamage = lineArr.[1].[0] }

let parse = parseEachLine extractInts >> parseBoss

let solve playerTurnDamage (input : Input) =
    let gameData = { BossDamage = input.BossDamage; PlayerTurnDamage = playerTurnDamage }
    let state = { BossHP = input.BossHP; PlayerHP = 50; PlayerMana = 500; Spells = Map.empty }
    match dijkstra state (fun s -> s.BossHP <= 0) gameData with
    | Some dist -> dist
    | None -> failwith "Unable to beat the boss"

let solver = { parse = parse; part1 = solve 0; part2 = solve 1 }