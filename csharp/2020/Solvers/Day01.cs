using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var length = 0;
        var numberSet = new byte[2048];
        var numbers = new int[512];
        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            var num = line[0] - '0';
            for (var i = 1; i < line.Length; i++)
            {
                num = num * 10 + (line[i] - '0');
            }

            numberSet[num] = 1;
            numbers[length++] = num;
        }

        Array.Sort(numbers, 0, length);

        var part1 = -1;
        var part2 = -1;
        for (var i = 0; i < length; i++)
        {
            var a = numbers[i];
            var part1B = 2020 - a;
            for (var j = i + 1; j < length; j++)
            {
                var b = numbers[j];
                if (b < part1B)
                {
                    var c = part1B - b;
                    if (numberSet[c] == 1)
                    {
                        part2 = a * b * c;
                        solution.SubmitPart2(part2);
                        if (part1 >= 0)
                            return;
                    }
                }
                else if (b == part1B)
                {
                    part1 = a * b;
                    solution.SubmitPart1(part1);
                    if (part2 >= 0)
                        return;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
