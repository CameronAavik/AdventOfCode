using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day06 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var counts = new int[width + 1, 26];

        var i = 0;
        while (i < input.Length)
        {
            for (var c = 0; c < width; c++)
            {
                var letter = input[i++] - 'a';
                counts[c, letter]++;
            }

            i++; // skip newline character
        }

        var part1 = new char[width];
        var part2 = new char[width];

        for (var c = 0; c < width; c++)
        {
            var minLetter = -1;
            var minCount = int.MaxValue;
            var maxLetter = -1;
            var maxCount = 0;

            for (var letter = 0; letter < 26; letter++)
            {
                var count = counts[c, letter];
                if (count > 0)
                {
                    if (count < minCount)
                    {
                        minCount = count;
                        minLetter = letter;
                    }

                    if (count > maxCount)
                    {
                        maxCount = count;
                        maxLetter = letter;
                    }
                }
            }

            part1[c] = (char)('a' + maxLetter);
            part2[c] = (char)('a' + minLetter);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
