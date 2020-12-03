using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day02 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var part1 = 0;
            var part2 = 0;

            foreach (var line in input.Split('\n'))
            {
                int i = 0;
                int left = ParsePosInt(line, until: '-', ref i);
                int right = ParsePosInt(line, until: ' ', ref i);
                char letter = line[i];
                var password = line.Slice(i + 3);

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

            return new Solution(part1, part2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int ParsePosInt(ReadOnlySpan<char> str, char until, ref int i)
        {
            char c = str[i++];
            int num = c - '0';
            while ((c = str[i++]) != until)
            {
                num = num * 10 + (c - '0');
            }

            return num;
        }
    }
}
