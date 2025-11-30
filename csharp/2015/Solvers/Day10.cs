using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.TrimEnd((byte)'\n');
        var currentSequence = new byte[input.Length];
        var curLength = input.Length;
        for (var i = 0; i < input.Length; i++)
        {
            currentSequence[i] = (byte)(input[i] - '0');
        }

        var part1 = 0;
        for (var i = 0; i < 50; i++)
        {
            if (i == 40)
            {
                part1 = curLength;
            }

            // maximum size
            var nextSequence = new byte[curLength * 2];
            var nextLength = 0;

            var curIndex = 0;
            while (curIndex < curLength)
            {
                var curVal = currentSequence[curIndex];
                byte amount = 1;
                for (var j = 1; j <= 2; j++)
                {
                    if (curIndex + j >= curLength || currentSequence[curIndex + j] != curVal)
                    {
                        break;
                    }

                    amount++;
                }

                nextSequence[nextLength++] = amount;
                nextSequence[nextLength++] = curVal;
                curIndex += amount;
            }

            currentSequence = nextSequence;
            curLength = nextLength;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(curLength);
    }
}
