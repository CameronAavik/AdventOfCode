using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day06 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lineLength = input.IndexOf((byte)'\n') + 1;

        long part1 = 0;
        long part2 = 0;

        var operatorIndex = input.Length - lineLength;
        var col = 0;
        while (col < lineLength - 1)
        {
            var op = input[operatorIndex]; // '+' or '*'
            var width = input[(operatorIndex + 1)..].IndexOfAnyExcept((byte)' ');

            // handle last column not having a space separator
            if (col + width == lineLength - 2)
                width++;

            var isAddition = op == (byte)'+';
            var opBase = isAddition ? 0 : 1;

            long part1Val = opBase;
            for (var rowStartIndex = col; rowStartIndex < operatorIndex; rowStartIndex += lineLength)
            {
                var operand = 0;
                for (var i = rowStartIndex; i < rowStartIndex + width; i++)
                {
                    var c = input[i];
                    if (c != ' ')
                        operand = operand * 10 + (c - (byte)'0');
                }

                part1Val = isAddition ? part1Val + operand : part1Val * operand;
            }

            part1 += part1Val;

            long part2Val = opBase;
            for (var colStartIndex = col; colStartIndex < col + width; colStartIndex++)
            {
                var operand = 0;
                for (var i = colStartIndex; i < operatorIndex; i += lineLength)
                {
                    var c = input[i];
                    if (c != ' ')
                        operand = operand * 10 + (c - (byte)'0');
                }

                part2Val = isAddition ? part2Val + operand : part2Val * operand;
            }

            part2 += part2Val;
            col += width + 1;
            operatorIndex += width + 1;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
