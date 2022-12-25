using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day15 : ISolver
{
    public record Ingredient(int[] Qualities, int Calories);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var ingredients = new Ingredient[4];
        int ingredientIndex = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            reader.SkipUntil(':');
            reader.SkipLength(" capacity ".Length);
            int capacity = reader.ReadIntUntil(',');
            reader.SkipLength(" durability ".Length);
            int durability = reader.ReadIntUntil(',');
            reader.SkipLength(" flavor ".Length);
            int flavor = reader.ReadIntUntil(',');
            reader.SkipLength(" texture ".Length);
            int texture = reader.ReadIntUntil(',');
            reader.SkipLength(" calories ".Length);
            int calories = reader.ReadIntUntil('\n');
            ingredients[ingredientIndex++] = new Ingredient(new[] { capacity, durability, flavor, texture }, calories);
        }

        int part1 = 0;
        int part2 = 0;

        for (int ing0 = 0; ing0 < 100; ing0++)
        {
            for (int ing1 = 0; ing1 < 100 - ing0; ing1++)
            {
                for (int ing2 = 0; ing2 < 100 - ing0 - ing1; ing2++)
                {
                    int ing3 = 100 - ing0 - ing1 - ing2;
                    int score = 1;
                    for (int quality = 0; quality < 4; quality++)
                    {
                        int qualityScore =
                            ing0 * ingredients[0].Qualities[quality] +
                            ing1 * ingredients[1].Qualities[quality] +
                            ing2 * ingredients[2].Qualities[quality] +
                            ing3 * ingredients[3].Qualities[quality];

                        score *= Math.Max(qualityScore, 0);
                    }

                    int calories =
                        ing0 * ingredients[0].Calories +
                        ing1 * ingredients[1].Calories +
                        ing2 * ingredients[2].Calories +
                        ing3 * ingredients[3].Calories;

                    part1 = Math.Max(part1, score);
                    if (calories == 500)
                    {
                        part2 = Math.Max(part2, score);
                    }
                }
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
