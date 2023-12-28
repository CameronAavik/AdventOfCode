using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

// I will add some documentation later about how this works. Huge thanks to Hegdahl (https://github.com/Hegdahl) who
// came up with the idea for this algorithm.
public class Day23 : ISolver
{
    public enum Direction : byte { East, West, South, North }
    public readonly record struct Edge(int NodeId, int Distance, bool HasReverseSlope);
    public readonly record struct Node(int Id, Edge? East = null, Edge? West = null, Edge? South = null, Edge? North = null)
    {
        public Node AddEdge(Direction incomingDirection, Edge? edge)
        {
            return incomingDirection switch
            {
                Direction.East => this with { West = edge },
                Direction.West => this with { East = edge },
                Direction.South => this with { North = edge },
                Direction.North => this with { South = edge },
                _ => this
            };
        }
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Node[] graph = GetGraph(input, out int baseDistance);

        int part1 = baseDistance + SolvePart1(graph);
        solution.SubmitPart1(part1);

        int part2 = baseDistance + SolvePart2(graph);
        solution.SubmitPart2(part2);
    }

    private static Node[] GetGraph(ReadOnlySpan<byte> input, out int baseDistance)
    {
        int width = input.IndexOf((byte)'\n');
        int rowLen = width + 1;

        HashSet<int> visitedNodes = new(36);
        Dictionary<int, Node> nodes = new(36);

        int startIndex = input.IndexOf((byte)'.');
        int endIndex = input.LastIndexOf((byte)'.');

        int startNode = MoveUntilNextNode(input, startIndex, rowLen, Direction.South, out int startDistance, out Direction startArrival, out _, out _);
        int endNode = MoveUntilNextNode(input, endIndex, rowLen, Direction.North, out int endDistance, out Direction endArrival, out _, out _);

        // total unavoidable distance at start and end
        baseDistance = startDistance + endDistance;

        nodes[startNode] = new Node(0).AddEdge(startArrival, new Edge(-1, 0, false));
        nodes[endNode] = new Node(1).AddEdge(endArrival, new Edge(-1, 0, false));

        Queue<int> nodeQueue = new();
        nodeQueue.Enqueue(startNode);
        while (nodeQueue.TryDequeue(out int nodeId))
        {
            if (visitedNodes.Contains(nodeId))
                continue;

            visitedNodes.Add(nodeId);

            Node node = nodes[nodeId];

            if (node.North is null && input[nodeId - rowLen] != '#')
            {
                int northNodeId = MoveUntilNextNode(input, nodeId, rowLen, Direction.North, out int distance, out Direction direction, out bool hasSlope, out bool hasReverseSlope);
                if (!nodes.TryGetValue(northNodeId, out Node northNode))
                    northNode = new(nodes.Count);

                node = node with { North = new Edge(northNode.Id, distance, hasReverseSlope) };
                nodes[northNodeId] = northNode.AddEdge(direction, new Edge(node.Id, distance, hasSlope));
                nodeQueue.Enqueue(northNodeId);
            }

            if (node.South is null && input[nodeId + rowLen] != '#')
            {
                int southNodeId = MoveUntilNextNode(input, nodeId, rowLen, Direction.South, out int distance, out Direction direction, out bool hasSlope, out bool hasReverseSlope);
                if (!nodes.TryGetValue(southNodeId, out Node southNode))
                    southNode = new(nodes.Count);

                node = node with { South = new Edge(southNode.Id, distance, hasReverseSlope) };
                nodes[southNodeId] = southNode.AddEdge(direction, new Edge(node.Id, distance, hasSlope));
                nodeQueue.Enqueue(southNodeId);
            }

            if (node.West is null && input[nodeId - 1] != '#')
            {
                int westNodeId = MoveUntilNextNode(input, nodeId, rowLen, Direction.West, out int distance, out Direction direction, out bool hasSlope, out bool hasReverseSlope);
                if (!nodes.TryGetValue(westNodeId, out Node westNode))
                    westNode = new(nodes.Count);

                node = node with { West = new Edge(westNode.Id, distance, hasReverseSlope) };
                nodes[westNodeId] = westNode.AddEdge(direction, new Edge(node.Id, distance, hasSlope));
                nodeQueue.Enqueue(westNodeId);
            }

            if (node.East is null && input[nodeId + 1] != '#')
            {
                int eastNodeId = MoveUntilNextNode(input, nodeId, rowLen, Direction.East, out int distance, out Direction direction, out bool hasSlope, out bool hasReverseSlope);
                if (!nodes.TryGetValue(eastNodeId, out Node eastNode))
                    eastNode = new(nodes.Count);

                node = node with { East = new Edge(eastNode.Id, distance, hasReverseSlope) };
                nodes[eastNodeId] = eastNode.AddEdge(direction, new Edge(node.Id, distance, hasSlope));
                nodeQueue.Enqueue(eastNodeId);
            }

            nodes[nodeId] = node;
        }

        // remove edges connected to entry and exit
        nodes[startNode] = nodes[startNode].AddEdge(startArrival, null);
        nodes[endNode] = nodes[endNode].AddEdge(endArrival, null);

        var nodeArray = new Node[nodes.Count];
        foreach (Node v in nodes.Values)
            nodeArray[v.Id] = v;

        return nodeArray;
    }

