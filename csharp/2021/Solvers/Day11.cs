using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day11 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // Energy levels are offset by 128, so an energy level of 2 is represented by 130
        // If the value is less than 128, it means that the octopus has flashed in the step
        Span<byte> octopi = stackalloc byte[100];
        int cursor = 0;
        for (int rowStart = 0; rowStart < 100; rowStart += 10)
        {
            for (int i = rowStart; i < rowStart + 10; i++)
                octopi[i] = (byte)(input[cursor++] - '0' + 128);
            cursor++;
        }

        int totalFlashes = 0;
        for (int step = 0; step < 100; step++)
            totalFlashes += Step(octopi);

        solution.SubmitPart1(totalFlashes);

        int step2 = 100;
        while (Step(octopi) != 100)
            step2++;

        solution.SubmitPart2(step2 + 1);
    }

    private static int Step(Span<byte> octopi)
    {
        // Increase all the octopi by 1.
        // If the value was less than 128, then set it to 128 first.
        for (int i = 0; i < 100; i++)
            octopi[i] = (byte)(Math.Max(octopi[i], (byte)128) + 1);

        int stepFlashes = 0;
        int flashes;
        do
        {
            flashes = 0;

            if (octopi[0] >= 138)
            {
                octopi[1]++;
                octopi[10]++;
                octopi[11]++;

                octopi[0] = 0;
                flashes++;
            }

            for (int i = 1; i < 9; i++)
            {
                if (octopi[i] >= 138)
                {
                    octopi[i - 1]++;
                    octopi[i + 1]++;
                    octopi[i + 9]++;
                    octopi[i + 10]++;
                    octopi[i + 11]++;

                    octopi[i] = 0;
                    flashes++;
                }
            }

            if (octopi[9] >= 138)
            {
                octopi[8]++;
                octopi[18]++;
                octopi[19]++;

                octopi[9] = 0;
                flashes++;
            }

            for (int rowStart = 10; rowStart < 90; rowStart += 10)
            {
                if (octopi[rowStart] >= 138)
                {
                    octopi[rowStart - 10]++;
                    octopi[rowStart - 9]++;
                    octopi[rowStart + 1]++;
                    octopi[rowStart + 10]++;
                    octopi[rowStart + 11]++;

                    octopi[rowStart] = 0;
                    flashes++;
                }

                for (int i = rowStart + 1; i < rowStart + 9; i++)
                {
                    if (octopi[i] >= 138)
                    {
                        octopi[i - 11]++;
                        octopi[i - 10]++;
                        octopi[i - 9]++;
                        octopi[i - 1]++;
                        octopi[i + 1]++;
                        octopi[i + 9]++;
                        octopi[i + 10]++;
                        octopi[i + 11]++;

                        octopi[i] = 0;
                        flashes++;
                    }
                }

                if (octopi[rowStart + 9] >= 138)
                {
                    octopi[rowStart - 2]++;
                    octopi[rowStart - 1]++;
                    octopi[rowStart + 8]++;
                    octopi[rowStart + 18]++;
                    octopi[rowStart + 19]++;

                    octopi[rowStart + 9] = 0;
                    flashes++;
                }
            }

            if (octopi[90] >= 138)
            {
                octopi[80]++;
                octopi[81]++;
                octopi[91]++;

                octopi[90] = 0;
                flashes++;
            }

            for (int i = 91; i < 99; i++)
            {
                if (octopi[i] >= 138)
                {
                    octopi[i - 11]++;
                    octopi[i - 10]++;
                    octopi[i - 9]++;
                    octopi[i - 1]++;
                    octopi[i + 1]++;

                    octopi[i] = 0;
                    flashes++;
                }
            }

            if (octopi[99] >= 138)
            {
                octopi[88]++;
                octopi[89]++;
                octopi[98]++;

                octopi[99] = 0;
                flashes++;
            }

            stepFlashes += flashes;
        }
        while (flashes > 0);

        return stepFlashes;
    }

    //private static int Step2(Span<byte> octopi)
    //{
    //    // Increase all the octopi by 1.
    //    // If the value was less than 128, then set it to 128 first.
    //    for (int i = 0; i < 100; i++)
    //        octopi[i] = (byte)(Math.Max(octopi[i], (byte)128) + 1);
    //
    //    int stepFlashes = 0;
    //    int flashes;
    //    do
    //    {
    //        flashes = 0;
    //
    //        for (int y = 0; y < 10; y++)
    //        {
    //            int rowStart = y * 10;
    //            for (int x = 0; x < 10; x++)
    //            {
    //                int index = rowStart + x;
    //                if (octopi[index] >= 138)
    //                {
    //                    if (x > 0)
    //                    {
    //                        octopi[index - 1]++;
    //                        if (y > 0)
    //                            octopi[index - 11]++;
    //
    //                        if (y < 9)
    //                            octopi[index + 9]++;
    //                    }
    //
    //                    if (y > 0)
    //                        octopi[index - 10]++;
    //
    //                    if (x < 9)
    //                    {
    //                        octopi[index + 1]++;
    //
    //                        if (y > 0)
    //                            octopi[index - 9]++;
    //
    //                        if (y < 9)
    //                            octopi[index + 11]++;
    //                    }
    //
    //                    if (y < 9)
    //                        octopi[index + 10]++;
    //
    //                    octopi[index] = 0;
    //                    flashes++;
    //                }
    //            }
    //        }
    //
    //        stepFlashes += flashes;
    //    }
    //    while (flashes > 0);
    //
    //    return stepFlashes;
    //}
}
