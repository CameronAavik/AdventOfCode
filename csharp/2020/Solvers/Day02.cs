using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        foreach (ReadOnlySpan<byte> line in input.SplitLines())
        {
            int i = 0;
            int left = ParsePosInt(line, until: '-', ref i);
            int right = ParsePosInt(line, until: ' ', ref i);
            byte letter = line[i];
            ReadOnlySpan<byte> password = line.Slice(i + 3);

            int letterCount = password.Count(letter);
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
        byte c = str[i++];
        int num = c - '0';
        while ((c = str[i++]) != until)
        {
            num = num * 10 + (c - '0');
        }

        return num;
    }
}
