using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day05 : ISolver
{
    public record struct Mapping(long FromStart, long FromEnd, long ToStart) : IComparable<Mapping>
    {
        public readonly int CompareTo(Mapping other) => FromStart.CompareTo(other.FromStart);
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.Slice("seeds: ".Length);

        int seedsEndIndex = input.IndexOf((byte)'\n');
        ReadOnlySpan<byte> seedsLine = input.Slice(0, seedsEndIndex + 1);
        int numSeeds = seedsLine.Count((byte)' ') + 1;

        long[] seeds = new long[numSeeds];
        for (int i = 0; i < numSeeds; i++)
            seeds[i] = ReadLongUntil(ref input, (byte)(i == numSeeds - 1 ? '\n' : ' '));

        var part2Ranges = new (long X, long Y)[numSeeds / 2];
        for (int i = 0; i < numSeeds; i += 2)
            part2Ranges[i / 2] = (seeds[i], seeds[i] + seeds[i + 1]);

        // Will reuse this list in each iteration
        var mappings = new List<Mapping>(64);

        // Keeps track of the mappings in the opposite direction
        // The mappings will be sorted and will not have any gaps
        var backwardsMappings = new List<List<Mapping>>(8);

        while (input.Length > 0)
        {
            // skip starting newline separator
            input = input.Slice(1); 

            // skip mapping name
            input = input.Slice(input.IndexOf((byte)'\n') + 1);

            mappings.Clear();
            while (input.Length > 0 && input[0] != '\n')
            {
                long dst = ReadLongUntil(ref input, (byte)' ');
                long src = ReadLongUntil(ref input, (byte)' ');
                long len = ReadLongUntil(ref input, (byte)'\n');
                mappings.Add(new Mapping(src, src + len, dst));
            }

            mappings.Sort();

            for (int i = 0; i < numSeeds; i++)
            {
                long seed = seeds[i];
                Mapping mapping = mappings[BinarySearch(mappings, seed)];
                if (mapping.FromStart <= seed && seed < mapping.FromEnd)
                    seeds[i] = mapping.ToStart + seed - mapping.FromStart;
            }

            var backwardsMapping = new List<Mapping>(mappings.Count);
            foreach (Mapping mapping in mappings)
                backwardsMapping.Add(new Mapping(mapping.ToStart, mapping.ToStart + mapping.FromEnd - mapping.FromStart, mapping.FromStart));

            backwardsMapping.Sort();
            backwardsMappings.Add(backwardsMapping);
        }

        long part1 = long.MaxValue;
        foreach (long seed in seeds)
            part1 = Math.Min(part1, seed);
        solution.SubmitPart1(part1);

        long part2 = SolvePart2();
        solution.SubmitPart2(part2);

        // Recursively iterate through all location ranges to the seed level and check which range overlaps with any of the seed ranges first
        long SolvePart2(long start = 0L, long end = long.MaxValue, int mappingIndex = 0, long startLocation = 0L)
        {
            if (mappingIndex == backwardsMappings.Count)
            {
                foreach ((long x, long y) in part2Ranges)
                {
                    if (x <= end && start <= y)
                        return startLocation + Math.Max(x, start) - start;
                }

                return -1;
            }

            List<Mapping> mappings = backwardsMappings[backwardsMappings.Count - mappingIndex - 1];
            int rangeIndex = BinarySearch(mappings, start);

            for (int i = rangeIndex; i < mappings.Count; i++)
            {
                (long xDst, long yDst, long x) = mappings[i];

                // this means that there is a gap in the mappings, continue to next mapping
                if (yDst < start)
                    continue;

                if (start < xDst)
                {
                    if (end < xDst)
                        return SolvePart2(start, end, mappingIndex + 1, startLocation);

                    long beforeSol = SolvePart2(start, xDst, mappingIndex + 1, startLocation);
                    if (beforeSol >= 0)
                        return beforeSol;

                    startLocation += xDst - start;
                    start = xDst;
                }

                long xDstMapped = start - xDst + x;

                if (end <= yDst)
                    return SolvePart2(xDstMapped, end - xDst + x, mappingIndex + 1, startLocation);

                long sol = SolvePart2(xDstMapped, yDst - xDst + x, mappingIndex + 1, startLocation);
                if (sol >= 0)
                    return sol;

                startLocation += yDst - start;
                start = yDst;
            }

            return -1;
        }
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

    // Find largest index of mapping where FromStart is less than or equal to the given value
    private static int BinarySearch(List<Mapping> mapping, long value)
    {
        int lo = 0;
        int hi = mapping.Count - 1;
        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);

            long x = mapping[i].FromStart;

            if (x == value)
                return i;

            if (x > value)
                hi = i - 1;
            else
                lo = i + 1;
        }

        // In the case that the value is less than the smallest mapping, just return 0
        return Math.Max(0, hi);
    }
}
