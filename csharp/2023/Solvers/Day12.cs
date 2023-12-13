using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day12 : ISolver
{
    // TODO: Investigate a faster algorithm, one that could theoretically scale to 1 million copies
    // In the meantime I have this which runs in 2ms but I think I can do a lot better.

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // Enough to fit a list of 24 springs per line
        Span<byte> springs = stackalloc byte[128];

        // Enough to fit 9 contiguous groups per line
        Span<int> contiguousGroups = stackalloc int[48];

        long part1 = 0;
        long part2 = 0;

        long[][] waysCache = new long[128][];
        for (int i = 0; i < waysCache.Length; i++)
            waysCache[i] = new long[48];

        ulong[] nonZeroCounts = new ulong[128];

        while (input.Length > 0)
        {
            int lineSpringsEnd = input.IndexOf((byte)' ');
            ReadOnlySpan<byte> lineSprings = input.Slice(0, lineSpringsEnd);
            input = input.Slice(lineSpringsEnd + 1);

            int numGroups = 0;
            int num = input[0] - '0';
            int index = 1;
            while (true)
            {
                byte c = input[index++];
                if (c >= '0')
                {
                    num = 10 * num + c - '0';
                }
                else
                {
                    contiguousGroups[numGroups++] = num;
                    if (c == '\n')
                        break;

                    num = input[index++] - '0';
                }
            }

            input = input.Slice(index);

            part1 += CountWays(lineSprings, contiguousGroups.Slice(0, numGroups), waysCache, nonZeroCounts);

            int newSpringLen = 5 * lineSprings.Length + 4;
            int newNumGroups = 5 * numGroups;
            for (int i = 0; i < newSpringLen; i += lineSprings.Length + 1)
            {
                lineSprings.CopyTo(springs.Slice(i));
                if (i + lineSprings.Length != newSpringLen)
                    springs[i + lineSprings.Length] = (byte)'?';
            }

            for (int i = numGroups; i < newNumGroups; i += numGroups)
                contiguousGroups.Slice(0, numGroups).CopyTo(contiguousGroups.Slice(i, numGroups));

            part2 += CountWays(springs.Slice(0, newSpringLen), contiguousGroups.Slice(0, newNumGroups), waysCache, nonZeroCounts);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static long CountWays(ReadOnlySpan<byte> springs, ReadOnlySpan<int> contiguousGroups, long[][] waysCache, ulong[] nonZeroCounts)
    {
        for (int i = 0; i <= springs.Length; i++)
            waysCache[i].AsSpan().Slice(0, contiguousGroups.Length + 1).Clear();
        nonZeroCounts.AsSpan().Slice(0, springs.Length + 1).Clear();
        waysCache[springs.Length][contiguousGroups.Length] = 1;
        nonZeroCounts[springs.Length] |= 1UL << contiguousGroups.Length;

        byte maxGroupIndex = 0;
        byte[] maxGroupsPerSpring = new byte[springs.Length + 1];

        for (int i = 1; i <= springs.Length; i++)
        {
            byte c = springs[springs.Length - i];

            if (maxGroupIndex < contiguousGroups.Length && (c is (byte)'#' or (byte)'?'))
            {
                bool matchesNum = true;
                int groupSize = contiguousGroups[contiguousGroups.Length - maxGroupIndex - 1] - 1;
                for (int j = 0; j < groupSize; j++)
                {
                    maxGroupsPerSpring[i++] = maxGroupIndex;
                    c = springs[springs.Length - i];
                    if (c == '.')
                    {
                        matchesNum = false;
                        break;
                    }
                }

                if (matchesNum)
                {
                    // can't start when adjacent to broken
                    while (i < springs.Length && springs[springs.Length - i - 1] == '#')
                        maxGroupsPerSpring[i++] = maxGroupIndex;

                    maxGroupIndex++;
                    if (i < springs.Length)
                        maxGroupsPerSpring[i++] = maxGroupIndex;
                }
            }

            if (i <= springs.Length)
                maxGroupsPerSpring[i] = maxGroupIndex;
        }

        int endingSprings = springs.Length - springs.LastIndexOf((byte)'#') - 1;
        long total = 0;
        for (int i = springs.Length; i >= 0; i--)
        {
            long[] groupCache = waysCache[i];
            ulong nonZeroCount = nonZeroCounts[i] & ~1UL & ((1UL << (maxGroupsPerSpring[i] + 1)) - 1);
            while (nonZeroCount != 0)
            {
                ulong t = nonZeroCount & (~nonZeroCount + 1);
                int j = BitOperations.TrailingZeroCount(t);

                byte currentSpring = springs[springs.Length - i];
                if (currentSpring != '#')
                {
                    waysCache[i - 1][j] += groupCache[j];
                    nonZeroCounts[i - 1] |= 1UL << j;
                }

                if (currentSpring != '.')
                {
                    int nextGroupSize = contiguousGroups[contiguousGroups.Length - j];
                    if (!springs.Slice(springs.Length - i + 1, nextGroupSize - 1).Contains((byte)'.'))
                    {
                        if (j == 1)
                        {
                            waysCache[i - nextGroupSize][j - 1] += groupCache[j];
                            nonZeroCounts[i - nextGroupSize] |= 1UL << (j - 1);
                        }
                        else if (springs[springs.Length - i + nextGroupSize] != '#')
                        {
                            waysCache[i - nextGroupSize - 1][j - 1] += groupCache[j];
                            nonZeroCounts[i - nextGroupSize - 1] |= 1UL << (j - 1);
                        }
                    }
                }

                nonZeroCount ^= t;
            }

            if (i <= endingSprings)
                total += groupCache[0];
        }

        return total;
    }
}
