using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day08 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        var cursor = 0;
        while (cursor < input.Length)
        {
            byte oneDigitMask = 0;
            byte fourDigitMask = 0;

            for (var i = 0; i < 10; i++)
            {
                var digit = ReadDigitAsMask(input, ' ', ref cursor, out var bits);
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

            var bdMask = (byte)(oneDigitMask ^ fourDigitMask);

            var digit1 = ReadDigitAsMask(input, ' ', ref cursor, out var bits1);
            var digit2 = ReadDigitAsMask(input, ' ', ref cursor, out var bits2);
            var digit3 = ReadDigitAsMask(input, ' ', ref cursor, out var bits3);
            var digit4 = ReadDigitAsMask(input, '\n', ref cursor, out var bits4);

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
        var digit = 1 << (input[cursor++] - 'a');
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
