using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day21 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var ingredientCount = new Dictionary<string, int>();
        var allgerenCandidates = new Dictionary<string, HashSet<string>>();

        var ingredientSet = new HashSet<string>();
        var reader = new SpanReader(input);

        int totalIngredients = 0;
        while (!reader.Done)
        {
            while (reader.Peek() != '(')
            {
                string ingredient = Encoding.ASCII.GetString(reader.ReadUntil(' '));
                ingredientCount[ingredient] = ingredientCount.GetValueOrDefault(ingredient) + 1;
                ingredientSet.Add(ingredient);
                totalIngredients++;
            }

            reader.SkipLength("(contains ".Length);
            foreach (ReadOnlySpan<byte> allergen in reader.ReadUntil(')').Split(", "u8))
            {
                string allergenStr = Encoding.ASCII.GetString(allergen);
                if (allgerenCandidates.TryGetValue(allergenStr, out HashSet<string>? curSet))
                {
                    curSet.IntersectWith(ingredientSet);
                }
                else
                {
                    allgerenCandidates[allergenStr] = new HashSet<string>(ingredientSet);
                }
            }

            reader.SkipLength(1);
            ingredientSet.Clear();
        }

        int allergenIndex = 0;
        string[] allergens = new string[allgerenCandidates.Count];
        foreach (string allergen in allgerenCandidates.Keys)
        {
            allergens[allergenIndex++] = allergen;
        }
        Array.Sort(allergens);

        string?[] ingredients = new string?[allergens.Length];
        int part1 = totalIngredients;

        for (int allergensLeft = 0; allergensLeft < allergens.Length; allergensLeft++)
        {
            string foundIngredient = string.Empty;
            for (int i = 0; i < allergens.Length; i++)
            {
                if (ingredients[i] != null)
                {
                    continue;
                }

                string allergen = allergens[i];

                HashSet<string> candidates = allgerenCandidates[allergen];
                if (candidates.Count == 1)
                {
                    foundIngredient = candidates.Single();
                    ingredients[i] = foundIngredient;
                    part1 -= ingredientCount[foundIngredient];
                    break;
                }
            }

            foreach (HashSet<string> candidates in allgerenCandidates.Values)
            {
                candidates.Remove(foundIngredient);
            }
        }

        string part2 = string.Join(',', ingredients);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
