using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day07 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var crabs = new List<int>();

        long sumLeft = 0;
        long sumRight = 0;

        long sumSquaredLeft = 0;
        long sumSquaredRight = 0;

        int cursor = 0;
        while (cursor < input.Length)
        {
            int crab = ReadCrab(input, ref cursor);
            crabs.Add(crab);
            sumRight += crab;
            sumSquaredRight += crab * (crab + 1) / 2;
        }

        crabs.Sort();
        int totalCrabs = crabs.Count;
        int prevMid = crabs[0];

        bool part1Found = false;
        bool part2Found = false;
        for (int i = 1; i < crabs.Count; i++)
        {
            int value = crabs[i];
            int diff = value - prevMid;

            if (diff == 0)
                continue;

            if (!part1Found && totalCrabs < 2 * i)
            {
                solution.SubmitPart1(sumLeft + sumRight);
                if (part2Found)
                    return;

                part1Found = true;
            }

            if (!part2Found)
            {
                long leftSumSquaredIncrease = sumLeft * diff + i * diff * (diff + 1) / 2;
                long rightSumSquaredDecrease = sumRight * diff - (totalCrabs - i) * (diff * (diff - 1) / 2);

                if (leftSumSquaredIncrease > rightSumSquaredDecrease)
                {
                    solution.SubmitPart2(sumSquaredLeft + sumSquaredRight);
                    if (part1Found)
                        return;

                    part2Found = true;
                }

                sumSquaredLeft += leftSumSquaredIncrease;
                sumSquaredRight -= rightSumSquaredDecrease;
            }

            sumLeft += diff * i;
            sumRight -= diff * (totalCrabs - i);
            prevMid = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadCrab(ReadOnlySpan<byte> span, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = span[i++] - '0';

        byte cur;
        while ((cur = span[i++]) is not ((byte)',' or (byte)'\n'))
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
