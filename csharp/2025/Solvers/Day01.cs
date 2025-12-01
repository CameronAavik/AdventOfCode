using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var dial = 50;
        var part1 = 0;
        var part2 = 0;
        var i = 0;
        while (i < input.Length)
        {
            var dir = input[i++];
            var onesDigit = input[i++] - '0';
            var tensDigit = 0;
            var fullRotations = 0;
            byte c;
            while ((c = input[i++]) != (byte)'\n')
            {
                fullRotations = fullRotations * 10 + tensDigit;
                tensDigit = onesDigit;
                onesDigit = c - '0';
            }

            part2 += fullRotations;
            var steps = tensDigit * 10 + onesDigit;

            if (dir <= 'L')
            {
                if (dial == 0)
                {
                    dial += 100 - steps;
                }
                else
                {
                    dial -= steps;
                    if (dial <= 0)
                    {
                        part2++;
                        if (dial < 0)
                            dial += 100;
                    }
                }
            }
            else
            {
                dial += steps;
                if (dial >= 100)
                {
                    part2++;
                    dial -= 100;
                }
            }

            if (dial == 0)
                part1++;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
