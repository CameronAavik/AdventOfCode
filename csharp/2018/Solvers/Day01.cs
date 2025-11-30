using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2018.Solvers;

public class Day01 : ISolver
{
    public class Frequency
    {
        public int Value { get; set; }

        public int Index { get; set; }

        public int ModTotal { get; set; }
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var freqIndex = 0;
        var freqTotal = 0;
        var freqs = new List<Frequency>();
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            freqs.Add(new Frequency { Value = freqTotal, Index = freqIndex });
            freqIndex++;

            var mul = reader.Peek() == '-' ? -1 : 1;
            reader.SkipLength(1);

            freqTotal += mul * reader.ReadPosIntUntil('\n');
        }

        foreach (var freq in freqs)
        {
            var mod = freq.Value % freqTotal;
            freq.ModTotal = mod < 0 ? mod + freqTotal : mod;
        }

        // sort by mods first, then by value
        freqs.Sort((a, b) => a.ModTotal != b.ModTotal
            ? a.ModTotal.CompareTo(b.ModTotal)
            : a.Value.CompareTo(b.Value));

        var prev = new Frequency { ModTotal = -1 };
        var minDiff = int.MaxValue;
        var minIndex = int.MaxValue;
        var minFreq = 0;
        foreach (var freq in freqs)
        {
            if (freq.ModTotal == prev.ModTotal)
            {
                var diff = freq.Value - prev.Value;
                if (diff < minDiff || (diff == minDiff && prev.Index < minIndex))
                {
                    minDiff = diff;
                    minIndex = prev.Index;
                    minFreq = freq.Value;
                }
            }

            prev = freq;
        }

        solution.SubmitPart1(freqTotal);
        solution.SubmitPart2(minFreq);
    }
}
