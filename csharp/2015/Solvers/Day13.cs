using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day13 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var peopleSet = new HashSet<string>();
        var happinessUnits = new Dictionary<(string A, string B), int>();

        foreach (var lineRange in input.SplitLines())
        {
            ParseLine(input[lineRange], out var personA, out var personB, out var happiness);

            _ = peopleSet.Add(personA);
            _ = peopleSet.Add(personB);

            happinessUnits[(personA, personB)] = happiness;
        }

        // convert adjacency list to adjacency matrix
        string[] people = [.. peopleSet];
        var numPeople = people.Length;

        // let's add one extra person for part 2
        var adjMatrix = new int[numPeople + 1, numPeople + 1];
        for (var a = 0; a < numPeople; a++)
        {
            for (var b = 0; b < numPeople; b++)
            {
                if (happinessUnits.TryGetValue((people[a], people[b]), out var happiness))
                {
                    adjMatrix[a, b] = happiness;
                }
            }
        }

        var part1 = GetOptimalHappiness(numPeople, adjMatrix);
        var part2 = GetOptimalHappiness(numPeople + 1, adjMatrix);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseLine(ReadOnlySpan<byte> line, out string personA, out string personB, out int happiness)
    {
        var reader = new SpanReader(line);
        personA = Encoding.ASCII.GetString(reader.ReadUntil(' '));
        reader.SkipLength("would ".Length);
        var sign = reader.Peek() == 'g' ? 1 : -1;
        reader.SkipLength("gain ".Length);
        happiness = sign * reader.ReadPosIntUntil(' ');
        reader.SkipLength("happiness units by sitting next to ".Length);
        personB = Encoding.ASCII.GetString(reader.ReadUntil('.'));
    }

    private static int GetOptimalHappiness(int numPeople, int[,] adjMatrix)
    {
        var people = new int[numPeople];
        for (var p = 0; p < numPeople; p++)
        {
            people[p] = p;
        }

        var maxHappiness = int.MinValue;
        foreach (var permutation in people.AsSpan().GetPermutations())
        {
            var totalHappiness = 0;
            var prevPerson = permutation[^1];
            for (var j = 0; j < permutation.Length; j++)
            {
                var person = permutation[j];
                totalHappiness += adjMatrix[prevPerson, person];
                totalHappiness += adjMatrix[person, prevPerson];
                prevPerson = person;
            }

            maxHappiness = Math.Max(totalHappiness, maxHappiness);
        }

        return maxHappiness;
    }
}
