using System;
using System.Collections.Generic;
using System.Xml.Schema;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

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

        int part1 = baseDistance + FindLongestDistance(graph, ignoreReverseSlope: false);
        solution.SubmitPart1(part1);

        int part2 = baseDistance + FindLongestDistance(graph, ignoreReverseSlope: true);
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
        foreach (var v in nodes.Values)
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

    private static int FindLongestDistance(Node[] graph, bool ignoreReverseSlope)
    {
        var nodes = new ((short Distance, short Destination)[] Edges, int BestEdgeDistance)[graph.Length];

        int maxPossibleDistance = 0;
        Span<(short Distance, short Destination)> edges = stackalloc (short Distance, short Destination)[4];
        for (int i = 0; i < graph.Length; i++)
        {
            Node node = graph[i];
            int numEdges = 0;

            if (node.South is Edge south && (ignoreReverseSlope || !south.HasReverseSlope))
                edges[numEdges++] = ((short)south.Distance, (short)south.NodeId);

            if (node.North is Edge north && (ignoreReverseSlope || !north.HasReverseSlope))
                edges[numEdges++] = ((short)north.Distance, (short)north.NodeId);

            if (node.West is Edge west && (ignoreReverseSlope || !west.HasReverseSlope))
                edges[numEdges++] = ((short)west.Distance, (short)west.NodeId);

            if (node.East is Edge east && (ignoreReverseSlope || !east.HasReverseSlope))
                edges[numEdges++] = ((short)east.Distance, (short)east.NodeId);

            int bestDistance = 0;
            if (i > 0 || numEdges >= 2)
            {
                for (int j = 0; j < numEdges; j++)
                    bestDistance = Math.Max(bestDistance, edges[j].Distance);
            }

            var actualEdges = edges.Slice(0, numEdges);
            actualEdges.Sort((x, y) => x.Destination.CompareTo(y.Destination)); // sort by destination, so that the DFS prefers to take nodes that are further away from the end

            maxPossibleDistance += bestDistance;
            nodes[i] = (actualEdges.ToArray(), bestDistance);
        }

        int maxDistance = 0;
        FindLongestInternal(0, 0, 0, maxPossibleDistance);
        return maxDistance;

        void FindLongestInternal(int nodeId, int distance, ulong seen, int maxPossibleDistance)
        {
            if (nodeId == 1)
            {
                maxDistance = Math.Max(maxDistance, distance);
                return;
            }

            if (maxPossibleDistance < maxDistance)
                return;

            ulong flag = 1UL << nodeId;
            if ((seen & flag) != 0)
                return;
            seen |= flag;

            ((short Distance, short Destination)[] nodeEdges, int bestEdgeDistance) = nodes[nodeId];

            for (int i = 0; i < nodeEdges.Length; i++)
            {
                (short newDistance, short newDestination) = nodeEdges[i];
                FindLongestInternal(newDestination, distance + newDistance, seen, maxPossibleDistance + newDistance - bestEdgeDistance);
            }
        }
    }
}
