using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day17 : ISolver
    {
        const int X = 1 << 0; // 5 bits for X
        const int Y = 1 << 5; // 5 bits for Y
        const int Z = 1 << 10; // 4 bits for Z
        const int W = 1 << 14; // 4 bits for W

        public Solution Solve(ReadOnlySpan<char> input)
        {
            const int originX = 6;
            const int originY = 6;

            int width = input.IndexOf('\n');
            int height = input.Length / (width + 1);

            List<int> activeCubes = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (input[x + (y * (width + 1))] == '#')
                    {
                        activeCubes.Add((x + originX) * X + (y + originY) * Y);
                    }
                }
            }

            int part1 = SolvePart1(activeCubes, width, height);
            int part2 = SolvePart2(activeCubes, width, height);

            return new Solution(part1, part2);
        }

        private static int SolvePart1(List<int> inputActiveCubes, int width, int height)
        {
            const int originZ = 6;

            int activeCubeCount = 0;
            int[] activeCubes = new int[(width + 12) * (height + 12) * 13];
            foreach (int cube in inputActiveCubes)
            {
                activeCubes[activeCubeCount++] = cube + originZ * Z;
            }

            int[] nextActiveCubes = new int[activeCubes.Length];
            var counter = new Day17NeighbourCounter(activeCubes.Length, (originZ + 7) * Z);
            for (int iteration = 0; iteration < 6; iteration++)
            {
                counter.ResetNeighbours();

                Span<int> activeCubesSpan = activeCubes.AsSpan().Slice(0, activeCubeCount);
                foreach (int cube in activeCubesSpan)
                {
                    counter.UpdateNeighbourTotalsXYZ(cube);
                }

                int nextActiveCubesCount = 0;
                foreach (int cube in activeCubesSpan)
                {
                    if (counter.GetNeighbourTotal(cube) is 3 or 4)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = cube;
                        counter.ResetNeighbourTotal(cube); // reset neighbour total for the cube so it doesn't get triggered by the next loop
                    }
                }

                foreach (int cube in counter.Neighbours)
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

            return activeCubeCount;
        }

        private static int SolvePart2(List<int> inputActiveCubes, int width, int height)
        {
            const int originZ = 6;
            const int originW = 6;

            int activeCubeCount = 0;
            int[] activeCubes = new int[(width + 12) * (height + 12) * 13 * 13];
            foreach (int cube in inputActiveCubes)
            {
                activeCubes[activeCubeCount++] = cube + originZ * Z + originW * W;
            }

            int[] nextActiveCubes = new int[activeCubes.Length];
            var counter = new Day17NeighbourCounter(activeCubes.Length, (originW + 7) * W);
            for (int iteration = 0; iteration < 6; iteration++)
            {
                counter.ResetNeighbours();

                Span<int> activeCubesSpan = activeCubes.AsSpan().Slice(0, activeCubeCount);
                foreach (int cube in activeCubesSpan)
                {
                    counter.UpdateNeighbourTotalsXYZW(cube);
                }

                int nextActiveCubesCount = 0;
                foreach (int cube in activeCubesSpan)
                {
                    if (counter.GetNeighbourTotal(cube) is 3 or 4)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = cube;
                        counter.ResetNeighbourTotal(cube); // reset neighbour total for the cube so it doesn't get triggered by the next loop
                    }
                }

                foreach (int cube in counter.Neighbours)
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

            return activeCubeCount;
        }

        class Day17NeighbourCounter
        {
            private int[] _neighbours;
            private int _neighbourLen = 0;
            private int[] _neighbourTotals;

            public Day17NeighbourCounter(int maxNeighboursLen, int maxNeighbourValue)
            {
                _neighbours = new int[maxNeighboursLen];
                _neighbourTotals = new int[maxNeighbourValue + 1];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetNeighbours()
            {
                _neighbourLen = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateNeighbourTotalsXYZW(int pos)
            {
                UpdateNeighbourTotalsXYZ(pos - W);
                UpdateNeighbourTotalsXYZ(pos);
                UpdateNeighbourTotalsXYZ(pos + W);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateNeighbourTotalsXYZ(int pos)
            {
                UpdateNeighbourTotalsXY(pos - Z);
                UpdateNeighbourTotalsXY(pos);
                UpdateNeighbourTotalsXY(pos + Z);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateNeighbourTotalsXY(int pos)
            {
                UpdateNeighbourTotalsX(pos - Y);
                UpdateNeighbourTotalsX(pos);
                UpdateNeighbourTotalsX(pos + Y);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateNeighbourTotalsX(int pos)
            {
                UpdateNeighbourTotal(pos - X);
                UpdateNeighbourTotal(pos);
                UpdateNeighbourTotal(pos + X);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateNeighbourTotal(int pos)
            {
                if (_neighbourTotals[pos]++ == 0)
                {
                    _neighbours[_neighbourLen++] = pos;
                }
            }

            public ReadOnlySpan<int> Neighbours
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return _neighbours.AsSpan().Slice(0, _neighbourLen);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetNeighbourTotal(int pos) => _neighbourTotals[pos];


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetNeighbourTotal(int pos) => _neighbourTotals[pos] = 0;
        }
    }
}
