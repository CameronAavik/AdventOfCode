using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day07 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ref var inputRef = ref MemoryMarshal.GetReference(input);

        var startIndex = input.IndexOf((byte)'S');
        var width = input[(startIndex + 1)..].IndexOf((byte)'\n') + startIndex + 1;

        var counts = width > 256 ? new long[width] : stackalloc long[256];
        counts = counts[..width];

        ref var countsRef = ref MemoryMarshal.GetReference(counts);
        Unsafe.Add(ref countsRef, startIndex) = 1;

        var part1 = 0;

        var minCol = startIndex;
        var maxCol = startIndex;
        var tachyons = Vector256.Create((byte)'^');
        for (var rowIndex = 0; rowIndex < input.Length - 1; rowIndex += width + 1)
        {
            var alignedStart = maxCol - Vector256<byte>.Count + 1;
            var extraVectorsNeeded = (alignedStart - minCol + Vector256<byte>.Count - 1) / Vector256<byte>.Count;
            alignedStart -= extraVectorsNeeded * Vector256<byte>.Count;

            if (alignedStart < 0)
            {
                alignedStart += Vector256<byte>.Count;
                var unalignedStart = minCol;
                var v = Vector256.LoadUnsafe(ref inputRef, (nuint)(rowIndex + unalignedStart));
                var tachyonLocations = Vector256.Equals(v, tachyons).ExtractMostSignificantBits();

                var bitsToProcess = alignedStart - unalignedStart;
                tachyonLocations &= (1U << bitsToProcess) - 1;

                while (tachyonLocations != 0)
                {
                    var t = tachyonLocations & (~tachyonLocations + 1);
                    var location = unalignedStart + BitOperations.TrailingZeroCount(t);
                    ref var prevRef = ref Unsafe.Add(ref countsRef, location);
                    var prev = prevRef;
                    if (prev > 0)
                    {
                        part1++;
                        Unsafe.Subtract(ref prevRef, 1) += prev;
                        Unsafe.Add(ref prevRef, 1) += prev;
                        prevRef = 0;

                        if (location == minCol)
                            minCol--;
                    }

                    tachyonLocations ^= t;
                }
            }

            var lastCol = -1;
            for (var col = alignedStart; col < maxCol; col += Vector256<byte>.Count)
            {
                var v = Vector256.LoadUnsafe(ref inputRef, (nuint)(rowIndex + col));
                var tachyonLocations = Vector256.Equals(v, tachyons).ExtractMostSignificantBits();
                while (tachyonLocations != 0)
                {
                    var t = tachyonLocations & (~tachyonLocations + 1);
                    lastCol = col + BitOperations.TrailingZeroCount(t);
                    ref var prevRef = ref Unsafe.Add(ref countsRef, lastCol);
                    var prev = prevRef;
                    if (prev > 0)
                    {
                        part1++;
                        Unsafe.Subtract(ref prevRef, 1) += prev;
                        Unsafe.Add(ref prevRef, 1) += prev;
                        prevRef = 0;

                        if (lastCol == minCol)
                            minCol--;
                    }

                    tachyonLocations ^= t;
                }
            }

            if (lastCol == maxCol)
                maxCol++;
        }

        var end = (nuint)(counts.Length - (counts.Length % Vector256<long>.Count));
        var part2Sum = Vector256<long>.Zero;
        for (nuint i = 0; i < end; i += (nuint)Vector256<long>.Count)
            part2Sum += Vector256.LoadUnsafe(ref countsRef, i);

        var part2 = Vector256.Sum(part2Sum);
        for (var i = end; i < (nuint)counts.Length; i++)
            part2 += Unsafe.Add(ref countsRef, i);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
