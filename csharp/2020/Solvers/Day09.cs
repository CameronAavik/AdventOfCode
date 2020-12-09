using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day09 : ISolver
    {
        private static readonly HashSet<long> s_last25Cache = new HashSet<long>(25);

        public Solution Solve(ReadOnlySpan<char> input)
        {
            int lines = input.Count('\n');
            long[] nums = new long[lines];
            int line = 0;

            var reader = new SpanReader(input);
            while (line < 25)
            {
                long num = reader.ReadPosLongUntil('\n');
                nums[line++] = num;
            }

            while (!reader.Done)
            {
                long num = reader.ReadPosLongUntil('\n');
                nums[line] = num;
                if (!IsSumInPrevious25(num, line, nums))
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
        private static bool IsSumInPrevious25(long num, int line, long[] nums)
        {
            s_last25Cache.Clear();
            for (int i = line - 25; i <= line - i; i++)
            {
                long v = nums[i];
                if (s_last25Cache.Contains(num - v))
                {
                    return true;
                }

                s_last25Cache.Add(v);
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
