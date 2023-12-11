using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day10 : ISolver
{
    private const int East = 0;
    private const int West = 1;
    private const int North = 2;
    private const int South = 3;

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // 'paths' stores a bit for every position that is on the path
        uint[] paths = new uint[(input.Length - 1) / 32 + 1];

        // 'crossings' stores a bit for every position that acts as a boundary between if something is enclosed inside the loop or not
        // All '|' are crossings, and every vertical S-bend, 'F---J' or 'L---7', counts as one crossing
        uint[] crossings = new uint[(input.Length - 1) / 32 + 1];

        int part1 = SolvePart1(input, crossings, paths);
        solution.SubmitPart1(part1);

        int part2 = SolvePart2(paths, crossings);
        solution.SubmitPart2(part2);
    }

    /// <summary>
    /// Solves part 1, while also outputting a bitset of all the crossing and the path positions.
    /// </summary>
    private static int SolvePart1(ReadOnlySpan<byte> input, uint[] crossings, uint[] paths)
    {
        int rowLen = input.IndexOf((byte)'\n') + 1;
        int startPos = input.IndexOf((byte)'S');

        byte startPipe = GetStartPipeTypeAndDirection(input, rowLen, startPos, out int dir);

        int steps = 0;
        int pos = startPos;

        byte lastHorizontalTurn = startPipe;

        // If we start on a '-' or '|', we need to set the start position onto the previous turn
        // This is because our loop that follows the path only stops on turns
        // In addition, we also need to find the last horizontal turn in case we are on a vertical S-bend
        if (startPipe == '-')
        {
            // Starting on '-', so find the last turn
            while (true)
            {
                paths[startPos / 32] |= 1U << startPos;
                byte c = input[++startPos];
                steps++;
                if (c != '-')
                {
                    lastHorizontalTurn = c;
                    break;
                }
            }
        }
        else if (startPipe == '|')
        {
            // Starting on '|', so find the last turn
            while (true)
            {
                paths[startPos / 32] |= 1U << startPos;
                crossings[startPos / 32] |= 1U << startPos;
                startPos -= rowLen;
                byte c = input[startPos];
                steps++;

                if (c != '|')
                    break;
            }
        }
        else
        {
            paths[startPos / 32] |= 1U << startPos;
        }

        while (true)
        {
            byte c;
            switch (dir)
            {
                case East:
                    while ((c = input[++pos]) == '-')
                    {
                        paths[pos / 32] |= 1U << pos;
                        steps++;
                    }

                    paths[pos / 32] |= 1U << pos;
                    steps++;

                    if (c == 'J')
                    {
                        if (lastHorizontalTurn == 'F')
                            crossings[pos / 32] |= 1U << pos;
                        dir = North;
                    }
                    else if (c == '7')
                    {
                        if (lastHorizontalTurn == 'L')
                            crossings[pos / 32] |= 1U << pos;
                        dir = South;
                    }

                    break;
                case West:
                    while ((c = input[--pos]) == '-')
                    {
                        paths[pos / 32] |= 1U << pos;
                        steps++;
                    }

                    paths[pos / 32] |= 1U << pos;
                    steps++;

                    if (c == 'L')
                    {
                        if (lastHorizontalTurn == '7')
                            crossings[pos / 32] |= 1U << pos;
                        dir = North;
                    }
                    else if (c == 'F')
                    {
                        if (lastHorizontalTurn == 'J')
                            crossings[pos / 32] |= 1U << pos;
                        dir = South;
                    }

                    break;
                case North:
                    while ((c = input[pos -= rowLen]) == '|')
                    {
                        paths[pos / 32] |= 1U << pos;
                        crossings[pos / 32] |= 1U << pos;
                        steps++;
                    }

                    paths[pos / 32] |= 1U << pos;
                    lastHorizontalTurn = c;
                    steps++;
                    dir = c == '7' ? West : East;
                    break;
                case South:
                    while ((c = input[pos += rowLen]) == '|')
                    {
                        paths[pos / 32] |= 1U << pos;
                        crossings[pos / 32] |= 1U << pos;
                        steps++;
                    }

                    paths[pos / 32] |= 1U << pos;
                    lastHorizontalTurn = c;
                    steps++;
                    dir = c == 'L' ? East : West;
                    break;
            }

            if (pos == startPos)
                break;
        }

        return steps / 2;
    }

    private static byte GetStartPipeTypeAndDirection(ReadOnlySpan<byte> input, int rowLen, int startPos, out int dir)
    {
        bool leftIncoming = input[startPos - 1] is (byte)'L' or (byte)'F' or (byte)'-';
        bool rightIncoming = input[startPos + 1] is (byte)'J' or (byte)'7' or (byte)'-';
        bool upIncoming = input[startPos - rowLen] is (byte)'F' or (byte)'7' or (byte)'|';
        bool downIncoming = input[startPos + rowLen] is (byte)'F' or (byte)'7' or (byte)'|';

        if (leftIncoming)
        {
            dir = West;
            if (upIncoming)
                return (byte)'J';
            else if (downIncoming)
                return (byte)'7';
            else
                return (byte)'-';
        }
        else if (rightIncoming)
        {
            dir = East;
            return (byte)(upIncoming ? 'L' : 'F');
        }
        else
        {
            dir = South;
            return (byte)'|';
        }
    }

    private static int SolvePart2(uint[] paths, uint[] crossings)
    {
        int part2 = 0;
        uint initialMask = 0U;
        for (int i = 0; i < paths.Length; i++)
        {
            // see https://lemire.me/blog/2018/02/21/iterating-over-set-bits-quickly/ for an explanation of how this works
            uint crossing = crossings[i];
            uint insideMask = initialMask;
            while (crossing != 0)
            {
                uint lsb = crossing & (~crossing + 1);
                insideMask ^= ~(lsb - 1); // flips all the bits after the lsb
                crossing ^= lsb;
                initialMask = ~initialMask;
            }

            part2 += BitOperations.PopCount(~paths[i] & insideMask);
        }

        return part2;
    }
}
