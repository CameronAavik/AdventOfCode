using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day22 : ISolver
{
    public readonly record struct Brick(byte X0, byte Y0, short Z0, byte X1, byte Y1, short Z1) : IComparable<Brick>
    {
        public readonly int CompareTo(Brick other) => Z0.CompareTo(other.Z0);
    }

    // Assumptions:
    // - There are 1500 bricks or less
    // - 0 <= x < 10
    // - 0 <= y < 10
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int width = 10;
        const int depth = 10;

        var bricksArray = new Brick[1500]; // max number of bricks I support
        var brickCount = 1; // leave first brick empty

        while (!input.IsEmpty)
            bricksArray[brickCount++] = ParseBrick(ref input);

        Array.Sort(bricksArray, 1, brickCount - 1);

        var xyPlane = new (short Height, short BrickId)[width * depth];
        Array.Fill(xyPlane, ((short)0, (short)-1));

        var dominators = new short[brickCount];
        var brickSupportedCounts = new short[brickCount];
        var bricksThatWillCauseFalls = new ulong[(brickCount - 1) / 64 + 1];
        var bricksOnTopOf = new short[width * depth];

        var part2 = 0;
        for (var i = 1; i < brickCount; i++)
        {
            var brick = bricksArray[i];
            var numBricksOnTopOf = 0;
            var maxHeight = 1;
            for (int y = brick.Y0; y <= brick.Y1; y++)
            {
                var offset = y * 10;
                for (int x = brick.X0; x <= brick.X1; x++)
                {
                    (var height, var brickId) = xyPlane[offset + x];
                    if (height > maxHeight)
                    {
                        bricksOnTopOf[0] = brickId;
                        numBricksOnTopOf = 1;
                        maxHeight = height;
                    }
                    else if (height == maxHeight && Array.IndexOf(bricksOnTopOf, brickId, 0, numBricksOnTopOf) < 0)
                    {
                        bricksOnTopOf[numBricksOnTopOf++] = brickId;
                    }
                }
            }

            if (numBricksOnTopOf > 0)
            {
                var dominator = bricksOnTopOf[0];
                if (numBricksOnTopOf == 1)
                {
                    bricksThatWillCauseFalls[dominator / 64] |= 1UL << dominator;
                }
                else
                {
                    while (true)
                    {
                        var nextDominator = bricksOnTopOf[0];
                        while (nextDominator > dominator)
                            nextDominator = dominators[nextDominator];

                        bricksOnTopOf[0] = nextDominator;

                        var allSame = true;
                        dominator = nextDominator;

                        for (var j = 1; j < numBricksOnTopOf; j++)
                        {
                            nextDominator = bricksOnTopOf[j];
                            while (nextDominator > dominator)
                                nextDominator = dominators[nextDominator];

                            bricksOnTopOf[j] = nextDominator;

                            if (nextDominator < dominator)
                            {
                                allSame = false;
                                dominator = nextDominator;
                                break;
                            }
                        }

                        if (allSame)
                            break;
                    }
                }

                dominators[i] = dominator;
                var numSupporting = brickSupportedCounts[dominator];
                part2 += numSupporting;
                brickSupportedCounts[i] = (short)(numSupporting + 1);
            }
            else
            {
                brickSupportedCounts[i] = 1;
            }

            var planeValue = ((short)(maxHeight + brick.Z1 - brick.Z0 + 1), (short)i);
            for (int y = brick.Y0; y <= brick.Y1; y++)
            {
                var offset = y * 10;
                for (int x = brick.X0; x <= brick.X1; x++)
                    xyPlane[offset + x] = planeValue;
            }
        }

        var part1 = brickCount - 1 - CountBits(bricksThatWillCauseFalls);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static Brick ParseBrick(ref ReadOnlySpan<byte> input)
    {
        byte c;

        var x0 = input[0] - '0';
        var y0 = input[2] - '0';
        var z0 = input[4] - '0';
        var i = 5;
        while ((c = input[i++]) != '~')
            z0 = 10 * z0 + c - '0';

        input = input[i..];

        var x1 = input[0] - '0';
        var y1 = input[2] - '0';
        var z1 = input[4] - '0';
        i = 5;
        while ((c = input[i++]) != '\n')
            z1 = 10 * z1 + c - '0';

        input = input[i..];

        return new Brick((byte)x0, (byte)y0, (short)z0, (byte)x1, (byte)y1, (short)z1);
    }

    private static int CountBits(Span<ulong> bitset)
    {
        var total = 0;
        for (var i = 0; i < bitset.Length; i++)
            total += BitOperations.PopCount(bitset[i]);
        return total;
    }
}
