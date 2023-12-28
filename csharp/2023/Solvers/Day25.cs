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
        int edgeCount = 0;
        while (!input.IsEmpty)
        {
            int lhsName = NameToId(input.Slice(0, 3));
            List<(int Destination, int EdgeId)> lhsList;
            if (!idLookup.TryGetValue(lhsName, out int lhsId))
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

            input = input.Slice(5);

            while (true)
            {
                int name = NameToId(input.Slice(0, 3));
                List<(int Destination, int EdgeId)> list;
                if (!idLookup.TryGetValue(name, out int id))
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

                bool hasNext = input[3] == ' ';
                input = input.Slice(4);

                if (!hasNext)
                    break;
            }
        }

        // Reusable bitset used in FindFurthestNode, FordFulkersonIteration, and CountReachableNodes
        ulong[] visited = new ulong[(graph.Count - 1) / 64 + 1];

        // We assume that s and t are on different sides of the cut.
        // It is possible to construct a graph that this is not true, but this is not the case for AoC inputs.
        int s = FindFurthestNode(0);
        int t = FindFurthestNode(s);

        ulong[] edgeFlows = new ulong[(edgeCount - 1) / 64 + 1];

        for (int i = 0; i < 3; i++)
            FordFulkersonIteration();

        int reachableFromSource = CountReachableNodes();
        int part1 = reachableFromSource * (graph.Count - reachableFromSource);
        solution.SubmitPart1(part1);

        int FindFurthestNode(int node)
        {
            Array.Clear(visited);
            visited[node / 64] = 1UL << node;
            int[] queue = new int[graph.Count];
            queue[0] = node;
            int queuePtr = 0;
            int queueLen = 1;

            while (queueLen < graph.Count)
            {
                node = queue[queuePtr++];

                foreach ((int destination, _) in graph[node])
                {
                    ulong flag = 1UL << destination;
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

                foreach ((int destination, int edgeId) in graph[node])
                {
                    ulong flag = 1UL << destination;
                    if ((visited[destination / 64] & flag) != 0)
                        continue;

                    ulong edgeFlag = 1UL << edgeId;
                    if ((edgeFlows[edgeId / 64] & edgeFlag) != 0)
                        continue;

                    visited[destination / 64] |= flag;
                    if (TryDFS(destination))
                    {
                        int inverseEdgeId = edgeId ^ 1;
                        ulong inverseEdgeFlag = 1UL << inverseEdgeId;

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
            int count = 1;
            DFS(s);
            return count;

            void DFS(int node)
            {
                foreach ((int destination, int edgeId) in graph[node])
                {
                    ulong flag = 1UL << destination;
                    if ((visited[destination / 64] & flag) != 0)
                        continue;

                    ulong edgeFlag = 1UL << edgeId;
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
