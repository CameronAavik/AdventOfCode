using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.CSharp.Common;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day11 : ISolver
{
    public readonly struct State(byte[] materials, int curFloor) : IEquatable<State>
    {
        public readonly byte[] MaterialLocations = materials;
        public readonly int CurFloor = curFloor;

        public bool Equals(State other)
        {
            if (CurFloor != other.CurFloor)
            {
                return false;
            }

            for (var i = 0; i < MaterialLocations.Length; i++)
            {
                if (!MaterialLocations[i].Equals(other.MaterialLocations[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() => HashCode.Combine(MaterialLocations.GetDjb2HashCode(), CurFloor);

        public override bool Equals(object? obj) => obj is State state && Equals(state);

        public static bool operator ==(State left, State right) => left.Equals(right);

        public static bool operator !=(State left, State right) => !(left == right);
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        byte[] part1Locs = [.. ParseInput(input)];
        Array.Sort(part1Locs);
        var part1State = new State(part1Locs, 1);
        var part1 = FindStepsToFinish(part1State);

        var part2Locs = new byte[part1Locs.Length + 2];
        Array.Copy(part1Locs, 0, part2Locs, 2, part1Locs.Length);
        part2Locs[0] = 1 << 4 | 1;
        part2Locs[1] = 1 << 4 | 1;
        var part2State = new State(part2Locs, 1);
        var part2 = FindStepsToFinish(part2State);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static List<byte> ParseInput(ReadOnlySpan<byte> input)
    {
        var materialLocations = new List<byte>();
        var materials = new Dictionary<string, int>();
        var reader = new SpanReader(input);
        for (var floor = 1; floor < 5; floor++)
        {
            reader.SkipLength(floor is 1 or 3 ? "The first floor contains ".Length : "The second floor contains ".Length);
            if (reader.Peek() == 'n') // nothing relevant
            {
                reader.SkipLength("nothing relevant.\n".Length);
                continue;
            }

            var isLastItem = false;
            while (!isLastItem)
            {
                ParseItem(ref reader, out var element, out var isGenerator, out isLastItem);
                if (!materials.TryGetValue(element, out var materialIndex))
                {
                    materialIndex = materialLocations.Count;
                    materials[element] = materialIndex;
                    materialLocations.Add(0);
                }

                materialLocations[materialIndex] |= (byte)(isGenerator ? floor << 4 : floor);
            }
        }

        return materialLocations;
    }

    private static void ParseItem(ref SpanReader reader, out string element, out bool isGenerator, out bool isLastItem)
    {
        isLastItem = reader[1] == 'n'; // "and a <something>"
        reader.SkipLength(isLastItem ? "and a ".Length : "a ".Length);
        var elementSpan = reader.ReadUntil(' ');
        isGenerator = reader.Peek() == 'g';
        reader.SkipUntil(isLastItem ? '\n' : ' ');

        if (!isGenerator)
        {
            elementSpan = elementSpan[..^"-compatible".Length];
        }

        element = Encoding.ASCII.GetString(elementSpan);
    }

    private static int FindStepsToFinish(State initState)
    {
        var len = initState.MaterialLocations.Length;

        var seen = new HashSet<State>();
        var frontier = new List<State> { initState };
        var steps = 0;

        while (frontier.Count > 0)
        {
            var newFrontier = new List<State>();
            foreach (var state in frontier)
            {
                if (!seen.Add(state))
                {
                    continue;
                }

                if (state.MaterialLocations[0] == (4 << 4 | 4))
                {
                    return steps;
                }

                var curFloor = state.CurFloor;

                for (var newLevel = curFloor - 1; newLevel <= curFloor + 1; newLevel += 2)
                {
                    // only move to valid levels
                    if (newLevel is 0 or 5)
                    {
                        continue;
                    }

                    for (var elem1 = 0; elem1 < len; elem1++)
                    {
                        var locations1 = state.MaterialLocations[elem1];
                        var isGen1OnFloor = locations1 >> 4 == curFloor;
                        var isChip1OnFloor = (locations1 & 0xF) == curFloor;

                        if (!isGen1OnFloor && !isChip1OnFloor)
                        {
                            continue;
                        }

                        // elem1 gen only
                        if (isGen1OnFloor && TryMove(state, newLevel, elem1, null, true, null, out var newState))
                        {
                            newFrontier.Add(newState.Value);
                        }

                        // elem1 chip only
                        if (isChip1OnFloor && TryMove(state, newLevel, elem1, null, false, null, out newState))
                        {
                            newFrontier.Add(newState.Value);
                        }

                        // elem1 chip and elem1 gen
                        if (isChip1OnFloor && isGen1OnFloor && TryMove(state, newLevel, elem1, elem1, true, false, out newState))
                        {
                            newFrontier.Add(newState.Value);
                        }

                        for (var elem2 = elem1 + 1; elem2 < len; elem2++)
                        {
                            var locations2 = state.MaterialLocations[elem2];

                            // elem1 chip and elem2 chip
                            if (isChip1OnFloor && (locations2 & 0xF) == curFloor && TryMove(state, newLevel, elem1, elem2, false, false, out newState))
                            {
                                newFrontier.Add(newState.Value);
                            }

                            // elem1 gen and elem2 gen
                            if (isGen1OnFloor && locations2 >> 4 == curFloor && TryMove(state, newLevel, elem1, elem2, true, true, out newState))
                            {
                                newFrontier.Add(newState.Value);
                            }
                        }
                    }
                }
            }

            frontier = newFrontier;
            steps++;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryMove(
        State curState,
        int newLevel,
        int elem1,
        int? elem2,
        bool isGen1,
        bool? isGen2,
        [NotNullWhen(returnValue: true)] out State? newState)
    {
        var numMaterials = curState.MaterialLocations.Length;
        var curFloor = curState.CurFloor;

        var hasUnmatchedChipCurLevel = false;
        var hasGeneratorCurLevel = false;
        var hasUnmatchedChipNewLevel = false;
        var hasGeneratorNewLevel = false;
        for (var i = 0; i < numMaterials; i++)
        {
            var locs = curState.MaterialLocations[i];

            // validate that the current level is valid after the move
            if (locs >> 4 == curFloor && (i != elem1 || !isGen1) && (i != elem2 || isGen2 != true))
            {
                hasGeneratorCurLevel = true;
            }
            else if ((locs & 0xF) == curFloor && (i != elem1 || isGen1) && (i != elem2 || isGen2 != false))
            {
                hasUnmatchedChipCurLevel = true;
            }

            // validate that the new level is valid after the move
            if (locs >> 4 == newLevel || (i == elem1 && isGen1) || (i == elem2 && isGen2 == true))
            {
                hasGeneratorNewLevel = true;
            }
            else if ((locs & 0xF) == newLevel || (i == elem1 && !isGen1) || (i == elem2 && isGen2 == false))
            {
                hasUnmatchedChipNewLevel = true;
            }
        }

        if ((hasUnmatchedChipCurLevel && hasGeneratorCurLevel) || (hasUnmatchedChipNewLevel && hasGeneratorNewLevel))
        {
            newState = null;
            return false;
        }

        // now we know this is a valid move
        var newLocs = new byte[numMaterials];
        Array.Copy(curState.MaterialLocations, newLocs, numMaterials);
        newLocs[elem1] = isGen1
            ? (byte)(newLevel << 4 | (curState.MaterialLocations[elem1] & 0xF))
            : (byte)((curState.MaterialLocations[elem1] & 0xF0) | newLevel);

        if (elem2 is int elem2Val)
        {
            newLocs[elem2Val] = isGen2!.Value
                ? (byte)(newLevel << 4 | (newLocs[elem2Val] & 0xF))
                : (byte)((newLocs[elem2Val] & 0xF0) | newLevel);
        }

        Array.Sort(newLocs);

        newState = new State(newLocs, newLevel);
        return true;
    }
}
