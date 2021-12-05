using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day15 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // really big array since much faster than a dictionary
        int[] buffer = new int[30000000];

        int i = 1;
        foreach (ReadOnlySpan<char> num in input.Split(','))
        {
            buffer[int.Parse(num)] = i++;
        }

        int cur = 0;
        while (i < 2020)
        {
            int prev_t = buffer[cur];
            int value = prev_t == 0 ? 0 : i - prev_t;
            buffer[cur] = i++;
            cur = value;
        }

        int minZero = 0;
        while (buffer[minZero] != 0)
            minZero++;

        int part1 = cur;

        // we use step to control how frequently we recalculate the smallest seen zero
        const int step = 2048;
        for (; i + step < 30000000; i += step)
        {
            for (int j = i; j < i + step; j++)
            {
                int prev = buffer[cur];
                buffer[cur] = j;

                // while comparing against minZero might seem redundant, it makes the branch much more predictable and saves a lot of time
                if (cur < minZero || prev != 0)
                {
                    cur = j - prev;
                }
                else
                {
                    cur = 0;
                }
            }

            while (buffer[minZero] != 0)
                minZero++;
        }

        // since 30000000 is not always divisible by the step, we use one last loop to get to the end
        for (; i < 30000000; i++)
        {
            int prev_t = buffer[cur];
            int value = prev_t == 0 ? 0 : i - prev_t;
            buffer[cur] = i;
            cur = value;
        }

        int part2 = cur;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
