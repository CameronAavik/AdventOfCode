using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day08 : ISolver
{
    public record struct Coord(int X, int Y, int Z)
    {
        public readonly long DistanceSquaredTo(Coord other)
        {
            var xDiff = X - other.X;
            var yDiff = Y - other.Y;
            var zDiff = Z - other.Z;
            return (long)xDiff * xDiff + (long)yDiff * yDiff + (long)zDiff * zDiff;
        }
    }

    public record struct Edge(int From, int To);

    public class DisjointSet
    {
        // Union by size disjoint set
        private readonly int[] _parent;
        private readonly int[] _size;

        public DisjointSet(int size)
        {
            _parent = new int[size];
            for (var i = 0; i < size; i++)
                _parent[i] = i;
            _size = new int[size];
            Array.Fill(_size, 1);
        }

        public int Find(int i)
        {
            if (_parent[i] != i)
                _parent[i] = Find(_parent[i]);
            return _parent[i];
        }

        public bool Union(int a, int b)
        {
            var rootA = Find(a);
            var rootB = Find(b);
            if (rootA == rootB)
                return false;

            var sizeA = _size[rootA];
            var sizeB = _size[rootB];
            if (sizeA < sizeB)
            {
                _parent[rootA] = rootB;
                _size[rootB] += sizeA;
            }
            else
            {
                _parent[rootB] = rootA;
                _size[rootA] += sizeB;
            }

            return true;
        }

        public int ComputePart1()
        {
            var l1 = 1;
            var l2 = 1;
            var l3 = 1;
            for (var i = 0; i < _parent.Length; i++)
            {
                if (_parent[i] != i)
                    continue;

                var size = _size[i];
                if (size > l1)
                {
                    l3 = l2;
                    l2 = l1;
                    l1 = size;
                }
                else if (size > l2)
                {
                    l3 = l2;
                    l2 = size;
                }
                else if (size > l3)
                {
                    l3 = size;
                }
            }

            var part1 = l1 * l2 * l3;
            return part1;
        }
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var coords = ParseCoordinates(input, out var maxCoord);
        var partitions = PartitionCoordinates(coords, maxCoord, out var partitionSize);
        var edges = GetEdgesFromNeighbouringPartitions(coords, partitions, partitionSize);

        var disjointSet = new DisjointSet(coords.Count);

        var count = 0;
        for (var i = 0; i < 1000; i++)
        {
            var (from, to) = edges.Dequeue();
            if (disjointSet.Union(from, to))
                count++;
        }

        var part1 = disjointSet.ComputePart1();
        solution.SubmitPart1(part1);

        for (var i = 1000; i < edges.Count; i++)
        {
            var (from, to) = edges.Dequeue();
            if (disjointSet.Union(from, to))
            {
                count++;
                if (count == coords.Count - 1)
                {
                    var part2 = (long)coords[from].X * coords[to].X;
                    solution.SubmitPart2(part2);
                    return;
                }
            }

            if (i == edges.Count - 1)
                AddRemainingEdges(coords, edges, disjointSet);
        }
    }

    private static List<Coord> ParseCoordinates(ReadOnlySpan<byte> input, out int maxCoord)
    {
        var coords = new List<Coord>(input.Length / 16);
        maxCoord = 0;
        var inputIndex = 0;
        while (inputIndex < input.Length)
        {
            var x = ParseIntUntil(input, (byte)',', ref inputIndex);
            var y = ParseIntUntil(input, (byte)',', ref inputIndex);
            var z = ParseIntUntil(input, (byte)'\n', ref inputIndex);

            coords.Add(new Coord(x, y, z));
            maxCoord = Math.Max(maxCoord, Math.Max(x, Math.Max(y, z)));
        }

        return coords;
    }

    private static int ParseIntUntil(ReadOnlySpan<byte> input, byte endChar, ref int i)
    {
        var value = input[i++] - (byte)'0';
        while (input[i++] is byte c && c != endChar)
            value = value * 10 + (c - (byte)'0');
        return value;
    }

    private static List<int>[] PartitionCoordinates(List<Coord> coords, int maxCoord, out int partitionSize)
    {
        // Assume coordinates are uniformly distributed, therefore partition using cube root of count with padding
        partitionSize = (int)Math.Cbrt(coords.Count) - 1;

        var partitionWidth = (maxCoord / partitionSize) + 1;

        var partitions = new List<int>[partitionSize * partitionSize * partitionSize];
        for (var i = 0; i < partitions.Length; i++)
            partitions[i] = [];

        for (var i = 0; i < coords.Count; i++)
        {
            var (x, y, z) = coords[i];
            var px = x / partitionWidth;
            var py = y / partitionWidth;
            var pz = z / partitionWidth;
            var partitionKey = px + partitionSize * py + partitionSize * partitionSize * pz;
            partitions[partitionKey].Add(i);
        }

        return partitions;
    }

    private static void AddRemainingEdges(List<Coord> coords, PriorityQueue<Edge, long> edgeQueue, DisjointSet disjointSet)
    {
        // Add all edges that are not connected in case the graph is still disconnected
        for (var coord1Index = 0; coord1Index < coords.Count; coord1Index++)
        {
            var (x1, y1, z1) = coords[coord1Index];
            for (var coord2Index = coord1Index + 1; coord2Index < coords.Count; coord2Index++)
            {
                // Skip if already connected
                if (disjointSet.Find(coord1Index) == disjointSet.Find(coord2Index))
                    continue;

                var (x2, y2, z2) = coords[coord2Index];
                var xDiff = x1 - x2;
                var yDiff = y1 - y2;
                var zDiff = z1 - z2;
                var distanceSquared = (long)xDiff * xDiff + (long)yDiff * yDiff + (long)zDiff * zDiff;
                edgeQueue.Enqueue(new Edge(coord1Index, coord2Index), distanceSquared);
            }
        }
    }

    private static PriorityQueue<Edge, long> GetEdgesFromNeighbouringPartitions(List<Coord> coords, List<int>[] partitions, int partitionSize)
    {
        var pq = new PriorityQueue<Edge, long>(coords.Count * 20);

        var zMul = partitionSize * partitionSize;
        var yMul = partitionSize;

        for (var i = 0; i < partitions.Length; i++)
        {
            var partition = partitions[i];
            if (partition.Count == 0)
                continue;

            var px = i % partitionSize;
            var py = (i / yMul) % partitionSize;
            var pz = i / zMul;

            for (var nz = pz - 1; nz <= pz + 1; nz++)
            {
                if (nz < 0 || nz >= partitionSize)
                    continue;

                for (var ny = py - 1; ny <= py + 1; ny++)
                {
                    if (ny < 0 || ny >= partitionSize)
                        continue;

                    for (var nx = px - 1; nx <= px + 1; nx++)
                    {
                        if (nx < 0 || nx >= partitionSize)
                            continue;

                        var neighbourPartitionKey = nx + partitionSize * ny + partitionSize * partitionSize * nz;
                        var neighbourPartition = partitions[neighbourPartitionKey];
                        if (neighbourPartition.Count == 0)
                            continue;

                        foreach (var coordIndex1 in partition)
                        {
                            var (x1, y1, z1) = coords[coordIndex1];
                            foreach (var coordIndex2 in neighbourPartition)
                            {
                                if (coordIndex1 >= coordIndex2)
                                    continue;

                                var (x2, y2, z2) = coords[coordIndex2];
                                var xDiff = x1 - x2;
                                var yDiff = y1 - y2;
                                var zDiff = z1 - z2;
                                var distanceSquared = (long)xDiff * xDiff + (long)yDiff * yDiff + (long)zDiff * zDiff;

                                pq.Enqueue(new Edge(coordIndex1, coordIndex2), distanceSquared);
                            }
                        }
                    }
                }
            }
        }

        return pq;
    }
}
