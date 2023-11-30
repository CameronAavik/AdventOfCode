using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day20 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var numbers = new List<int>();
        int inputCursor = 0;
        while (inputCursor < input.Length)
            numbers.Add(ReadIntegerFromInput(input, ref inputCursor));

        solution.SubmitPart1(Solve(numbers, 1, 1));
        solution.SubmitPart2(Solve(numbers, 10, 811589153));
    }

    private static long Solve(List<int> numbers, int timesToMix, long key)
    {
        int len = numbers.Count;
        int mod = len - 1;

        short[] numberOrder = new short[len];
        short[] offsets = new short[len];
        for (int i = 0; i < len; i++)
        {
            numberOrder[i] = (short)i;
            offsets[i] = (short)(((numbers[i] * key % mod) + mod) % mod);
        }

        for (int mixes = 0; mixes < timesToMix; mixes++)
        {
            for (short i = 0; i < len; i++)
            {
                int currentPosition = Array.IndexOf(numberOrder, i, 0, len);
                int newPosition = offsets[i] + currentPosition;

                if (newPosition >= mod)
                {
                    newPosition -= mod;
                    Array.Copy(numberOrder, newPosition, numberOrder, newPosition + 1, currentPosition - newPosition);
                }
                else
                {
                    Array.Copy(numberOrder, currentPosition + 1, numberOrder, currentPosition, newPosition - currentPosition);
                }

                numberOrder[newPosition] = i;
            }
        }

        long total = 0;
        int zeroNodeOrder = Array.IndexOf(numberOrder, (short)numbers.IndexOf(0));
        for (int offset = 1000; offset <= 3000; offset += 1000)
            total += numbers[numberOrder[(zeroNodeOrder + offset) % len]];

        return total * key;
    }

    public static int ReadIntegerFromInput(ReadOnlySpan<byte> span, ref int i)
    {
        // Assume that the first character is always a digit
        byte c = span[i++];

        int mul;
        int ret;
        if (c == '-')
        {
            mul = -1;
            ret = 0;
        }
        else
        {
            mul = 1;
            ret = c - '0';
        }

        byte cur;
        while ((cur = span[i++]) != '\n')
            ret = ret * 10 + (cur - '0');

        return mul * ret;
    }
}
