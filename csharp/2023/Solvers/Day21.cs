using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day21 : ISolver
{
    // This solution assumes:
    // - there is a horizontal and vertical line through the center of the grid with no rocks
    // - the 4 borders of the grid have no rocks
    // - there are no rocks at any position with a manhattan distance of 65 away from the center
    // - the grid is 131x131 (this means that part 1 does not rely on an infinite grid)
    // - the S tile is in the center
    // These assumptions are all true for actual inputs that can appear
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // Since there is a horizontal and vertical line through the center with no rocks and around the grid, we can simply just look at the 4 64x64 quadrants
        // Initialise totals with values that are not in quadrants
        int totalOdd = 32 * 4 + 65 * 4;
        int totalEven = 32 * 4 + 65 * 4 + 1;
        int visitedEven = 32 * 4 + 1;
        int visitedOdd = 33 * 4;

        SimulateQuadrant(input, isRight: false, isBottom: false, ref totalEven, ref totalOdd, ref visitedEven, ref visitedOdd);
        SimulateQuadrant(input, isRight: true, isBottom: false, ref totalEven, ref totalOdd, ref visitedEven, ref visitedOdd);
        SimulateQuadrant(input, isRight: false, isBottom: true, ref totalEven, ref totalOdd, ref visitedEven, ref visitedOdd);
        SimulateQuadrant(input, isRight: true, isBottom: true, ref totalEven, ref totalOdd, ref visitedEven, ref visitedOdd);

        solution.SubmitPart1(visitedEven);

        // Part 2 solution comes from villuna: https://github.com/villuna/aoc23/wiki/A-Geometric-solution-to-advent-of-code-2023,-day-21
        const long n = 202300;
        int oddCorners = totalOdd - visitedOdd;
        int evenCorners = totalEven - visitedEven;

        long part2 = (n + 1) * (n + 1) * totalOdd + n * n * totalEven - (n + 1) * oddCorners + n * evenCorners;
        solution.SubmitPart2(part2);
    }

    [SkipLocalsInit]
    private static void SimulateQuadrant(ReadOnlySpan<byte> input, bool isRight, bool isBottom, ref int totalEven, ref int totalOdd, ref int visitedEven, ref int visitedOdd)
    {
        const int width = 131;
        const int rowLength = width + 1;
        const int center = width / 2;
        const ulong evenBits = 0x5555555555555555UL;
        const ulong oddBits = 0xAAAAAAAAAAAAAAAAUL;

        int step = isBottom ? rowLength : -rowLength;
        ref byte quadrantStartRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(input), center * rowLength + center);
        quadrantStartRef = ref Unsafe.Add(ref quadrantStartRef, isRight ? 1 : -64);
        quadrantStartRef = ref Unsafe.Add(ref quadrantStartRef, step);

        Span<ulong> plots = stackalloc ulong[64];

        // The machine doesn't need AVX-512 for this to work, .NET will still emulate it on AVX2
        for (int i = 0; i < 64; i++)
        {
            ulong plotBits = ~Vector512.Equals(Vector512.LoadUnsafe(ref quadrantStartRef), Vector512.Create((byte)'#')).ExtractMostSignificantBits();

            // If the quadrant is on the left, reverse the bits so that it acts like it was on the right
            if (!isRight)
                plotBits = ReverseBits(plotBits);

            plots[i] = plotBits;
            quadrantStartRef = ref Unsafe.Add(ref quadrantStartRef, step);
        }

        Span<ulong> visitedIn64Steps = stackalloc ulong[64];

        // record number of positions accessible after 64 or 65 steps
        // start off by simulating first 3 steps before loop
        ulong centerRow = 0b111;
        visitedIn64Steps[0] = 3 & plots[0];
        visitedIn64Steps[1] = 1 & plots[1];
        for (int i = 3; i <= 64; i++)
        {
            ulong prev = visitedIn64Steps[0];
            ulong cur = visitedIn64Steps[1];

            visitedIn64Steps[0] = centerRow & plots[0];
            centerRow = (centerRow << 1) | 1;
            for (int j = 0; j < i - 3; j++)
            {
                ulong next = visitedIn64Steps[j + 2];
                visitedIn64Steps[j + 1] = (cur | (cur << 1) | (cur >> 1) | prev | next) & plots[j + 1];
                prev = cur;
                cur = next;
            }

            // set 2nd to last row, can skip getting next row
            visitedIn64Steps[i - 2] = (cur | (cur << 1) | (cur >> 1) | prev) & plots[i - 2];

            // set last row which will just have the furthest bit to the right set
            visitedIn64Steps[i - 1] = 1 & plots[i - 1];
        }

        for (int i = 0; i < 64; i += 2)
        {
            ulong evenStartVisitedRow = visitedIn64Steps[i];
            ulong oddStartVisitedRow = visitedIn64Steps[i + 1];
            visitedEven += BitOperations.PopCount(evenStartVisitedRow & evenBits) + BitOperations.PopCount(oddStartVisitedRow & oddBits);
            visitedOdd += BitOperations.PopCount(oddStartVisitedRow & evenBits) + BitOperations.PopCount(evenStartVisitedRow & oddBits);
        }

        // record number of positions that are accessible from anywhere
        visitedIn64Steps[63] = plots[63];
        for (int i = 1; i < 63; i++)
            visitedIn64Steps[i] |= 1UL << 63;

        int changes = 1;
        while (changes != 0)
        {
            changes = 0;

            ulong prev = visitedIn64Steps[0];
            ulong cur = visitedIn64Steps[1];
            for (int j = 1; j < 63; j++)
            {
                ulong next = visitedIn64Steps[j + 1];
                ulong newValue = (cur | (cur << 1) | (cur >> 1) | prev | next) & plots[j];
                changes += newValue == cur ? 0 : 1;
                visitedIn64Steps[j] = newValue;
                prev = cur;
                cur = next;
            }
        }

        for (int i = 0; i < 64; i += 2)
        {
            ulong evenStartVisitedRow = visitedIn64Steps[i];
            ulong oddStartVisitedRow = visitedIn64Steps[i + 1];
            totalEven += BitOperations.PopCount(evenStartVisitedRow & evenBits) + BitOperations.PopCount(oddStartVisitedRow & oddBits);
            totalOdd += BitOperations.PopCount(oddStartVisitedRow & evenBits) + BitOperations.PopCount(evenStartVisitedRow & oddBits);
        }
    }

    private static ulong ReverseBits(ulong v)
    {
        v = BinaryPrimitives.ReverseEndianness(v);
        v = ((v >> 1) & 0x5555555555555555UL) | ((v & 0x5555555555555555UL) << 1);
        v = ((v >> 2) & 0x3333333333333333UL) | ((v & 0x3333333333333333UL) << 2);
        v = ((v >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((v & 0x0F0F0F0F0F0F0F0FUL) << 4);
        return v;
    }
}
