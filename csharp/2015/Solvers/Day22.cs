using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day22 : ISolver
{
    public record GameState(int Mana, int PlayerHP, int BossHP, int Shield, int Poison, int Recharge);

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        ParseInput(input, out int bossHp, out int bossDamage);
        int part1 = Solve(bossHp, bossDamage, false);
        int part2 = Solve(bossHp, bossDamage, true);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int Solve(int bossHp, int bossDamage, bool hasExtraDamage)
    {
        const int startingMana = 500;
        const int startingHp = 50;

        var pq = new PrioritySet<GameState, int>();

        var firstState = new GameState(startingMana, startingHp, bossHp, 0, 0, 0);
        pq.Enqueue(firstState, 0);

        while (pq.TryDequeue(out GameState state, out int usedMana))
        {
            // check if boss is dead
            if (state.BossHP == 0)
            {
                return usedMana;
            }

            // If it is part 2, apply damage to the player
            if (hasExtraDamage)
            {
                int newHp = state.PlayerHP - 1;
                if (newHp == 0)
                {
                    continue;
                }

                state = state with { PlayerHP = newHp };
            }

            // Apply all active effects
            if (state.Shield > 0)
            {
                state = state with { Shield = state.Shield - 1 };
            }

            if (state.Poison > 0)
            {
                state = state with { BossHP = Math.Max(0, state.BossHP - 3), Poison = state.Poison - 1 };
                if (state.BossHP == 0)
                {
                    return usedMana;
                }
            }

            if (state.Recharge > 0)
            {
                state = state with { Mana = state.Mana + 101, Recharge = state.Recharge - 1 };
            }

            // Try use magic missile
            if (state.Mana >= 53)
            {
                GameState stateAfterPlayerTurn = state with { BossHP = Math.Max(state.BossHP - 4, 0), Mana = state.Mana - 53 };
                if (SimulateBossTurn(stateAfterPlayerTurn, bossDamage, out GameState stateAfterBossTurn))
                {
                    pq.EnqueueOrUpdate(stateAfterBossTurn, usedMana + 53);
                }
            }

            // Try use drain
            if (state.Mana >= 73)
            {
                GameState stateAfterPlayerTurn = state with
                {
                    BossHP = Math.Max(state.BossHP - 2, 0),
                    PlayerHP = state.PlayerHP + 2,
                    Mana = state.Mana - 73
                };

                if (SimulateBossTurn(stateAfterPlayerTurn, bossDamage, out GameState stateAfterBossTurn))
                {
                    pq.EnqueueOrUpdate(stateAfterBossTurn, usedMana + 73);
                }
            }

            // Try use shield
            if (state.Mana >= 113 & state.Shield == 0)
            {
                GameState stateAfterPlayerTurn = state with { Shield = 6, Mana = state.Mana - 113 };
                if (SimulateBossTurn(stateAfterPlayerTurn, bossDamage, out GameState stateAfterBossTurn))
                {
                    pq.EnqueueOrUpdate(stateAfterBossTurn, usedMana + 113);
                }
            }

            // Try use poison
            if (state.Mana >= 173 & state.Poison == 0)
            {
                GameState stateAfterPlayerTurn = state with { Poison = 6, Mana = state.Mana - 173 };
                if (SimulateBossTurn(stateAfterPlayerTurn, bossDamage, out GameState stateAfterBossTurn))
                {
                    pq.EnqueueOrUpdate(stateAfterBossTurn, usedMana + 173);
                }
            }

            // Try use recharge
            if (state.Mana >= 229 & state.Recharge == 0)
            {
                GameState stateAfterPlayerTurn = state with { Recharge = 5, Mana = state.Mana - 229 };
                if (SimulateBossTurn(stateAfterPlayerTurn, bossDamage, out GameState stateAfterBossTurn))
                {
                    pq.EnqueueOrUpdate(stateAfterBossTurn, usedMana + 229);
                }
            }
        }

        ThrowHelper.ThrowException("Unable to beat the boss");
        return default;
    }

    private static void ParseInput(ReadOnlySpan<char> input, out int bossHp, out int bossDamage)
    {
        int firstNewlineIndex = input.IndexOf('\n');
        bossHp = int.Parse(input[12..firstNewlineIndex]);
        bossDamage = int.Parse(input[(firstNewlineIndex + 9)..]);
    }

    private static bool SimulateBossTurn(GameState state, int bossDamage, out GameState newState)
    {
        // if the boss is already dead, don't try simulate.
        if (state.BossHP == 0)
        {
            newState = state;
            return true;
        }

        // apply effects
        bool hasArmor = state.Shield > 0;
        if (hasArmor)
        {
            state = state with { Shield = state.Shield - 1 };
        }

        if (state.Poison > 0)
        {
            state = state with { BossHP = Math.Max(0, state.BossHP - 3), Poison = state.Poison - 1 };

            // if the boss has died, end simulating the bosses turn.
            if (state.BossHP == 0)
            {
                newState = state;
                return true;
            }
        }

        if (state.Recharge > 0)
        {
            state = state with { Mana = state.Mana + 101, Recharge = state.Recharge - 1 };
        }

        // make attack
        int damage = Math.Max(1, bossDamage - (hasArmor ? 7 : 0));
        newState = state with { PlayerHP = Math.Max(0, state.PlayerHP - damage) };
        return newState.PlayerHP > 0;
    }
}
