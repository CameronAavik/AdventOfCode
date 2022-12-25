using System;
using System.Collections.Generic;
using System.Text;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day07 : ISolver
{
    readonly record struct Bag(string Modifier, string Colour);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var containsShinyGoldCache = new Dictionary<Bag, bool>();
        var totalChildBagsCache = new Dictionary<Bag, int>();
        var bagContents = new Dictionary<Bag, List<(int Count, Bag Bag)>>();

        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            ParseLine(ref reader, out Bag bag, out bool containsShiny, out List<(int Count, Bag Bag)> contents);

            if (containsShiny)
            {
                containsShinyGoldCache[bag] = true;
            }
            else if (contents.Count == 0)
            {
                containsShinyGoldCache[bag] = false;
                totalChildBagsCache[bag] = 0;
            }

            bagContents[bag] = contents;
        }

        int part1 = 0;
        foreach (Bag colour in bagContents.Keys)
        {
            if (ContainsShinyGold(colour, containsShinyGoldCache, bagContents))
            {
                part1++;
            }
        }

        solution.SubmitPart1(part1);

        int part2 = GetTotalChildBags(new("shiny", "gold"), totalChildBagsCache, bagContents);
        solution.SubmitPart2(part2);
    }

    private static void ParseLine(
        ref SpanReader reader,
        out Bag bag,
        out bool containsShinyGold,
        out List<(int Count, Bag Bag)> bagContents)
    {
        containsShinyGold = false;
        bag = new(BytesToString(reader.ReadUntil(' ')), BytesToString(reader.ReadUntil(' ')));
        reader.SkipLength("bags contain ".Length);

        bagContents = new List<(int Count, Bag Colour)>();

        if (reader.Peek() == 'n') // no other bags
        {
            reader.SkipLength("no other bags.\n".Length);
            return;
        }

        while (true)
        {
            int count = reader.ReadPosIntUntil(' ');
            Bag childBag = new(BytesToString(reader.ReadUntil(' ')), BytesToString(reader.ReadUntil(' ')));
            if (childBag is { Modifier: "shiny", Colour: "gold" })
            {
                containsShinyGold = true;
            }

            bagContents.Add((count, childBag));

            reader.SkipLength(count == 1 ? "bag".Length : "bags".Length);
            if (reader.Peek() == '.')
            {
                reader.SkipLength(".\n".Length);
                return;
            }

            reader.SkipLength(", ".Length);
        }
    }

    private static string BytesToString(ReadOnlySpan<byte> str) => Encoding.ASCII.GetString(str);

    private static bool ContainsShinyGold(
        Bag bag,
        Dictionary<Bag, bool> cache,
        Dictionary<Bag, List<(int Count, Bag Bag)>> bagContents)
    {
        if (cache.TryGetValue(bag, out bool containsShinyGold))
        {
            return containsShinyGold;
        }

        foreach ((int _, Bag childBag) in bagContents[bag])
        {
            if (ContainsShinyGold(childBag, cache, bagContents))
            {
                cache[bag] = true;
                return true;
            }
        }

        cache[bag] = false;
        return false;
    }

    private static int GetTotalChildBags(
        Bag bag,
        Dictionary<Bag, int> cache,
        Dictionary<Bag, List<(int Count, Bag Bag)>> bagContents)
    {
        if (cache.TryGetValue(bag, out int childCount))
        {
            return childCount;
        }

        int total = 0;
        foreach ((int count, Bag childBag) in bagContents[bag])
        {
            total += count * (1 + GetTotalChildBags(childBag, cache, bagContents));
        }

        cache[bag] = total;
        return total;
    }
}
