using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day09 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.TrimEnd((byte)'\n');
        int part1 = SolvePart1(input);
        long part2 = SolvePart2(input);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int SolvePart1(ReadOnlySpan<byte> input)
    {
        int length = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            if (reader.Read() == '(')
            {
                int repLength = reader.ReadPosIntUntil('x');
                int repCount = reader.ReadPosIntUntil(')');

                length += repLength * repCount;
                reader.SkipLength(repLength);
            }
            else
            {
                length += 1;
            }
        }

        return length;
    }

    private static long SolvePart2(ReadOnlySpan<byte> input)
    {
        long length = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            if (reader.Read() == '(')
            {
                int repLength = reader.ReadPosIntUntil('x');
                int repCount = reader.ReadPosIntUntil(')');
                ReadOnlySpan<byte> rep = reader.ReadBytes(repLength);
                length += SolvePart2(rep) * repCount;
            }
            else
            {
                length += 1;
            }
        }

        return length;
    }
}
