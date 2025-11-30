using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day13 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        while (true)
        {
            var gridEnd = input.IndexOf("\n\n"u8);
            var grid = gridEnd == -1 ? input : input[..(gridEnd + 1)];

            FindMirrorsInGrid(grid, ref part1, ref part2);

            // last grid will end with single newline
            if (gridEnd == -1)
                break;

            input = input[(gridEnd + 2)..];
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void FindMirrorsInGrid(ReadOnlySpan<byte> grid, ref int part1, ref int part2)
    {
        Span<uint> rows = stackalloc uint[24];
        Span<uint> cols = stackalloc uint[24];

        ExtractRowsAndCols(grid, rows, cols, out var width, out var height);

        var hasSolvedPart1 = false;
        var hasSolvedPart2 = false;

        // Find mirror on the rows
        FindMirrors(rows[..height], 100, ref part1, ref part2, ref hasSolvedPart1, ref hasSolvedPart2);

        if (hasSolvedPart1 && hasSolvedPart2)
            return;

        // Find mirrors on the cols
        FindMirrors(cols[..width], 1, ref part1, ref part2, ref hasSolvedPart1, ref hasSolvedPart2);
    }

    private static void ExtractRowsAndCols(ReadOnlySpan<byte> grid, Span<uint> rows, Span<uint> cols, out int width, out int height)
    {
        width = grid.IndexOf((byte)'\n');
        height = grid.Length / (width + 1);

        ref var gridRef = ref MemoryMarshal.GetReference(grid);

        var rowOffset = 0;
        for (var y = 0; y < height; y++)
        {
            // load row using SIMD
            uint row;
            if (grid.Length - rowOffset > Vector256<byte>.Count)
            {
                var rowVec = Vector256.LoadUnsafe(ref gridRef, (nuint)rowOffset);
                var bits = Vector256.Equals(rowVec, Vector256.Create((byte)'#')).ExtractMostSignificantBits();
                row = bits & ((1U << width) - 1);
            }
            else
            {
                // If the row is too small for the buffer, then load the row into the end of the vector
                var rowVec = Vector256.LoadUnsafe(ref gridRef, (nuint)(rowOffset + width - Vector256<byte>.Count));
                var bits = Vector256.Equals(rowVec, Vector256.Create((byte)'#')).ExtractMostSignificantBits();
                row = bits >> (Vector256<byte>.Count - width);
            }

            rows[y] = row;

            var rowBit = 1U << y;

            // iterate over row bits with https://lemire.me/blog/2018/02/21/iterating-over-set-bits-quickly/
            while (row != 0)
            {
                var t = row & (~row + 1);
                cols[BitOperations.TrailingZeroCount(t)] |= rowBit;
                row ^= t;
            }

            rowOffset += width + 1;
        }
    }

    private static void FindMirrors(Span<uint> rowsOrCols, int multiplier, ref int part1, ref int part2, ref bool hasSolvedPart1, ref bool hasSolvedPart2)
    {
        var prev = rowsOrCols[0];
        for (var i = 1; i < rowsOrCols.Length; i++)
        {
            var cur = rowsOrCols[i];
            var xorValue = cur ^ prev;
            if (xorValue == 0 || BitOperations.IsPow2(xorValue))
            {
                var hasOneDiff = xorValue != 0;
                if (VerifyCandidateMirror(rowsOrCols, i, ref hasOneDiff))
                {
                    if (!hasOneDiff)
                    {
                        part1 += i * multiplier;
                        hasSolvedPart1 = true;
                        if (hasSolvedPart2)
                            return;
                    }
                    else
                    {
                        part2 += i * multiplier;
                        hasSolvedPart2 = true;
                        if (hasSolvedPart1)
                            return;
                    }
                }
            }

            prev = cur;
        }
    }

    private static bool VerifyCandidateMirror(Span<uint> rowsOrCols, int i, ref bool hasOneDiff)
    {
        var maxJ = Math.Min(i, rowsOrCols.Length - i);
        for (var j = 1; j < maxJ; j++)
        {
            var pairXor = rowsOrCols[i - 1 - j] ^ rowsOrCols[i + j];
            if (pairXor != 0)
            {
                if (hasOneDiff || !BitOperations.IsPow2(pairXor))
                    return false;

                hasOneDiff = true;
            }
        }

        return true;
    }
}
