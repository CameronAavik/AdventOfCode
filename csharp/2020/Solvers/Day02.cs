using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            var i = 0;
            var left = ParsePosInt(line, until: '-', ref i);
            var right = ParsePosInt(line, until: ' ', ref i);
            var letter = line[i];
            var password = line[(i + 3)..];

            var letterCount = password.Count(letter);
            if (left <= letterCount && letterCount <= right)
            {
                part1++;
            }

            if (password[left - 1] == letter ^ password[right - 1] == letter)
            {
                part2++;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int ParsePosInt(ReadOnlySpan<byte> str, char until, ref int i)
    {
        var c = str[i++];
        var num = c - '0';
        while ((c = str[i++]) != until)
        {
            num = num * 10 + (c - '0');
        }

        return num;
    }
}
