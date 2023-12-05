using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day05 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.Slice("seeds: ".Length);

        int seedsEndIndex = input.IndexOf((byte)'\n');
        ReadOnlySpan<byte> seedsLine = input.Slice(0, seedsEndIndex + 1);
        int numSeeds = seedsLine.Count((byte)' ') + 1;

        long[] seeds = new long[numSeeds];
        for (int i = 0; i < numSeeds; i++)
            seeds[i] = ReadLongUntil(ref input, (byte)(i == numSeeds - 1 ? '\n' : ' '));

        var ranges = new List<(long X, long Y)>(numSeeds / 2);
        for (int i = 0; i < numSeeds; i += 2)
            ranges.Add((seeds[i], seeds[i] + seeds[i + 1]));

        while (input.Length > 0)
        {
            // skip starting newline separator
            input = input.Slice(1); 

            // skip mapping name
            input = input.Slice(input.IndexOf((byte)'\n') + 1);

            var mappings = new List<(long X, long Y, long XDst)>(32);
            while (input.Length > 0 && input[0] != '\n')
            {
                long dst = ReadLongUntil(ref input, (byte)' ');
                long src = ReadLongUntil(ref input, (byte)' ');
                long len = ReadLongUntil(ref input, (byte)'\n');
                mappings.Add((src, src + len, dst));
            }

            mappings.Sort((l, r) => l.X.CompareTo(r.X));

            var newRanges = new List<(long X, long Y)>(ranges.Count * 2); // assume each range might get divided into 4 new ranges

            for (int i = 0; i < numSeeds; i++)
            {
                long seed = seeds[i];
                int rangeIndex = BinarySearch(mappings, seed);
                (long x1, long y1, long xDst) = mappings[rangeIndex];

                if (x1 <= seed && seed < y1)
                {
                    seeds[i] = xDst + seed - x1;
                }
            }

            foreach (var range in ranges)
            {
                (long x0, long y0) = range;

                int rangeIndex = BinarySearch(mappings, x0);

                bool addEnding = true;
                for (int i = rangeIndex; i < mappings.Count; i++)
                {
                    (long x1, long y1, long xDst) = mappings[i];

                    // skip to next mapping if range is ahead of mapping
                    if (y1 <= x0)
                        continue;

                    // if range is before mapping, then we can abort now
                    if (y0 <= x1)
                    {
                        newRanges.Add(range);
                        addEnding = false;
                        break;
                    }

                    // at this point we know that the ranges overlap in some way

                    // Map through any part of the range that exists before the mapping
                    if (x0 < x1)
                    {
                        newRanges.Add((x0, x1));
                        x0 = x1;
                    }

                    long startOffset = xDst + x0 - x1;
                    if (y0 <= y1)
                    {
                        newRanges.Add((startOffset, xDst + y0 - x1));
                        addEnding = false;
                        break;
                    }

                    newRanges.Add((startOffset, xDst + y1 - x1));

                    x0 = y1;
                }

                if (addEnding && x0 != y0)
                    newRanges.Add((x0, y0));
            }

            ranges = newRanges;
        }

        long part1 = long.MaxValue;
        foreach (long seed in seeds)
            part1 = Math.Min(part1, seed);

        long part2 = long.MaxValue;
        foreach ((long x, _) in ranges)
            part2 = Math.Min(part2, x);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static long ReadLongUntil(ref ReadOnlySpan<byte> input, byte c)
    {
        byte cur;
        long ret = input[0] - '0';
        int i = 1;
        while ((cur = input[i++]) != c)
            ret = ret * 10 + cur - '0';

        input = input.Slice(i);
        return ret;
    }

    private static int BinarySearch(List<(long X, long Y, long XDst)> mapping, long value)
    {
        int lo = 0;
        int hi = mapping.Count - 1;
        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);

            long x = mapping[i].X;

            if (x == value)
                return i;

            if (x > value)
                hi = i - 1;
            else
                lo = i + 1;
        }

        return Math.Max(0, hi);
    }
}
