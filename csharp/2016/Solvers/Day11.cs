using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day11 : ISolver
{
    public readonly struct State : IEquatable<State>
    {
        public readonly byte[] MaterialLocations;
        public readonly int CurFloor;

        public State(byte[] materials, int curFloor)
        {
            MaterialLocations = materials;
            CurFloor = curFloor;
        }

        public bool Equals(State other)
        {
            if (CurFloor != other.CurFloor)
            {
                return false;
            }

            for (int i = 0; i < MaterialLocations.Length; i++)
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

    public Solution Solve(ReadOnlySpan<char> input)
    {
        byte[] part1Locs = ParseInput(input).ToArray();
        Array.Sort(part1Locs);
        var part1State = new State(part1Locs, 1);
        int part1 = FindStepsToFinish(part1State);

        byte[] part2Locs = new byte[part1Locs.Length + 2];
        Array.Copy(part1Locs, 0, part2Locs, 2, part1Locs.Length);
        part2Locs[0] = 1 << 4 | 1;
        part2Locs[1] = 1 << 4 | 1;
        var part2State = new State(part2Locs, 1);
        int part2 = FindStepsToFinish(part2State);

        return new Solution(part1, part2);
    }

    private static List<byte> ParseInput(ReadOnlySpan<char> input)
    {
        var materialLocations = new List<byte>();
        var materials = new Dictionary<string, int>();
        var reader = new SpanReader(input);
        for (int floor = 1; floor < 5; floor++)
        {
            reader.SkipLength(floor is 1 or 3 ? "The first floor contains ".Length : "The second floor contains ".Length);
            if (reader.Peek() == 'n') // nothing relevant
            {
                reader.SkipLength("nothing relevant.\n".Length);
                continue;
            }

            bool isLastItem = false;
            while (!isLastItem)
            {
                ParseItem(ref reader, out string element, out bool isGenerator, out isLastItem);
                if (!materials.TryGetValue(element, out int materialIndex))
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
        ReadOnlySpan<char> elementSpan = reader.ReadUntil(' ');
        isGenerator = reader.Peek() == 'g';
        reader.SkipUntil(isLastItem ? '\n' : ' ');

        if (!isGenerator)
        {
            elementSpan = elementSpan.Slice(0, elementSpan.Length - "-compatible".Length);
        }

        element = elementSpan.ToString();
    }

    private static int FindStepsToFinish(State initState)
    {
        int len = initState.MaterialLocations.Length;

        var seen = new HashSet<State>();
        var frontier = new List<State> { initState };
        int steps = 0;

        while (frontier.Count > 0)
        {
            var newFrontier = new List<State>();
            foreach (State state in frontier)
            {
                if (!seen.Add(state))
                {
                    continue;
                }

                if (state.MaterialLocations[0] == (4 << 4 | 4))
                {
                    return steps;
                }

                int curFloor = state.CurFloor;

                for (int newLevel = curFloor - 1; newLevel <= curFloor + 1; newLevel += 2)
                {
                    // only move to valid levels
                    if (newLevel is 0 or 5)
                    {
                        continue;
                    }

                    for (int elem1 = 0; elem1 < len; elem1++)
                    {
                        byte locations1 = state.MaterialLocations[elem1];
                        bool isGen1OnFloor = locations1 >> 4 == curFloor;
                        bool isChip1OnFloor = (locations1 & 0xF) == curFloor;

                        if (!isGen1OnFloor && !isChip1OnFloor)
                        {
                            continue;
                        }

                        // elem1 gen only
                        if (isGen1OnFloor && TryMove(state, newLevel, elem1, null, true, null, out State? newState))
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

                        for (int elem2 = elem1 + 1; elem2 < len; elem2++)
                        {
                            byte locations2 = state.MaterialLocations[elem2];

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
        int numMaterials = curState.MaterialLocations.Length;
        int curFloor = curState.CurFloor;

        bool hasUnmatchedChipCurLevel = false;
        bool hasGeneratorCurLevel = false;
        bool hasUnmatchedChipNewLevel = false;
        bool hasGeneratorNewLevel = false;
        for (int i = 0; i < numMaterials; i++)
        {
            byte locs = curState.MaterialLocations[i];

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
        byte[] newLocs = new byte[numMaterials];
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
