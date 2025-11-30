using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day09 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lines = input.Count((byte)'\n');
        var nums = new long[lines];
        var line = 0;

        var reader = new SpanReader(input);
        while (line < 25)
        {
            nums[line++] = reader.ReadPosLongUntil('\n');
        }

        while (!reader.Done)
        {
            var num = reader.ReadPosLongUntil('\n');
            nums[line] = num;
            if (!IsSumInPrevious25(nums, line, num))
            {
                break;
            }

            line++;
        }

        var part1 = nums[line];
        var part2 = SolvePart2(nums, part1);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSumInPrevious25(long[] nums, int line, long num)
    {
        for (var i = line - 25; i <= line - 2; i++)
        {
            var target = num - nums[i];
            for (var j = i + 1; j <= line - 1; j++)
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
        var l = 0;
        var r = 0;
        var sum = nums[0];
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

        var min = nums[l];
        var max = nums[l];
        for (var i = l + 1; i <= r; i++)
        {
            var v = nums[i];
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