    private static int MoveUntilNextNode(ReadOnlySpan<byte> input, int i, int rowLength, Direction startDirection, out int distance, out Direction direction, out bool hasSlope, out bool hasReverseSlope)
    {
        hasSlope = false;
        hasReverseSlope = false;
        distance = 0;
        direction = startDirection;

        while (true)
        {
            switch (direction)
            {
                case Direction.East:
                    byte nextEast = input[i + 1];
                    while (true)
                    {
                        hasSlope = hasSlope || nextEast == '>';
                        hasReverseSlope = hasReverseSlope || nextEast == '<';

                        distance++;
                        i++;
                        nextEast = input[i + 1];
                        if (nextEast == '#')
                        {
                            if (input[i - rowLength] == '#')
                                direction = Direction.South;
                            else if (input[i + rowLength] == '#')
                                direction = Direction.North;
                            else
                                return i;
                            break;
                        }
                        else if (input[i - rowLength] != '#' || input[i + rowLength] != '#')
                        {
                            return i;
                        }
                    }

                    break;
                case Direction.West:
                    byte nextWest = input[i - 1];
                    while (true)
                    {
                        hasSlope = hasSlope || nextWest == '<';
                        hasReverseSlope = hasReverseSlope || nextWest == '>';

                        distance++;
                        i--;
                        nextWest = input[i - 1];
                        if (nextWest == '#')
                        {
                            if (input[i - rowLength] == '#')
                                direction = Direction.South;
                            else if (input[i + rowLength] == '#')
                                direction = Direction.North;
                            else
                                return i;
                            break;
                        }
                        else if (input[i - rowLength] != '#' || input[i + rowLength] != '#')
                        {
                            return i;
                        }
                    }

                    break;
                case Direction.South:
                    byte nextSouth = input[i + rowLength];
                    while (true)
                    {
                        hasSlope = hasSlope || nextSouth == 'v';
                        hasReverseSlope = hasReverseSlope || nextSouth == '^';

                        distance++;
                        i += rowLength;
                        nextSouth = input[i + rowLength];
                        if (nextSouth == '#')
                        {
                            if (input[i - 1] == '#')
                                direction = Direction.East;
                            else if (input[i + 1] == '#')
                                direction = Direction.West;
                            else
                                return i;
                            break;
                        }
                        else if (input[i - 1] != '#' || input[i + 1] != '#')
                        {
                            return i;
                        }
                    }

                    break;
                case Direction.North:
                    byte nextNorth = input[i - rowLength];
                    while (true)
                    {
                        hasSlope = hasSlope || nextNorth == '^';
                        hasReverseSlope = hasReverseSlope || nextNorth == 'v';

                        distance++;
                        i -= rowLength;
                        nextNorth = input[i - rowLength];
                        if (nextNorth == '#')
                        {
                            if (input[i - 1] == '#')
                                direction = Direction.East;
                            else if (input[i + 1] == '#')
                                direction = Direction.West;
                            else
                                return i;
                            break;
                        }
                        else if (input[i - 1] != '#' || input[i + 1] != '#')
                        {
                            return i;
                        }
                    }

                    break;
            }
        }
    }

