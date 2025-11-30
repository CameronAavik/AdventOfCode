using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day12 : ISolver
{
    readonly record struct Cave(bool IsBig = false, int Id = 0)
    {
        public static readonly Cave Start = new(false, 0);
        public static readonly Cave End = new(false, 1);
    }

    readonly record struct State(int AvailableCaves, bool CanRepeat, int CurrentCave);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // caveEdges is an 'adjacency matrix' encoded using an array of longs. There is a long for each small cave, and
        // it stores adjacency information to the other small caves. Adjadency information is stored as the total
        // number of ways to get from one small cave to another only using direct connections or through large caves.
        // e.g. "(caveEdges[2] >> (5 * 4)) & 0b1111" returns the number of ways to get from cave 2 to cave 5. Caves 0
        // and 1 are the 'start' and 'end' cave, and remaining entries are the other small caves.
        var caveEdges = GetCaveEdgesFromInput(input).ToArray();
        var caveAdjacencies = new int[caveEdges.Length];
        for (var i = 0; i < caveEdges.Length; i++)
        {
            var edge = caveEdges[i];
            for (var j = 1; j < caveEdges.Length; j++)
            {
                if (((edge >> (j * 4)) & 0b1111) > 0)
                    caveAdjacencies[i] |= 1 << j;
            }
        }

        var cache = new Dictionary<State, int>();

        var startingAvailableCaves = (1 << caveEdges.Length) - 2; // all caves available, except start
        var part1 = GetPaths(new State(startingAvailableCaves, CanRepeat: false, CurrentCave: 0));
        solution.SubmitPart1(part1);

        var part2 = GetPaths(new State(startingAvailableCaves, CanRepeat: true, CurrentCave: 0));
        solution.SubmitPart2(part2);

        int GetPaths(State state)
        {
            // Check if we have reached the end cave
            if (state.CurrentCave == 1)
                return 1;

            if (!cache.TryGetValue(state, out var numPaths))
            {
                var adjacencies = caveAdjacencies[state.CurrentCave];
                var edgeCounts = caveEdges[state.CurrentCave];
                var availableCaves = state.AvailableCaves & adjacencies;
                var total = 0;
                while (availableCaves != 0)
                {
                    var t = availableCaves & -availableCaves;
                    var caveId = BitOperations.TrailingZeroCount(availableCaves);
                    var edgeCount = (int)((edgeCounts >> (caveId * 4)) & 0b1111);

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
                        var caveId = BitOperations.TrailingZeroCount(availableCaves);
                        var edgeCount = (int)((edgeCounts >> (caveId * 4)) & 0b1111);

                        total += edgeCount * GetPaths(new State(state.AvailableCaves, false, caveId));

                        availableCaves ^= t;
                    }
                }

                numPaths = cache[state] = total;
            }

            return numPaths;
        }
    }

    private static ReadOnlySpan<long> GetCaveEdgesFromInput(ReadOnlySpan<byte> input)
    {
        var numSmall = 2; // starts off with 2 for start and end
        var numBig = 0;
        var caveLookup = new Dictionary<string, Cave>();

        var caveEdges = new long[16];
        var bigToSmallCaveEdges = new long[16];

        var inputCursor = 0;
        while (TryReadLine(input, ref inputCursor, out var from, out var to))
        {
            var fromCave = ParseCave(from);
            var toCave = ParseCave(to);

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

        for (var i = 0; i < numBig; i++)
        {
            var bigToSmall = bigToSmallCaveEdges[i];
            for (var j = 0; j < numSmall; j++)
            {
                var caveFlag = 1L << (4 * j);
                if ((bigToSmall & caveFlag) > 0)
                    caveEdges[j] += bigToSmall;
            }
        }

        return caveEdges.AsSpan()[..numSmall];

        static bool TryReadLine(ReadOnlySpan<byte> input, ref int cursor, out ReadOnlySpan<byte> from, out ReadOnlySpan<byte> to)
        {
            if (cursor >= input.Length)
            {
                from = null;
                to = null;
                return false;
            }

            var dashIndex = input[cursor..].IndexOf((byte)'-');
            from = input.Slice(cursor, dashIndex);

            cursor += dashIndex + 1;

            var newLineIndex = input[cursor..].IndexOf((byte)'\n');
            to = input.Slice(cursor, newLineIndex);

            cursor += newLineIndex + 1;
            return true;
        }

        Cave ParseCave(ReadOnlySpan<byte> caveName)
        {
            if (caveName.SequenceEqual("start"u8))
                return Cave.Start;

            if (caveName.SequenceEqual("end"u8))
                return Cave.End;

            var caveNameString = Encoding.ASCII.GetString(caveName);
            if (caveLookup.TryGetValue(caveNameString, out var cave))
                return cave;

            var isBig = caveName[0] is >= (byte)'A' and <= (byte)'Z';
            var id = isBig ? numBig++ : numSmall++;
            return caveLookup[caveNameString] = new(isBig, id);
        }
    }
}
