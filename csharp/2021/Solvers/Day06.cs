using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day06 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        Span<long> counts = stackalloc long[9];
        counts.Clear();

        int cursor = 0;
        while (cursor < input.Length)
        {
            counts[input[cursor] - '0']++;
            cursor += 2;
        }

        Iterate(counts, 80);

        long part1 = 0;
        foreach (long count in counts)
            part1 += count;

        solution.SubmitPart1(part1);

        Iterate(counts, 256 - 80);

        long part2 = 0;
        foreach (long count in counts)
            part2 += count;

        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Iterate(Span<long> counts, int n)
    {
        int i = 0;
        while (i + 7 < n)
        {
            long sevens = counts[7];
            counts[7] = counts[5];
            counts[5] += counts[3];
            counts[3] += counts[1];
            counts[1] += counts[8];
            counts[8] = counts[6];
            counts[6] += counts[4];
            counts[4] += counts[2];
            counts[2] += counts[0];
            counts[0] += sevens;

            i += 7;
        }

        while (i < n)
        {
            long zeroes = counts[0];
            for (int j = 0; j < 8; j++)
                counts[j] = counts[j + 1];

            counts[6] += zeroes;
            counts[8] = zeroes;

            i++;
        }
    }
}
