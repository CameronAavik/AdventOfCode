using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day02 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int horizontal = 0;
        int part1DepthPart2Aim = 0; // depth for part 1, aim for part 2
        int part2Depth = 0;

        int i = 0;
        while (i < input.Length)
        {
            switch (input[i])
            {
                case (byte)'f': // forward
                    int amount = CharToValue(input[i + "forward ".Length]);
                    horizontal += amount;
                    part2Depth += part1DepthPart2Aim * amount;
                    i += "forward x\n".Length;
                    break;
                case (byte)'d': // down
                    part1DepthPart2Aim += CharToValue(input[i + "down ".Length]);
                    i += "down x\n".Length;
                    break;
                default: // up
                    part1DepthPart2Aim -= CharToValue(input[i + "up ".Length]);
                    i += "up x\n".Length;
                    break;
            }
        }

        solution.SubmitPart1(horizontal * part1DepthPart2Aim);
        solution.SubmitPart2(horizontal * part2Depth);
    }

    private static int CharToValue(byte c) => c - '0';
}
