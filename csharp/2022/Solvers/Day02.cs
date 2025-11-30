using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;
        for (var i = 0; i < input.Length; i += "A Z\n".Length)
        {
            var l = input[i];
            var r = input[i + 2];

            part1 += r - 'W'; // score for choice
            part1 += 3 * ((r - l + 2) % 3); // score for outcome

            part2 += (l + r + 2) % 3 + 1; // score for choice
            part2 += 3 * (r - 'X'); // score for outcome
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
