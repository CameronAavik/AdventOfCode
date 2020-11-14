using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day15 : ISolver
    {
        public record Ingredient(int[] Qualities, int Calories);

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var ingredients = new Ingredient[4];
            int ingredientIndex = 0;
            foreach (var line in input.Split('\n'))
            {
                int[] qualities = new int[4];
                int calories = 0;
                int i = 0;
                int qualityIndex = 0;
                foreach (var token in line.Split(' '))
                {
                    switch (i++)
                    {
                        case 2 or 4 or 6 or 8:
                            qualities[qualityIndex++] = Int32.Parse(token[..^1]);
                            break;
                        case 10:
                            calories = Int32.Parse(token);
                            break;
                    }
                }

                ingredients[ingredientIndex++] = new Ingredient(qualities, calories);
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

            return new Solution(part1, part2);
        }
    }
}
