namespace CameronAavik.AdventOfCode

open System
open System.IO
open System.Text.RegularExpressions

// This is a set of methods that I found myself using many times in my solutions
module Common =
    // Every day has a corresponding Day record which defines how to parse the file
    // then two functions for soving each part respectively
    type Day<'a, 'b, 'c> = { parse: string -> 'a; part1: 'a -> 'b; part2: 'a -> 'c }

    // helper methods for parsing
    let parseFirstLine f (fileName : string) =
        use fs = new FileStream(fileName, FileMode.Open)
        use reader = new StreamReader(fs)
        f (reader.ReadLine())

    let parseEachLine f = File.ReadLines >> Seq.map f
    let parseEachLineIndexed f = File.ReadLines >> Seq.mapi f
    let asString : string -> string = id
    let asInt : string -> int = int
    let asStringArray : string [] -> string [] = Array.map string
    let asIntArray : string [] -> int [] = Array.map int
    let splitBy (c : string) f (str : string) = str.Split([| c |], StringSplitOptions.None) |> f
    let extractInts str = [| for m in Regex.Matches(str, "(-?\d+)") -> int m.Value |]
    let withRegex regex str = [| for m in Regex.Match(str, regex).Groups -> m.Value|] |> Array.tail
    let charsToStr (chars : char seq) = chars |> Seq.map string |> String.concat ""