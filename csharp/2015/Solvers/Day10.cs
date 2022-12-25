using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.TrimEnd((byte)'\n');
        byte[] currentSequence = new byte[input.Length];
        int curLength = input.Length;
        for (int i = 0; i < input.Length; i++)
        {
            currentSequence[i] = (byte)(input[i] - '0');
        }

        int part1 = 0;
        for (int i = 0; i < 50; i++)
        {
            if (i == 40)
            {
                part1 = curLength;
            }

            // maximum size
            byte[] nextSequence = new byte[curLength * 2];
            int nextLength = 0;

            int curIndex = 0;
            while (curIndex < curLength)
            {
                byte curVal = currentSequence[curIndex];
                byte amount = 1;
                for (int j = 1; j <= 2; j++)
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
