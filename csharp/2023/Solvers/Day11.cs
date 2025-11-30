using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day11 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var rowLen = input.IndexOf((byte)'\n') + 1;
        var height = input.Length / rowLen;

        var galaxiesPerCol = new int[rowLen - 1];

        long part1 = 0;
        long part2 = 0;

        var seenGalaxies = 0;
        long part1TotalDistanceFromSeenGalaxies = 0;
        long part2TotalDistanceFromSeenGalaxies = 0;
        for (var y = 0; y < height; y++)
        {
            var line = input[..(rowLen - 1)];

            var i = 0;
            var rowIsGap = true;
            while (true)
            {
                var nextGalaxyLocation = line.IndexOf((byte)'#');
                if (nextGalaxyLocation == -1)
                    break;

                part1 += part1TotalDistanceFromSeenGalaxies;
                part2 += part2TotalDistanceFromSeenGalaxies;

                rowIsGap = false;
                seenGalaxies++;
                galaxiesPerCol[i + nextGalaxyLocation]++;
                line = line[(nextGalaxyLocation + 1)..];
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

            input = input[rowLen..];
        }

        seenGalaxies = 0;
        part1TotalDistanceFromSeenGalaxies = 0;
        part2TotalDistanceFromSeenGalaxies = 0;
        foreach (var n in galaxiesPerCol)
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
