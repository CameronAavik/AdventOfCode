using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day25 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // no part 2 today
        solution.SubmitPart2(string.Empty);

        var idLookup = new Dictionary<int, int>(2000);
        var graph = new List<List<(int Destination, int EdgeId)>>(2000);
        var edgeCount = 0;
        while (!input.IsEmpty)
        {
            var lhsName = NameToId(input[..3]);
            List<(int Destination, int EdgeId)> lhsList;
            if (!idLookup.TryGetValue(lhsName, out var lhsId))
            {
                lhsId = idLookup.Count;
                lhsList = new(10);
                graph.Add(lhsList);
                idLookup[lhsName] = lhsId;
            }
            else
            {
                lhsList = graph[lhsId];
            }

            input = input[5..];

            while (true)
            {
                var name = NameToId(input[..3]);
                List<(int Destination, int EdgeId)> list;
                if (!idLookup.TryGetValue(name, out var id))
                {
                    id = idLookup.Count;
                    list = new(10);
                    graph.Add(list);
                    idLookup[name] = id;
                }
                else
                {
                    list = graph[id];
                }

                lhsList.Add((id, edgeCount++));
                list.Add((lhsId, edgeCount++));

                var hasNext = input[3] == ' ';
                input = input[4..];

                if (!hasNext)
                    break;
            }
        }

        // Reusable bitset used in FindFurthestNode, FordFulkersonIteration, and CountReachableNodes
        var visited = new ulong[(graph.Count - 1) / 64 + 1];

        // We assume that s and t are on different sides of the cut.
        // It is possible to construct a graph that this is not true, but this is not the case for AoC inputs.
        var s = FindFurthestNode(0);
        var t = FindFurthestNode(s);

        var edgeFlows = new ulong[(edgeCount - 1) / 64 + 1];

        for (var i = 0; i < 3; i++)
            FordFulkersonIteration();

        var reachableFromSource = CountReachableNodes();
        var part1 = reachableFromSource * (graph.Count - reachableFromSource);
        solution.SubmitPart1(part1);

        int FindFurthestNode(int node)
        {
            Array.Clear(visited);
            visited[node / 64] = 1UL << node;
            var queue = new int[graph.Count];
            queue[0] = node;
            var queuePtr = 0;
            var queueLen = 1;

            while (queueLen < graph.Count)
            {
                node = queue[queuePtr++];

                foreach ((var destination, _) in graph[node])
                {
                    var flag = 1UL << destination;
                    if ((visited[destination / 64] & flag) == 0)
                    {
                        visited[destination / 64] |= flag;
                        queue[queueLen++] = destination;
                    }
                }
            }

            return queue[queueLen - 1];
        }

        void FordFulkersonIteration()
        {
            Array.Clear(visited);
            visited[s / 64] = 1UL << s;

            TryDFS(s);

            bool TryDFS(int node)
            {
                if (node == t)
                    return true;

                foreach ((var destination, var edgeId) in graph[node])
                {
                    var flag = 1UL << destination;
                    if ((visited[destination / 64] & flag) != 0)
                        continue;

                    var edgeFlag = 1UL << edgeId;
                    if ((edgeFlows[edgeId / 64] & edgeFlag) != 0)
                        continue;

                    visited[destination / 64] |= flag;
                    if (TryDFS(destination))
                    {
                        var inverseEdgeId = edgeId ^ 1;
                        var inverseEdgeFlag = 1UL << inverseEdgeId;

                        // if the inverse is 1, then we need to set the inverse to 0
                        // if the inverse is 0, then we need to set the edge to 1
                        if ((edgeFlows[inverseEdgeId / 64] & inverseEdgeFlag) != 0)
                        {
                            edgeFlows[inverseEdgeId / 64] ^= inverseEdgeFlag;
                        }
                        else
                        {
                            edgeFlows[edgeId / 64] |= edgeFlag;
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        int CountReachableNodes()
        {
            Array.Clear(visited);
            visited[s / 64] = 1UL << s;
            var count = 1;
            DFS(s);
            return count;

            void DFS(int node)
            {
                foreach ((var destination, var edgeId) in graph[node])
                {
                    var flag = 1UL << destination;
                    if ((visited[destination / 64] & flag) != 0)
                        continue;

                    var edgeFlag = 1UL << edgeId;
                    if ((edgeFlows[edgeId / 64] & edgeFlag) != 0)
                        continue;

                    visited[destination / 64] |= flag;
                    count++;
                    DFS(destination);
                }
            }
        }
    }

    private static int NameToId(ReadOnlySpan<byte> name) => (name[0] << 16) | (name[1] << 8) | (name[2]);
}
