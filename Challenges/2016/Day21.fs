module Year2016Day21

open CameronAavik.AdventOfCode.Common

type Operation = 
    | RotateLeft of int
    | RotateRight of int
    | SwapLetter of char * char
    | SwapPosition of int * int
    | RotateByLetter of char
    | UndoRotateByLetter of char
    | Reverse of int * int
    | Move of int * int

let asOperation str =
    let parts : string [] = splitBy " " id str
    match (parts.[0] + " " + parts.[1]) with
    | "rotate right" -> RotateRight (int parts.[2])
    | "rotate left" -> RotateLeft (int parts.[2])
    | "swap letter" -> SwapLetter (parts.[2].[0], parts.[5].[0])
    | "rotate based" -> RotateByLetter (parts.[6].[0])
    | "swap position" -> SwapPosition (int parts.[2], int parts.[5])
    | "reverse positions" -> Reverse (int parts.[2], int parts.[4])
    | "move position" -> Move (int parts.[2], int parts.[5])
    | _ -> failwithf "Invalid: %A" str

let parse = parseEachLine asOperation

let applyOperation (str : char []) operation = 
    match operation with
    | RotateLeft i -> str |> Array.permute (fun j -> (j - i + str.Length) % str.Length)
    | RotateRight i -> str |> Array.permute (fun j -> (j + i) % str.Length)
    | SwapLetter (a, b) ->
        let aI = Array.findIndex (fun j -> j = a) str
        let bI = Array.findIndex (fun j -> j = b) str
        str |> Array.permute (fun j -> if j = aI then bI elif j = bI then aI else j)
    | SwapPosition (i, j) -> str |> Array.permute (fun k -> if k = i then j elif k = j then i else k)
    | RotateByLetter a ->
        let aI = Array.findIndex (fun j -> j = a) str
        let offset = if aI >= 4 then aI + 2 else aI + 1
        str |> Array.permute (fun j -> (j + offset) % str.Length)
    | UndoRotateByLetter a ->
        let aI = Array.findIndex (fun j -> j = a) str
        let offset = 
            match aI with
            | 0 -> 7 | 1 -> 0 | 2 -> 4 | 3 -> 1 | 4 -> 5 | 5 -> 2 | 6 -> 6 | 7 -> 3 | _ -> failwithf "Invalid offset: %i" aI
        let offset = if offset >= 4 then offset + 2 else offset + 1
        str |> Array.permute (fun j -> (j - offset + 2 * str.Length) % str.Length)
    | Reverse (i, j) ->
        str |> Array.permute (fun k -> if k < i || k > j then k else j - (k - i))
    | Move (i, j) ->
        if i < j then
            str |> Array.permute (fun k -> if k < i || k > j then k elif k = i then j else (k - 1))
        else
            str |> Array.permute (fun k -> if k > i || k < j then k elif k = i then j else (k + 1))
        
let solve starter operations =
    operations
    |> Seq.fold applyOperation (starter |> Seq.toArray)
    |> charsToStr

let solvePart1 operations = solve "abcdefgh" operations

let reverseOperation operation = 
    match operation with
    | RotateRight i -> RotateLeft i
    | RotateLeft i -> RotateRight i
    | RotateByLetter a -> UndoRotateByLetter a
    | Move (i, j) -> Move (j, i)
    | _ -> operation

let solvePart2 operations = 
    operations
    |> Seq.map reverseOperation
    |> Seq.rev
    |> solve "fbgdceah"

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }