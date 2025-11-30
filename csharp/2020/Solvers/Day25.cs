using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day25 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reader = new SpanReader(input);
        var key1 = reader.ReadPosIntUntil('\n');
        var key2 = reader.ReadPosIntUntil('\n');

        var n = 1;
        var loopSize = 0;
        while (n != key1)
        {
            n = (7 * n) % 20201227;
            loopSize++;
        }

        var part1 = BigInteger.ModPow(key2, loopSize, 20201227);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(string.Empty);
    }
}
