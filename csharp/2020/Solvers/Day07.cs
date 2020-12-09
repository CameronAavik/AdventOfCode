using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day07 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var containsShinyGoldCache = new Dictionary<string, bool>();
            var totalChildBagsCache = new Dictionary<string, int>();
            var bagContents = new Dictionary<string, List<(int Count, string Colour)>>();

            var reader = new SpanReader(input);
            while (!reader.Done)
            {
                ParseLine(ref reader, out string colour, out bool containsShiny, out List<(int Count, string Colour)> contents);

                if (containsShiny)
                {
                    containsShinyGoldCache[colour] = true;
                }
                else if (contents.Count == 0)
                {
                    containsShinyGoldCache[colour] = false;
                    totalChildBagsCache[colour] = 0;
                }

                bagContents[colour] = contents;
            }

            int part1 = 0;
            foreach (string? colour in bagContents.Keys)
            {
                if (ContainsShinyGold(colour, containsShinyGoldCache, bagContents))
                {
                    part1++;
                }
            }

            int part2 = GetTotalChildBags("shiny gold", totalChildBagsCache, bagContents);
            return new Solution(part1, part2);
        }

        private static void ParseLine(
            ref SpanReader reader,
            out string bagColour,
            out bool containsShinyGold,
            out List<(int Count, string Colour)> bagContents)
        {
            containsShinyGold = false;
            bagColour = reader.ReadUntil(" bags").ToString();
            reader.SkipLength(" contain ".Length);
            bagContents = new List<(int Count, string Colour)>();

            if (reader.Peek() == 'n') // no other bags
            {
                reader.SkipLength("no other bags.\n".Length);
                return;
            }

            while (true)
            {
                int count = reader.ReadPosIntUntil(' ');
                string? colour = reader.ReadUntil(count == 1 ? " bag" : " bags").ToString();
                if (colour == "shiny gold")
                {
                    containsShinyGold = true;
                }

                bagContents.Add((count, colour));
                if (reader.Peek() == '.')
                {
                    reader.SkipLength(".\n".Length);
                    return;
                }

                reader.SkipLength(", ".Length);
            }
        }

        private static bool ContainsShinyGold(
            string colour,
            Dictionary<string, bool> cache,
            Dictionary<string, List<(int Count, string Colour)>> bagContents)
        {
            if (cache.TryGetValue(colour, out bool containsShinyGold))
            {
                return containsShinyGold;
            }

            foreach ((int _, string contentColour) in bagContents[colour])
            {
                if (ContainsShinyGold(contentColour, cache, bagContents))
                {
                    cache[colour] = true;
                    return true;
                }
            }

            cache[colour] = false;
            return false;
        }

        private static int GetTotalChildBags(
            string colour,
            Dictionary<string, int> cache,
            Dictionary<string, List<(int Count, string Colour)>> bagContents)
        {
            if (cache.TryGetValue(colour, out int childCount))
            {
                return childCount;
            }

            int total = 0;
            foreach ((int count, string contentColour) in bagContents[colour])
            {
                total += count * (1 + GetTotalChildBags(contentColour, cache, bagContents));
            }

            cache[colour] = total;
            return total;
        }
    }
}
