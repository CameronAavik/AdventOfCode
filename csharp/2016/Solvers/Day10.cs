using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day10 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // negative means output, positive means bot
        var botOutputs = new Dictionary<int, (bool IsLowBot, int Low, bool IsHighBot, int High)>();
        var valueQueue = new Queue<(bool IsBot, int Destination, int Value)>();

        int maxBotId = 0;
        int maxOutputId = 0;

        // first pass we just keep track of where the bot sends its chips
        var parser = new SpanReader(input);
        while (!parser.Done)
        {
            if (parser.Peek() == 'b')
            {
                ParseBotInstruction(ref parser, out int bot, out bool isLowBot, out int low, out bool isHighBot, out int high);
                botOutputs[bot] = (isLowBot, low, isHighBot, high);

                maxBotId = Math.Max(bot, maxBotId);
                if (!isLowBot)
                {
                    maxOutputId = Math.Max(maxOutputId, low);
                }

                if (!isHighBot)
                {
                    maxOutputId = Math.Max(maxOutputId, high);
                }
            }
            else
            {
                ParseBotStartingValue(ref parser, out int bot, out int value);
                valueQueue.Enqueue((true, bot, value));
            }
        }

        int[]? botValues = new int[maxBotId + 1];
        int[]? outputValues = new int[maxOutputId + 1];

        int part1 = 0;
        while (valueQueue.TryDequeue(out (bool IsBot, int Destination, int Value) x))
        {
            (bool isBot, int destination, int value) = x;

            if (!isBot)
            {
                outputValues[destination] = value;
                continue;
            }

            int curValue = botValues[destination];
            if (curValue == 0)
            {
                botValues[destination] = value;
            }
            else
            {
                (bool IsLowBot, int Low, bool IsHighBot, int High) = botOutputs[destination];

                int low = Math.Min(curValue, value);
                int high = Math.Max(curValue, value);

                if (low == 17 && high == 61)
                {
                    part1 = destination;
                }

                valueQueue.Enqueue((IsLowBot, Low, low));
                valueQueue.Enqueue((IsHighBot, High, high));
            }
        }

        int part2 = outputValues[0] * outputValues[1] * outputValues[2];
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseBotInstruction(ref SpanReader parser, out int bot, out bool isLowBot, out int low, out bool isHighBot, out int high)
    {
        parser.SkipLength("bot ".Length);
        bot = parser.ReadPosIntUntil(' ');
        parser.SkipLength("gives low to ".Length);
        isLowBot = parser.Peek() == 'b';
        parser.SkipLength(isLowBot ? "bot ".Length : "output ".Length);
        low = parser.ReadPosIntUntil(' ');
        parser.SkipLength("and high to ".Length);
        isHighBot = parser.Peek() == 'b';
        parser.SkipLength(isHighBot ? "bot ".Length : "output ".Length);
        high = parser.ReadPosIntUntil('\n');
    }

    private static void ParseBotStartingValue(ref SpanReader parser, out int bot, out int value)
    {
        parser.SkipLength("value ".Length);
        value = parser.ReadPosIntUntil(' ');
        parser.SkipLength("goes to bot ".Length);
        bot = parser.ReadPosIntUntil('\n');
    }
}
