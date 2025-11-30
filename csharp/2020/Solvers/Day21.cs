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
        var allergenCandidates = new Dictionary<string, HashSet<string>>();

        var ingredientSet = new HashSet<string>();
        var reader = new SpanReader(input);

        var totalIngredients = 0;
        while (!reader.Done)
        {
            while (reader.Peek() != '(')
            {
                var ingredient = Encoding.ASCII.GetString(reader.ReadUntil(' '));
                ingredientCount[ingredient] = ingredientCount.GetValueOrDefault(ingredient) + 1;
                ingredientSet.Add(ingredient);
                totalIngredients++;
            }

            reader.SkipLength("(contains ".Length);
            var allergensSpan = reader.ReadUntil(')');
            foreach (var allergenRange in allergensSpan.Split(", "u8))
            {
                var allergenStr = Encoding.ASCII.GetString(allergensSpan[allergenRange]);
                if (allergenCandidates.TryGetValue(allergenStr, out var curSet))
                {
                    curSet.IntersectWith(ingredientSet);
                }
                else
                {
                    allergenCandidates[allergenStr] = [.. ingredientSet];
                }
            }

            reader.SkipLength(1);
            ingredientSet.Clear();
        }

        var allergenIndex = 0;
        var allergens = new string[allergenCandidates.Count];
        foreach (var allergen in allergenCandidates.Keys)
        {
            allergens[allergenIndex++] = allergen;
        }
        Array.Sort(allergens);

        var ingredients = new string?[allergens.Length];
        var part1 = totalIngredients;

        for (var allergensLeft = 0; allergensLeft < allergens.Length; allergensLeft++)
        {
            var foundIngredient = string.Empty;
            for (var i = 0; i < allergens.Length; i++)
            {
                if (ingredients[i] != null)
                {
                    continue;
                }

                var allergen = allergens[i];

                var candidates = allergenCandidates[allergen];
                if (candidates.Count == 1)
                {
                    foundIngredient = candidates.Single();
                    ingredients[i] = foundIngredient;
                    part1 -= ingredientCount[foundIngredient];
                    break;
                }
            }

            foreach (var candidates in allergenCandidates.Values)
            {
                candidates.Remove(foundIngredient);
            }
        }

        var part2 = string.Join(',', ingredients);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
