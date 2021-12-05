using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2019.Solvers;

public class Day01 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;
        foreach (ReadOnlySpan<char> massStr in input.SplitLines())
        {
            int mass = int.Parse(massStr);
            int fuel = mass / 3 - 2;
            part1 += fuel;
            while (fuel > 0)
            {
                part2 += fuel;
                fuel = fuel / 3 - 2;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
