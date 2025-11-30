using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day17 : ISolver
{
    // The solutions to parts 1 and 2 are very similar, but there are enough small differences that I couldn't find
    // any nice way to abstract them away so I have just written each part separately.
    //
    // The solution is essentially A* using Manhattan Distance as the heuristic. I use a bucket queue and a bitset to
    // iterate through the possible states in order. A state is essentially (int x, int y, bool isVertical) where x,y
    // represents the position and isVertical represents if the last move to reach this space was a vertical or
    // horizontal move. I pack these three values into a ushort, with the least significant bit representing isVertical
    // and the bits above it equal to y * width + x. When I process a new state, I first check if it is seen in the
    // bitset and if it isn't, I iterate through all the possible full strides I can make from my current position.
    // This means that I am not moving one step at a time, but each turn I am alternating between a horizontal move and
    // a vertical move.
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var rowLength = width + 1;
        var height = input.Length / rowLength;

        // Create reusable buckets for both parts
        var buckets = new List<ushort>[1300];
        for (var i = 0; i < 128; i++)
            buckets[i] = new List<ushort>(1500);
        for (var i = 128; i < buckets.Length; i++)
            buckets[i] = buckets[i % 128];

        var part1 = SolvePart1(input, width, height, buckets);
        solution.SubmitPart1(part1);

        for (var i = 0; i < 128; i++)
            buckets[i].Clear();

        var part2 = SolvePart2(input, width, height, buckets);
        solution.SubmitPart2(part2);
    }

    public static int SolvePart1(ReadOnlySpan<byte> input, int width, int height, List<ushort>[] buckets)
    {
        var rowLength = width + 1;
        var numStates = rowLength * height * 2;
        var targetState = (height - 1) * rowLength + (width - 1);

        const int xMul = 2;
        var yMul = 2 * rowLength;

        var seen = new ulong[(numStates - 1) / 64 + 1];

        var bucketPtr = 0;
        buckets[0].Add(0);
        buckets[0].Add(1);

        while (true)
        {
            var bucket = buckets[bucketPtr];

            for (var i = 0; i < bucket.Count; i++)
            {
                var element = bucket[i];
                ref var seenBitset = ref seen[element / 64];
                var elementBit = 1UL << element;
                if ((seenBitset & elementBit) != 0)
                    continue;
                seenBitset |= elementBit;

                var rowOffset = Math.DivRem(element, 2, out var isHorizontal);
                if (rowOffset == targetState)
                    return bucketPtr + (width + height - 2);

                var y = Math.DivRem(rowOffset, rowLength, out var x);

                if (isHorizontal == 0)
                {
                    var total = 0;
                    var maxX = Math.Min(4, width - x);
                    for (var x2 = 1; x2 < maxX; x2++)
                    {
                        total += input[rowOffset + x2] - '0' - 1;
                        buckets[bucketPtr + total].Add((ushort)(element + xMul * x2 + 1));
                    }

                    total = 0;
                    var minX = Math.Max(-3, -x);
                    for (var x2 = -1; x2 >= minX; x2--)
                    {
                        total += input[rowOffset + x2] - '0' + 1;
                        buckets[bucketPtr + total].Add((ushort)(element + xMul * x2 + 1));
                    }
                }
                else
                {
                    var total = 0;
                    var maxY = Math.Min(4, height - y);
                    for (var y2 = 1; y2 < maxY; y2++)
                    {
                        total += input[rowOffset + rowLength * y2] - '0' - 1;
                        buckets[bucketPtr + total].Add((ushort)(element + yMul * y2 - 1));
                    }

                    total = 0;
                    var minY = Math.Max(-3, -y);
                    for (var y2 = -1; y2 >= minY; y2--)
                    {
                        total += input[rowOffset + rowLength * y2] - '0' + 1;
                        buckets[bucketPtr + total].Add((ushort)(element + yMul * y2 - 1));
                    }
                }
            }

            bucket.Clear();
            bucketPtr++;
        }
    }

    public static int SolvePart2(ReadOnlySpan<byte> input, int width, int height, List<ushort>[] buckets)
    {
        var rowLength = width + 1;
        var numStates = rowLength * height * 2;
        var targetState = (height - 1) * rowLength + (width - 1);

        const int xMul = 2;
        var yMul = 2 * rowLength;

        var seen = new ulong[(numStates - 1) / 64 + 1];

        var bucketPtr = 0;
        buckets[0].Add(0);
        buckets[0].Add(1);

        while (true)
        {
            var bucket = buckets[bucketPtr];

            for (var i = 0; i < bucket.Count; i++)
            {
                var element = bucket[i];
                var elementBit = 1UL << element;
                if ((seen[element / 64] & elementBit) != 0)
                    continue;
                seen[element / 64] |= elementBit;

                var rowOffset = Math.DivRem(element, 2, out var isHorizontal);
                if (rowOffset == targetState)
                    return bucketPtr + (width + height - 2);

                var y = Math.DivRem(rowOffset, rowLength, out var x);

                if (isHorizontal == 0)
                {
                    if (x < width - 4)
                    {
                        var total = 0;
                        for (var x2 = 1; x2 < 4; x2++)
                            total += input[rowOffset + x2] - '0' - 1;

                        var maxX = Math.Min(11, width - x);
                        for (var x2 = 4; x2 < maxX; x2++)
                        {
                            total += input[rowOffset + x2] - '0' - 1;
                            buckets[bucketPtr + total].Add((ushort)(element + xMul * x2 + 1));
                        }
                    }

                    if (x >= 4)
                    {
                        var total = 0;
                        for (var x2 = -1; x2 >= -3; x2--)
                            total += input[rowOffset + x2] - '0' + 1;

                        var minX = Math.Max(-10, -x);
                        for (var x2 = -4; x2 >= minX; x2--)
                        {
                            total += input[rowOffset + x2] - '0' + 1;
                            buckets[bucketPtr + total].Add((ushort)(element + xMul * x2 + 1));
                        }
                    }
                }
                else
                {
                    if (y < height - 4)
                    {
                        var total = 0;
                        for (var y2 = 1; y2 < 4; y2++)
                            total += input[rowOffset + rowLength * y2] - '0' - 1;

                        var maxY = Math.Min(11, height - y);
                        for (var y2 = 4; y2 < maxY; y2++)
                        {
                            total += input[rowOffset + rowLength * y2] - '0' - 1;
                            buckets[bucketPtr + total].Add((ushort)(element + yMul * y2 - 1));
                        }
                    }

                    if (y >= 4)
                    {
                        var total = 0;
                        for (var y2 = -1; y2 >= -3; y2--)
                            total += input[rowOffset + rowLength * y2] - '0' + 1;

                        var minY = Math.Max(-10, -y);
                        for (var y2 = -4; y2 >= minY; y2--)
                        {
                            total += input[rowOffset + rowLength * y2] - '0' + 1;
                            buckets[bucketPtr + total].Add((ushort)(element + yMul * y2 - 1));
                        }
                    }
                }
            }

            bucket.Clear();
            bucketPtr++;
        }
    }
}
