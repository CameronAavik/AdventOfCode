using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var horizontal = 0;
        var part1DepthPart2Aim = 0; // depth for part 1, aim for part 2
        var part2Depth = 0;

        var i = 0;
        while (i < input.Length)
        {
            switch (input[i])
            {
                case (byte)'f': // forward
                    var amount = CharToValue(input[i + "forward ".Length]);
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
