using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day11 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int rowLen = input.IndexOf((byte)'\n') + 1;
        int height = input.Length / rowLen;

        int[] galaxiesPerCol = new int[rowLen - 1];

        long part1 = 0;
        long part2 = 0;

        int seenGalaxies = 0;
        long part1TotalDistanceFromSeenGalaxies = 0;
        long part2TotalDistanceFromSeenGalaxies = 0;
        for (int y = 0; y < height; y++)
        {
            ReadOnlySpan<byte> line = input.Slice(0, rowLen - 1);

            int i = 0;
            bool rowIsGap = true;
            while (true)
            {
                int nextGalaxyLocation = line.IndexOf((byte)'#');
                if (nextGalaxyLocation == -1)
                    break;

                part1 += part1TotalDistanceFromSeenGalaxies;
                part2 += part2TotalDistanceFromSeenGalaxies;

                rowIsGap = false;
                seenGalaxies++;
                galaxiesPerCol[i + nextGalaxyLocation]++;
                line = line.Slice(nextGalaxyLocation + 1);
                i += nextGalaxyLocation + 1;
            }

            if (rowIsGap)
            {
                part1TotalDistanceFromSeenGalaxies += seenGalaxies * 2;
                part2TotalDistanceFromSeenGalaxies += seenGalaxies * 1000000;
            }
            else
            {
                part1TotalDistanceFromSeenGalaxies += seenGalaxies;
                part2TotalDistanceFromSeenGalaxies += seenGalaxies;
            }

            input = input.Slice(rowLen);
        }

        seenGalaxies = 0;
        part1TotalDistanceFromSeenGalaxies = 0;
        part2TotalDistanceFromSeenGalaxies = 0;
        foreach (int n in galaxiesPerCol)
        {
            if (n == 0)
            {
                part1TotalDistanceFromSeenGalaxies += seenGalaxies * 2;
                part2TotalDistanceFromSeenGalaxies += seenGalaxies * 1000000;
            }
            else
            {

                part1 += n * part1TotalDistanceFromSeenGalaxies;
                part2 += n * part2TotalDistanceFromSeenGalaxies;

                seenGalaxies += n;

                part1TotalDistanceFromSeenGalaxies += seenGalaxies;
                part2TotalDistanceFromSeenGalaxies += seenGalaxies;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

}
