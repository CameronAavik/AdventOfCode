using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day10 : ISolver
{
    public enum Dir { East, West, North, South }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var rowLen = input.IndexOf((byte)'\n') + 1;
        var startPosIndex = input.IndexOf((byte)'S');
        var i = startPosIndex;

        // assume that S = (0, 0)
        var x = 0;
        var y = 0;

        Dir dir;
        if (input[i - 1] is (byte)'L' or (byte)'F' or (byte)'-')
            dir = Dir.West;
        else if (input[i + 1] is (byte)'J' or (byte)'7' or (byte)'-')
            dir = Dir.East;
        else
            dir = Dir.South;

        var steps = 0;
        var area = 0;

        while (true)
        {
            byte c;
            var count = 1;
            switch (dir)
            {
                case Dir.East:
                    while ((c = input[++i]) == '-')
                        count++;

                    steps += count;
                    x += count;
                    area -= count * y;
                    dir = c == 'J' ? Dir.North : Dir.South;
                    break;
                case Dir.West:
                    while ((c = input[--i]) == '-')
                        count++;

                    steps += count;
                    x -= count;
                    area += count * y;
                    dir = c == 'L' ? Dir.North : Dir.South;
                    break;
                case Dir.North:
                    while ((c = input[i -= rowLen]) == '|')
                        count++;

                    steps += count;
                    y -= count;
                    area -= count * x;
                    dir = c == '7' ? Dir.West : Dir.East;
                    break;
                case Dir.South:
                    while ((c = input[i += rowLen]) == '|')
                        count++;

                    steps += count;
                    y += count;
                    area += count * x;
                    dir = c == 'L' ? Dir.East : Dir.West;
                    break;
            }

            if (i == startPosIndex)
                break;
        }

        solution.SubmitPart1(steps / 2);
        solution.SubmitPart2((Math.Abs(area) - steps) / 2 + 1);
    }
}
