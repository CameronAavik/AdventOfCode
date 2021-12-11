using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day11 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // Energy levels are offset by 32, so an energy level of 2 is represented by 34
        // If the value is less than 32, it means that the octopus has flashed in the step
        // The grid is 12x12 but the input will be placed in the center 10x10
        Span<int> octopi = stackalloc int[144];
        octopi.Clear();

        int cursor = 0;
        for (int rowStart = 12; rowStart < 132; rowStart += 12)
        {
            for (int i = rowStart + 1; i < rowStart + 11; i++)
                octopi[i] = input[cursor++] - '0' + 32;
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

    private static int Step(Span<int> octopi)
    {
        // Increase all the octopi by 1.
        // If the value was less than 128, then set it to 128 first.
        for (int rowStart = 12; rowStart < 132; rowStart += 12)
            for (int i = rowStart + 1; i < rowStart + 11; i++)
                octopi[i] = Math.Max(octopi[i], 32) + 1;

        int stepFlashes = 0;
        int flashes;
        do
        {
            flashes = 0;

            for (int rowStart = 12; rowStart < 132; rowStart += 12)
            {
                for (int i = rowStart + 1; i < rowStart + 11; i++)
                {
                    if (octopi[i] >= 42)
                    {
                        octopi[i - 13]++;
                        octopi[i - 12]++;
                        octopi[i - 11]++;
                        octopi[i - 1]++;
                        octopi[i + 1]++;
                        octopi[i + 11]++;
                        octopi[i + 12]++;
                        octopi[i + 13]++;

                        octopi[i] = 0;
                        flashes++;
                    }
                }
            }

            stepFlashes += flashes;
        }
        while (flashes > 0);

        return stepFlashes;
    }
}
