using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
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

        // set first and last row to all walls to make things easier later
        for (int i = 0; i < 4; i++)
        {
            walls[i] = 0xFFFFFFFFU;
            walls[(height + 1) * 4 + i] = 0xFFFFFFFFU;
        }

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
            // if the grid is smaller than 96x95, fill teh rest with walls
            for (; index < 4; index++)
                walls[index] = 0xFFFFFFFFU;

            inputRef = ref Unsafe.Add(ref inputRef, rowLength);
        }

        TiltNorthWest();

        int part1 = ScoreGrid();
        solution.SubmitPart1(part1);

        TiltSouthEast();

        var d = new Dictionary<int, int>(250);
        var scores = new List<int>(250);

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
                Vector128<uint> wallsVec = Vector128.LoadUnsafe(ref wallsRef);
                ref uint nextRocksRef = ref Unsafe.Add(ref rocksRef, 4);
                ref uint nextWallsRef = ref Unsafe.Add(ref wallsRef, 4);
                Vector128<uint> freeSpace = ~(rocksVec | wallsVec | Vector128.LoadUnsafe(ref nextWallsRef) | prevFreeSpace);
                while (freeSpace != Vector128<uint>.Zero)
                {
                    Vector128<uint> newRocksVec = Vector128.LoadUnsafe(ref nextRocksRef);
                    Vector128<uint> rocksToAdd = newRocksVec & freeSpace;
                    rocksVec |= rocksToAdd;
                    Vector128.StoreUnsafe(newRocksVec ^ rocksToAdd, ref nextRocksRef);
                    nextRocksRef = ref Unsafe.Add(ref nextRocksRef, 4);
                    nextWallsRef = ref Unsafe.Add(ref nextWallsRef, 4);
                    freeSpace &= ~Vector128.LoadUnsafe(ref nextWallsRef) & ~rocksToAdd;
                }

                Vector128<uint> tiltedWest = TiltRowWest(wallsVec, rocksVec);
                Vector128.StoreUnsafe(tiltedWest, ref rocksRef);
                prevFreeSpace = ~(rocksVec | wallsVec);
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
                        Vector128<uint> shifted = ShiftLeftByOne(walls) | Vector128.Create(1U, 0U, 0U, 0U); // add fake wall on left
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
                Vector128<uint> wallsVec = Vector128.LoadUnsafe(ref wallsRef);
                ref uint nextRocksRef = ref Unsafe.Subtract(ref rocksRef, 4);
                ref uint nextWallsRef = ref Unsafe.Subtract(ref wallsRef, 4);
                Vector128<uint> freeSpace = ~(rocksVec | wallsVec | Vector128.LoadUnsafe(ref nextWallsRef) | prevFreeSpace);
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

                Vector128<uint> tiltedEast = TiltRowEast(wallsVec, rocksVec);
                Vector128.StoreUnsafe(tiltedEast, ref rocksRef);

                prevFreeSpace = ~(rocksVec | wallsVec);
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

        static Vector128<uint> ShiftLeftByOne(Vector128<uint> v) => (v << 1) | Sse2.ShiftLeftLogical128BitLane(v >> 31, 4);

        static Vector128<uint> ShiftRightByOne(Vector128<uint> v) => (v >> 1) | Sse2.ShiftRightLogical128BitLane(v << 31, 4);
    }
}
