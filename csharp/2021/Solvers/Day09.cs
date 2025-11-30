using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day09 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var height = input.Length / (width + 1);

        Span<int> rowBasins = stackalloc int[width];
        Span<int> basins = stackalloc int[4096]; // assume a max of 4096 basins
        var basinCount = 0;

        var cursor = 0;

        // first row
        var prevBasin = -1;
        for (var x = 0; x < width; x++)
        {
            var locationHeight = input[cursor++] - '0';
            if (locationHeight == 9)
            {
                rowBasins[x] = -1;
                prevBasin = -1;
                continue;
            }

            if (prevBasin == -1)
            {
                prevBasin = basinCount++;
                basins[prevBasin] = (1 << (locationHeight + 16)) | 1;
            }
            else
            {
                basins[prevBasin] = (1 << (locationHeight + 16)) | (basins[prevBasin] + 1);
            }

            rowBasins[x] = prevBasin;
        }

        cursor++; // skip newline

        for (var y = 1; y < height; y++)
        {
            prevBasin = -1;
            for (var x = 0; x < width; x++)
            {
                var locationHeight = input[cursor++] - '0';
                if (locationHeight == 9)
                {
                    rowBasins[x] = -1;
                    prevBasin = -1;
                    continue;
                }

                var prevRowBasin = rowBasins[x];

                if (prevBasin == -1)
                {
                    if (prevRowBasin == -1)
                    {
                        prevBasin = basinCount++;
                        basins[prevBasin] = (1 << (locationHeight + 16)) | 1;
                    }
                    else
                    {
                        int prevRowBasinCount;
                        while ((prevRowBasinCount = basins[prevRowBasin]) < 0)
                            prevRowBasin = -prevRowBasinCount;

                        basins[prevRowBasin] = (1 << (locationHeight + 16)) | (prevRowBasinCount + 1);
                        prevBasin = prevRowBasin;
                    }
                }
                else
                {
                    if (prevRowBasin == -1)
                    {
                        basins[prevBasin] = (1 << (locationHeight + 16)) | (basins[prevBasin] + 1);
                    }
                    else
                    {
                        // merge scenario
                        int prevRowBasinCount;
                        while ((prevRowBasinCount = basins[prevRowBasin]) < 0)
                            prevRowBasin = -prevRowBasinCount;

                        // if they are different, then merge the basins
                        if (prevRowBasin != prevBasin)
                        {
                            var prevBasinCount = basins[prevBasin];
                            var basinCombinedHeights = (prevBasinCount | prevRowBasinCount | (1 << (locationHeight + 16))) & 0x7FFF0000;
                            var basinCombinedCounts = (prevBasinCount + prevRowBasinCount + 1) & 0xFFFF;
                            basins[prevBasin] = basinCombinedHeights | basinCombinedCounts;
                            basins[prevRowBasin] = -prevBasin;
                        }
                        else
                        {
                            basins[prevBasin] = (1 << (locationHeight + 16)) | (prevRowBasinCount + 1);
                        }
                    }
                }

                rowBasins[x] = prevBasin;
            }

            cursor++;
        }

        var riskLevelSum = 0;
        var max1 = 0;
        var max2 = 0;
        var max3 = 0;
        for (var i = 0; i < basinCount; i++)
        {
            var basin = basins[i];
            if (basin > 0)
            {
                var basinLowestHeight = BitOperations.TrailingZeroCount(basin >> 16);
                riskLevelSum += basinLowestHeight + 1;

                var basinSize = basin & 0xFFFF;

                if (basinSize < max3)
                    continue;

                if (basinSize < max2)
                {
                    max3 = basinSize;
                    continue;
                }

                max3 = max2;

                if (basinSize < max1)
                {
                    max2 = basinSize;
                }
                else
                {
                    max2 = max1;
                    max1 = basinSize;
                }
            }
        }

        solution.SubmitPart1(riskLevelSum);
        solution.SubmitPart2(max1 * max2 * max3);
    }
}
