using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day02 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        int horizontal = 0;
        int part1DepthPart2Aim = 0; // depth for part 1, aim for part 2
        int part2Depth = 0;

        int i = 0;
        while (i < input.Length)
        {
            switch (input[i])
            {
                case 'f': // forward
                    i += "forward ".Length;
                    int amount = ReadLineAsInteger(input, ref i);
                    horizontal += amount;
                    part2Depth += part1DepthPart2Aim * amount;
                    break;
                case 'd': // down
                    i += "down ".Length;
                    part1DepthPart2Aim += ReadLineAsInteger(input, ref i);
                    break;
                default: // up
                    i += "up ".Length;
                    part1DepthPart2Aim -= ReadLineAsInteger(input, ref i);
                    break;
            }
        }

        return new Solution(part1: horizontal * part1DepthPart2Aim, part2: horizontal * part2Depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadLineAsInteger(ReadOnlySpan<char> span, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = span[i++] - '0';

        char cur;
        while ((cur = span[i++]) != '\n')
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
