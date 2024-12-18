﻿using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day06 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int timesLineEnd = input.IndexOf((byte)'\n');
        ReadOnlySpan<byte> timesLine = input.Slice(0, timesLineEnd);
        ReadOnlySpan<byte> distanceLine = input.Slice(timesLineEnd + 1, input.Length - timesLineEnd - 2);

        long part1 = 1;
        long part2Time = 0;
        long part2Distance = 0;

        while (timesLine.Length > 0)
        {
            long time = ParseNum(ref timesLine, ref part2Time);
            long distance = ParseNum(ref distanceLine, ref part2Distance);
            part1 *= NumWaysToWin(time, distance);
        }

        solution.SubmitPart1(part1);

        long part2 = NumWaysToWin(part2Time, part2Distance);
        solution.SubmitPart2(part2);
    }

    private static long ParseNum(ref ReadOnlySpan<byte> line, ref long part2Var)
    {
        byte c;
        int nextStart = line.IndexOfAnyInRange((byte)'0', (byte)'9');
        line = line.Slice(nextStart);
        long time = 0;
        int i = 0;
        while (i < line.Length && (c = line[i]) != ' ')
        {
            time = time * 10 + c - '0';
            part2Var = part2Var * 10 + c - '0';
            i++;
        }

        line = line.Slice(i);
        return time;
    }

    static long NumWaysToWin(long time, long distance)
    {
        // solve for distance = (time - x) * x
        // x = (time +- sqrt(time^2 - 4 * distance)) / 2
        // time^2 overflows the long on part 2, so we can rewrite it as follows:
        // x = (time +- sqrt(time - 2 * sqrt(distance)) * sqrt(time + 2 * sqrt(distance))) / 2

        double sqrtDistance = Math.Sqrt(distance);
        double sqrt = Math.Sqrt(time - 2 * sqrtDistance) * Math.Sqrt(time + 2 * sqrtDistance);
        long low = Convert.ToInt64(Math.Ceiling((time - sqrt) / 2));
        long high = Convert.ToInt64(Math.Floor((time + sqrt) / 2));

        // handle ties or precision issues

        if ((time - low) * low <= distance)
            low++;

        if ((time - high) * high <= distance)
            high--;

        return high - low + 1;
    }
}
