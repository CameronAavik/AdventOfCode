﻿using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day05 : ISolver
{
    public readonly record struct Horizontal(int Y, int X1, int X2) : IComparable<Horizontal>
    {
        public int CompareTo(Horizontal other)
        {
            int c = Y.CompareTo(other.Y);
            if (c != 0)
                return c;

            return X1.CompareTo(other.X1);
        }
    }

    public readonly record struct Event(int Y, int X) : IComparable<Event>
    {
        public int CompareTo(Event other) => Y.CompareTo(other.Y);
    }

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        const ulong onesFlag = 0b0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001UL;

        var horizontals = new List<Horizontal>();
        var verticalEvents = new List<Event>();
        var posDiagEvents = new List<Event>();
        var negDiagEvents = new List<Event>();

        ParseInput(input, horizontals, verticalEvents, posDiagEvents, negDiagEvents, out int maxX, out int maxY);

        horizontals.Sort();
        verticalEvents.Sort();
        posDiagEvents.Sort();
        negDiagEvents.Sort();

        // Add events to the end so that we don't have to do a bounds check
        horizontals.Add(new Horizontal(int.MaxValue, 0, 0));
        verticalEvents.Add(new Event(int.MaxValue, 0));
        posDiagEvents.Add(new Event(int.MaxValue, 0));
        negDiagEvents.Add(new Event(int.MaxValue, 0));

        int horizIndex = 0;
        int vertIndex = 0;
        int posDiagIndex = 0;
        int negDiagIndex = 0;

        int countsLength = (maxX / 16) + 1;
        Span<ulong> rowHorizontalCounts = stackalloc ulong[countsLength];
        Span<ulong> currentVerticalCounts = stackalloc ulong[countsLength];
        Span<ulong> currentPosDiagCounts = stackalloc ulong[countsLength];
        Span<ulong> currentNegDiagCounts = stackalloc ulong[countsLength];

        int part1 = 0;
        int part2 = 0;

        for (int y = 0; y <= (maxY * 2); y += 2)
        {
            ProcessHorizontals(horizontals, ref horizIndex, rowHorizontalCounts, y);
            ProcessVerticals(verticalEvents, ref vertIndex, currentVerticalCounts, y);
            ProcessPositiveDiagonals(posDiagEvents, ref posDiagIndex, currentPosDiagCounts, y);
            ProcessNegativeDiagonals(negDiagEvents, ref negDiagIndex, currentNegDiagCounts, y);

            for (int i = 0; i < countsLength; i++)
            {
                // Adding 6 to each value so that the high bit is set to 1 when there is an overlap
                ulong part1Overlaps = rowHorizontalCounts[i] + currentVerticalCounts[i] + onesFlag * 6;
                ulong part2Overlaps = part1Overlaps + currentPosDiagCounts[i] + currentNegDiagCounts[i];
                part1 += BitOperations.PopCount(part1Overlaps & onesFlag * 8);
                part2 += BitOperations.PopCount(part2Overlaps & onesFlag * 8);
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseInput(ReadOnlySpan<char> input, List<Horizontal> horizontals, List<Event> verticalEvents, List<Event> posDiagEvents, List<Event> negDiagEvents, out int maxX, out int maxY)
    {
        int inputCursor = 0;
        maxX = 0;
        maxY = 0;
        while (inputCursor < input.Length)
        {
            int x1 = ReadIntegerUntil(input, ',', ref inputCursor);
            int y1 = ReadIntegerUntil(input, ' ', ref inputCursor);
            inputCursor += "-> ".Length;
            int x2 = ReadIntegerUntil(input, ',', ref inputCursor);
            int y2 = ReadIntegerUntil(input, '\n', ref inputCursor);

            if (x1 > maxX)
                maxX = x1;
            if (x2 > maxX)
                maxX = x2;
            if (y1 > maxY)
                maxY = y1;
            if (y2 > maxY)
                maxY = y2;

            int xDiff = x2 - x1;
            int yDiff = y2 - y1;

            if (y1 == y2)
            {
                horizontals.Add(new(y1 * 2, Math.Min(x1, x2), Math.Max(x1, x2)));
            }
            else if (x1 == x2)
            {
                verticalEvents.Add(new(Math.Min(y1, y2) * 2, x1));
                verticalEvents.Add(new(Math.Max(y1, y2) * 2 + 1, x1));
            }
            else if (yDiff == xDiff)
            {
                if (y1 < y2)
                {
                    posDiagEvents.Add(new(y1 * 2, x1));
                    posDiagEvents.Add(new(y2 * 2 + 1, x2));
                }
                else
                {
                    posDiagEvents.Add(new(y2 * 2, x2));
                    posDiagEvents.Add(new(y1 * 2 + 1, x1));
                }
            }
            else if (yDiff == -xDiff)
            {
                if (y1 < y2)
                {
                    negDiagEvents.Add(new(y1 * 2, x1));
                    negDiagEvents.Add(new(y2 * 2 + 1, x2));
                }
                else
                {
                    negDiagEvents.Add(new(y2 * 2, x2));
                    negDiagEvents.Add(new(y1 * 2 + 1, x1));
                }
            }
        }
    }

    private static void ProcessHorizontals(List<Horizontal> horizontals, ref int horizIndex, Span<ulong> rowHorizontalCounts, int y)
    {
        const ulong onesFlag = 0b0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001_0001UL;

        rowHorizontalCounts.Clear();
        Horizontal horiz;
        while ((horiz = horizontals[horizIndex]).Y == y)
        {
            (int x1Cell, int x1Offset) = Math.DivRem(horiz.X1, 16);
            (int x2Cell, int x2Offset) = Math.DivRem(horiz.X2 + 1, 16);

            if (x1Cell == x2Cell)
            {
                rowHorizontalCounts[x1Cell] += (((1UL << (x2Offset * 4)) - 1) ^ ((1UL << (x1Offset * 4)) - 1)) & onesFlag;
            }
            else
            {
                rowHorizontalCounts[x1Cell] += (ulong.MaxValue ^ ((1UL << (x1Offset * 4)) - 1)) & onesFlag;

                for (int cell = x1Cell + 1; cell < x2Cell; cell++)
                    rowHorizontalCounts[cell] += onesFlag;

                rowHorizontalCounts[x2Cell] += ((1UL << (x2Offset * 4)) - 1) & onesFlag;
            }

            horizIndex++;
        }
    }

    private static void ProcessVerticals(List<Event> verticalEvents, ref int vertIndex, Span<ulong> currentVerticalCounts, int y)
    {
        Event vert;
        while (vertIndex < verticalEvents.Count && (vert = verticalEvents[vertIndex]).Y == y - 1)
        {
            (int cell, int offset) = Math.DivRem(vert.X, 16);
            currentVerticalCounts[cell] -= 1UL << (4 * offset);
            vertIndex++;
        }

        while (vertIndex < verticalEvents.Count && (vert = verticalEvents[vertIndex]).Y == y)
        {
            (int cell, int offset) = Math.DivRem(vert.X, 16);
            currentVerticalCounts[cell] += 1UL << (4 * offset);
            vertIndex++;
        }
    }

    private static void ProcessPositiveDiagonals(List<Event> posDiagEvents, ref int posDiagIndex, Span<ulong> currentPosDiagCounts, int y)
    {
        Event posDiag;
        while ((posDiag = posDiagEvents[posDiagIndex]).Y == y - 1)
        {
            (int cell, int offset) = Math.DivRem(posDiag.X, 16);
            currentPosDiagCounts[cell] -= 1UL << (4 * offset);
            posDiagIndex++;
        }

        ulong carry = 0;
        for (int i = 0; i < currentPosDiagCounts.Length; i++)
        {
            ulong c = currentPosDiagCounts[i];
            ulong nextCarry = c >> 60;
            currentPosDiagCounts[i] = (c << 4) + carry;
            carry = nextCarry;
        }

        while ((posDiag = posDiagEvents[posDiagIndex]).Y == y)
        {
            (int cell, int offset) = Math.DivRem(posDiag.X, 16);
            currentPosDiagCounts[cell] += 1UL << (4 * offset);
            posDiagIndex++;
        }
    }

    private static void ProcessNegativeDiagonals(List<Event> negDiagEvents, ref int negDiagIndex, Span<ulong> currentNegDiagCounts, int y)
    {
        Event negDiag;
        while ((negDiag = negDiagEvents[negDiagIndex]).Y == y - 1)
        {
            (int cell, int offset) = Math.DivRem(negDiag.X, 16);
            currentNegDiagCounts[cell] -= 1UL << (4 * offset);
            negDiagIndex++;
        }

        ulong carry = 0;
        for (int i = currentNegDiagCounts.Length - 1; i >= 0; i--)
        {
            ulong c = currentNegDiagCounts[i];
            ulong nextCarry = c & 0b1111;
            currentNegDiagCounts[i] = (c >> 4) + (carry << 60);
            carry = nextCarry;
        }

        while ((negDiag = negDiagEvents[negDiagIndex]).Y == y)
        {
            (int cell, int offset) = Math.DivRem(negDiag.X, 16);
            currentNegDiagCounts[cell] += 1UL << (4 * offset);
            negDiagIndex++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerUntil(ReadOnlySpan<char> span, char c, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = span[i++] - '0';

        char cur;
        while ((cur = span[i++]) != c)
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
