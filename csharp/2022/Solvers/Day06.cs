using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day06 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int numUniquePart1 = 0;
        int numUniquePart2 = 0;
        Span<byte> countsPart1 = stackalloc byte[26];
        Span<byte> countsPart2 = stackalloc byte[26];

        bool foundPart1 = false;

        int i;
        for (i = 0; i < 4; i++)
        {
            byte c = input[i];
            AddCharacter(c, ref countsPart1, ref numUniquePart1);
            AddCharacter(c, ref countsPart2, ref numUniquePart2);
        }

        // Handle the case that the first 4 characters of the input are distinct
        if (numUniquePart1 == 4)
        {
            solution.SubmitPart1(3);
            foundPart1 = true;
        }

        for (i = 4; i < 14; i++)
        {
            RemoveCharacter(input[i - 4], ref countsPart1, ref numUniquePart1);

            byte c = input[i];
            AddCharacter(c, ref countsPart1, ref numUniquePart1);
            AddCharacter(c, ref countsPart2, ref numUniquePart2);

            if (!foundPart1 && numUniquePart1 == 4)
            {
                solution.SubmitPart1(i + 1);
                foundPart1 = true;
            }
        }

        // Search for part 1 solution first (part 1 solution will always occur before part 2 solution)
        if (!foundPart1)
        {
            for (i = 14; i < input.Length && numUniquePart1 < 4; i++)
            {
                RemoveCharacter(input[i - 4], ref countsPart1, ref numUniquePart1);
                RemoveCharacter(input[i - 14], ref countsPart2, ref numUniquePart2);

                byte c = input[i];
                AddCharacter(c, ref countsPart1, ref numUniquePart1);
                AddCharacter(c, ref countsPart2, ref numUniquePart2);
            }

            solution.SubmitPart1(i);
        }

        // Since part 1 solution was found, focus only on part 2
        for (; i < input.Length && numUniquePart2 < 14; i++)
        {
            RemoveCharacter(input[i - 14], ref countsPart2, ref numUniquePart2);
            AddCharacter(input[i], ref countsPart2, ref numUniquePart2);
        }

        solution.SubmitPart2(i);
    }

    private static void RemoveCharacter(byte c, ref Span<byte> counts, ref int uniqueCount)
    {
        if (--counts[c - 'a'] == 0)
            uniqueCount--;
    }

    private static void AddCharacter(byte c, ref Span<byte> counts, ref int uniqueCount)
    {
        if (++counts[c - 'a'] == 1)
            uniqueCount++;
    }
}
