using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day16 : ISolver
    {
        public void Solve(ReadOnlySpan<char> input, Solution solution)
        {
            var items = new Dictionary<string, int>
            {
                ["children"] = 3,
                ["cats"] = 7,
                ["samoyeds"] = 2,
                ["pomeranians"] = 3,
                ["akitas"] = 0,
                ["vizslas"] = 0,
                ["goldfish"] = 5,
                ["trees"] = 3,
                ["cars"] = 2,
                ["perfumes"] = 1
            };

            int sue1 = 0;
            int sue2 = 0;

            int sue = 1;
            foreach (ReadOnlySpan<char> line in input.SplitLines())
            {
                int tokenIndex = 0;
                string itemName = string.Empty;
                bool canBePart1 = true;
                bool canBePart2 = true;
                foreach (ReadOnlySpan<char> token in line.Split(' '))
                {
                    if (tokenIndex % 2 == 0 && tokenIndex > 0)
                    {
                        itemName = token[..^1].ToString();
                    }
                    else if (tokenIndex % 2 == 1 && tokenIndex > 1)
                    {
                        if (items.TryGetValue(itemName, out int expectedCount))
                        {
                            int count = int.Parse(token.TrimEnd(','));

                            bool isValidPart1 = count == expectedCount;
                            bool isValidPart2 = itemName switch
                            {
                                "cats" or "trees" => count > expectedCount,
                                "pomeranians" or "goldfish" => count < expectedCount,
                                _ => count == expectedCount,
                            };

                            canBePart1 = canBePart1 && isValidPart1;
                            canBePart2 = canBePart2 && isValidPart2;
                        }
                    }

                    tokenIndex++;
                }

                if (canBePart1)
                {
                    sue1 = sue;
                }

                if (canBePart2)
                {
                    sue2 = sue;
                }

                sue++;
            }

            solution.SubmitPart1(sue1);
            solution.SubmitPart2(sue2);
        }
    }
}
