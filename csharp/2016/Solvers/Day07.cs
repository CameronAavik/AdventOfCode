using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day07 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1 = 0;
            int part2 = 0;

            byte[] abas = new byte[26 * 26];
            byte[] babs = new byte[26 * 26];
            foreach (ReadOnlySpan<char> line in input.SplitLines())
            {
                Array.Clear(abas, 0, abas.Length);
                Array.Clear(babs, 0, babs.Length);

                bool hasAbbaOutsideHypernet = false;
                bool hasAbbaInsideHypernet = false;
                bool supportsSSL = false;

                // c1, c2, c3, c4 defines a sliding window of 4 characters across the line
                char c1 = line[0], c2 = line[1], c3 = line[2];
                bool insideHypernet = false;
                for (int i = 3; i <= line.Length; i++)
                {
                    char c4 = i == line.Length ? '\0' : line[i];
                    if (c1 == '[')
                    {
                        insideHypernet = true;
                    }
                    else if (c1 == ']')
                    {
                        insideHypernet = false;
                    }
                    else
                    {
                        if (c1 == c4 && c2 == c3 && c2 != '[' && c1 != c2) // found an ABBA
                        {
                            if (insideHypernet)
                            {
                                hasAbbaInsideHypernet = true;
                            }
                            else
                            {
                                hasAbbaOutsideHypernet = true;
                            }
                        }

                        if (c1 == c3 && c2 != c1 && c2 != '[' && c2 != ']')
                        {
                            if (insideHypernet)
                            {
                                babs[(c1 - 'a') * 26 + (c2 - 'a')] = 1;
                                if (abas[(c2 - 'a') * 26 + (c1 - 'a')] == 1)
                                {
                                    supportsSSL = true;
                                }
                            }
                            else
                            {
                                abas[(c1 - 'a') * 26 + (c2 - 'a')] = 1;
                                if (babs[(c2 - 'a') * 26 + (c1 - 'a')] == 1)
                                {
                                    supportsSSL = true;
                                }
                            }
                        }
                    }

                    // slide the window down
                    c1 = c2;
                    c2 = c3;
                    c3 = c4;
                }

                if (hasAbbaOutsideHypernet && !hasAbbaInsideHypernet)
                {
                    part1 += 1;
                }

                if (supportsSSL)
                {
                    part2 += 1;
                }
            }

            return new Solution(part1, part2);
        }
    }
}
