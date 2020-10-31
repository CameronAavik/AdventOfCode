using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Net;

namespace AdventOfCode.CSharp.Y2018.Solvers
{
    public class Day1 : ISolver
    {
        public class Frequency
        {
            public int Value { get; set; }

            public int Index { get; set; }

            public int ModTotal { get; set; }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            int freqIndex = 0;
            int freqTotal = 0;
            var freqs = new List<Frequency>();
            foreach (var freqChange in input.Split('\n'))
            {
                freqs.Add(new Frequency { Value = freqTotal, Index = freqIndex });
                freqIndex++;
                freqTotal += Int32.Parse(freqChange);
            }

            foreach (var freq in freqs)
            {
                int mod = freq.Value % freqTotal;
                freq.ModTotal = mod < 0 ? mod + freqTotal : mod;
            }

            // sort by mods first, then by value
            freqs.Sort((a, b) => a.ModTotal != b.ModTotal
                ? a.ModTotal.CompareTo(b.ModTotal)
                : a.Value.CompareTo(b.Value));

            var prev = new Frequency { ModTotal = -1 };
            int minDiff = Int32.MaxValue;
            int minIndex = Int32.MaxValue;
            int minFreq = 0;
            foreach (var freq in freqs)
            {
                if (freq.ModTotal == prev.ModTotal)
                {
                    int diff = freq.Value - prev.Value;
                    if (diff < minDiff || (diff == minDiff && prev.Index < minIndex))
                    {
                        minDiff = diff;
                        minIndex = prev.Index;
                        minFreq = freq.Value;
                    }
                }

                prev = freq;
            }

            return new Solution(
                part1: freqTotal.ToString(),
                part2: minFreq.ToString());
        }
    }
}
