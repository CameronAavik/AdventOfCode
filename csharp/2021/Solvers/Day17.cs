using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day17 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ParseInput(input, out int x1, out int x2, out int y1, out int y2);

        // Maximum y velocity is -y1 since otherwise it will overshoot the target area.
        // The y position at the peak can be expressed as the sum from 1 to the y velocity.
        int part1MaxVelY = -y1;
        solution.SubmitPart1(part1MaxVelY * (part1MaxVelY - 1) / 2);

        // Add number of ways to get the target area on the first step
        int part2 = (x2 - x1 + 1) * (y2 - y1 + 1);

        int triangular = 0;
        int finalMinVelX = 0;
        int finalMaxVelX = 0;
        int prevMinVelX = x1;
        int prevMaxVelX = x2;
        int prevMinVelY = y1;
        int prevMaxVelY = y2;
        for (int step = 1; step <= part1MaxVelY * 2 + 1; step++)
        {
            triangular += step;

            if (finalMinVelX == 0 && triangular > x1)
                finalMinVelX = step;

            if (finalMaxVelX == 0 && triangular > x2)
                finalMaxVelX = step - 1;

            // x = triangular + (step + 1) * (vx - step)
            // vx = step + (x - triangular) / (step + 1);
            int minVelX = finalMinVelX == 0 ? step + DivideAndRoundUp(x1 - triangular, step + 1) : finalMinVelX; // need to round up for x1
            int maxVelX = finalMaxVelX == 0 ? step + (x2 - triangular) / (step + 1) : finalMaxVelX;

            int xRange = maxVelX - minVelX + 1;
            int xRangePreviousOverlap = Math.Max(0, Math.Min(maxVelX, prevMaxVelX) - Math.Max(minVelX, prevMinVelX) + 1);

            prevMinVelX = minVelX;
            prevMaxVelX = maxVelX;

            // Handle case where we start with negative or 0 y velocity
            if (triangular <= -y1)
            {
                // y = (step + 1) * vy - triangular
                // vy = (y + triangular) / (step + 1)
                int minVelY = (y1 + triangular) / (step + 1);
                int maxVelY = y2 >= -triangular ? 0 : -DivideAndRoundUp(-(y2 + triangular), step + 1);

                int yRangePreviousOverlap = Math.Max(0, Math.Min(maxVelY, prevMaxVelY) - Math.Max(minVelY, prevMinVelY) + 1);
                prevMinVelY = minVelY;
                prevMaxVelY = maxVelY;

                int yRange = maxVelY - minVelY + 1;
                part2 += xRange * yRange - xRangePreviousOverlap * yRangePreviousOverlap;
            }

            // Handle case where we start with positive y velocty
            // Since the target area is below x axis, we know it will cross y=0 at step 2*vy
            // This means that there will always be an odd number of steps once it reaches the axis
            // This forces whether or not the number of steps after y=0 is even or odd.
            int minStepsAfterAxis = 2 - (step % 2);
            int triangular2 = 0;
            for (int stepsAfterAxis = minStepsAfterAxis; stepsAfterAxis < step; stepsAfterAxis += 2)
            {
                triangular2 += 2 * stepsAfterAxis - 1;

                int stepsAtYintersect = step - stepsAfterAxis;
                int vy = stepsAtYintersect / 2;
                int y = -(triangular2 + vy * stepsAfterAxis);

                if (y < y1)
                {
                    break;
                }
                else if (y <= y2)
                {
                    int yAtPreviousStep = y + vy + stepsAfterAxis;
                    part2 += yAtPreviousStep <= y2 ? xRange - xRangePreviousOverlap : xRange;
                }
            }
        }

        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DivideAndRoundUp(int numerator, int denominator) => (numerator - 1) / denominator + 1;

    private static void ParseInput(ReadOnlySpan<byte> input, out int x1, out int x2, out int y1, out int y2)
    {
        int i = "target area: x=".Length;
        x1 = ReadInteger(input, '.', ref i);
        i += ".".Length;
        x2 = ReadInteger(input, ',', ref i);
        i += " y=".Length;
        y1 = ReadInteger(input, '.', ref i);
        i += ".".Length;
        y2 = ReadInteger(input, '\n', ref i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInteger(ReadOnlySpan<byte> span, char until, ref int i)
    {
        // Assume that the first character is always a digit
        byte c = span[i++];

        int mul;
        int ret;
        if (c == '-')
        {
            mul = -1;
            ret = 0;
        }
        else
        {
            mul = 1;
            ret = c - '0';
        }

        byte cur;
        while ((cur = span[i++]) != until)
            ret = ret * 10 + (cur - '0');

        return mul * ret;
    }
}
