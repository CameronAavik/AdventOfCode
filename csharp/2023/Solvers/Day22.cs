using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day22 : ISolver
{
    public readonly record struct Coord(byte X, byte Y, short Z);
    public readonly record struct Brick(short Id, Coord Min, Coord Max);

    // Assumptions:
    // - There are 1500 bricks or less
    // - 0 <= x < 10
    // - 0 <= y < 10
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int width = 10;
        const int depth = 10;

        var bricksArray = new Brick[1500]; // max number of bricks I support

        short brickCount = 0;
        int inputIndex = 0;
        while (inputIndex < input.Length)
        {
            ParseLine(input, ref inputIndex, out byte x0, out byte y0, out short z0, out byte x1, out byte y1, out short z1);
            var minCoord = new Coord(x0, y0, z0);
            var maxCoord = new Coord(x1, y1, z1);
            var brick = new Brick(brickCount, minCoord, maxCoord);
            bricksArray[brickCount++] = brick;
        }

        Span<Brick> bricks = bricksArray.AsSpan().Slice(0, brickCount);
        bricks.Sort((x, y) => x.Min.Z.CompareTo(y.Min.Z));

        var xyPlane = new (short Height, short BrickId)[width * depth];
        Array.Fill(xyPlane, ((short)0, (short)-1));

        int brickBitsetLength = (bricks.Length - 1) / 64 + 1;
        Span<ulong> dependenciesPerBrick = new ulong[brickBitsetLength * brickCount];
        Span<ulong> bricksThatWillCauseFalls = new ulong[brickBitsetLength];

        short[] bricksOnTopOf = new short[width * depth];
        int numBricksOnTopOf = 0;
        for (int i = 0; i < bricks.Length; i++)
        {
            Brick brick = bricks[i];
            int brickHeight = brick.Max.Z - brick.Min.Z + 1;
            numBricksOnTopOf = 0;
            int maxHeight = 1;
            for (int y = brick.Min.Y; y <= brick.Max.Y; y++)
            {
                int offset = y * 10;
                for (int x = brick.Min.X; x <= brick.Max.X; x++)
                {
                    (short height, short topBrickId) = xyPlane[offset + x];
                    if (height > maxHeight)
                    {
                        bricksOnTopOf[0] = topBrickId;
                        numBricksOnTopOf = 1;
                        maxHeight = height;
                    }
                    else if (height == maxHeight)
                    {
                        bricksOnTopOf[numBricksOnTopOf++] = topBrickId;
                    }
                }
            }

            Span<ulong> brickDependencies = dependenciesPerBrick.Slice(i * brickBitsetLength, brickBitsetLength);

            if (numBricksOnTopOf > 0)
            {
                Array.Sort(bricksOnTopOf, 0, numBricksOnTopOf);

                int prev = bricksOnTopOf[0];
                Span<ulong> firstDependency = dependenciesPerBrick.Slice(prev * brickBitsetLength, brickBitsetLength);
                firstDependency.CopyTo(brickDependencies);

                int count = 1;
                for (int j = 1; j < numBricksOnTopOf; j++)
                {
                    int brickOnTopOf = bricksOnTopOf[j];
                    if (brickOnTopOf == prev)
                        continue;

                    prev = brickOnTopOf;
                    Span<ulong> newDependency = dependenciesPerBrick.Slice(prev * brickBitsetLength, brickBitsetLength);
                    AndBitsets(brickDependencies, newDependency);
                    count++;
                }

                if (count == 1)
                    bricksThatWillCauseFalls[prev / 64] |= 1UL << prev;
            }

            brickDependencies[i / 64] |= 1UL << i;

            short newHeight = (short)(maxHeight + brickHeight);
            for (int y = brick.Min.Y; y <= brick.Max.Y; y++)
            {
                int offset = y * 10;
                for (int x = brick.Min.X; x <= brick.Max.X; x++)
                    xyPlane[offset + x] = (newHeight, (short)i);
            }
        }

        int part1 = bricks.Length - CountBits(bricksThatWillCauseFalls);
        solution.SubmitPart1(part1);

        int part2 = CountBits(dependenciesPerBrick) - bricks.Length;
        solution.SubmitPart2(part2);
    }

    private static void ParseLine(ReadOnlySpan<byte> input, ref int i, out byte x0, out byte y0, out short z0, out byte x1, out byte y1, out short z1)
    {
        byte c;

        x0 = (byte)(input[i++] - '0');
        i++;

        y0 = (byte)(input[i++] - '0');
        i++;

        z0 = (short)(input[i++] - '0');
        while ((c = input[i++]) != '~')
            z0 = (short)(10 * z0 + c - '0');

        x1 = (byte)(input[i++] - '0');
        i++;

        y1 = (byte)(input[i++] - '0');
        i++;

        z1 = (short)(input[i++] - '0');
        while ((c = input[i++]) != '\n')
            z1 = (short)(10 * z1 + c - '0');
    }

    private static void AndBitsets(Span<ulong> bitset1, Span<ulong> bitset2)
    {
        ref ulong bitset1Ref = ref MemoryMarshal.GetReference(bitset1);
        ref ulong bitset2Ref = ref MemoryMarshal.GetReference(bitset2);
        ref ulong bitset1End = ref Unsafe.Add(ref bitset1Ref, bitset1.Length);
        ref ulong oneAwayFromEnd = ref Unsafe.Subtract(ref bitset1End, Vector256<ulong>.Count);

        while (!Unsafe.IsAddressGreaterThan(ref bitset1Ref, ref oneAwayFromEnd))
        {
            var v1 = Vector256.LoadUnsafe(ref bitset1Ref);
            var v2 = Vector256.LoadUnsafe(ref bitset2Ref);
            Vector256.StoreUnsafe(v1 & v2, ref bitset1Ref);

            bitset1Ref = ref Unsafe.Add(ref bitset1Ref, Vector256<ulong>.Count);
            bitset2Ref = ref Unsafe.Add(ref bitset2Ref, Vector256<ulong>.Count);
        }

        while (Unsafe.IsAddressLessThan(ref bitset1Ref, ref bitset1End))
        {
            bitset1Ref &= bitset2Ref;
            bitset1Ref = ref Unsafe.Add(ref bitset1Ref, 1);
            bitset2Ref = ref Unsafe.Add(ref bitset2Ref, 1);
        }
    }

    private static int CountBits(Span<ulong> bitset)
    {
        int total = 0;
        for (int i = 0; i < bitset.Length; i++)
            total += BitOperations.PopCount(bitset[i]);
        return total;
    }
}
