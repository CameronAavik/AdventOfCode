using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day15 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // really big array since much faster than a dictionary
        var buffer = new int[30000000];

        var i = 1;
        var reader = new SpanReader(input.TrimEnd((byte)'\n'));
        while (!reader.Done)
            buffer[reader.ReadPosIntUntil(',')] = i++;

        var cur = 0;
        while (i < 2020)
        {
            var prev_t = buffer[cur];
            var value = prev_t == 0 ? 0 : i - prev_t;
            buffer[cur] = i++;
            cur = value;
        }

        var minZero = 0;
        while (buffer[minZero] != 0)
            minZero++;

        var part1 = cur;

        // we use step to control how frequently we recalculate the smallest seen zero
        const int step = 2048;
        for (; i + step < 30000000; i += step)
        {
            for (var j = i; j < i + step; j++)
            {
                var prev = buffer[cur];
                buffer[cur] = j;

                // while comparing against minZero might seem redundant, it makes the branch much more predictable and saves a lot of time
                cur = cur < minZero || prev != 0 ? j - prev : 0;
            }

            while (buffer[minZero] != 0)
                minZero++;
        }

        // since 30000000 is not always divisible by the step, we use one last loop to get to the end
        for (; i < 30000000; i++)
        {
            var prev_t = buffer[cur];
            var value = prev_t == 0 ? 0 : i - prev_t;
            buffer[cur] = i;
            cur = value;
        }

        var part2 = cur;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
