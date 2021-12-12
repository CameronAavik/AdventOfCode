using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day12 : ISolver
{
    readonly record struct Cave(bool IsBig = false, int Id = 0)
    {
        public static readonly Cave Start = new(false, 0);
        public static readonly Cave End = new(false, 1);
    }

    readonly record struct State(int AvailableCaves, bool CanRepeat, int CurrentCave);

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // caveEdges is an 'adjacency matrix' encoded using an array of longs. There is a long for each small cave, and
        // it stores adjacency information to the other small caves. Adjadency information is stored as the total
        // number of ways to get from one small cave to another only using direct connections or through large caves.
        // e.g. "(caveEdges[2] >> (5 * 4)) & 0b1111" returns the number of ways to get from cave 2 to cave 5. Caves 0
        // and 1 are the 'start' and 'end' cave, and remaining entries are the other small caves.
        long[] caveEdges = GetCaveEdgesFromInput(input).ToArray();
        int[] caveAdjacencies = new int[caveEdges.Length];
        for (int i = 0; i < caveEdges.Length; i++)
        {
            var edge = caveEdges[i];
            for (int j = 1; j < caveEdges.Length; j++)
            {
                if (((edge >> (j * 4)) & 0b1111) > 0)
                    caveAdjacencies[i] |= 1 << j;
            }
        }

        var cache = new Dictionary<State, int>();

        int startingAvailableCaves = (1 << caveEdges.Length) - 2; // all caves available, except start
        var part1 = GetPaths(new State(startingAvailableCaves, CanRepeat: false, CurrentCave: 0));
        solution.SubmitPart1(part1);

        var part2 = GetPaths(new State(startingAvailableCaves, CanRepeat: true, CurrentCave: 0));
        solution.SubmitPart2(part2);

        int GetPaths(State state)
        {
            // Check if we have reached the end cave
            if (state.CurrentCave == 1)
                return 1;

            if (!cache.TryGetValue(state, out int numPaths))
            {
                var adjacencies = caveAdjacencies[state.CurrentCave];
                var edgeCounts = caveEdges[state.CurrentCave];
                var availableCaves = state.AvailableCaves & adjacencies;
                int total = 0;
                while (availableCaves != 0)
                {
                    var t = availableCaves & -availableCaves;
                    int caveId = BitOperations.TrailingZeroCount(availableCaves);
                    int edgeCount = (int)((edgeCounts >> (caveId * 4)) & 0b1111);

                    total += edgeCount * GetPaths(new State(state.AvailableCaves ^ t, state.CanRepeat, caveId));
                    availableCaves ^= t;
                }

                // If we can repeat, then try repeat
                if (state.CanRepeat)
                {
                    availableCaves = ~state.AvailableCaves & adjacencies;
                    while (availableCaves != 0)
                    {
                        var t = availableCaves & -availableCaves;
                        int caveId = BitOperations.TrailingZeroCount(availableCaves);
                        int edgeCount = (int)((edgeCounts >> (caveId * 4)) & 0b1111);

                        total += edgeCount * GetPaths(new State(state.AvailableCaves, false, caveId));

                        availableCaves ^= t;
                    }
                }

                numPaths = cache[state] = total;
            }

            return numPaths;
        }
    }

    private static ReadOnlySpan<long> GetCaveEdgesFromInput(ReadOnlySpan<char> input)
    {
        int numSmall = 2; // starts off with 2 for start and end
        int numBig = 0;
        var caveLookup = new Dictionary<string, Cave>();

        long[] caveEdges = new long[16];
        long[] bigToSmallCaveEdges = new long[16];

        int inputCursor = 0;
        while (TryReadLine(input, ref inputCursor, out ReadOnlySpan<char> from, out ReadOnlySpan<char> to))
        {
            Cave fromCave = ParseCave(from);
            Cave toCave = ParseCave(to);

            if (fromCave.IsBig)
            {
                bigToSmallCaveEdges[fromCave.Id] |= 1L << (4 * toCave.Id);
            }
            else if (toCave.IsBig)
            {
                bigToSmallCaveEdges[toCave.Id] |= 1L << (4 * fromCave.Id);
            }
            else
            {
                caveEdges[fromCave.Id] |= 1L << (4 * toCave.Id);
                caveEdges[toCave.Id] |= 1L << (4 * fromCave.Id);
            }
        }

        for (int i = 0; i < numBig; i++)
        {
            long bigToSmall = bigToSmallCaveEdges[i];
            for (int j = 0; j < numSmall; j++)
            {
                long caveFlag = 1L << (4 * j);
                if ((bigToSmall & caveFlag) > 0)
                    caveEdges[j] += bigToSmall;
            }
        }

        return caveEdges.AsSpan().Slice(0, numSmall);

        static bool TryReadLine(ReadOnlySpan<char> input, ref int cursor, out ReadOnlySpan<char> from, out ReadOnlySpan<char> to)
        {
            if (cursor >= input.Length)
            {
                from = null;
                to = null;
                return false;
            }

            int dashIndex = input.Slice(cursor).IndexOf('-');
            from = input.Slice(cursor, dashIndex);

            cursor += dashIndex + 1;

            int newLineIndex = input.Slice(cursor).IndexOf('\n');
            to = input.Slice(cursor, newLineIndex);

            cursor += newLineIndex + 1;
            return true;
        }

        Cave ParseCave(ReadOnlySpan<char> caveName)
        {
            if (caveName.Equals("start", StringComparison.Ordinal))
                return Cave.Start;

            if (caveName.Equals("end", StringComparison.Ordinal))
                return Cave.End;

            string caveNameString = caveName.ToString();
            if (caveLookup.TryGetValue(caveNameString, out Cave cave))
                return cave;

            bool isBig = char.IsUpper(caveName[0]);
            int id = isBig ? numBig++ : numSmall++;
            return caveLookup[caveNameString] = new(isBig, id);
        }
    }
}
