using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // Rather than computing the "joltage" value on each line which would involve lots of multiplications and
        // additions, we can instead just sum the total of each digit position across all lines, and then compute the
        // final value at the end. In addition, we also don't subtract '0' from each digit as we read it to convert it
        // from ASCII to integer, instead we subtract it all at once at the end by multiplying by the number of lines.
        Span<int> part1DigitTotals = stackalloc int[2];
        Span<int> part2DigitTotals = stackalloc int[12];

        // This stack holds the current best 12 digits for part 2, but has an extra slot at the start which is set to
        // int.MaxValue to prevent the need for an additional bounds check when we're popping values off the stack.
        Span<int> part2Stack = stackalloc int[13];
        part2Stack[0] = int.MaxValue;

        var lines = 0;
        var inputPtr = 0;
        while (inputPtr < input.Length)
        {
            // Technically this could be hardcoded or we could assume it remains the same for all lines as this holds
            // true in all real inputs, but it doesn't really save that much time over just handling the general case.
            var lineLength = input[inputPtr..].IndexOf((byte)'\n');

            // Initialize the stack with the first digit of the line and the stack pointer at position 2 as position 0
            // contains a fake value and position 1 contains the digit we just pushed.
            part2Stack[1] = input[inputPtr];
            var stackPtr = 2;

            // We maintain a minimum stack pointer which starts out as negative and increases on each iteration. As we
            // approach the end of the line, this ensures we always end up with 12 digits.
            var minStackPtr = 14 - lineLength;
            for (var j = 1; j < lineLength; j++)
            {
                var c = input[inputPtr + j];
                while (stackPtr > minStackPtr && part2Stack[stackPtr - 1] < c)
                    stackPtr--;

                if (stackPtr < 13)
                    part2Stack[stackPtr++] = c;

                minStackPtr++;
            }

            inputPtr += lineLength + 1;

            // The solution for part 1 will be contained within the digits of the solution for part 2, so we calculate
            // it here instead of processing the whole line twice for each part.
            var part1Digit1 = 0;
            var part1Digit2 = 0;
            for (var j = 0; j < 12; j++)
            {
                var v = part2Stack[j + 1];
                part2DigitTotals[j] += v;

                if (j != 11 && v > part1Digit1)
                {
                    part1Digit1 = v;
                    part1Digit2 = 0;
                }
                else if (v > part1Digit2)
                {
                    part1Digit2 = v;
                }
            }

            part1DigitTotals[0] += part1Digit1;
            part1DigitTotals[1] += part1Digit2;
            lines++;
        }

        var offset = '0' * lines;
        var part1 = 10 * part1DigitTotals[0] + part1DigitTotals[1] - 11 * offset;

        long part2 = part2DigitTotals[0] - offset;
        for (var j = 1; j < 12; j++)
            part2 = 10 * part2 + part2DigitTotals[j] - offset;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
