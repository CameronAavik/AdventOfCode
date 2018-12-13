module Year2018Day13

open CameronAavik.AdventOfCode.Common

type NextTurn = Left | Straight | Right
type Cart = {x: int; y: int; xDir: int; yDir: int; nextTurn: NextTurn}
let newCart x y xDir yDir = {x=x; y=y; xDir=xDir; yDir=yDir; nextTurn=Left}

type TrackPiece = Vertical | Horizontal | Intersection | ForwardCurve | BackCurve | NoTrack

let extractCarts tracks =
    let grid = tracks |> array2D
    let carts = seq {
        for y = 0 to (grid.GetLength 0) - 1 do
            for x = 0 to (grid.GetLength 1) - 1 do
                match grid.[y,x] with
                | '^' -> yield newCart x y  0 -1
                | 'v' -> yield newCart x y  0  1
                | '<' -> yield newCart x y -1  0
                | '>' -> yield newCart x y  1  0
                | _ -> () }
    let cartToTrack =
        function
        | '^' | 'v' | '|' -> Vertical
        | '>' | '<' | '-' -> Horizontal
        | '+' -> Intersection
        | '/' -> ForwardCurve
        | '\\' -> BackCurve
        | ' ' -> NoTrack
        | _ -> failwith "Invalid Track Piece"
    let extractedGrid = grid |> Array2D.map cartToTrack
    extractedGrid, carts

let tickCart (grid : TrackPiece [,]) cart =
    let newX, newY = cart.x + cart.xDir, cart.y + cart.yDir
    let cartInNewPos = {cart with x = newX; y = newY}
    match grid.[newY, newX] with
    | Vertical | Horizontal -> cartInNewPos
    | ForwardCurve -> {cartInNewPos with xDir=(-cart.yDir); yDir=(-cart.xDir)}
    | BackCurve -> {cartInNewPos with xDir=cart.yDir; yDir=cart.xDir}
    | Intersection ->
        match cart.nextTurn with
        | Left -> {cartInNewPos with xDir=cart.yDir; yDir=(-cart.xDir); nextTurn=Straight}
        | Straight -> {cartInNewPos with nextTurn=Right}
        | Right -> {cartInNewPos with xDir=(-cart.yDir); yDir=cart.xDir; nextTurn=Left}
    | NoTrack -> failwithf "Cart fell off the track at %i, %i" newX newY

type TrackState = {cartLocations: Map<int * int, int>; carts: Map<int, Cart>; collisions: (int * int) list}
let applyNewTickedCart trackState oldCart newCart cartId =
    let newLocations = Map.remove (oldCart.x, oldCart.y) trackState.cartLocations
    let newPos = (newCart.x, newCart.y)
    if Map.containsKey newPos newLocations then
        let collidedId = Map.find newPos newLocations
        let newLocations' = Map.remove newPos newLocations
        let newCarts =
            trackState.carts
            |> Map.remove collidedId
            |> Map.remove cartId
        {cartLocations=newLocations'; carts=newCarts; collisions=(newPos::trackState.collisions)}
    else
        let newLocations' = Map.add newPos cartId newLocations
        let newCarts = Map.add cartId newCart trackState.carts
        {cartLocations=newLocations'; carts=newCarts; collisions=trackState.collisions}

let tick grid trackState =
    let cartList = trackState.carts |> Map.toSeq |> Seq.toList
    let cartOrder = cartList |> List.sortBy (fun (_, c) -> (c.y, c.x))
    let rec processCart trackState carts =
        match carts with
        | [] -> trackState
        | (i, cart) :: carts ->
            if Map.containsKey i trackState.carts then
                let tickedCart = tickCart grid cart
                let newTrackState = applyNewTickedCart trackState cart tickedCart i
                processCart newTrackState carts
            else
                processCart trackState carts
    processCart trackState cartOrder

let buildTrackState carts =
    let indexedCarts = carts |> Seq.mapi (fun i cart -> (i, cart))
    let locationMap = indexedCarts |> Seq.map (fun (i, cart) -> (cart.x, cart.y), i) |> Map.ofSeq
    let cartMap = indexedCarts |> Map.ofSeq
    {cartLocations=locationMap; carts=cartMap; collisions=[]}

let solvePart1 lines =
    let grid, carts = extractCarts lines
    let rec getFirstCollision trackState =
        let nextState = tick grid trackState
        if List.isEmpty nextState.collisions then
            getFirstCollision nextState
        else
            let x, y = List.last nextState.collisions
            sprintf "%i,%i" x y
    let initialTrackState = buildTrackState carts
    getFirstCollision initialTrackState

let solvePart2 lines =
    let grid, carts = extractCarts lines
    let rec getLastCartLocation trackState =
        let nextState = tick grid trackState
        if Map.count nextState.carts > 1 then
            getLastCartLocation nextState
        else
            let _, cart = nextState.carts |> Map.toSeq |> Seq.head
            sprintf "%i,%i" cart.x cart.y
    let initialTrackState = buildTrackState carts
    getLastCartLocation initialTrackState

let solver = {parse = parseEachLine asString; part1 = solvePart1; part2 = solvePart2}