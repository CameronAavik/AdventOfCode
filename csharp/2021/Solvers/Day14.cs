using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day14 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // Stores the counts of each pair in the polymer.
        // Pairs are represented by (pair[0] - 'A') * 26 + (pair[1] - 'A');
        Span<long> pairCounts = stackalloc long[26 * 26];

        // Parse the polymer template
        int startingElement = input[0] - 'A';
        int prevElement = startingElement;
        int i = 1;

        char c;
        while ((c = input[i++]) != '\n')
        {
            int element = c - 'A';
            pairCounts[prevElement * 26 + element]++;
            prevElement = element;
        }

        // Given the pair as the key, returns the two pairs that get created as a result
        // The two pairs appear in the top and bottom 16 bits of the integer value
        Span<int> lookups = stackalloc int[26 * 26];

        // Records a list of all the pair that can appear
        Span<int> possiblePairs = stackalloc int[26 * 26];

        // Parse the production rules
        int numPossiblePairs = 0;
        i++;
        while (i < input.Length)
        {
            int element1 = input[i] - 'A';
            int element2 = input[i + 1] - 'A';
            int elementResult = input[i + 6] - 'A';

            int newPair1 = element1 * 26 + elementResult;
            int newPair2 = elementResult * 26 + element2;

            int pairLookup = element1 * 26 + element2;
            lookups[pairLookup] = newPair1 << 16 | newPair2;
            possiblePairs[numPossiblePairs++] = pairLookup;

            i += 8;
        }

        possiblePairs = possiblePairs.Slice(0, numPossiblePairs);

        for (int step = 0; step < 10; step++)
            Iterate(pairCounts, lookups, possiblePairs);

        solution.SubmitPart1(GetAnswer(possiblePairs, pairCounts, startingElement));

        for (int step = 10; step < 40; step++)
            Iterate(pairCounts, lookups, possiblePairs);

        solution.SubmitPart2(GetAnswer(possiblePairs, pairCounts, startingElement));
    }

    private static void Iterate(Span<long> pairCounts, Span<int> lookups, Span<int> possiblePairs)
    {
        Span<long> newPairCounts = stackalloc long[26 * 26];

        foreach (int pair in possiblePairs)
        {
            long count = pairCounts[pair];
            int newPairs = lookups[pair];
            newPairCounts[newPairs >> 16] += count;
            newPairCounts[newPairs & ushort.MaxValue] += count;
        }

        newPairCounts.CopyTo(pairCounts);
    }

    private static long GetAnswer(Span<int> possiblePairs, Span<long> pairCounts, int startingElement)
    {
        Span<long> elementCounts = stackalloc long[26];

        foreach (int pair in possiblePairs)
        {
            long count = pairCounts[pair];
            elementCounts[pair % 26] += count;
        }

        elementCounts[startingElement]++;

        long max = 0;
        long min = long.MaxValue;
        foreach (long count in elementCounts)
        {
            if (count > max)
                max = count;
            else if (count < min && count > 0)
                min = count;
        }

        return max - min;
    }
}
