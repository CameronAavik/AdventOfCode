using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day14 : ISolver
{
    // I apologise to anyone who tries to read this

    public class Region(byte x, byte y, byte count)
    {
        public static readonly Region Null = new(255, 255, 255);
        public byte X { get; } = x;
        public byte Y { get; } = y;
        public byte Count { get; set; } = count;
        public List<Region> Offsets { get; } = new List<Region>(16);
        public bool IsNull() => X == byte.MaxValue;
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ref byte inputRef = ref MemoryMarshal.GetReference(input);
        int width = input.IndexOf((byte)'\n');
        int height = input.Length / (width + 1);

        List<Region>[] horizontalRegions = new List<Region>[height];
        for (int i = 0; i < height; i++)
            horizontalRegions[i] = new List<Region>(width / 2);

        List<Region>[] verticalRegions = new List<Region>[width];
        for (int i = 0; i < width; i++)
            verticalRegions[i] = new List<Region>(height / 2);

        Span<Region> currentVerticalRegions = new Region[32];

        for (int x = 0; x < width; x += Vector256<byte>.Count)
        {
            int numElements = x + Vector256<byte>.Count < width ? 32 : (width % Vector256<byte>.Count);
            currentVerticalRegions.Fill(Region.Null);
            for (int y = 0; y < height; y++)
            {
                List<Region> horizontalRegionsForRow = horizontalRegions[y];

                uint rockBits;
                uint wallBits;
                if (x + Vector256<byte>.Count < width)
                {
                    var v = Vector256.LoadUnsafe(ref inputRef, (nuint)((y * (width + 1)) + x));
                    rockBits = Vector256.Equals(v, Vector256.Create((byte)'O')).ExtractMostSignificantBits();
                    wallBits = Vector256.Equals(v, Vector256.Create((byte)'#')).ExtractMostSignificantBits();
                }
                else
                {
                    rockBits = 0;
                    wallBits = 0;
                    for (int i = x; i < width; i++)
                    {
                        byte c = Unsafe.Add(ref inputRef, (nint)((y * (width + 1)) + i));
                        if (c == '#')
                            wallBits |= 1U << i;
                        else if (c == 'O')
                            rockBits |= 1U << i;
                    }
                }

                bool createAtEnd = true;
                int bitOffset = 0;

                // handle overlap with previous horizontal region on the same row
                if (horizontalRegionsForRow.Count > 0)
                {
                    int lastRegionIndex = horizontalRegionsForRow.Count - 1;
                    Region lastRegion = horizontalRegionsForRow[lastRegionIndex];
                    uint t = wallBits & (~wallBits + 1);

                    int wallOffset = Math.Min(numElements, BitOperations.TrailingZeroCount(wallBits));
                    if (wallOffset != 0)
                    {
                        List<Region> regionOffsets = lastRegion.Offsets;
                        for (int i = 0; i < wallOffset; i++)
                        {
                            Region currentVerticalRegion = currentVerticalRegions[i];
                            if (currentVerticalRegion.IsNull())
                                currentVerticalRegion = currentVerticalRegions[i] = new Region((byte)(x + i), (byte)y, 0);
                            currentVerticalRegion.Offsets.Add(lastRegion);
                            currentVerticalRegion.Count += (byte)((rockBits >> i) & 1);
                            regionOffsets.Add(currentVerticalRegion);
                        }
                    }
                    else if (lastRegion.Offsets.Count == 0)
                    {
                        horizontalRegionsForRow.RemoveAt(lastRegionIndex);
                    }

                    if (wallBits != 0)
                    {
                        Region wallVerticalRegion = currentVerticalRegions[wallOffset];
                        if (!wallVerticalRegion.IsNull())
                        {
                            verticalRegions[x + wallOffset].Add(wallVerticalRegion);
                            currentVerticalRegions[wallOffset] = Region.Null;
                        }
                    }
                    else
                    {
                        createAtEnd = false;
                    }

                    bitOffset = wallOffset + 1;
                    wallBits ^= t;
                }

                while (wallBits != 0)
                {
                    uint t = wallBits & (~wallBits + 1);
                    int wallOffset = BitOperations.TrailingZeroCount(wallBits);
                    if (wallOffset != bitOffset)
                    {
                        var region = new Region((byte)(x + bitOffset), (byte)y, 0);
                        for (int i = bitOffset; i < wallOffset; i++)
                        {
                            Region currentVerticalRegion = currentVerticalRegions[i];
                            if (currentVerticalRegion.IsNull())
                                currentVerticalRegion = currentVerticalRegions[i] = new Region((byte)(x + i), (byte)y, 0);
                            currentVerticalRegion.Offsets.Add(region);
                            currentVerticalRegion.Count += (byte)((rockBits >> i) & 1);
                            region.Offsets.Add(currentVerticalRegion);
                        }
                        horizontalRegionsForRow.Add(region);
                    }

                    Region wallVerticalRegion = currentVerticalRegions[wallOffset];
                    if (!wallVerticalRegion.IsNull())
                    {
                        verticalRegions[x + wallOffset].Add(wallVerticalRegion);
                        currentVerticalRegions[wallOffset] = Region.Null;
                    }

                    bitOffset = wallOffset + 1;
                    wallBits ^= t;
                }

                // handle last rocks
                if (createAtEnd)
                {
                    var region = new Region((byte)(x + bitOffset), (byte)y, 0);
                    for (int i = bitOffset; i < numElements; i++)
                    {
                        Region currentVerticalRegion = currentVerticalRegions[i];
                        if (currentVerticalRegion.IsNull())
                            currentVerticalRegion = currentVerticalRegions[i] = new Region((byte)(x + i), (byte)y, 0);
                        currentVerticalRegion.Offsets.Add(region);
                        currentVerticalRegion.Count += (byte)((rockBits >> i) & 1);
                        region.Offsets.Add(currentVerticalRegion);
                    }

                    if (region.Offsets.Count > 0 || x + Vector256<byte>.Count < width)
                        horizontalRegionsForRow.Add(region);
                }
            }

            for (int i = 0; i < numElements; i++)
            {
                Region verticalRegion = currentVerticalRegions[i];
                if (!verticalRegion.IsNull())
                    verticalRegions[x + i].Add(verticalRegion);
            }
        }

        // flatten the regions so that it can be iterated over faster
        Region[] horizontalRegionsFlattened = horizontalRegions.SelectMany(r => r).ToArray();
        Region[] verticalRegionsFlattened = verticalRegions.SelectMany(r => r).ToArray();

        int part1 = 0;
        foreach (Region region in verticalRegionsFlattened)
        {
            int upperScore = height - region.Y;
            int lowerScore = upperScore - region.Count + 1;
            part1 += (upperScore + lowerScore) * region.Count / 2;
        }

        solution.SubmitPart1(part1);

        // already tilted north in input
        TiltWithRocksAtStart(verticalRegionsFlattened);
        TiltWithRocksAtStart(horizontalRegionsFlattened);
        TiltWithRocksAtEnd(verticalRegionsFlattened);

        var d = new Dictionary<int, int>();
        var scores = new List<int>(200);

        Span<byte> hashInputBytes = new byte[horizontalRegionsFlattened.Length];

        int iterations = 0;
        while (true)
        {
            int score = 0;
            for (int i = 0; i < horizontalRegionsFlattened.Length; i++)
            {
                Region region = horizontalRegionsFlattened[i];
                hashInputBytes[i] = region.Count;
                score += region.Count * (height - region.Y);
            }

            var hashCode = new HashCode();
            hashCode.AddBytes(hashInputBytes);
            int hash = hashCode.ToHashCode();

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
                scores.Add(score);
            }

            TiltWithRocksAtEnd(horizontalRegionsFlattened);
            TiltWithRocksAtStart(verticalRegionsFlattened);
            TiltWithRocksAtStart(horizontalRegionsFlattened);
            TiltWithRocksAtEnd(verticalRegionsFlattened);

            iterations++;
        }
    }

    private static void TiltWithRocksAtEnd(Region[] regions)
    {
        ref Region regionRef = ref MemoryMarshal.GetArrayDataReference(regions);
        for (int i = 0; i < regions.Length; i++)
        {
            Region region = Unsafe.Add(ref regionRef, i);
            Span<Region> offsets = CollectionsMarshal.AsSpan(region.Offsets);
            ref Region offsetsRef = ref MemoryMarshal.GetReference(offsets);
            for (nint j = offsets.Length - 1; j >= offsets.Length - region.Count; j--)
                Unsafe.Add(ref offsetsRef, j).Count++;

            region.Count = 0;
        }
    }

    private static void TiltWithRocksAtStart(Region[] regions)
    {
        ref Region regionRef = ref MemoryMarshal.GetArrayDataReference(regions);
        for (int i = 0; i < regions.Length; i++)
        {
            Region region = Unsafe.Add(ref regionRef, i);
            Span<Region> offsets = CollectionsMarshal.AsSpan(region.Offsets);
            ref Region offsetsRef = ref MemoryMarshal.GetReference(offsets);
            for (nint j = 0; j < region.Count; j++)
                Unsafe.Add(ref offsetsRef, j).Count++;

            region.Count = 0;
        }
    }
}
