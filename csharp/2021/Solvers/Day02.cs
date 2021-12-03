using AdventOfCode.CSharp.Common;
using System;

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
                    int amount = input[i + 8] - '0';
                    horizontal += amount;
                    part2Depth += part1DepthPart2Aim * amount;
                    i += 10;
                    break;
                case 'd': // down
                    part1DepthPart2Aim += input[i + 5] - '0';
                    i += 7;
                    break;
                default: // up
                    part1DepthPart2Aim -= input[i + 3] - '0';
                    i += 5;
                    break;
            }
        }

        return new Solution(
            part1: horizontal * part1DepthPart2Aim,
            part2: horizontal * part2Depth);
    }
}
