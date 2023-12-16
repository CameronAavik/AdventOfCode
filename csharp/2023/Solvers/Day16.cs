using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

// Explanation of solution:
//   When a beam gets split at a '|' or '-' mirror, it will always visit the same tiles afterwards regardless of what
//   happened before it. For every such mirror I cache a bitset of all the tiles that get visited afterwards. Then
//   when I am iterating over all the possible entry points into the grid I only need to follow until the beam is first
//   split into two beams and I can load the bitset that I cached earlier and 'or' it together with a bitset of the
//   path taken to get there.
public class Day16 : ISolver
{
    public enum Dir { North, South, East, West }

    [SkipLocalsInit]
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int width = input.IndexOf((byte)'\n');
        int rowLength = width + 1;
        int height = input.Length / rowLength;

        // This dictionary is a cache of all the tiles that will be visited after getting split at each '|' and '-' tile
        // This cache is stored using a bitset in a ulong[]
        Dictionary<int, ulong[]> seenBitsAtEachMirror = [];

        // Iterate through all the '|' and '-' mirrors to cache the bitsets
        int splittingMirrorIndex = 0;
        while (true)
        {
            int nextIndex = input.Slice(splittingMirrorIndex).IndexOfAny("-|"u8);
            if (nextIndex < 0)
                break;

            GetOrCacheSeenBitsAtSplittingMirror(input, splittingMirrorIndex + nextIndex, rowLength, seenBitsAtEachMirror, [], out _);
            splittingMirrorIndex += nextIndex + 1;
        }

        // Reusable ulong[] bitset which tracks all the tiles that are visited after entering the grid.
        // This is created here so that we don't need to allocate this array for every single entry point.
        ulong[] seenBits = new ulong[(input.Length - 1) / 64 + 1];

        int part1 = SolveAtEntry(input, 0, 0, Dir.East, rowLength, seenBits, seenBitsAtEachMirror);
        solution.SubmitPart1(part1);

        int part2 = part1;

        // Save having to check if y == 0 inside the for loop by doing the other y == 0 case outside it
        part2 = Math.Max(part2, SolveAtEntry(input, rowLength - 1, 0, Dir.West, rowLength, seenBits, seenBitsAtEachMirror));

        // Loop over all the potential entry points on the left and right sides
        for (int y = 1; y < height; y++)
        {
            int left = SolveAtEntry(input, 0, y, Dir.East, rowLength, seenBits, seenBitsAtEachMirror);
            int right = SolveAtEntry(input, width - 1, y, Dir.West, rowLength, seenBits, seenBitsAtEachMirror);
            part2 = Math.Max(left, Math.Max(right, part2));
        }

        // Loop over all the potential entry points on the top and bottom sides
        for (int x = 0; x < width; x++)
        {
            int top = SolveAtEntry(input, x, 0, Dir.South, rowLength, seenBits, seenBitsAtEachMirror);
            int bottom = SolveAtEntry(input, x, height - 1, Dir.North, rowLength, seenBits, seenBitsAtEachMirror);
            part2 = Math.Max(top, Math.Max(bottom, part2));
        }