    // Solve using simple recursive DFS with a ulong bitset to mark which nodes were seen
    private static int SolvePart1(Node[] graph)
    {
        int maxDistance = 0;
        FindLongestInternal(0, 0, 0);
        return maxDistance;

        void FindLongestInternal(int nodeId, int distance, ulong seen)
        {
            if (nodeId == 1)
            {
                maxDistance = Math.Max(maxDistance, distance);
                return;
            }

            ulong flag = 1UL << nodeId;
            if ((seen & flag) != 0)
                return;
            seen |= flag;

            Node node = graph[nodeId];

            if (node.South is Edge south && !south.HasReverseSlope)
                FindLongestInternal(south.NodeId, distance + south.Distance, seen);

            if (node.North is Edge north && !north.HasReverseSlope)
                FindLongestInternal(north.NodeId, distance + north.Distance, seen);

            if (node.West is Edge west && !west.HasReverseSlope)
                FindLongestInternal(west.NodeId, distance + west.Distance, seen);

            if (node.East is Edge east && !east.HasReverseSlope)
                FindLongestInternal(east.NodeId, distance + east.Distance, seen);
        }
    }

    public readonly record struct DPKey(ulong FromEdges, ulong ToEdges, byte NumEdges) : IEquatable<DPKey>
    {
        public DPKey InsertEdge(byte From, byte To)
        {
            for (int i = 0; i < NumEdges; i++)
            {
                ulong u = (FromEdges >> (8 * i)) & 0xFF;
                if (u > From)
                {
                    ulong belowMask = (1UL << (8 * i)) - 1;
                    ulong belowFrom = FromEdges & belowMask;
                    ulong belowTo = ToEdges & belowMask;
                    ulong newEdgeFrom = (ulong)From << (8 * i);
                    ulong newEdgeTo = (ulong)To << (8 * i);
                    ulong aboveFrom = (FromEdges & ~belowMask) << 8;
                    ulong aboveTo = (ToEdges & ~belowMask) << 8;
                    return new DPKey(belowFrom | newEdgeFrom | aboveFrom, belowTo | newEdgeTo | aboveTo, (byte)(NumEdges + 1));
                }
            }

            {
                ulong newEdgeFrom = (ulong)From << (8 * NumEdges);
                ulong newEdgeTo = (ulong)To << (8 * NumEdges);
                return new DPKey(FromEdges | newEdgeFrom, ToEdges | newEdgeTo, (byte)(NumEdges + 1));
            }
        }

        public DPKey RemoveEdge(byte From)
        {
            for (int i = 0; i < NumEdges; i++)
            {
                ulong u = (FromEdges >> (8 * i)) & 0xFF;
                if (u == From)
                {
                    ulong belowMask = (1UL << (8 * i)) - 1;
                    ulong belowFrom = FromEdges & belowMask;
                    ulong belowTo = ToEdges & belowMask;
                    ulong aboveFrom = (FromEdges & (~belowMask << 8)) >> 8;
                    ulong aboveTo = (ToEdges & (~belowMask << 8)) >> 8;
                    return new DPKey(belowFrom | aboveFrom, belowTo | aboveTo, (byte)(NumEdges - 1));
                }
            }

            return this;
        }
    }

