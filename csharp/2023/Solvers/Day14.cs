using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day14 : ISolver
{
    // Assumes grid width <= 131
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int width = input.IndexOf((byte)'\n');
        int rowLength = width + 1;
        int height = input.Length / rowLength;
        uint[] walls = new uint[(height + 2) * 4];
        uint[] rocks = new uint[(height + 2) * 4];

        Array.Fill(walls, 0xFFFFFFFFU);

        ref byte inputRef = ref MemoryMarshal.GetReference(input);

        for (int i = 0; i < height; i++)
        {
            int index = 4 * (i + 1);
            for (int j = 0; j + Vector256<byte>.Count < width; j += Vector256<byte>.Count, index++)
            {
                var v = Vector256.LoadUnsafe(ref Unsafe.Add(ref inputRef, j));
                walls[index] = Vector256.Equals(v, Vector256.Create((byte)'#')).ExtractMostSignificantBits();
                rocks[index] = Vector256.Equals(v, Vector256.Create((byte)'O')).ExtractMostSignificantBits();
            }

            uint lastWallsRow = ~((1U << width) - 1); // add fake walls on the edges
            uint lastRocksRow = 0;
            for (int j = (index % 4) * Vector256<byte>.Count; j < width; j++)
            {
                byte c = Unsafe.Add(ref inputRef, j);
                lastWallsRow |= (c == '#' ? 1U : 0U) << j;
                lastRocksRow |= (c == 'O' ? 1U : 0U) << j;
            }

            walls[index] = lastWallsRow;
            rocks[index] = lastRocksRow;

            // add wall on the left side
            uint wallsCarry = 1;
            uint rocksCarry = 0;
            for (int j = 4 * (i + 1); j <= index; j++)
            {
                uint prev = walls[j];
                walls[j] = (prev << 1) | wallsCarry;
                wallsCarry = prev >> 31;

                prev = rocks[j];
                rocks[j] = (prev << 1) | rocksCarry;
                rocksCarry = prev >> 31;
            }

            inputRef = ref Unsafe.Add(ref inputRef, rowLength);
        }

        TiltNorthWest();

        int part1 = ScoreGrid();
        solution.SubmitPart1(part1);

        TiltSouthEast();

        var d = new Dictionary<int, int>(300);
        var scores = new List<int>(300);

        int iterations = 0;
        while (true)
        {
            int hash = HashGrid();

            if (d.TryGetValue(hash, out int j))
            {
                int cycleLen = iterations - j;
                int cycleOffset = (1000000000 - iterations) % cycleLen;
                solution.SubmitPart2(scores[j + cycleOffset - 1]);
                break;
            }
            else
            {
                d[hash] = iterations;
                scores.Add(ScoreGrid());
            }

            TiltNorthWest();
            TiltSouthEast();

            iterations++;
        }

        int ScoreGrid()
        {
            int score = 0;
            for (int i = 4; i < rocks.Length - 4; i++)
                score += BitOperations.PopCount(rocks[i]) * (height - (i / 4) + 1);
            return score;
        }

        int HashGrid()
        {
            var hashCode = new HashCode();
            hashCode.AddBytes(MemoryMarshal.Cast<uint, byte>(rocks.AsSpan().Slice(4, rocks.Length - 8)));
            return hashCode.ToHashCode();
        }

        void TiltNorthWest()
        {
            ref uint rocksRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(rocks), 4);
            ref uint wallsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(walls), 4);
            Vector128<uint> prevFreeSpace = Vector128<uint>.Zero;
            for (int row = 0; row < height; row++)
            {
                Vector128<uint> rocksVec = Vector128.LoadUnsafe(ref rocksRef);
                ref uint nextRocksRef = ref Unsafe.Add(ref rocksRef, 4);
                ref uint nextWallsRef = ref Unsafe.Add(ref wallsRef, 4);
                Vector128<uint> freeSpace = ~(rocksVec | Vector128.LoadUnsafe(ref wallsRef) | Vector128.LoadUnsafe(ref nextWallsRef) | prevFreeSpace);
                while (freeSpace != Vector128<uint>.Zero)
                {
                    Vector128<uint> rocksToAdd = Vector128.LoadUnsafe(ref nextRocksRef) & freeSpace;
                    rocksVec |= rocksToAdd;
                    Vector128.StoreUnsafe(Vector128.LoadUnsafe(ref nextRocksRef) ^ rocksToAdd, ref nextRocksRef);
                    nextRocksRef = ref Unsafe.Add(ref nextRocksRef, 4);
                    nextWallsRef = ref Unsafe.Add(ref nextWallsRef, 4);
                    freeSpace &= ~Vector128.LoadUnsafe(ref nextWallsRef) & ~rocksToAdd;
                }

                Vector128<uint> tiltedWest = TiltRowWest(Vector128.LoadUnsafe(ref wallsRef), rocksVec);
                Vector128.StoreUnsafe(tiltedWest, ref rocksRef);
                prevFreeSpace = ~(rocksVec | Vector128.LoadUnsafe(ref wallsRef));
                rocksRef = ref Unsafe.Add(ref rocksRef, 4);
                wallsRef = ref Unsafe.Add(ref wallsRef, 4);
            }

            static Vector128<uint> TiltRowWest(Vector128<uint> walls, Vector128<uint> rocks)
            {
                while (true)
                {
                    // mark any rocks that are in their correct place as walls
                    while (true)
                    {
                        Vector128<uint> shifted = ShiftLeftByOne(walls);
                        Vector128<uint> newWalls = walls | (shifted & rocks);
                        if (walls == newWalls)
                            break;
                        walls = newWalls;
                    }

                    Vector128<uint> movingRocks = ~walls & rocks;
                    if (movingRocks == Vector128<uint>.Zero)
                        break;

                    rocks = (rocks & walls) | ShiftRightByOne(movingRocks);
                }

                return rocks;
            }
        }

        void TiltSouthEast()
        {
            ref uint rocksRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(rocks), height * 4);
            ref uint wallsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(walls), height * 4);
            Vector128<uint> prevFreeSpace = Vector128<uint>.Zero;
            for (int row = 0; row < height; row++)
            {
                Vector128<uint> rocksVec = Vector128.LoadUnsafe(ref rocksRef);
                ref uint nextRocksRef = ref Unsafe.Subtract(ref rocksRef, 4);
                ref uint nextWallsRef = ref Unsafe.Subtract(ref wallsRef, 4);
                Vector128<uint> freeSpace = ~(rocksVec | Vector128.LoadUnsafe(ref wallsRef) | Vector128.LoadUnsafe(ref nextWallsRef) | prevFreeSpace);
                while (freeSpace != Vector128<uint>.Zero)
                {
                    Vector128<uint> newRocksVec = Vector128.LoadUnsafe(ref nextRocksRef);
                    Vector128<uint> rocksToAdd = newRocksVec & freeSpace;
                    rocksVec |= rocksToAdd;
                    Vector128.StoreUnsafe(newRocksVec ^ rocksToAdd, ref nextRocksRef);
                    nextRocksRef = ref Unsafe.Subtract(ref nextRocksRef, 4);
                    nextWallsRef = ref Unsafe.Subtract(ref nextWallsRef, 4);
                    freeSpace &= ~Vector128.LoadUnsafe(ref nextWallsRef) & ~rocksToAdd;
                }

                Vector128<uint> tiltedEast = TiltRowEast(Vector128.LoadUnsafe(ref wallsRef), rocksVec);
                Vector128.StoreUnsafe(tiltedEast, ref rocksRef);

                prevFreeSpace = ~(rocksVec | Vector128.LoadUnsafe(ref wallsRef));
                rocksRef = ref Unsafe.Subtract(ref rocksRef, 4);
                wallsRef = ref Unsafe.Subtract(ref wallsRef, 4);
            }

            static Vector128<uint> TiltRowEast(Vector128<uint> walls, Vector128<uint> rocks)
            {
                while (true)
                {
                    // mark any rocks that are in their correct place as walls
                    while (true)
                    {
                        Vector128<uint> shifted = ShiftRightByOne(walls);
                        Vector128<uint> newWalls = walls | (shifted & rocks);
                        if (walls == newWalls)
                            break;
                        walls = newWalls;
                    }

                    Vector128<uint> movingRocks = ~walls & rocks;
                    if (movingRocks == Vector128<uint>.Zero)
                        break;

                    rocks = (rocks & walls) | ShiftLeftByOne(movingRocks);
                }

                return rocks;
            }
        }

        static Vector128<uint> ShiftLeftByOne(Vector128<uint> v) => (v << 1) | Vector128.Shuffle(v >> 31, Vector128.Create(3U, 0U, 1U, 2U));

        static Vector128<uint> ShiftRightByOne(Vector128<uint> v) => (v >> 1) | Vector128.Shuffle(v << 31, Vector128.Create(1U, 2U, 3U, 0U));
    }
}