        solution.SubmitPart2(part2);
    }

    public static ulong[]? GetOrCacheSeenBitsAtSplittingMirror(
        ReadOnlySpan<byte> input,
        int index,
        int rowLen,
        Dictionary<int, ulong[]> seenBitsAtEachMirror,
        HashSet<int> mirrorPath,
        out Dictionary<int, HashSet<int>>? mirrorsInCycle)
    {
        mirrorsInCycle = null;
        if (seenBitsAtEachMirror.TryGetValue(index, out ulong[]? cachedSeenBits))
            return cachedSeenBits;

        // Detect that a cycle has occurred. If it has, we will pass this information upwards so that when we get back
        // to when this index first appeared that it sets all the mirrors in the cycle to have the same seenBits
        if (mirrorPath.Contains(index))
        {
            mirrorsInCycle = new() { [index] = [] };
            return null;
        }

        ulong[] seenBits = new ulong[(input.Length - 1) / 64 + 1];

        // Find the next mirror split that you run into when going in either of the two directions after the splitting
        // on the current mirror. MoveUntilNextMirrorSplit will return -1 if the beam leaves the grid before hitting
        // another mirror
        int mirrorIndex1;
        int mirrorIndex2;
        if (input[index] == '-')
        {
            mirrorIndex1 = MoveUntilNextMirrorSplit(input, index - 1, Dir.West, seenBits, rowLen);
            mirrorIndex2 = MoveUntilNextMirrorSplit(input, index + 1, Dir.East, seenBits, rowLen);
        }
        else
        {
            mirrorIndex1 = MoveUntilNextMirrorSplit(input, index - rowLen, Dir.North, seenBits, rowLen);
            mirrorIndex2 = MoveUntilNextMirrorSplit(input, index + rowLen, Dir.South, seenBits, rowLen);
        }

        // Add the mirror to the path so it can be checked for cycles later
        mirrorPath.Add(index);

        Dictionary<int, HashSet<int>>? mirrorsInCycle1 = null;
        if (mirrorIndex1 >= 0)
        {
            ulong[]? mirrorSeenBits = GetOrCacheSeenBitsAtSplittingMirror(input, mirrorIndex1, rowLen, seenBitsAtEachMirror, mirrorPath, out mirrorsInCycle1);
            if (mirrorSeenBits != null)
                CombineBitsets(seenBits, mirrorSeenBits);
        }

        Dictionary<int, HashSet<int>>? mirrorsInCycle2 = null;
        if (mirrorIndex2 >= 0)
        {
            ulong[]? mirrorSeenBits = GetOrCacheSeenBitsAtSplittingMirror(input, mirrorIndex2, rowLen, seenBitsAtEachMirror, mirrorPath, out mirrorsInCycle2);
            if (mirrorSeenBits != null)
                CombineBitsets(seenBits, mirrorSeenBits);
        }

        mirrorsInCycle = MergeMirrorCycleDictionaries(mirrorsInCycle1, mirrorsInCycle2);

        if (mirrorsInCycle != null)
        {
            // If the current mirror was the start of a cycle, then remove it from the cycle dictionary and set all the
            // mirrors in the cycle to have the same seenBits as the current mirror as they should all have the same value
            if (mirrorsInCycle.TryGetValue(index, out var cycleIndexes))
            {
                foreach (int cycleIndex in cycleIndexes)
                    seenBitsAtEachMirror[cycleIndex] = seenBits;
                mirrorsInCycle.Remove(index);
            }

            if (mirrorsInCycle.Count == 0)
            {
                mirrorsInCycle = null;
            }
            else
            {
                // Insert the current mirror index into all the current cycles
                foreach (var cycleIndexSet in mirrorsInCycle.Values)
                    cycleIndexSet.Add(index);
            }
        }

        mirrorPath.Remove(index);

        // If we are in a cycle, don't cache the seen bits yet as they will be cached later. There is still value in
        // returning the seenBits to the caller though so that any tiles between this mirror and the previous one are
        // included in the bitset
        if (mirrorsInCycle == null)
            seenBitsAtEachMirror[index] = seenBits;

        return seenBits;

        static Dictionary<int, HashSet<int>>? MergeMirrorCycleDictionaries(Dictionary<int, HashSet<int>>? mirrorsInCycle1, Dictionary<int, HashSet<int>>? mirrorsInCycle2)
        {
            if (mirrorsInCycle1 == null)
            {
                return mirrorsInCycle2;
            }
            else if (mirrorsInCycle2 == null)
            {
                return mirrorsInCycle1;
            }
            else
            {
                foreach (var cycle in mirrorsInCycle2)
                {
                    if (mirrorsInCycle1.TryGetValue(cycle.Key, out var cycleIndexes))
                    {
                        foreach (var cycleIndex in cycle.Value)
                            cycleIndexes.Add(cycleIndex);
                    }
                    else
                    {
                        mirrorsInCycle1[cycle.Key] = cycle.Value;
                    }
                }
            }

            return mirrorsInCycle1;
        }
    }

    private static void CombineBitsets(ulong[] bitset1, ulong[] bitset2)
    {
        ref ulong bitset1Ref = ref MemoryMarshal.GetArrayDataReference(bitset1);
        ref ulong bitset2Ref = ref MemoryMarshal.GetArrayDataReference(bitset2);
        for (nuint i = 0; i + (nuint)Vector256<ulong>.Count < (nuint)bitset1.Length; i += (nuint)Vector256<ulong>.Count)
        {
            var v1 = Vector256.LoadUnsafe(ref bitset1Ref, i);
            var v2 = Vector256.LoadUnsafe(ref bitset2Ref, i);
            Vector256.StoreUnsafe(v1 | v2, ref bitset1Ref, i);
        }

        int remainderLength = bitset1.Length % Vector256<ulong>.Count;
        for (int i = bitset1.Length - remainderLength; i < bitset1.Length; i++)
            bitset1[i] |= bitset2[i];
    }

    public static int SolveAtEntry(ReadOnlySpan<byte> input, int initX, int initY, Dir initDir, int rowLen, ulong[] seenBits, Dictionary<int, ulong[]> seenBitsAtEachMirror)
    {
        Array.Clear(seenBits);
        int mirrorIndex = MoveUntilNextMirrorSplit(input, initY * rowLen + initX, initDir, seenBits, rowLen);
        if (mirrorIndex >= 0)
            CombineBitsets(seenBits, seenBitsAtEachMirror[mirrorIndex]);

        int seen = 0;
        foreach (ulong seenBitSet in seenBits)
            seen += BitOperations.PopCount(seenBitSet);
        return seen;
    }

    // This method simply follows the tiles until we hit a '|' or '-' mirror that causes the beam to split into two
    // It also keeps track of all the tiles seen along the way in the seenBits array. If the beam leaves the grid
    // before splitting on a mirror, this method returns -1.
    private static int MoveUntilNextMirrorSplit(ReadOnlySpan<byte> input, int i, Dir dir, ulong[] seenBits, int rowLen)
    {
        ref byte inputRef = ref MemoryMarshal.GetReference(input);

        byte c = 0;
        while (true)
        {
            switch (dir)
            {
                case Dir.East:
                    while ((c = Unsafe.Add(ref inputRef, (nuint)i)) is (byte)'.' or (byte)'-')
                    {
                        seenBits[i / 64] |= 1UL << i;
                        i++;
                    }

                    if (c == '\n')
                        return -1;

                    seenBits[i / 64] |= 1UL << i;
                    switch (c)
                    {
                        case (byte)'\\':
                            dir = Dir.South;
                            i += rowLen;
                            break;
                        case (byte)'/':
                            dir = Dir.North;
                            i -= rowLen;
                            break;
                        case (byte)'|':
                            return i;
                    }
                    break;
                case Dir.West:
                    while (i >= 0 && (c = Unsafe.Add(ref inputRef, (nuint)i)) is (byte)'.' or (byte)'-')
                    {
                        seenBits[i / 64] |= 1UL << i;
                        i--;
                    }

                    if (i < 0 || c == '\n')
                        return -1;

                    seenBits[i / 64] |= 1UL << i;
                    switch (c)
                    {
                        case (byte)'\\':
                            dir = Dir.North;
                            i -= rowLen;
                            break;
                        case (byte)'/':
                            dir = Dir.South;
                            i += rowLen;
                            break;
                        case (byte)'|':
                            return i;
                    }
                    break;
                case Dir.North:
                    while (i >= 0 && (c = Unsafe.Add(ref inputRef, (nuint)i)) is (byte)'.' or (byte)'|')
                    {
                        seenBits[i / 64] |= 1UL << i;
                        i -= rowLen;
                    }

                    if (i < 0)
                        return -1;

                    seenBits[i / 64] |= 1UL << i;
                    switch (c)
                    {
                        case (byte)'\\':
                            dir = Dir.West;
                            i--;
                            break;
                        case (byte)'/':
                            dir = Dir.East;
                            i++;
                            break;
                        case (byte)'-':
                            return i;
                    }
                    break;
                case Dir.South:
                    while (i < input.Length && (c = Unsafe.Add(ref inputRef, (nuint)i)) is (byte)'.' or (byte)'|')
                    {
                        seenBits[i / 64] |= 1UL << i;
                        i += rowLen;
                    }

                    if (i >= input.Length)
                        return -1;

                    seenBits[i / 64] |= 1UL << i;
                    switch (c)
                    {
                        case (byte)'\\':
                            dir = Dir.East;
                            i++;
                            break;
                        case (byte)'/':
                            dir = Dir.West;
                            i--;
                            break;
                        case (byte)'-':
                            return i;
                    }
                    break;
            }
        }
    }
}
