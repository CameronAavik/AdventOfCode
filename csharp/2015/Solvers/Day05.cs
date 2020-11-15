using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day05 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1 = 0;
            int part2 = 0;

            foreach (ReadOnlySpan<char> line in input.Split('\n'))
            {
                int numVowels = 0;
                bool containsPair = false;
                bool containsForbiddenPair = false;

                // bitset which keeps track of seen characters.
                // this is used to help with finding repeat pairs.
                int seenChars = 0;
                bool containsRepeatPair = false;
                bool containsPalindromicTriple = false;

                char prev2 = '\0';
                char prev = '\0';
                for (int i = 0; i < line.Length; i++)
                {
                    // Part 1 Rules
                    char cur = line[i];
                    if (cur is 'a' or 'e' or 'i' or 'o' or 'u')
                    {
                        numVowels++;
                    }

                    if (cur == prev)
                    {
                        containsPair = true;
                    }

                    if ((prev is 'a' or 'c' or 'p' or 'x') && cur == prev + 1)
                    {
                        containsForbiddenPair = true;
                    }

                    // Part 2 Rules

                    // check if we have seen the current and previous letter before
                    // if we have, scan the line for a repeat pair
                    int prevBitMask = 1 << (prev - 'a');
                    if ((seenChars & prevBitMask) > 0 && (seenChars & 1 << (cur - 'a')) > 0)
                    {
                        for (int j = 0; j < i - 2; j++)
                        {
                            if (line[j] == prev && line[j + 1] == cur)
                            {
                                containsRepeatPair = true;
                            }
                        }
                    }

                    if (prev2 == cur)
                    {
                        containsPalindromicTriple = true;
                    }

                    // we only add a letter to the seen characters two iterations after it first appears
                    // to avoid scanning the line for an overlap.
                    seenChars |= prevBitMask;
                    prev2 = prev;
                    prev = cur;
                }

                if (numVowels >= 3 && containsPair && !containsForbiddenPair)
                {
                    part1++;
                }

                if (containsRepeatPair && containsPalindromicTriple)
                {
                    part2++;
                }
            }

            return new Solution(part1, part2);
        }
    }
}
