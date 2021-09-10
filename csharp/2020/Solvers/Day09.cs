using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day09 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int lines = input.Count('\n');
            long[] nums = new long[lines];
            int line = 0;

            var reader = new SpanReader(input);
            while (line < 25)
            {
                nums[line++] = reader.ReadPosLongUntil('\n');
            }

            while (!reader.Done)
            {
                long num = reader.ReadPosLongUntil('\n');
                nums[line] = num;
                if (!IsSumInPrevious25(nums, line, num))
                {
                    break;
                }

                line++;
            }

            long part1 = nums[line];
            long part2 = SolvePart2(nums, part1);
            return new Solution(part1.ToString(), part2.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSumInPrevious25(long[] nums, int line, long num)
        {
            for (int i = line - 25; i <= line - 2; i++)
            {
                long target = num - nums[i];
                for (int j = i + 1; j <= line - 1; j++)
                {
                    if (nums[j] == target)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long SolvePart2(long[] nums, long part1)
        {
            int l = 0;
            int r = 0;
            long sum = nums[0];
            while (sum != part1)
            {
                if (sum < part1)
                {
                    sum += nums[++r];
                }
                else
                {
                    sum -= nums[l++];
                }
            }

            long min = nums[l];
            long max = nums[l];
            for (int i = l + 1; i <= r; i++)
            {
                long v = nums[i];
                if (v < min)
                {
                    min = v;
                }
                else if (v > max)
                {
                    max = v;
                }
            }

            return min + max;
        }
    }
}
