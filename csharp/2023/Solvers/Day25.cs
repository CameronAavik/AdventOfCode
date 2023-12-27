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
        var edges = new List<(int A, int B)>(4000);
        while (!input.IsEmpty)
        {
            int lhsName = NameToId(input.Slice(0, 3));
            if (!idLookup.TryGetValue(lhsName, out int lhsId))
            {
                lhsId = idLookup.Count;
                idLookup[lhsName] = lhsId;
            }

            input = input.Slice(5);

            while (true)
            {
                int name = NameToId(input.Slice(0, 3));
                if (!idLookup.TryGetValue(name, out int id))
                {
                    id = idLookup.Count;
                    idLookup[name] = id;
                }

                edges.Add((lhsId, id));

                bool hasNext = input[3] == ' ';
                input = input.Slice(4);

                if (!hasNext)
                    break;
            }
        }

        var remainingEdges = new (int, int)[edges.Count];

        int[] initParent = new int[idLookup.Count];
        for (int i = 0; i < initParent.Length; i++)
            initParent[i] = i;

        int[] parent = new int[idLookup.Count];
        int[] size = new int[idLookup.Count];
        int random = 0;
        while (true)
        {
            int numRemainingEdges = remainingEdges.Length;
            edges.CopyTo(remainingEdges);

            Array.Copy(initParent, parent, parent.Length);
            Array.Fill(size, 1);

            int count = 0;
            while (count < parent.Length - 2)
            {
                // "random" edge choice
                int i = random++ % numRemainingEdges;
                (int, int) edge = remainingEdges[i];
                remainingEdges[i] = remainingEdges[--numRemainingEdges];

                // Union
                int a = FindParent(edge.Item1);
                int b = FindParent(edge.Item2);

                if (a != b)
                {
                    if (size[b] > size[a])
                    {
                        parent[a] = b;
                        size[b] += size[a];
                    }
                    else
                    {
                        parent[b] = a;
                        size[a] += size[b];
                    }

                    count++;
                }
            }

            int cutEdge = 0;
            int numCutEdges = 0;
            for (int i = 0; i < numRemainingEdges; i++)
            {
                (int, int) edge = remainingEdges[i];
                if (FindParent(edge.Item1) != FindParent(edge.Item2))
                {
                    cutEdge = i;
                    numCutEdges++;

                    // We know we are looking for a cut involving 3 edges, so skip if we have 4
                    if (numCutEdges == 4)
                        break;
                }
            }

            // If solution is found
            if (numCutEdges == 3)
            {
                (int, int) edge = remainingEdges[cutEdge];
                int part1 = size[FindParent(edge.Item1)] * size[FindParent(edge.Item2)];
                solution.SubmitPart1(part1);
                return;
            }
        }

        int FindParent(int i)
        {
            int iParent = parent[i];
            if (iParent == i)
                return i;

            return parent[i] = FindParent(iParent);
        }
    }

    private static int NameToId(ReadOnlySpan<byte> name) => (name[0] << 16) | (name[1] << 8) | (name[2]);
}
