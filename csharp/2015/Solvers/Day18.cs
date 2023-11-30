using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day18 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // we allocate two grids, each iteration switches which grid is used.
        // this means we can keep reusing the same grids over and over with no extra allocations.
        int curGridIndex = 0;
        int[][,] grids =
        [
                new int[102, 102],
                new int[102, 102],
            ];

        int row = 1;
        int col = 1;
        foreach (byte c in input)
        {
            if (c == '\n')
            {
                col = 1;
                row++;
            }
            else
            {
                grids[0][row, col] = c == '#' ? (1 << 16) | 1 : 0;
                col++;
            }
        }

        for (int i = 0; i < 100; i++)
        {
            int[,] curGrid = grids[curGridIndex];
            int[,] newGrid = grids[1 - curGridIndex];

            for (int y = 1; y <= 100; y++)
            {
                for (int x = 1; x <= 100; x++)
                {
                    int neighbourTotal = GetNeighbourTotal(curGrid, x, y);

                    int newValue = 0;
                    switch (neighbourTotal & 0xF)
                    {
                        case 2:
                            newValue |= curGrid[y, x] & 1;
                            break;
                        case 3:
                            newValue |= 1;
                            break;
                    }

                    switch (neighbourTotal >> 16)
                    {
                        case 2:
                            newValue |= curGrid[y, x] & (1 << 16);
                            break;
                        case 3:
                            newValue |= 1 << 16;
                            break;
                    }

                    newGrid[y, x] = newValue;
                }
            }

            // ensure the corners stay on for part 2
            newGrid[1, 1] |= 1 << 16;
            newGrid[1, 100] |= 1 << 16;
            newGrid[100, 1] |= 1 << 16;
            newGrid[100, 100] |= 1 << 16;
            curGridIndex = 1 - curGridIndex;
        }

        int sum = 0;
        foreach (int light in grids[curGridIndex])
        {
            sum += light;
        }

        solution.SubmitPart1(sum & 0xFFFF);
        solution.SubmitPart2(sum >> 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetNeighbourTotal(int[,] grid, int x, int y)
    {
        return
            grid[y - 1, x - 1] +
            grid[y - 1, x] +
            grid[y - 1, x + 1] +
            grid[y, x - 1] +
            grid[y, x + 1] +
            grid[y + 1, x - 1] +
            grid[y + 1, x] +
            grid[y + 1, x + 1];
    }
}