    private static int SolvePart2(Node[] graph)
    {
        var dp = new Dictionary<DPKey, int>(2000)
        {
            [new DPKey(0, 0, 0)] = 0
        };

        int queuePtr = 0;
        int queueLength = 1;
        int[] queue = new int[graph.Length];
        queue[0] = 0;

        int[] remainingDegree = new int[graph.Length];

        ulong inQueue = 1;
        ulong added = 0;

        // intermediate data structures used inside local functions
        var newDP = new Dictionary<DPKey, int>(2000);
        var newEntries = new List<(DPKey, int)>(1000);
        var entriesWithSelfEdge = new List<(DPKey, int)>(1000);
        var entriesToRemove = new List<DPKey>(1000);

        while (queuePtr < queueLength)
        {
            int nodeId = queue[queuePtr++];
            Node nodeData = graph[nodeId];
            IntroduceNode(nodeData);
            added |= 1UL << nodeId;

            if (nodeData.South is Edge south)
                IntroduceEdge(nodeId, south);

            if (nodeData.North is Edge north)
                IntroduceEdge(nodeId, north);

            if (nodeData.West is Edge west)
                IntroduceEdge(nodeId, west);

            if (nodeData.East is Edge east)
                IntroduceEdge(nodeId, east);
        }


        return dp[new DPKey(0, 1, 1)];

        void IntroduceNode(Node node)
        {
            byte id = (byte)node.Id;
            newDP.Clear();
            foreach ((DPKey k, int v) in dp)
                newDP[k.InsertEdge(id, id)] = v;

            int degree = 0;
            if (node.South is not null) degree++;
            if (node.East is not null) degree++;
            if (node.North is not null) degree++;
            if (node.West is not null) degree++;
            remainingDegree[id] = degree;

            (newDP, dp) = (dp, newDP);
        }

        void IntroduceEdge(int u, Edge edge)
        {
            (int v, int newDistance, _) = edge;
            ulong vFlag = 1UL << v;

            if ((added & vFlag) != 0)
            {
                newEntries.Clear();

                foreach ((DPKey key, int bestDistance) in dp)
                {
                    byte uCounterpart = 255;
                    byte uFrom = 255;
                    byte vCounterpart = 255;
                    byte vFrom = 255;

                    ulong fromK = key.FromEdges;
                    ulong toK = key.ToEdges;

                    for (int i = 0; i < key.NumEdges; i++)
                    {
                        byte ku = (byte)(fromK & 0xFF);
                        byte kv = (byte)(toK & 0xFF);

                        if (ku == u)
                        {
                            uCounterpart = kv;
                            uFrom = ku;
                        }
                        else if (ku == v)
                        {
                            vCounterpart = kv;
                            vFrom = ku;
                        }

                        if (kv == u)
                        {
                            uCounterpart = ku;
                            uFrom = ku;
                        }
                        else if (kv == v)
                        {
                            vCounterpart = ku;
                            vFrom = ku;
                        }

                        fromK >>= 8;
                        toK >>= 8;
                    }

                    if (uCounterpart == 255 || vCounterpart == 255 || uCounterpart == v)
                        continue;

                    if (u <= 1 && uCounterpart != u)
                        continue;

                    if (v <= 1 && vCounterpart != v)
                        continue;

                    if (vCounterpart < uCounterpart)
                        (uCounterpart, vCounterpart) = (vCounterpart, uCounterpart);

                    DPKey newKey = key
                        .RemoveEdge(uFrom)
                        .RemoveEdge(vFrom)
                        .InsertEdge(uCounterpart, vCounterpart);

                    newEntries.Add((newKey, bestDistance + newDistance));
                }

                foreach ((DPKey, int) entry in newEntries)
                {
                    if (!dp.TryGetValue(entry.Item1, out int bestDist) || bestDist < entry.Item2)
                        dp[entry.Item1] = entry.Item2;
                }

                if (--remainingDegree[u] == 0)
                    ForgetNode(u);

                if (--remainingDegree[v] == 0)
                    ForgetNode(v);
            }
            else if ((inQueue & vFlag) == 0)
            {
                inQueue |= vFlag;
                queue[queueLength++] = v;
            }
        }

        void ForgetNode(int u)
        {
            entriesToRemove.Clear();
            entriesWithSelfEdge.Clear();

            foreach ((DPKey k, int d) in dp)
            {
                ulong fromK = k.FromEdges;
                ulong toK = k.ToEdges;

                for (int i = 0; i < k.NumEdges; i++)
                {
                    byte ku = (byte)(fromK & 0xFF);
                    byte kv = (byte)(toK & 0xFF);
                    if (ku == u && kv == u)
                    {
                        if (u <= 1)
                            entriesToRemove.Add(k);
                        else
                            entriesWithSelfEdge.Add((k, d));
                        break;
                    }

                    if (u > 1 && (ku == u || kv == u))
                    {
                        entriesToRemove.Add(k);
                        break;
                    }

                    fromK >>= 8;
                    toK >>= 8;
                }
            }

            foreach (DPKey entry in entriesToRemove)
                dp.Remove(entry);

            foreach ((DPKey, int) entry in entriesWithSelfEdge)
            {
                dp.Remove(entry.Item1);
                DPKey newKey = entry.Item1.RemoveEdge((byte)u);
                if (!dp.TryGetValue(newKey, out int currentDist) || currentDist < entry.Item2)
                    dp[newKey] = entry.Item2;
            }
        }
    }
}
