using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day07 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var startIndex = input.IndexOf((byte)'S');
        var width = input[(startIndex + 1)..].IndexOf((byte)'\n') + startIndex + 1;

        var counts = new long[width];
        counts[startIndex] = 1;

        var part1 = 0;

        var minCol = startIndex;
        var maxCol = startIndex;
        for (var rowIndex = 0; rowIndex < input.Length - 1; rowIndex += width + 1)
        {
            var row = input[rowIndex..(rowIndex + maxCol + 1)];

            var col = minCol + 1;
            var prevTachyon = -1;

            if (row[minCol] == (byte)'^')
            {
                var prev = counts[minCol];
                if (prev > 0)
                {
                    part1++;
                    counts[minCol - 1] += prev;
                    counts[minCol] = 0;
                    counts[minCol + 1] += prev;
                }

                col = minCol + 2;
                prevTachyon = minCol;
                minCol--;
            }

            while (col < row.Length)
            {
                var nextTachyon = row[col..].IndexOf((byte)'^');
                if (nextTachyon == -1)
                    break;

                col += nextTachyon;

                var prev = counts[col];
                if (prev > 0)
                {
                    part1++;
                    counts[col - 1] += prev;
                    counts[col] = 0;
                    counts[col + 1] += prev;
                }

                prevTachyon = col;
                col += 2;
            }

            if (prevTachyon == maxCol)
                maxCol++;
        }

        long part2 = 0;
        foreach (var count in counts)
            part2 += count;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
