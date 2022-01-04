using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day13 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var peopleSet = new HashSet<string>();
        var happinesses = new Dictionary<(string A, string B), int>();

        foreach (ReadOnlySpan<byte> line in input.SplitLines())
        {
            ParseLine(line, out string personA, out string personB, out int happiness);

            _ = peopleSet.Add(personA);
            _ = peopleSet.Add(personB);

            happinesses[(personA, personB)] = happiness;
        }

        // convert adjacency list to adjacency matrix
        string[] people = peopleSet.ToArray();
        int numPeople = people.Length;

        // let's add one extra person for part 2
        int[,] adjMatrix = new int[numPeople + 1, numPeople + 1];
        for (int a = 0; a < numPeople; a++)
        {
            for (int b = 0; b < numPeople; b++)
            {
                if (happinesses.TryGetValue((people[a], people[b]), out int happiness))
                {
                    adjMatrix[a, b] = happiness;
                }
            }
        }

        int part1 = GetOptimalHappiness(numPeople, adjMatrix);
        int part2 = GetOptimalHappiness(numPeople + 1, adjMatrix);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseLine(ReadOnlySpan<byte> line, out string personA, out string personB, out int happiness)
    {
        var reader = new SpanReader(line);
        personA = Encoding.ASCII.GetString(reader.ReadUntil(' '));
        reader.SkipLength("would ".Length);
        int sign = reader.Peek() == 'g' ? 1 : -1;
        reader.SkipLength("gain ".Length);
        happiness = sign * reader.ReadPosIntUntil(' ');
        reader.SkipLength("happiness units by sitting next to ".Length);
        personB = Encoding.ASCII.GetString(reader.ReadUntil('.'));
    }

    private static int GetOptimalHappiness(int numPeople, int[,] adjMatrix)
    {
        int[] people = new int[numPeople];
        for (int p = 0; p < numPeople; p++)
        {
            people[p] = p;
        }

        int maxHappiness = int.MinValue;
        foreach (Span<int> permutation in people.AsSpan().GetPermutations())
        {
            int totalHappiness = 0;
            int prevPerson = permutation[^1];
            for (int j = 0; j < permutation.Length; j++)
            {
                int person = permutation[j];
                totalHappiness += adjMatrix[prevPerson, person];
                totalHappiness += adjMatrix[person, prevPerson];
                prevPerson = person;
            }

            maxHappiness = Math.Max(totalHappiness, maxHappiness);
        }

        return maxHappiness;
    }
}
