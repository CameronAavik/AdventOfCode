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

        var cursor = 0;
        while (cursor < input.Length)
        {
            var crab = ReadCrab(input, ref cursor);
            crabs.Add(crab);
            sumRight += crab;
            sumSquaredRight += crab * (crab + 1) / 2;
        }

        crabs.Sort();
        var totalCrabs = crabs.Count;
        var prevMid = crabs[0];

        var part1Found = false;
        var part2Found = false;
        for (var i = 1; i < crabs.Count; i++)
        {
            var value = crabs[i];
            var diff = value - prevMid;

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
                var leftSumSquaredIncrease = sumLeft * diff + i * diff * (diff + 1) / 2;
                var rightSumSquaredDecrease = sumRight * diff - (totalCrabs - i) * (diff * (diff - 1) / 2);

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
        var ret = span[i++] - '0';

        byte cur;
        while ((cur = span[i++]) is not ((byte)',' or (byte)'\n'))
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
