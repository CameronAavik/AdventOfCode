using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

// I will add some documentation later about how this works. Maybe. This will only work with the AoC inputs which are
// in the form of a 6x6 grid graph when considering only the intersections in the maze
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

        public int CountEdges() => (East == null ? 0 : 1) + (West == null ? 0 : 1) + (South == null ? 0 : 1) + (North == null ? 0 : 1);

        public IEnumerable<Edge> GetEdges()
        {
            if (North is Edge north) yield return north;
            if (South is Edge south) yield return south;
            if (West is Edge west) yield return west;
            if (East is Edge east) yield return east;
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

    public record struct RowDPState(byte Column0, byte Column1, byte Column2, byte Column3, byte Column4, byte Column5) : IEquatable<RowDPState>
    {
        // Just pretend you never saw SetColumn and GetColumn
        public readonly RowDPState SetColumn(int index, byte value)
        {
            RowDPState copy = this;
            ref byte copyRef = ref Unsafe.As<RowDPState, byte>(ref copy);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref copyRef, index), value);
            return copy;
        }

        public byte GetColumn(int index)
        {
            ref byte self = ref Unsafe.As<RowDPState, byte>(ref this);
            return Unsafe.Add(ref self, index);
        }

        public readonly RowDPState Canonicalize()
        {
            RowDPState newState = this;
            byte nextExpectedId = 1;
            for (int i = 0; i < 6; i++)
            {
                byte colValue = newState.GetColumn(i);
                if (colValue < nextExpectedId)
                    continue;

                if (colValue != nextExpectedId)
                {
                    // swap needs to occur
                    newState = newState.SetColumn(i, nextExpectedId);
                    for (int j = i + 1; j < 6; j++)
                    {
                        byte colValueB = newState.GetColumn(j);
                        if (colValueB == nextExpectedId)
                            newState = newState.SetColumn(j, colValue);
                        else if (colValueB == colValue)
                            newState = newState.SetColumn(j, nextExpectedId);
                    }
                }

                nextExpectedId++;
            }

            return newState;
        }
    }

    private static int SolvePart2(Node[] graph)
    {
        ulong seenNodes = 1;
        Node[] curRowNodes = new Node[6];
        int[] horizontalDistances = new int[5];
        int[] verticalDistances = new int[6];

        // populate nodes for top row
        curRowNodes[0] = graph[0];
        for (int i = 1; i < 5; i++)
        {
            Node lastNode = curRowNodes[i - 1];
            foreach (Edge edge in lastNode.GetEdges())
            {
                ulong nodeFlag = 1UL << edge.NodeId;
                if ((seenNodes & nodeFlag) == 0)
                {
                    Node nextNode = graph[edge.NodeId];
                    if (nextNode.CountEdges() == 3)
                    {
                        seenNodes |= nodeFlag;
                        curRowNodes[i] = nextNode;
                        break;
                    }
                }
            }
        }

        curRowNodes[5] = curRowNodes[4];

        var dp = new Dictionary<RowDPState, int>(76)
        {
            [new RowDPState(1, 0, 0, 0, 0, 0)] = 0
        };

        var dpWorking = new Dictionary<RowDPState, int>(76);

        var nextStates = new (RowDPState key, int value)[32];

        for (int row = 0; row < 6; row++)
        {
            // Update horizontal distances
            for (int horizontalEdge = 0; horizontalEdge < 5; horizontalEdge++)
            {
                Node leftNode = curRowNodes[horizontalEdge];
                Node rightNode = curRowNodes[horizontalEdge + 1];
                foreach (Edge edge in leftNode.GetEdges())
                {
                    if (edge.NodeId == rightNode.Id)
                    {
                        horizontalDistances[horizontalEdge] = edge.Distance;
                        break;
                    }
                }
            }

            // update vertical distances and update curRowNodes to next row
            if (row < 5)
            {
                for (int col = 0; col < 6; col++)
                {
                    Node lastNodeInCol = curRowNodes[col];
                    foreach (Edge edge in lastNodeInCol.GetEdges())
                    {
                        ulong nodeFlag = 1UL << edge.NodeId;
                        if ((seenNodes & nodeFlag) == 0)
                        {
                            Node node = graph[edge.NodeId];
                            if (row == 0 && col == 4 && node.CountEdges() != 4)
                                continue;

                            verticalDistances[col] = edge.Distance;
                            curRowNodes[col] = node;

                            // in the second last row, the left two elements connect to the 2nd column
                            if (row != 4 || col != 0)
                                seenNodes |= nodeFlag;

                            break;
                        }
                    }
                }
            }
            else
            {
                horizontalDistances[0] = 0;
                Array.Clear(verticalDistances);
            }

            dpWorking.Clear();

            foreach (KeyValuePair<RowDPState, int> entry in dp)
            {
                int numStates = GetNextStates(entry.Key, entry.Value, horizontalDistances, verticalDistances, nextStates);
                for (int i = 0; i < numStates; i++)
                {
                    (RowDPState state, int distance) = nextStates[i];
                    int bestDistance = dpWorking.GetValueOrDefault(state, 0);
                    if (distance > bestDistance)
                        dpWorking[state] = distance;
                }
            }

            (dp, dpWorking) = (dpWorking, dp);
        }

        return dp[new RowDPState(0, 0, 0, 0, 0, 1)];

        static int GetNextStates(RowDPState state, int currentDistance, int[] horizontalDistances, int[] verticalDistances, (RowDPState key, int value)[] outputStates)
        {
            int statesLength = 0;
            Array.Clear(outputStates);
            FindStates(state, currentDistance, 0);
            return statesLength;

            void FindStates(RowDPState state, int currentDistance, int col)
            {
                if (col == 6)
                {
                    outputStates[statesLength++] = (state, currentDistance);
                    return;
                }

                byte colPathId = state.GetColumn(col);
                byte newPathId = colPathId;
                int verticalDistance = verticalDistances[col];

                if (colPathId == 0)
                {
                    // handle case that we decide to leave this cell unconnected, which we can only do if the above cell is unconnected
                    FindStates(state, currentDistance, col + 1);
                    for (int i = 0; i < col; i++)
                        newPathId = Math.Max(newPathId, state.GetColumn(i));
                    newPathId++;
                }
                else
                {
                    // handle case we decide to just keep this cell going straight down
                    FindStates(state, currentDistance + verticalDistance, col + 1);
                }

                for (int i = col + 1; i < 6; i++)
                {
                    byte nextColPathId = state.GetColumn(i);
                    currentDistance += horizontalDistances[i - 1];

                    if (nextColPathId == 0)
                    {
                        if (colPathId == 0)
                        {
                            // New path is being added
                            RowDPState updatedState = state
                                .SetColumn(col, newPathId)
                                .SetColumn(i, newPathId);

                            for (int j = col + 1; j < i; j++)
                                updatedState = updatedState.SetColumn(j, 0);

                            for (int j = i + 1; j < 6; j++)
                            {
                                int curValue = updatedState.GetColumn(j);
                                if (curValue >= newPathId)
                                    updatedState = updatedState.SetColumn(j, (byte)(curValue + 1));
                            }

                            FindStates(updatedState, currentDistance + verticalDistance + verticalDistances[i], i + 1);
                        }
                        else
                        {
                            // We are just moving the position of the path id from col to i
                            RowDPState updatedState = state
                                .SetColumn(col, 0)
                                .SetColumn(i, colPathId);

                            FindStates(updatedState, currentDistance + verticalDistances[i], i + 1);
                        }
                    }
                    else
                    {
                        if (colPathId == 0)
                        {
                            // We are just moving the position of the path id from i to col
                            RowDPState updatedState = state
                                .SetColumn(col, nextColPathId)
                                .SetColumn(i, 0);

                            FindStates(updatedState, currentDistance + verticalDistance, i + 1);
                        }
                        else if (colPathId != nextColPathId)
                        {
                            // need to merge together two separate paths, will choose the lower one and then decrease everything else
                            byte minPathId = Math.Min(colPathId, nextColPathId);
                            byte pathToDecrease = Math.Max(colPathId, nextColPathId);

                            RowDPState updatedState = state
                                .SetColumn(col, 0)
                                .SetColumn(i, 0);

                            // find other end of path that needs to change
                            for (int j = 0; j < 6; j++)
                            {
                                if (updatedState.GetColumn(j) == pathToDecrease)
                                {
                                    updatedState = updatedState.SetColumn(j, minPathId);
                                    break;
                                }
                            }

                            // Canonicalization must occur in case the path merging causes the ids to go out of order
                            updatedState = updatedState.Canonicalize();

                            FindStates(updatedState, currentDistance, i + 1);
                        }

                        break;
                    }
                }

            }
        }
    }
}
