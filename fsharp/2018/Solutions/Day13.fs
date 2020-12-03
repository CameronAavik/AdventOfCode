module Year2018Day13

open AdventOfCode.FSharp.Common

type NextTurn = Left | Straight | Right
type Cart = {x: int; y: int; xDir: int; yDir: int; nextTurn: NextTurn}
let newCart x y xDir yDir = {x=x; y=y; xDir=xDir; yDir=yDir; nextTurn=Left}
let getPos cart = cart.x, cart.y

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

// By maintaining x direction and y direction separately, it simplifies turning
// and corner handling logic
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

type TrackState = {
    cartLocations: Map<int * int, int>; // contains a lookup to see what cart id is at a coordinate
    carts: Map<int, Cart>; // contains the list of carts, can lookup by id
    collisions: (int * int) list // list of x, y coordinates of collisions in reverse order
}

let handleCollision trackState oldCart newCart cartId =
    let newPos = getPos newCart
    let collidedId = Map.find newPos trackState.cartLocations
    // remove both carts involved in the collision from both maps
    let updatedLocations =
        trackState.cartLocations
        |> Map.remove (getPos oldCart)
        |> Map.remove newPos
    let updatedCarts =
        trackState.carts
        |> Map.remove collidedId
        |> Map.remove cartId
    let updatedCollisions = newPos::trackState.collisions
    {cartLocations=updatedLocations; carts=updatedCarts; collisions=updatedCollisions}

let handleNoCollision trackState oldCart newCart cartId =
    // remove old cart location and add new cart location
    let updatedLocations =
        trackState.cartLocations
        |> Map.remove (getPos oldCart)
        |> Map.add (getPos newCart) cartId
    // update the cart map with the new cart data
    let updatedCarts = trackState.carts |> Map.add cartId newCart
    {cartLocations=updatedLocations; carts=updatedCarts; collisions=trackState.collisions}

let applyNewTickedCart trackState oldCart newCart cartId =
    if Map.containsKey (getPos newCart) trackState.cartLocations then
        handleCollision trackState oldCart newCart cartId
    else
        handleNoCollision trackState oldCart newCart cartId

let tick grid trackState =
    let rec processCarts trackState carts =
        match carts with
        | [] -> trackState
        | (i, cart) :: carts ->
            // check if the cart was already removed from the carts list
            // this may happen if we collide with a cart that is processed
            // later in the tick
            if Map.containsKey i trackState.carts then
                let tickedCart = tickCart grid cart
                let newTrackState = applyNewTickedCart trackState cart tickedCart i
                processCarts newTrackState carts
            else
                processCarts trackState carts
    trackState.carts
    |> Map.toSeq
    |> Seq.toList
    |> List.sortBy (fun (_, c) -> (c.y, c.x))
    |> processCarts trackState

let buildTrackState carts =
    let indexedCarts = carts |> Seq.mapi (fun i cart -> (i, cart))
    let locationMap = indexedCarts |> Seq.map (fun (i, cart) -> (cart.x, cart.y), i) |> Map.ofSeq
    let cartMap = indexedCarts |> Map.ofSeq
    {cartLocations=locationMap; carts=cartMap; collisions=[]}

let part1 grid =
    let rec getFirstCollision trackState =
        let nextState = tick grid trackState
        if List.isEmpty nextState.collisions then
            getFirstCollision nextState
        else
            List.last nextState.collisions
    getFirstCollision

let part2 grid =
    let rec getLastCartLocation trackState =
        let nextState = tick grid trackState
        if Map.count nextState.carts > 1 then
            getLastCartLocation nextState
        else
            nextState.cartLocations |> Map.toSeq |> Seq.head |> fst
    getLastCartLocation

let solve part lines =
    let grid, carts = extractCarts lines
    let x, y = part grid (buildTrackState carts)
    sprintf "%i,%i" x y

let solver = {parse = parseEachLine asString; part1 = solve part1; part2 = solve part2}