using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int length = 0;
        byte[]? numberSet = new byte[2048];
        int[]? numbers = new int[512];
        foreach (Range lineRange in input.SplitLines())
        {
            ReadOnlySpan<byte> line = input[lineRange];
            int num = line[0] - '0';
            for (int i = 1; i < line.Length; i++)
            {
                num = num * 10 + (line[i] - '0');
            }

            numberSet[num] = 1;
            numbers[length++] = num;
        }

        Array.Sort(numbers, 0, length);

        int part1 = -1;
        int part2 = -1;
        for (int i = 0; i < length; i++)
        {
            int a = numbers[i];
            int part1B = 2020 - a;
            for (int j = i + 1; j < length; j++)
            {
                int b = numbers[j];
                if (b < part1B)
                {
                    int c = part1B - b;
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
