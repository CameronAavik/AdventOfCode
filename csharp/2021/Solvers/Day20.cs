using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

// I plan on rewriting this eventually to use 4x8 quads to store the grid instead, but for now this will do
public class Day20 : ISolver
{
    private const int GridPadding = 51;

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var backgroundAlternates = input[0] == '#';
        if (backgroundAlternates && input[511] == '#')
            ThrowHelper.ThrowException("Answer will be infinity");

        Span<byte> evenEnhancementAlgorithm = stackalloc byte[512];
        Span<byte> oddEnhancementAlgorithm = stackalloc byte[512];
        for (var i = 0; i < evenEnhancementAlgorithm.Length; i++)
        {
            if (backgroundAlternates)
            {
                // Since the background alternates, we will store the grid such that on every odd iteration the grid will be inverted
                // This means we need difference enhancement algorithm for even and odd iterations
                // The even enhancement is the NOT of the input
                // The odd enhancement is the REVERSE of the input
                evenEnhancementAlgorithm[i] = (byte)((input[i] & 1) ^ 1);
                oddEnhancementAlgorithm[i] = (byte)(input[511 - i] & 1);
            }
            else
            {
                evenEnhancementAlgorithm[i] = oddEnhancementAlgorithm[i] = (byte)(input[i] & 1);
            }
        }

        var gridInput = input[514..];
        var initialWidth = gridInput.IndexOf((byte)'\n');
        var initialHeight = gridInput.Length / (initialWidth + 1);

        var finalWidth = initialWidth + GridPadding * 2;
        var finalHeight = initialHeight + GridPadding * 2;
        var ulongsPerRow = (finalWidth + 63) / 64;

        Span<ulong> grid = stackalloc ulong[ulongsPerRow * finalHeight];
        Span<ulong> grid2 = stackalloc ulong[ulongsPerRow * finalHeight];
        ParseGridInput(gridInput, grid, initialWidth, initialHeight, ulongsPerRow);

        var minY = GridPadding;
        var maxY = GridPadding + initialHeight - 1;

        Step(grid, grid2, evenEnhancementAlgorithm, --minY, ++maxY, ulongsPerRow);
        Step(grid2, grid, oddEnhancementAlgorithm, --minY, ++maxY, ulongsPerRow);

        var part1 = 0;
        foreach (var row in grid)
            part1 += BitOperations.PopCount(row);

        solution.SubmitPart1(part1);

        for (var i = 0; i < 24; i++)
        {
            Step(grid, grid2, evenEnhancementAlgorithm, --minY, ++maxY, ulongsPerRow);
            Step(grid2, grid, oddEnhancementAlgorithm, --minY, ++maxY, ulongsPerRow);
        }

        var part2 = 0;
        foreach (var row in grid)
            part2 += BitOperations.PopCount(row);

        solution.SubmitPart2(part2);
    }

    private static void ParseGridInput(ReadOnlySpan<byte> gridInput, Span<ulong> grid, int initialWidth, int initialHeight, int ulongsPerRow)
    {
        for (var row = 0; row < initialHeight; row++)
        {
            var y = row + GridPadding;
            var gridOffset = y * ulongsPerRow;
            var inputOffset = row * (initialWidth + 1);

            ulong firstCell = 0;
            var col = 0;
            while (col < Math.Min(64 - GridPadding, initialWidth))
            {
                firstCell <<= 1;
                if (gridInput[inputOffset + col++] == '#')
                    firstCell++;
            }

            if (initialWidth < 64 - GridPadding)
                firstCell <<= (64 - GridPadding) - initialWidth;

            grid[gridOffset++] = firstCell;

            while (col + 64 < initialWidth)
            {
                var colEnd = col + 64;
                ulong cell = 0;
                while (col < colEnd)
                {
                    cell <<= 1;
                    if (gridInput[inputOffset + col++] == '#')
                        cell++;
                }

                grid[gridOffset++] = cell;
            }

            var remaining = initialWidth - col;

            ulong finalCell = 0;
            while (col < initialWidth)
            {
                finalCell <<= 1;
                if (gridInput[inputOffset + col++] == '#')
                    finalCell++;
            }

            grid[gridOffset] = finalCell << (64 - remaining);
        }
    }

    private static void Step(Span<ulong> grid, Span<ulong> nextGrid, Span<byte> enhancement, int minY, int maxY, int ulongsPerRow)
    {
        var prevRow = grid.Slice((minY - 1) * ulongsPerRow, ulongsPerRow);
        var curRow = grid.Slice(minY * ulongsPerRow, ulongsPerRow);

        for (var row = minY; row <= maxY; row++)
        {
            var nextRow = grid.Slice((row + 1) * ulongsPerRow, ulongsPerRow);

            var nextGridRow = nextGrid.Slice(row * ulongsPerRow, ulongsPerRow);

            var lastBitCarry = 0;
            var prev = prevRow[0];
            var cur = curRow[0];
            var next = nextRow[0];
            for (var i = 0; i < ulongsPerRow; i++)
            {
                var firstBitIndex = lastBitCarry | (int)(((prev >> 62) << 6) | ((cur >> 62) << 3) | (next >> 62));

                ulong newValue = enhancement[firstBitIndex];

                for (var j = 1; j < 63; j++)
                {
                    var index = (int)((((prev >> (62 - j)) & 0b111) << 6) | (((cur >> (62 - j)) & 0b111) << 3) | ((next >> (62 - j)) & 0b111));
                    newValue <<= 1;
                    newValue += enhancement[index];
                }

                // take the last two bits of each row and leave the last bit empty
                var lastBitIndex = (int)((prev & 3) << 7 | (cur & 3) << 4 | (next & 3) << 1);

                if (i < ulongsPerRow - 1)
                {
                    lastBitCarry = (lastBitIndex << 1) & 0b100100100;

                    prev = prevRow[i + 1];
                    cur = curRow[i + 1];
                    next = nextRow[i + 1];

                    lastBitIndex |= (int)(((prev >> 63) << 6) | ((cur >> 63) << 3) | (next >> 63));
                }

                newValue <<= 1;
                newValue += enhancement[lastBitIndex];

                nextGridRow[i] = newValue;
            }

            prevRow = curRow;
            curRow = nextRow;
        }
    }
}
