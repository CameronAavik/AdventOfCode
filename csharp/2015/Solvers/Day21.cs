using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day21 : ISolver
    {
        public record Equipment(int Cost, int Damage, int Armor);

        private static readonly Equipment[] s_weapons = new Equipment[]
        {
            new(8, 4, 0),
            new(10, 5, 0),
            new(25, 6, 0),
            new(40, 7, 0),
            new(74, 8, 0)
        };

        private static readonly Equipment[] s_armor = new Equipment[]
        {
            new(0, 0, 0),
            new(13, 0, 1),
            new(31, 0, 2),
            new(53, 0, 3),
            new(75, 0, 4),
            new(102, 0, 5)
        };

        private static readonly Equipment[] s_rings = new Equipment[]
        {
            new(0, 0, 0),
            new(25, 1, 0),
            new(50, 2, 0),
            new(100, 3, 0),
            new(20, 0, 1),
            new(40, 0, 2),
            new(80, 0, 3)
        };

        public Solution Solve(ReadOnlySpan<char> input)
        {
            ParseInput(input, out int bossHp, out int bossDamage, out int bossArmor);

            int cheapestWin = int.MaxValue;
            int mostExpensiveLoss = int.MinValue;

            foreach (Equipment weapon in s_weapons)
            {
                foreach (Equipment armor in s_armor)
                {
                    for (int ring1Index = 0; ring1Index < s_rings.Length; ring1Index++)
                    {
                        Equipment ring1 = s_rings[ring1Index];
                        for (int ring2Index = ring1Index; ring2Index < s_rings.Length; ring2Index++)
                        {
                            // can't pick same ring twice, except for no ring
                            if (ring1Index == ring2Index && ring1Index > 0)
                            {
                                continue;
                            }

                            Equipment purchase = Purchase(weapon, armor, ring1, s_rings[ring2Index]);
                            if (DoesPlayerWin(purchase, bossHp, bossDamage, bossArmor))
                            {
                                cheapestWin = Math.Min(purchase.Cost, cheapestWin);
                            }
                            else
                            {
                                mostExpensiveLoss = Math.Max(purchase.Cost, mostExpensiveLoss);
                            }
                        }
                    }
                }
            }

            return new Solution(part1: cheapestWin, part2: mostExpensiveLoss);
        }

        private static void ParseInput(ReadOnlySpan<char> input, out int bossHp, out int bossDamage, out int bossArmor)
        {
            int firstNewlineIndex = input.IndexOf('\n');
            int secondNewLineIndex = input.LastIndexOf('\n');
            bossHp = int.Parse(input[12..firstNewlineIndex]);
            bossDamage = int.Parse(input[(firstNewlineIndex + 9)..secondNewLineIndex]);
            bossArmor = int.Parse(input[(secondNewLineIndex + 8)..]);
        }

        private static Equipment Purchase(Equipment weapon, Equipment armor, Equipment ring1, Equipment ring2)
        {
            return new Equipment(
                weapon.Cost + armor.Cost + ring1.Cost + ring2.Cost,
                weapon.Damage + armor.Damage + ring1.Damage + ring2.Damage,
                weapon.Armor + armor.Armor + ring1.Armor + ring2.Armor);
        }

        private static bool DoesPlayerWin(Equipment equipment, int bossHp, int bossDamage, int bossArmor)
        {
            const int playerHp = 100;
            int actualPlayerDamage = Math.Max(1, equipment.Damage - bossArmor);
            int actualBossDamage = Math.Max(1, bossDamage - equipment.Armor);

            int turnsTillPlayerDies = playerHp / actualBossDamage + (playerHp % actualBossDamage == 0 ? 0 : 1);
            int turnsTillBossDies = bossHp / actualPlayerDamage + (bossHp % actualPlayerDamage == 0 ? 0 : 1);

            return turnsTillBossDies <= turnsTillPlayerDies;
        }
    }
}
