using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day08 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        var cursor = 0;
        while (cursor < input.Length)
        {
            byte oneDigitMask = 0;
            byte fourDigitMask = 0;

            for (int i = 0; i < 10; i++)
            {
                byte digit = ReadDigitAsMask(input, ' ', ref cursor, out int bits);
                switch (bits)
                {
                    case 2:
                        oneDigitMask = digit;
                        break;
                    case 4:
                        fourDigitMask = digit;
                        break;
                }
            }

            // skip the "| "
            cursor += 2;

            byte bdMask = (byte)(oneDigitMask ^ fourDigitMask);

            byte digit1 = ReadDigitAsMask(input, ' ', ref cursor, out int bits1);
            byte digit2 = ReadDigitAsMask(input, ' ', ref cursor, out int bits2);
            byte digit3 = ReadDigitAsMask(input, ' ', ref cursor, out int bits3);
            byte digit4 = ReadDigitAsMask(input, '\n', ref cursor, out int bits4);

            part2 +=
                CalculateDigit(digit1, bits1, oneDigitMask, bdMask, ref part1) * 1000 +
                CalculateDigit(digit2, bits2, oneDigitMask, bdMask, ref part1) * 100 +
                CalculateDigit(digit3, bits3, oneDigitMask, bdMask, ref part1) * 10 +
                CalculateDigit(digit4, bits4, oneDigitMask, bdMask, ref part1);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateDigit(int digitMask, int bits, byte oneDigitMask, byte bdMask, ref int part1)
    {
        switch (bits)
        {
            case 2:
                part1++;
                return 1;
            case 3:
                part1++;
                return 7;
            case 4:
                part1++;
                return 4;
            case 5:
                if ((digitMask & oneDigitMask) == oneDigitMask)
                    return 3;
                else if ((digitMask & bdMask) == bdMask)
                    return 5;
                else
                    return 2;
            case 6:
                if ((digitMask & oneDigitMask) != oneDigitMask)
                    return 6;
                else if ((digitMask & bdMask) != bdMask)
                    return 0;
                else
                    return 9;
            case 7:
                part1++;
                return 8;
            default:
                return 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ReadDigitAsMask(ReadOnlySpan<byte> input, char until, ref int cursor, out int bits)
    {
        int digit = 1 << (input[cursor++] - 'a');
        bits = 1;

        byte c;
        while ((c = input[cursor++]) != until)
        {
            digit |= 1 << (c - 'a');
            bits++;
        }

        return (byte)digit;
    }
}
