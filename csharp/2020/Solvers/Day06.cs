using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day06 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1 = 0;
            int part2 = 0;

            uint personAnswers = 0;
            uint groupAnswers = 0;
            uint groupAllAnswers = uint.MaxValue;
            foreach (char c in input)
            {
                if (c == '\n')
                {
                    if (personAnswers == 0)
                    {
                        part1 += BitOperations.PopCount(groupAnswers);
                        part2 += BitOperations.PopCount(groupAllAnswers);

                        groupAnswers = 0;
                        groupAllAnswers = uint.MaxValue;
                    }
                    else
                    {
                        groupAnswers |= personAnswers;
                        groupAllAnswers &= personAnswers;
                        personAnswers = 0;
                    }
                }
                else
                {
                    personAnswers |= 1u << (c - 'a');
                }
            }

            // process the last group
            part1 += BitOperations.PopCount(groupAnswers | personAnswers);
            part2 += BitOperations.PopCount(groupAllAnswers & personAnswers);

            return new Solution(part1, part2);
        }
    }
}
