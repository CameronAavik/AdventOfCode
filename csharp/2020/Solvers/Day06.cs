using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day06 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        uint personAnswers = 0;
        uint groupAnswers = 0;
        var groupAllAnswers = uint.MaxValue;
        foreach (var c in input)
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
        part1 += BitOperations.PopCount(groupAnswers);
        part2 += BitOperations.PopCount(groupAllAnswers);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
