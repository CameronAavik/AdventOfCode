using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int inputCursor = 0;

        int elf1 = 0;
        int elf2 = 0;
        int elf3 = 0;
        while (inputCursor < input.Length)
        {
            int elfTotal = 0;
            while (inputCursor < input.Length && input[inputCursor] != '\n')
            {
                elfTotal += ReadLineAsInteger(input, ref inputCursor);
            }

            if (elfTotal > elf3)
            {
                if (elfTotal > elf2)
                {
                    elf3 = elf2;
                    if (elfTotal > elf1)
                    {
                        elf2 = elf1;
                        elf1 = elfTotal;
                    }
                    else
                    {
                        elf2 = elfTotal;
                    }
                }
                else
                {
                    elf3 = elfTotal;
                }
            }

            inputCursor++;
        }

        solution.SubmitPart1(elf1);
        solution.SubmitPart2(elf1 + elf2 + elf3);
    }

    private static int ReadLineAsInteger(ReadOnlySpan<byte> input, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = input[i++] - '0';

        byte cur;
        while ((cur = input[i++]) != '\n')
            ret = ret * 10 + cur - '0';

        return ret;
    }
}
