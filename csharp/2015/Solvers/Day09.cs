using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day09 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // get list of towns and edges from input
        var townSet = new HashSet<string>();
        var distances = new Dictionary<(string From, string To), int>();
        foreach (var lineRange in input.SplitLines())
        {
            ParseLine(input[lineRange], out var distance, out var fromName, out var toName);

            townSet.Add(fromName);
            townSet.Add(toName);

            distances[(fromName, toName)] = distance;
            distances[(toName, fromName)] = distance;
        }

        // convert adjacency list to adjacency matrix
        string[] townNames = [.. townSet];
        var numTowns = townNames.Length;

        var adjMatrix = new int[numTowns, numTowns];
        var towns = new int[numTowns];
        for (var a = 0; a < numTowns; a++)
        {
            towns[a] = a;
            for (var b = 0; b < numTowns; b++)
            {
                if (a != b)
                {
                    adjMatrix[a, b] = distances[(townNames[a], townNames[b])];
                }
            }
        }

        var minDistance = int.MaxValue;
        var maxDistance = int.MinValue;
        foreach (var perm in towns.AsSpan().GetPermutations())
        {
            var pathDistance = 0;
            var prevTown = perm[0];
            for (var j = 1; j < numTowns; j++)
            {
                var town = perm[j];
                pathDistance += adjMatrix[prevTown, town];
                prevTown = town;
            }

            minDistance = Math.Min(minDistance, pathDistance);
            maxDistance = Math.Max(maxDistance, pathDistance);
        }

        solution.SubmitPart1(minDistance);
        solution.SubmitPart2(maxDistance);
    }

    private static void ParseLine(ReadOnlySpan<byte> line, out int distance, out string fromName, out string toName)
    {
        var reader = new SpanReader(line);
        fromName = Encoding.ASCII.GetString(reader.ReadUntil(' '));
        reader.SkipLength("to ".Length);
        toName = Encoding.ASCII.GetString(reader.ReadUntil(' '));
        reader.SkipLength("= ".Length);
        distance = reader.ReadPosIntUntilEnd();
    }
}
