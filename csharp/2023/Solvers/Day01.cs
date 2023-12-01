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

        while (input.Length >= 1) // last character always newline
        {
            byte c = input[0];
            input = input.Slice(1);
            switch (c)
            {
                case (byte)'\n':
                    part1 += 10 * firstNumericalDigit + lastNumericalDigit;
                    part2 += 10 * firstDigit + lastDigit;
                    firstNumericalDigit = 0;
                    lastNumericalDigit = 0;
                    firstDigit = 0;
                    lastDigit = 0;
                    break;
                case >= (byte)'1' and <= (byte)'9':
                    lastNumericalDigit = c - '0';
                    lastDigit = lastNumericalDigit;
                    break;
                case (byte)'o':
                    if (input.StartsWith("ne"u8))
                    {
                        lastDigit = 1;
                        input = input.Slice(1); // e could be start of eight
                    }
                    break;
                case (byte)'t':
                    if (input.StartsWith("wo"u8))
                    {
                        lastDigit = 2;
                        input = input.Slice(1); // o could be start of one
                    }
                    else if (input.StartsWith("hree"u8))
                    {
                        lastDigit = 3;
                        input = input.Slice(3); // e could be start of eight
                    }
                    break;
                case (byte)'f':
                    if (input.StartsWith("our"u8))
                    {
                        lastDigit = 4;
                        input = input.Slice(3);
                    }
                    else if (input.StartsWith("ive"u8))
                    {
                        lastDigit = 5;
                        input = input.Slice(2); // e could be start of eight
                    }
                    break;
                case (byte)'s':
                    if (input.StartsWith("ix"u8))
                    {
                        lastDigit = 6;
                        input = input.Slice(2);
                    }
                    else if (input.StartsWith("even"u8))
                    {
                        lastDigit = 7;
                        input = input.Slice(3); // n could be start of nine
                    }
                    break;
                case (byte)'e':
                    if (input.StartsWith("ight"u8))
                    {
                        lastDigit = 8;
                        input = input.Slice(3); // t could be start of two or three
                    }
                    break;
                case (byte)'n':
                    if (input.StartsWith("ine"u8))
                    {
                        lastDigit = 9;
                        input = input.Slice(2); // e could be start of eight
                    }
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

