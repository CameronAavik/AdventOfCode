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

        var waysCache = new long[128][];
        for (var i = 0; i < waysCache.Length; i++)
            waysCache[i] = new long[48];

        var nonZeroCounts = new ulong[128];

        while (input.Length > 0)
        {
            var lineSpringsEnd = input.IndexOf((byte)' ');
            var lineSprings = input[..lineSpringsEnd];
            input = input[(lineSpringsEnd + 1)..];

            var numGroups = 0;
            var num = input[0] - '0';
            var index = 1;
            while (true)
            {
                var c = input[index++];
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

            input = input[index..];

            var newSpringLen = 5 * lineSprings.Length + 4;
            var newNumGroups = 5 * numGroups;
            for (var i = 0; i < newSpringLen; i += lineSprings.Length + 1)
            {
                lineSprings.CopyTo(springs[i..]);
                if (i + lineSprings.Length != newSpringLen)
                    springs[i + lineSprings.Length] = (byte)'?';
            }

            for (var i = numGroups; i < newNumGroups; i += numGroups)
                contiguousGroups[..numGroups].CopyTo(contiguousGroups.Slice(i, numGroups));

            CountWays(springs[..newSpringLen], contiguousGroups[..newNumGroups], waysCache, nonZeroCounts, ref part1, ref part2);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void CountWays(ReadOnlySpan<byte> springs, ReadOnlySpan<int> contiguousGroups, long[][] waysCache, ulong[] nonZeroCounts, ref long part1, ref long part2)
    {
        for (var i = 0; i <= springs.Length; i++)
            waysCache[i].AsSpan()[..(contiguousGroups.Length + 1)].Clear();
        nonZeroCounts.AsSpan()[..(springs.Length + 1)].Clear();
        waysCache[springs.Length][contiguousGroups.Length] = 1;
        nonZeroCounts[springs.Length] |= 1UL << contiguousGroups.Length;

        byte maxGroupIndex = 0;
        var maxGroupsPerSpring = new byte[springs.Length + 1];

        for (var i = 1; i <= springs.Length; i++)
        {
            var c = springs[^i];

            if (maxGroupIndex < contiguousGroups.Length && (c is (byte)'#' or (byte)'?'))
            {
                var matchesNum = true;
                var groupSize = contiguousGroups[contiguousGroups.Length - maxGroupIndex - 1] - 1;
                for (var j = 0; j < groupSize; j++)
                {
                    maxGroupsPerSpring[i++] = maxGroupIndex;
                    c = springs[^i];
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

        var endingSprings = springs.Length - springs.LastIndexOf((byte)'#') - 1;
        for (var i = springs.Length; i >= 0; i--)
        {
            var groupCache = waysCache[i];
            var nonZeroCount = nonZeroCounts[i] & ~1UL & ((1UL << (maxGroupsPerSpring[i] + 1)) - 1);
            while (nonZeroCount != 0)
            {
                var t = nonZeroCount & (~nonZeroCount + 1);
                var j = BitOperations.TrailingZeroCount(t);

                var currentSpring = springs[^i];
                if (currentSpring != '#')
                {
                    waysCache[i - 1][j] += groupCache[j];
                    nonZeroCounts[i - 1] |= 1UL << j;
                }

                if (currentSpring != '.')
                {
                    var nextGroupSize = contiguousGroups[^j];
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
        }

        part1 += waysCache[4 * (springs.Length / 5) + 3][4 * (contiguousGroups.Length / 5)];
        for (var i = endingSprings; i >= 0; i--)
            part2 += waysCache[i][0];
    }
}
