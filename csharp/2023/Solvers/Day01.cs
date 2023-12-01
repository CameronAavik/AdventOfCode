using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        int firstNumericalDigit = 0;
        int lastNumericalDigit = 0;
        int firstDigit = 0;
        int lastDigit = 0;

        while (input.Length > 0) // last character always newline
        {
            switch (input)
            {
                case [(byte)'\n', ..]:
                    part1 += 10 * firstNumericalDigit + lastNumericalDigit;
                    part2 += 10 * firstDigit + lastDigit;
                    firstNumericalDigit = 0;
                    lastNumericalDigit = 0;
                    firstDigit = 0;
                    lastDigit = 0;
                    input = input.Slice(1);
                    break;
                case [>= (byte)'1' and <= (byte)'9', ..]:
                    lastNumericalDigit = input[0] - '0';
                    lastDigit = lastNumericalDigit;
                    input = input.Slice(1);
                    break;
                case [(byte)'o', (byte)'n', (byte)'e', ..]:
                    lastDigit = 1;
                    input = input.Slice(2); // e could be start of eight
                    break;
                case [(byte)'t', (byte)'w', (byte)'o', ..]:
                    lastDigit = 2;
                    input = input.Slice(2); // o could be start of one
                    break;
                case [(byte)'t', (byte)'h', (byte)'r', (byte)'e', (byte)'e', ..]:
                    lastDigit = 3;
                    input = input.Slice(4); // e could be start of eight
                    break;
                case [(byte)'f', (byte)'o', (byte)'u', (byte)'r', ..]:
                    lastDigit = 4;
                    input = input.Slice(4);
                    break;
                case [(byte)'f', (byte)'i', (byte)'v', (byte)'e', ..]:
                    lastDigit = 5;
                    input = input.Slice(3); // e could be start of eight
                    break;
                case [(byte)'s', (byte)'i', (byte)'x', ..]:
                    lastDigit = 6;
                    input = input.Slice(3);
                    break;
                case [(byte)'s', (byte)'e', (byte)'v', (byte)'e', (byte)'n', ..]:
                    lastDigit = 7;
                    input = input.Slice(4); // n could be start of nine
                    break;
                case [(byte)'e', (byte)'i', (byte)'g', (byte)'h', (byte)'t', ..]:
                    lastDigit = 8;
                    input = input.Slice(4); // t could be start of two or three
                    break;
                case [(byte)'n', (byte)'i', (byte)'n', (byte)'e', ..]:
                    lastDigit = 9;
                    input = input.Slice(3); // e could be start of eight
                    break;
                default:
                    input = input.Slice(1);
                    break;
            }

            if (firstDigit == 0)
                firstDigit = lastDigit;

            if (firstNumericalDigit == 0)
                firstNumericalDigit = lastNumericalDigit;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}

