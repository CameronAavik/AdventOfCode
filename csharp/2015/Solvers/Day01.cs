using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var currentFloor = 0;
        var firstBasementFloor = 0;
        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == '(')
            {
                currentFloor++;
            }
            else
            {
                currentFloor--;
                if (currentFloor == -1 && firstBasementFloor == 0)
                {
                    firstBasementFloor = i + 1;
                }
            }
        }

        solution.SubmitPart1(currentFloor);
        solution.SubmitPart2(firstBasementFloor);
    }
}
