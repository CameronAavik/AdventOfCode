using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day17 : ISolver
    {
        const int X = 1 << 0;
        const int Y = 1 << 5;
        const int Z = 1 << 10;
        const int W = 1 << 15;

        public Solution Solve(ReadOnlySpan<char> input)
        {
            const int originX = 8;
            const int originY = 8;

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
            const int originZ = 8;

            int activeCubeCount = 0;
            int[] activeCubes = new int[(width + 12) * (height + 12) * 13];
            foreach (int cube in inputActiveCubes)
            {
                activeCubes[activeCubeCount++] = cube + originZ * Z;
            }

            int[] nextActiveCubes = new int[activeCubes.Length];
            var neighbourTotals = new Dictionary<int, int>();
            for (int iteration = 0; iteration < 6; iteration++)
            {
                neighbourTotals.Clear();
                for (int i = 0; i < activeCubeCount; i++)
                {
                    UpdateNeighbourTotalsXYZ(activeCubes[i], neighbourTotals);
                }

                int nextActiveCubesCount = 0;
                for (int i = 0; i < activeCubeCount; i++)
                {
                    int cube = activeCubes[i];
                    if (neighbourTotals.Remove(cube, out int neighbours) && neighbours is 3 or 4)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = cube;
                    }
                }

                foreach ((int inactiveCube, int neighbours) in neighbourTotals)
                {
                    if (neighbours == 3)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = inactiveCube;
                    }
                }

                activeCubes = nextActiveCubes;
                activeCubeCount = nextActiveCubesCount;
            }

            return activeCubeCount;
        }

        private static int SolvePart2(List<int> inputActiveCubes, int width, int height)
        {
            const int originZ = 8;
            const int originW = 8;

            int activeCubeCount = 0;
            int[] activeCubes = new int[(width + 12) * (height + 12) * 13 * 13];
            foreach (int cube in inputActiveCubes)
            {
                activeCubes[activeCubeCount++] = cube + originZ * Z + originW * W;
            }

            int[] nextActiveCubes = new int[activeCubes.Length];
            var neighbourTotals = new Dictionary<int, int>();
            for (int iteration = 0; iteration < 6; iteration++)
            {
                neighbourTotals.Clear();
                for (int i = 0; i < activeCubeCount; i++)
                {
                    UpdateNeighbourTotalsXYZW(activeCubes[i], neighbourTotals);
                }

                int nextActiveCubesCount = 0;
                for (int i = 0; i < activeCubeCount; i++)
                {
                    int cube = activeCubes[i];
                    if (neighbourTotals.Remove(cube, out int neighbours) && neighbours is 3 or 4)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = cube;
                    }
                }

                foreach ((int inactiveCube, int neighbours) in neighbourTotals)
                {
                    if (neighbours == 3)
                    {
                        nextActiveCubes[nextActiveCubesCount++] = inactiveCube;
                    }
                }

                activeCubes = nextActiveCubes;
                activeCubeCount = nextActiveCubesCount;
            }

            return activeCubeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateNeighbourTotalsXYZW(int pos, Dictionary<int, int> neighbourTotals)
        {
            UpdateNeighbourTotalsXYZ(pos - W, neighbourTotals);
            UpdateNeighbourTotalsXYZ(pos, neighbourTotals);
            UpdateNeighbourTotalsXYZ(pos + W, neighbourTotals);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateNeighbourTotalsXYZ(int pos, Dictionary<int, int> neighbourTotals)
        {
            UpdateNeighbourTotalsXY(pos - Z, neighbourTotals);
            UpdateNeighbourTotalsXY(pos, neighbourTotals);
            UpdateNeighbourTotalsXY(pos + Z, neighbourTotals);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateNeighbourTotalsXY(int pos, Dictionary<int, int> neighbourTotals)
        {
            UpdateNeighbourTotalsX(pos - Y, neighbourTotals);
            UpdateNeighbourTotalsX(pos, neighbourTotals);
            UpdateNeighbourTotalsX(pos + Y, neighbourTotals);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateNeighbourTotalsX(int pos, Dictionary<int, int> neighbourTotals)
        {
            UpdateNeighbourTotal(pos - X, neighbourTotals);
            UpdateNeighbourTotal(pos, neighbourTotals);
            UpdateNeighbourTotal(pos + X, neighbourTotals);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateNeighbourTotal(int pos, Dictionary<int, int> neighbourTotals)
        {
            neighbourTotals[pos] = neighbourTotals.GetValueOrDefault(pos) + 1;
        }
    }
}
