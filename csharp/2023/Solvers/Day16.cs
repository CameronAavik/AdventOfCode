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
    public record struct LineSegment(int Start, int End, int Step) : IEquatable<LineSegment>;
    public class SplittingMirror(int InputIndex, int NextMirror1, int NextMirror2, LineSegment[] LineSegments, int NumLineSegments)
    {
        public int InputIndex { get; } = InputIndex;
        public int NextMirrorIndex1 { get; } = NextMirror1;
        public int NextMirrorIndex2 { get; } = NextMirror2;
        public LineSegment[] LineSegments { get; } = LineSegments;
        public int NumLineSegments { get; } = NumLineSegments;
        public int SccIndex { get; set; } = -1; // used in Tarjan's Algorithm
        public int LowLink { get; set; } = -1; // Used in Tarjan's Algorithm
        public StronglyConnectedComponent Scc { get; set; } = default!;
        public SplittingMirror? NextMirror1 { get; set; } = null;
        public SplittingMirror? NextMirror2 { get; set; } = null;
        public ulong[]? Bitset { get; set; } = default!;
        public bool OnStack { get; set; } = false; // Used in Tarjan's Algorithm
    }

    public class StronglyConnectedComponent(int id, List<SplittingMirror> mirrors)
    {
        public int Id { get; } = id;
        public List<SplittingMirror> Mirrors { get; } = mirrors;
        public bool Processed { get; set; } = false;
    }


    [SkipLocalsInit]
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int width = input.IndexOf((byte)'\n');
        int rowLength = width + 1;
        int height = input.Length / rowLength;

        // This dictionary is a cache of all the tiles that will be visited after getting split at each '|' and '-' tile
        // This cache is stored using a bitset in a ulong[]
        Dictionary<int, SplittingMirror> splittingMirrors = GetSeenBitsForEachSplittingMirror(input, rowLength);

        // Reusable ulong[] bitset which tracks all the tiles that are visited after entering the grid.
        // This is created here so that we don't need to allocate this array for every single entry point.
        ulong[] seenBits = new ulong[(input.Length - 1) / 64 + 1];

        var lineSegments = new LineSegment[16];
        int part1 = SolveAtEntry(input, 0, 0, Dir.East, rowLength, seenBits, splittingMirrors, lineSegments);
        solution.SubmitPart1(part1);

        int part2 = part1;

        // Save having to check if y == 0 inside the for loop by doing the other y == 0 case outside it
        part2 = Math.Max(part2, SolveAtEntry(input, rowLength - 1, 0, Dir.West, rowLength, seenBits, splittingMirrors, lineSegments));

        // Loop over all the potential entry points on the left and right sides
        for (int y = 1; y < height; y++)
        {
            int left = SolveAtEntry(input, 0, y, Dir.East, rowLength, seenBits, splittingMirrors, lineSegments);
            int right = SolveAtEntry(input, width - 1, y, Dir.West, rowLength, seenBits, splittingMirrors, lineSegments);
            part2 = Math.Max(left, Math.Max(right, part2));
        }

        // Loop over all the potential entry points on the top and bottom sides
        for (int x = 0; x < width; x++)
        {
            int top = SolveAtEntry(input, x, 0, Dir.South, rowLength, seenBits, splittingMirrors, lineSegments);
            int bottom = SolveAtEntry(input, x, height - 1, Dir.North, rowLength, seenBits, splittingMirrors, lineSegments);
            part2 = Math.Max(top, Math.Max(bottom, part2));
        }

        solution.SubmitPart2(part2);
    }

    private static Dictionary<int, SplittingMirror> GetSeenBitsForEachSplittingMirror(ReadOnlySpan<byte> input, int rowLength)
    {
        Dictionary<int, SplittingMirror> splitMirrorGraph = BuildSplitMirrorGraph(input, rowLength);
        List<StronglyConnectedComponent> components = FindAllStronglyConnectedComponents(splitMirrorGraph);

        int bitSetLength = (input.Length - 1) / 64 + 1;
        foreach (StronglyConnectedComponent scc in components)
            PopulateSeenBits(scc);

        return splitMirrorGraph;

        [SkipLocalsInit]
        void PopulateSeenBits(StronglyConnectedComponent scc)
        {
            if (scc.Processed)
                return;

            int sccId = scc.Id;
            ulong[] seenBits = new ulong[bitSetLength];
            bool isFirstEmpty = true;

            foreach (SplittingMirror graphNode in scc.Mirrors)
            {
                SplittingMirror? nextMirror1 = graphNode.NextMirror1;
                if (nextMirror1 != null && sccId != nextMirror1.Scc.Id)
                {
                    PopulateSeenBits(nextMirror1.Scc);
                    CombineBitsets(seenBits, nextMirror1.Bitset!, ref isFirstEmpty);
                }

                SplittingMirror? nextMirror2 = graphNode.NextMirror2;
                if (nextMirror2 != null && sccId != nextMirror2.Scc.Id)
                {
                    PopulateSeenBits(nextMirror2.Scc);
                    CombineBitsets(seenBits, nextMirror2.Bitset!, ref isFirstEmpty);
                }

                if (isFirstEmpty)
                {
                    Array.Clear(seenBits);
                    isFirstEmpty = false;
                }

                CombineBitsetWithSegments(seenBits, graphNode.LineSegments, graphNode.NumLineSegments);
                graphNode.Bitset = seenBits;
            }

            scc.Processed = true;
        }
    }

    private static Dictionary<int, SplittingMirror> BuildSplitMirrorGraph(ReadOnlySpan<byte> input, int rowLength)
    {
        Dictionary<int, SplittingMirror> splitMirrorGraph = new(600);

        // Iterate through all the '|' and '-' mirrors to get the mirrors they connect to and the bitset of the path travelled to get there
        int splittingMirrorIndex = 0;
        while (true)
        {
            int nextIndex = input.Slice(splittingMirrorIndex).IndexOfAny("-|"u8);
            if (nextIndex < 0)
                break;

            splittingMirrorIndex += nextIndex;

            int numSegments = 0;
            var lineSegments = new LineSegment[16];

            // Find the next mirror split that you run into when going in either of the two directions after the splitting
            // on the current mirror. MoveUntilNextMirrorSplit will return -1 if the beam leaves the grid before hitting
            // another mirror
            int mirrorIndex1;
            int mirrorIndex2;
            if (input[splittingMirrorIndex] == '-')
            {
                mirrorIndex1 = MoveUntilNextMirrorSplit(input, splittingMirrorIndex - 1, Dir.West, lineSegments, ref numSegments, rowLength);
                mirrorIndex2 = MoveUntilNextMirrorSplit(input, splittingMirrorIndex + 1, Dir.East, lineSegments, ref numSegments, rowLength);
            }
            else
            {
                mirrorIndex1 = MoveUntilNextMirrorSplit(input, splittingMirrorIndex - rowLength, Dir.North, lineSegments, ref numSegments, rowLength);
                mirrorIndex2 = MoveUntilNextMirrorSplit(input, splittingMirrorIndex + rowLength, Dir.South, lineSegments, ref numSegments, rowLength);
            }

            splitMirrorGraph[splittingMirrorIndex] = new(splittingMirrorIndex, mirrorIndex1, mirrorIndex2, lineSegments, numSegments);
            splittingMirrorIndex++;
        }

        return splitMirrorGraph;
    }

    private static List<StronglyConnectedComponent> FindAllStronglyConnectedComponents(Dictionary<int, SplittingMirror> splitMirrorGraph)
    {
        // Use Tarjan's Algorithm to find all strongly connected components (cycles)

        int sccIndex = 0;
        List<StronglyConnectedComponent> sccs = new(splitMirrorGraph.Count);
        SplittingMirror[] stack = new SplittingMirror[300];
        int stackLen = 0;

        foreach (SplittingMirror mirror in splitMirrorGraph.Values)
        {
            if (mirror.SccIndex == -1)
                StrongConnect(mirror);
        }

        return sccs;

        void StrongConnect(SplittingMirror mirror)
        {
            mirror.SccIndex = sccIndex;
            mirror.LowLink = sccIndex;
            sccIndex++;
            mirror.OnStack = true;

            stack[stackLen++] = mirror;

            if (mirror.NextMirrorIndex1 >= 0)
            {
                SplittingMirror graphNode = splitMirrorGraph[mirror.NextMirrorIndex1];
                mirror.NextMirror1 = graphNode;
                if (graphNode.SccIndex == -1)
                {
                    StrongConnect(graphNode);
                    mirror.LowLink = Math.Min(mirror.LowLink, graphNode.LowLink);
                }
                else if (graphNode.OnStack)
                {
                    mirror.LowLink = Math.Min(mirror.LowLink, graphNode.SccIndex);
                }
            }

            if (mirror.NextMirrorIndex2 >= 0)
            {
                SplittingMirror graphNode = splitMirrorGraph[mirror.NextMirrorIndex2];
                mirror.NextMirror2 = graphNode;
                if (graphNode.SccIndex == -1)
                {
                    StrongConnect(graphNode);
                    mirror.LowLink = Math.Min(mirror.LowLink, graphNode.LowLink);
                }
                else if (graphNode.OnStack)
                {
                    mirror.LowLink = Math.Min(mirror.LowLink, graphNode.SccIndex);
                }
            }

            if (mirror.SccIndex == mirror.LowLink)
            {
                var mirrorsInScc = new List<SplittingMirror>(8);
                var scc = new StronglyConnectedComponent(sccs.Count, mirrorsInScc);
                while (stackLen > 0)
                {
                    SplittingMirror nextMirror = stack[--stackLen];
                    nextMirror.Scc = scc;
                    mirrorsInScc.Add(nextMirror);
                    nextMirror.OnStack = false;
                    if (nextMirror.InputIndex == mirror.InputIndex)
                        break;
                }

                sccs.Add(scc);
            }
        }
    }

    private static void CombineBitsets(ulong[] bitset1, ulong[] bitset2, ref bool isFirstEmpty)
    {
        if (isFirstEmpty)
        {
            Array.Copy(bitset2, bitset1, bitset1.Length);
            isFirstEmpty = false;
            return;
        }

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

    private static void CombineBitsetWithSegments(ulong[] bitset, LineSegment[] lineSegments, int numSegments)
    {
        for (int seg = 0; seg < numSegments; seg++)
        {
            LineSegment segment = lineSegments[seg];
            if (segment.Step > 0)
            {
                for (int i = segment.Start; i <= segment.End; i += segment.Step)
                    bitset[i / 64] |= 1UL << i;
            }
            else
            {
                for (int i = segment.Start; i >= segment.End; i += segment.Step)
                    bitset[i / 64] |= 1UL << i;
            }
        }
    }

    public static int SolveAtEntry(ReadOnlySpan<byte> input, int initX, int initY, Dir initDir, int rowLen, ulong[] seenBits, Dictionary<int, SplittingMirror> splittingMirrors, LineSegment[] lineSegments)
    {
        int numSegments = 0;
        int mirrorIndex = MoveUntilNextMirrorSplit(input, initY * rowLen + initX, initDir, lineSegments, ref numSegments, rowLen);

        if (mirrorIndex >= 0)
            Array.Copy(splittingMirrors[mirrorIndex].Bitset!, seenBits, seenBits.Length);
        else
            Array.Clear(seenBits);

        CombineBitsetWithSegments(seenBits, lineSegments, numSegments);

        int seen = 0;
        foreach (ulong seenBitSet in seenBits)
            seen += BitOperations.PopCount(seenBitSet);
        return seen;
    }

    // This method simply follows the tiles until we hit a '|' or '-' mirror that causes the beam to split into two
    // It also keeps track of all the tiles seen along the way in the seenBits array. If the beam leaves the grid
    // before splitting on a mirror, this method returns -1.
    private static int MoveUntilNextMirrorSplit(ReadOnlySpan<byte> input, int i, Dir dir, LineSegment[] segments, ref int numSegments, int rowLen)
    {
        byte c = 0;
        while (true)
        {
            int start = i;
            switch (dir)
            {
                case Dir.East:

                    while ((c = input[i]) is (byte)'.' or (byte)'-')
                        i++;

                    if (c == '\n')
                    {
                        segments[numSegments++] = new(start, i-1, 1);
                        return -1;
                    }

                    segments[numSegments++] = new(start, i, 1);
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
                    while (i >= 0 && (c = input[i]) is (byte)'.' or (byte)'-')
                        i--;

                    if (i < 0 || c == '\n')
                    {
                        segments[numSegments++] = new(start, i + 1, -1);
                        return -1;
                    }

                    segments[numSegments++] = new(start, i, -1);

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
                    while (i >= 0 && (c = input[i]) is (byte)'.' or (byte)'|')
                        i -= rowLen;

                    if (i < 0)
                    {
                        segments[numSegments++] = new(start, i + rowLen, -rowLen);
                        return -1;
                    }

                    segments[numSegments++] = new(start, i, -rowLen);

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
                    while (i < input.Length && (c = input[i]) is (byte)'.' or (byte)'|')
                        i += rowLen;

                    if (i >= input.Length)
                    {
                        segments[numSegments++] = new(start, i - rowLen, rowLen);
                        return -1;
                    }

                    segments[numSegments++] = new(start, i, rowLen);

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

            // If our line segments array is getting too long, there is probably a cycle
            // Find the cycle and trim all excess line segments
            if (numSegments == segments.Length)
            {
                for (int j = 0; j < numSegments; j++)
                {
                    LineSegment segment = segments[j];
                    for (int k = j + 1;  k < numSegments; k++)
                    {
                        LineSegment otherSegment = segments[k];
                        if (segment.Equals(otherSegment))
                        {
                            numSegments = k;
                            return -1;
                        }
                    }
                }

                return -1;
            }
        }
    }
}
