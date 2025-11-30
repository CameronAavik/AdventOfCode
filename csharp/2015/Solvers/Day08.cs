using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day08 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            part1 += GetPart1Length(line);
            part2 += GetPart2Length(line);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int GetPart1Length(ReadOnlySpan<byte> line)
    {
        var charCount = 0;

        var i = 1;
        while (i < line.Length - 1)
        {
            charCount++;
            if (line[i] == '\\')
            {
                switch (line[i + 1])
                {
                    case (byte)'\\':
                    case (byte)'\"':
                        i += 2;
                        break;
                    case (byte)'x':
                        i += 4;
                        break;
                    default:
                        i++;
                        break;
                }
            }
            else
            {
                i++;
            }
        }

        return line.Length - charCount;
    }

    private static int GetPart2Length(ReadOnlySpan<byte> line)
    {
        var charCount = 2 + line.Length;
        foreach (var c in line)
        {
            if (c is (byte)'\\' or (byte)'\"')
            {
                charCount++;
            }
        }

        return charCount - line.Length;
    }
}
