using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var len = input.Count((byte)'\n');
        var nums = new int[len];

        var reader = new SpanReader(input);
        for (var i = 0; i < len; i++)
        {
            nums[i] = reader.ReadPosIntUntil('\n');
        }

        Array.Sort(nums);

        var oneDiffs = 0;
        var threeDiffs = 1; // there will always be a diff of three at the end, so start at 1

        long ways0 = 0;
        long ways1 = 0;
        long ways2 = 1;

        var prev0 = int.MinValue;
        var prev1 = int.MinValue;
        var prev2 = 0;

        foreach (var num in nums)
        {
            // part 1
            var diff = num - prev2;
            if (diff == 1)
            {
                oneDiffs++;
            }
            else if (diff == 3)
            {
                threeDiffs++;
            }

            // part 2
            var ways = ways2;
            if (num - prev1 <= 3)
            {
                ways += ways1;
                if (num - prev0 <= 3)
                {
                    ways += ways0;
                }
            }

            prev0 = prev1;
            prev1 = prev2;
            prev2 = num;

            ways0 = ways1;
            ways1 = ways2;
            ways2 = ways;
        }

        var part1 = oneDiffs * threeDiffs;
        var part2 = ways2;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
