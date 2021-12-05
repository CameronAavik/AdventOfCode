using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day25 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        var reader = new SpanReader(input);
        reader.SkipLength("To continue, please consult the code grid in the manual.  Enter the code at row ".Length);
        int row = reader.ReadPosIntUntil(',');
        reader.SkipLength(" column ".Length);
        int column = reader.ReadPosIntUntil('.');

        int n = row + column - 1;
        int diagEnd = n * (n + 1) / 2;
        int repetitions = diagEnd - row;

        BigInteger part1 = (BigInteger.ModPow(252533, repetitions, 33554393) * 20151125) % 33554393;
        solution.SubmitPart1(part1);
        solution.SubmitPart2(string.Empty);
    }
}
