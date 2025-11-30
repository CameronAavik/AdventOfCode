using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day19 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.TrimEnd((byte)'\n');
        var elements = new Dictionary<string, int>();
        var replacements = new List<List<List<int>>>(); // replacements[element][replacement][replacementElement]

        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            if (line.Length == 0)
            {
                break;
            }

            var lhsLen = line[1] == ' ' ? 1 : 2;
            var lhs = Encoding.ASCII.GetString(line[0..lhsLen]);
            var rhs = line[(lhsLen + 4)..];

            var replacement = ParseMolecule(elements, replacements, rhs);
            if (!elements.TryGetValue(lhs, out var lhsIndex))
            {
                lhsIndex = elements.Count;
                elements.Add(lhs, lhsIndex);
                replacements.Add([replacement]);
            }
            else
            {
                replacements[lhsIndex].Add(replacement);
            }
        }

        var moleculeSpan = input[(input.LastIndexOf((byte)'\n') + 1)..];
        var molecule = ParseMolecule(elements, replacements, moleculeSpan);

        var part1 = SolvePart1(replacements, molecule);
        var part2 = SolvePart2(elements, molecule);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static List<int> ParseMolecule(
        Dictionary<string, int> elements,
        List<List<List<int>>> replacements,
        ReadOnlySpan<byte> moleculeSpan)
    {
        var replacement = new List<int>();
        var i = 0;
        while (i < moleculeSpan.Length)
        {
            string element;
            if (i == moleculeSpan.Length - 1 || moleculeSpan[i + 1] is >= (byte)'A' and <= (byte)'Z')
            {
                element = char.ToString((char)moleculeSpan[i]);
                i++;
            }
            else
            {
                element = Encoding.ASCII.GetString(moleculeSpan.Slice(i, 2));
                i += 2;
            }

            if (!elements.TryGetValue(element, out var elementIndex))
            {
                elementIndex = elements.Count;
                elements.Add(element, elementIndex);
                replacements.Add([]);
            }

            replacement.Add(elementIndex);
        }

        return replacement;
    }

    private static int SolvePart1(List<List<List<int>>> replacements, List<int> molecule)
    {
        var total = 0;

        for (var i = 0; i < molecule.Count; i++)
        {
            var cur = molecule[i];

            // the last molecule can't overlap with any next molecules
            // so we just assume all replacements are valid
            if (i == molecule.Count - 1)
            {
                total += replacements[cur].Count;
                break;
            }

            var next = molecule[i + 1];

            // for each replacement for the current molecule, see if rep1 + next == cur + rep2
            // if it is possible, don't count it.
            foreach (var rep1 in replacements[cur])
            {
                if (rep1[0] != cur)
                {
                    total++;
                    continue;
                }

                var shouldCount = true;
                foreach (var rep2 in replacements[next])
                {
                    if (rep1.Count == rep2.Count && rep2[^1] == next)
                    {
                        var overlaps = true;
                        for (var j = 1; j < rep1.Count; j++)
                        {
                            if (rep1[j] != rep2[j - 1])
                            {
                                overlaps = false;
                                break;
                            }
                        }

                        if (overlaps)
                        {
                            shouldCount = false;
                            break;
                        }
                    }
                }

                if (shouldCount)
                {
                    total++;
                }
            }
        }

        return total;
    }

    private static int SolvePart2(Dictionary<string, int> replacements, List<int> molecule)
    {
        var yElemIndex = replacements["Y"];
        var rnElemIndex = replacements["Rn"];
        var arElemIndex = replacements["Ar"];

        var yCount = 0;
        var rnCount = 0;
        var arCount = 0;
        foreach (var element in molecule)
        {
            if (element == yElemIndex)
            {
                yCount++;
            }
            else if (element == rnElemIndex)
            {
                rnCount++;
            }
            else if (element == arElemIndex)
            {
                arCount++;
            }
        }

        return molecule.Count - rnCount - arCount - 2 * yCount - 1;
    }
}
