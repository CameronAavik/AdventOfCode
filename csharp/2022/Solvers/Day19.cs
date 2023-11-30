using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day19 : ISolver
{
    record struct Blueprint(int OreToMakeOreBot, int OreToMakeClayBot, int OreToMakeObsidianBot, int ClayToMakeObisidianBot, int OreToMakeGeodeBot, int ObisdianToMakeGeodeBot);
    record struct State(int Ore, int Clay, int Obsidian, int OreBots, int ClayBots, int ObsidianBots, int MinutesLeft, int Geodes);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int inputCursor = 0;

        int part1 = 0;
        int part2 = 1;

        for (int blueprintId = 1; inputCursor < input.Length; blueprintId++)
        {
            Blueprint blueprint = ParseBlueprint(input, blueprintId, ref inputCursor);
            var solver = new BlueprintSolver(blueprint);

            part1 += blueprintId * solver.GetMaxGeodes(24);
            if (blueprintId <= 3)
                part2 *= solver.GetMaxGeodes(32);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static Blueprint ParseBlueprint(ReadOnlySpan<byte> input, int id, ref int cursor)
    {
        cursor += "Blueprint ".Length;
        cursor += id < 10 ? 1 : 2;
        cursor += ": Each ore robot costs ".Length;
        int oreToMakeOreBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "ore. Each clay robot costs ".Length;
        int oreToMakeClayBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "ore. Each obsidian robot costs ".Length;
        int oreToMakeObsidianBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "ore and ".Length;
        int clayToMakeObisidianBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "clay. Each geode robot costs ".Length;
        int oreToMakeGeodeBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "ore and ".Length;
        int obisdianToMakeGeodeBot = ReadIntUntilSpace(input, ref cursor);
        cursor += "obsidian.\n".Length;
        return new Blueprint(oreToMakeOreBot, oreToMakeClayBot, oreToMakeObsidianBot, clayToMakeObisidianBot, oreToMakeGeodeBot, obisdianToMakeGeodeBot);
    }

    private static int ReadIntUntilSpace(ReadOnlySpan<byte> input, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = input[i++] - '0';

        byte cur;
        while ((cur = input[i++]) != ' ')
            ret = ret * 10 + cur - '0';

        return ret;
    }

    class BlueprintSolver
    {
        private readonly int _oreToMakeOreBot;
        private readonly int _oreToMakeClayBot;
        private readonly int _oreToMakeObsidianBot;
        private readonly int _clayToMakeObisidianBot;
        private readonly int _oreToMakeGeodeBot;
        private readonly int _obisdianToMakeGeodeBot;
        private readonly int _maxOreBotsNeeded;

        public BlueprintSolver(Blueprint blueprint)
        {
            _oreToMakeOreBot = blueprint.OreToMakeOreBot;
            _oreToMakeClayBot = blueprint.OreToMakeClayBot;
            _oreToMakeObsidianBot = blueprint.OreToMakeObsidianBot;
            _clayToMakeObisidianBot = blueprint.ClayToMakeObisidianBot;
            _oreToMakeGeodeBot = blueprint.OreToMakeGeodeBot;
            _obisdianToMakeGeodeBot = blueprint.ObisdianToMakeGeodeBot;
            _maxOreBotsNeeded = Math.Max(Math.Max(_oreToMakeOreBot, _oreToMakeClayBot), Math.Max(_oreToMakeObsidianBot, _oreToMakeGeodeBot));
        }

        public int GetMaxGeodes(int totalMinutes)
        {
            var initState = new State(Ore: 0, Clay: 0, Obsidian: 0, OreBots: 1, ClayBots: 0, ObsidianBots: 0, MinutesLeft: totalMinutes, Geodes: 0);
            int minGeodes = 0;
            int maxGeodes = GetUpperBoundGeodes(0, 0, 0, 1, 0, 0, totalMinutes);

            if (maxGeodes == 0)
                return 0;

            var stacks = new Stack<State>[maxGeodes + 1];
            for (int i = 0; i < stacks.Length; i++)
                stacks[i] = new Stack<State>();

            stacks[maxGeodes].Push(initState);

            for (int i = maxGeodes; i > minGeodes; i--)
            {
                Stack<State> stack = stacks[i];
                while (stack.TryPop(out State state))
                {
                    (int ore, int clay, int obsidian, int oreBots, int clayBots, int obsidianBots, int minutesLeft, int geodes) = state;

                    // Make Ore Bot
                    if (ore < (_maxOreBotsNeeded - oreBots) * minutesLeft)
                    {
                        int minutesToBuild = MinutesUntilCanBuildBot(ore, oreBots, _oreToMakeOreBot);
                        if (minutesToBuild < minutesLeft)
                        {
                            int newOre = ore + minutesToBuild * oreBots - _oreToMakeOreBot;
                            int newClay = clay + minutesToBuild * clayBots;
                            int newObsidian = obsidian + minutesToBuild * obsidianBots;
                            int newOreBots = oreBots + 1;
                            int newMinutesLeft = minutesLeft - minutesToBuild;

                            int upperBound = GetUpperBoundGeodes(newOre, newClay, newObsidian, newOreBots, clayBots, obsidianBots, newMinutesLeft) + geodes;
                            if (upperBound > minGeodes)
                                stacks[upperBound].Push(new State(newOre, newClay, newObsidian, newOreBots, clayBots, obsidianBots, newMinutesLeft, geodes));
                        }
                    }

                    // Make Clay Bot
                    if (clay < (_clayToMakeObisidianBot - clayBots) * minutesLeft)
                    {
                        int minutesToBuild = MinutesUntilCanBuildBot(ore, oreBots, _oreToMakeClayBot);
                        if (minutesToBuild < minutesLeft)
                        {
                            int newOre = ore + minutesToBuild * oreBots - _oreToMakeClayBot;
                            int newClay = clay + minutesToBuild * clayBots;
                            int newObsidian = obsidian + minutesToBuild * obsidianBots;
                            int newClayBots = clayBots + 1;
                            int newMinutesLeft = minutesLeft - minutesToBuild;

                            int upperBound = GetUpperBoundGeodes(newOre, newClay, newObsidian, oreBots, newClayBots, obsidianBots, newMinutesLeft) + geodes;
                            if (upperBound > minGeodes)
                                stacks[upperBound].Push(new State(newOre, newClay, newObsidian, oreBots, newClayBots, obsidianBots, newMinutesLeft, geodes));
                        }
                    }

                    // Make Obsidian Bot
                    if (clayBots > 0)
                    {
                        int minutesToBuild = Math.Max(MinutesUntilCanBuildBot(ore, oreBots, _oreToMakeObsidianBot), MinutesUntilCanBuildBot(clay, clayBots, _clayToMakeObisidianBot));
                        if (minutesToBuild < minutesLeft)
                        {
                            int newOre = ore + minutesToBuild * oreBots - _oreToMakeObsidianBot;
                            int newClay = clay + minutesToBuild * clayBots - _clayToMakeObisidianBot;
                            int newObsidian = obsidian + minutesToBuild * obsidianBots;
                            int newObsidianBots = obsidianBots + 1;
                            int newMinutesLeft = minutesLeft - minutesToBuild;

                            int upperBound = GetUpperBoundGeodes(newOre, newClay, newObsidian, oreBots, clayBots, newObsidianBots, newMinutesLeft) + geodes;
                            if (upperBound > minGeodes)
                                stacks[upperBound].Push(new State(newOre, newClay, newObsidian, oreBots, clayBots, newObsidianBots, newMinutesLeft, geodes));
                        }
                    }

                    // Make Geode Bot
                    if (obsidianBots > 0)
                    {
                        int minutesToBuild = Math.Max(MinutesUntilCanBuildBot(ore, oreBots, _oreToMakeGeodeBot), MinutesUntilCanBuildBot(obsidian, obsidianBots, _obisdianToMakeGeodeBot));
                        if (minutesToBuild < minutesLeft)
                        {
                            int newOre = ore + minutesToBuild * oreBots - _oreToMakeGeodeBot;
                            int newClay = clay + minutesToBuild * clayBots;
                            int newObsidian = obsidian + minutesToBuild * obsidianBots - _obisdianToMakeGeodeBot;
                            int newMinutesLeft = minutesLeft - minutesToBuild;
                            int newGeodes = geodes + newMinutesLeft;

                            int upperBound = GetUpperBoundGeodes(newOre, newClay, newObsidian, oreBots, clayBots, obsidianBots, newMinutesLeft) + newGeodes;
                            if (upperBound > minGeodes)
                            {
                                if (newGeodes > minGeodes)
                                    minGeodes = newGeodes;

                                if (newGeodes == i)
                                    return newGeodes;

                                stacks[upperBound].Push(new State(newOre, newClay, newObsidian, oreBots, clayBots, obsidianBots, newMinutesLeft, newGeodes));
                            }
                        }
                    }
                }
            }

            return minGeodes;
        }

        private int GetUpperBoundGeodes(int ore, int clay, int obsidian, int oreBots, int clayBots, int obsidianBots, int minutesLeft)
        {
            int geodes = 0;
            int oreForClayBots = ore;

            for (int i = 0; i < minutesLeft - 2; i++)
            {
                if (obsidian >= _obisdianToMakeGeodeBot)
                {
                    geodes += minutesLeft - i - 1;
                    obsidian -= _obisdianToMakeGeodeBot;
                }

                obsidian += obsidianBots;
                if (clay >= _clayToMakeObisidianBot)
                {
                    obsidianBots++;
                    clay -= _clayToMakeObisidianBot;
                }

                clay += clayBots;
                if (oreForClayBots >= _oreToMakeClayBot)
                {
                    clayBots++;
                    oreForClayBots -= _oreToMakeClayBot;
                }

                oreForClayBots += oreBots;

                if (ore >= _oreToMakeOreBot)
                {
                    oreBots++;
                    ore -= _oreToMakeOreBot - 1; // subtracting one to counteract the next line where we add the oreBots
                }

                ore += oreBots;
            }

            if (obsidian >= _obisdianToMakeGeodeBot)
                return geodes + 1;

            return geodes;
        }

        private static int MinutesUntilCanBuildBot(int currentResource, int rate, int cost)
        {
            if (currentResource >= cost)
                return 1;

            (int div, int rem) = Math.DivRem(cost - currentResource, rate);
            return div + (rem == 0 ? 1 : 2);
        }
    }
}
