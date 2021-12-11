using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day09 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int width = input.IndexOf('\n');
        int height = input.Length / (width + 1);

        Span<int> rowBasins = stackalloc int[width];
        Span<int> basins = stackalloc int[4096]; // assume a max of 4096 basins
        int basinCount = 0;

        int cursor = 0;

        // first row
        int prevBasin = -1;
        for (int x = 0; x < width; x++)
        {
            int locationHeight = input[cursor++] - '0';
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

        for (int y = 1; y < height; y++)
        {
            prevBasin = -1;
            for (int x = 0; x < width; x++)
            {
                int locationHeight = input[cursor++] - '0';
                if (locationHeight == 9)
                {
                    rowBasins[x] = -1;
                    prevBasin = -1;
                    continue;
                }

                int prevRowBasin = rowBasins[x];

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
                            int basinCombinedHeights = (prevBasinCount | prevRowBasinCount | (1 << (locationHeight + 16))) & 0x7FFF0000;
                            int basinCombinedCounts = (prevBasinCount + prevRowBasinCount + 1) & 0xFFFF;
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

        int riskLevelSum = 0;
        int max1 = 0;
        int max2 = 0;
        int max3 = 0;
        for (int i = 0; i < basinCount; i++)
        {
            var basin = basins[i];
            if (basin > 0)
            {
                int basinLowestHeight = BitOperations.TrailingZeroCount(basin >> 16);
                riskLevelSum += basinLowestHeight + 1;

                int basinSize = basin & 0xFFFF;

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
