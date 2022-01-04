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

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int freqIndex = 0;
        int freqTotal = 0;
        var freqs = new List<Frequency>();
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            freqs.Add(new Frequency { Value = freqTotal, Index = freqIndex });
            freqIndex++;

            int mul = reader.Peek() == '-' ? -1 : 1;
            reader.SkipLength(1);

            freqTotal += mul * reader.ReadPosIntUntil('\n');
        }

        foreach (Frequency freq in freqs)
        {
            int mod = freq.Value % freqTotal;
            freq.ModTotal = mod < 0 ? mod + freqTotal : mod;
        }

        // sort by mods first, then by value
        freqs.Sort((a, b) => a.ModTotal != b.ModTotal
            ? a.ModTotal.CompareTo(b.ModTotal)
            : a.Value.CompareTo(b.Value));

        var prev = new Frequency { ModTotal = -1 };
        int minDiff = int.MaxValue;
        int minIndex = int.MaxValue;
        int minFreq = 0;
        foreach (Frequency freq in freqs)
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

        solution.SubmitPart1(freqTotal);
        solution.SubmitPart2(minFreq);
    }
}
