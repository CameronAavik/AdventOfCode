using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day20 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var numbers = new List<int>();
        var inputCursor = 0;
        while (inputCursor < input.Length)
            numbers.Add(ReadIntegerFromInput(input, ref inputCursor));

        solution.SubmitPart1(Solve(numbers, 1, 1));
        solution.SubmitPart2(Solve(numbers, 10, 811589153));
    }

    private static long Solve(List<int> numbers, int timesToMix, long key)
    {
        var len = numbers.Count;
        var mod = len - 1;

        var numberOrder = new short[len];
        var offsets = new short[len];
        for (var i = 0; i < len; i++)
        {
            numberOrder[i] = (short)i;
            offsets[i] = (short)(((numbers[i] * key % mod) + mod) % mod);
        }

        for (var mixes = 0; mixes < timesToMix; mixes++)
        {
            for (short i = 0; i < len; i++)
            {
                var currentPosition = Array.IndexOf(numberOrder, i, 0, len);
                var newPosition = offsets[i] + currentPosition;

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
        var zeroNodeOrder = Array.IndexOf(numberOrder, (short)numbers.IndexOf(0));
        for (var offset = 1000; offset <= 3000; offset += 1000)
            total += numbers[numberOrder[(zeroNodeOrder + offset) % len]];

        return total * key;
    }

    public static int ReadIntegerFromInput(ReadOnlySpan<byte> span, ref int i)
    {
        // Assume that the first character is always a digit
        var c = span[i++];

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
