using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day17 : ISolver
{
    const int X = 1 << 0; // 5 bits for X
    const int Y = 1 << 5; // 5 bits for Y
    const int Z = 1 << 10; // 3 bits for Z
    const int W = 1 << 13; // 3 bits for W

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int originX = 6;
        const int originY = 6;

        var width = input.IndexOf((byte)'\n');
        var height = input.Length / (width + 1);

        List<int> activeCubes = [];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (input[x + (y * (width + 1))] == '#')
                {
                    activeCubes.Add((x + originX) * X + (y + originY) * Y);
                }
            }
        }

        var part1 = SolvePart1(activeCubes, width, height);
        var part2 = SolvePart2(activeCubes, width, height);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int SolvePart1(List<int> inputActiveCubes, int width, int height)
    {
        const int originZ = 0;

        var activeCubeCount = 0;
        var activeCubes = new int[(width + 12) * (height + 12) * 7];
        foreach (var cube in inputActiveCubes)
        {
            activeCubes[activeCubeCount++] = cube + originZ * Z;
        }

        var nextActiveCubes = new int[activeCubes.Length];
        var counter = new NeighbourCounter(activeCubes.Length, (originZ + 7) * Z);
        for (var iteration = 0; iteration < 6; iteration++)
        {
            counter.ResetNeighbours();

            var activeCubesSpan = activeCubes.AsSpan()[..activeCubeCount];
            foreach (var cube in activeCubesSpan)
            {
                counter.UpdateNeighbourTotalsXYZ(cube);
            }

            var nextActiveCubesCount = 0;
            foreach (var cube in activeCubesSpan)
            {
                if (counter.GetNeighbourTotal(cube) is 3 or 4)
                {
                    nextActiveCubes[nextActiveCubesCount++] = cube;
                    counter.ResetNeighbourTotal(cube); // reset neighbour total for the cube so it doesn't get triggered by the next loop
                }
            }

            foreach (var cube in counter.Neighbours)
            {
                if (counter.GetNeighbourTotal(cube) == 3)
                {
                    nextActiveCubes[nextActiveCubesCount++] = cube;
                }

                counter.ResetNeighbourTotal(cube);
            }

            activeCubes = nextActiveCubes;
            activeCubeCount = nextActiveCubesCount;
        }

        var totalCubeCount = 0;
        for (var i = 0; i < activeCubeCount; i++)
        {
            var cube = activeCubes[i];

            var z = cube >> 10;
            if (z == 0)
            {
                totalCubeCount += 1;
            }
            else
            {
                totalCubeCount += 2;
            }
        }

        return totalCubeCount;
    }

    private static int SolvePart2(List<int> inputActiveCubes, int width, int height)
    {
        const int originZ = 0;
        const int originW = 0;

        var activeCubeCount = 0;
        var activeCubes = new int[(width + 12) * (height + 12) * 7 * 7];
        foreach (var cube in inputActiveCubes)
        {
            activeCubes[activeCubeCount++] = cube + originZ * Z + originW * W;
        }

        var nextActiveCubes = new int[activeCubes.Length];
        var counter = new NeighbourCounter(activeCubes.Length, (originW + 7) * W);
        for (var iteration = 0; iteration < 6; iteration++)
        {
            counter.ResetNeighbours();

            var activeCubesSpan = activeCubes.AsSpan()[..activeCubeCount];
            foreach (var cube in activeCubesSpan)
            {
                counter.UpdateNeighbourTotalsXYZW(cube);
            }

            var nextActiveCubesCount = 0;
            foreach (var cube in activeCubesSpan)
            {
                if (counter.GetNeighbourTotal(cube) is 3 or 4)
                {
                    nextActiveCubes[nextActiveCubesCount++] = cube;
                    counter.ResetNeighbourTotal(cube); // reset neighbour total for the cube so it doesn't get triggered by the next loop
                }
            }

            foreach (var cube in counter.Neighbours)
            {
                if (counter.GetNeighbourTotal(cube) == 3)
                {
                    nextActiveCubes[nextActiveCubesCount++] = cube;
                }

                counter.ResetNeighbourTotal(cube);
            }

            activeCubes = nextActiveCubes;
            activeCubeCount = nextActiveCubesCount;
        }

        var zwPlane = 0;
        var wAxis = 0;
        var wzAxis = 0;
        var other = 0;
        for (var i = 0; i < activeCubeCount; i++)
        {
            var cube = activeCubes[i];

            var z = (cube >> 10) & 0b111;
            var w = cube >> 13;

            if (z == 0 && w == 0)
            {
                zwPlane++;
            }
            else if (w == 0)
            {
                wAxis++;
            }
            else if (w == z)
            {
                wzAxis++;
            }
            else
            {
                other++;
            }
        }

        return zwPlane + 4 * wAxis + 4 * wzAxis + 8 * other;
    }

    class NeighbourCounter(int maxNeighboursLen, int maxNeighbourValue)
    {
        private readonly int[] _neighbours = new int[maxNeighboursLen];
        private readonly int[] _neighbourTotals = new int[maxNeighbourValue + 1];
        private int _neighbourLen;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetNeighbours()
        {
            _neighbourLen = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNeighbourTotalsXYZW(int pos)
        {
            var w = pos >> 13;
            var z = (pos >> 10) & 0b111;

            // +w, +z
            UpdateNeighbourTotalsXY(pos + W + Z, 1);

            // +w
            if (w < z)
            {
                UpdateNeighbourTotalsXY(pos + W, z != w + 1 ? 1 : 2);
            }

            // +w, -z
            if (w + 1 < z)
            {
                UpdateNeighbourTotalsXY(pos + W - Z, z != w + 2 ? 1 : 2);
            }

            // +z
            UpdateNeighbourTotalsXY(pos + Z);

            // +0

            UpdateNeighbourTotalsXY(pos, z != w + 1 ? 1 : (w != 0 ? 2 : 3));

            // -z
            if (w < z)
            {
                var inc = 1;
                if (z == 1)
                    inc *= 2;
                if (z == w + 1)
                    inc *= 2;
                UpdateNeighbourTotalsXY(pos - Z, inc);
            }

            // -w
            if (w != 0)
            {
                UpdateNeighbourTotalsXYZ(pos - W, w != 1 ? 1 : 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNeighbourTotalsXYZ(int pos, int inc = 1)
        {
            var z = (pos >> 10) & 0b111;

            if (z > 0)
            {
                UpdateNeighbourTotalsXY(pos - Z, z != 1 ? inc : inc * 2);
            }

            UpdateNeighbourTotalsXY(pos, inc);
            UpdateNeighbourTotalsXY(pos + Z, inc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNeighbourTotalsXY(int pos, int inc = 1)
        {
            UpdateNeighbourTotalsX(pos - Y, inc);
            UpdateNeighbourTotalsX(pos, inc);
            UpdateNeighbourTotalsX(pos + Y, inc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNeighbourTotalsX(int pos, int inc = 1)
        {
            UpdateNeighbourTotal(pos - X, inc);
            UpdateNeighbourTotal(pos, inc);
            UpdateNeighbourTotal(pos + X, inc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateNeighbourTotal(int pos, int inc = 1)
        {
            if ((_neighbourTotals[pos] += inc) == inc)
            {
                _neighbours[_neighbourLen++] = pos;
            }
        }

        public ReadOnlySpan<int> Neighbours
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _neighbours.AsSpan()[.._neighbourLen];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNeighbourTotal(int pos) => _neighbourTotals[pos];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetNeighbourTotal(int pos) => _neighbourTotals[pos] = 0;
    }
}
