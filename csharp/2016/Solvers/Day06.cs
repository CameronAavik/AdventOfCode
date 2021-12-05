using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day06 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int width = input.IndexOf('\n');
        int[,] counts = new int[width + 1, 26];

        int i = 0;
        while (i < input.Length)
        {
            for (int c = 0; c < width; c++)
            {
                int letter = input[i++] - 'a';
                counts[c, letter]++;
            }

            i++; // skip newline character
        }

        char[] part1 = new char[width];
        char[] part2 = new char[width];

        for (int c = 0; c < width; c++)
        {
            int minLetter = -1;
            int minCount = int.MaxValue;
            int maxLetter = -1;
            int maxCount = 0;

            for (int letter = 0; letter < 26; letter++)
            {
                int count = counts[c, letter];
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
