using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

/// <summary>
/// Implementation Details:
/// - Uses SIMD-like techniques for fast parsing of numeric IDs using bit manipulation
/// - Employs histogram sort (counting sort) for O(n) sorting, with 256-width bins for cache efficiency
/// - Encodes left/right position in least significant bit to maintain order during sort
/// - Single pass over sorted data calculates both:
///   * Part1: Tracks imbalance between left/right IDs for scoring
///   * Part2: Detects consecutive matching pairs via running count
///
/// Assumptions:
/// - All IDs in the document are the exact same length
/// - IDs are 8 digits or less.
/// - IDs are always separated by three spaces
/// </summary>
public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        long part2 = 0;

        var lineLength = input.IndexOf((byte)'\n');
        var numLines = input.Length / (lineLength + 1);
        var idLength = (lineLength - 3) / 2; // Each line contains: leftId + "   " + rightId
        var maxId = (int)(Math.Pow(10, idLength) * 2 - 1);

        Span<int> ids = new int[numLines * 2];

        // Histogram sort setup - each bin is 256 numbers wide for optimal cache line usage
        const int bucketWidth = 256;
        var numBuckets = (maxId + bucketWidth - 1) / bucketWidth;
        Span<short> bucketCounts = new short[numBuckets];

        var idIndex = 0;
        if (idLength is >= 3 and <= 8)
        {
            // SWAR (SIMD Within A Register) technique for parsing IDs
            // Memory layout and processing for a line with 5-digit IDs: "12345   67890"
            // 
            // Left ID:  Read from start of line using little-endian (LE) load
            //    "12345   " -> 0x2020203534333231 -> 35 34 33 32 31 00 00 00 (aligned left)
            //                                     -> 35 34 33 32 31 30 30 30 (with '0' padding)
            //
            // Right ID: Read from end of line using LE load
            //    "   67890" -> 0x3039383736202020 -> 30 39 38 37 36 00 00 00 (masked)
            //                                     -> 30 39 38 37 36 30 30 30 (with '0' padding)
            //
            // Both IDs are now 8-byte aligned ASCII strings ready for SWAR parsing
            var bitsMask = 0xFFFFFFFFFFFFFFFFUL << (64 - idLength * 8);
            var zeroPad = 0x3030303030303030UL & ~bitsMask;
            var leftBitsShift = 64 - idLength * 8;

            for (var i = 0; i < input.Length; i += lineLength + 1)
            {
                // Parse left ID using bit manipulation
                var leftBits = (BinaryPrimitives.ReadUInt64LittleEndian(input[i..]) << leftBitsShift) | zeroPad;
                var leftValue = 2 * ParseEightDigits(leftBits);
                ids[idIndex] = leftValue;
                bucketCounts[leftValue / bucketWidth]++;

                // Parse right ID using bit manipulation
                var rightBits = (BinaryPrimitives.ReadUInt64LittleEndian(input[(i + lineLength - 8)..]) & bitsMask) | zeroPad;
                var rightValue = 2 * ParseEightDigits(rightBits) + 1;
                ids[idIndex + 1] = rightValue;
                bucketCounts[rightValue / bucketWidth]++;
                idIndex += 2;
            }
        }
        else
        {
            // Slow path for unusual ID lengths
            for (var i = 0; i < input.Length; i += lineLength + 1)
            {
                var leftValue = int.Parse(input.Slice(i, idLength), CultureInfo.InvariantCulture);
                var rightValue = int.Parse(input[(i + idLength + 3)..], CultureInfo.InvariantCulture);
                ids[idIndex] = leftValue;
                ids[idIndex + 1] = rightValue;
                bucketCounts[leftValue / bucketWidth]++;
                bucketCounts[rightValue / bucketWidth]++;
                idIndex += 2;
            }
        }

        // Convert counts into offsets - each value becomes the starting index for its bucket in the final sorted array
        short prev = 0;
        for (var i = 0; i < bucketCounts.Length; i++)
        {
            var count = bucketCounts[i];
            bucketCounts[i] = prev;
            prev += count;
        }

        // Distribute IDs into buckets
        Span<int> sortedIds = new int[ids.Length];
        foreach (var id in ids)
            sortedIds[bucketCounts[id / bucketWidth]++] = id;

        // Sort within each bucket
        var bucketStart = 0;
        foreach (int bucketEnd in bucketCounts)
        {
            sortedIds[bucketStart..bucketEnd].Sort();
            bucketStart = bucketEnd;
        }

        // Track IDs for detecting left/right imbalances and matching pairs
        var sideIndexDiff = 0; // Positive when more left IDs seen, negative for right
        var prevLeftId = 0;
        var prevLeftCount = 0;

        foreach (var id in sortedIds)
        {
            var side = id % 2; // 0 = left side, 1 = right side
            var realId = id / 2;
            int part1Multiplier;

            if (side == 0)
            {
                sideIndexDiff++;
                // Returns -1 when more left IDs than right IDs seen so far, else 1
                part1Multiplier = -Math.Sign(2 * sideIndexDiff - 1);
                prevLeftCount = prevLeftId == realId ? prevLeftCount + 1 : 1;
                prevLeftId = realId;
            }
            else
            {
                sideIndexDiff--;
                // Returns -1 when more right IDs than left IDs seen so far, else 1
                part1Multiplier = -Math.Sign(2 * -sideIndexDiff - 1);
                if (prevLeftId == realId)
                    part2 += prevLeftId * prevLeftCount;
            }

            part1 += part1Multiplier * realId;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    // Implementation from https://lemire.me/blog/2022/01/21/swar-explained-parsing-eight-digits/
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseEightDigits(ulong val)
    {
        const ulong mask = 0x000000FF000000FF;
        const ulong mul1 = 0x000F424000000064;
        // 100 + (1000000ULL << 32)
        const ulong mul2 = 0x0000271000000001;
        // 1 + (10000ULL << 32)
        val -= 0x3030303030303030;
        val = (val * 10) + (val >> 8); // val = (val * 2561) >> 8;
        val = (((val & mask) * mul1) + (((val >> 16) & mask) * mul2)) >> 32;
        return (int)val;
    }
}
