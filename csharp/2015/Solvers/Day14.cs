using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day14 : ISolver
{
    public record Reindeer(int Speed, int FlyDuration, int RestDuration);

    public class ReindeerState(Reindeer data)
    {
        public Reindeer Data { get; init; } = data;

        public int DistanceTraveled { get; set; }

        public int Points { get; set; }

        public int DurationLeft { get; set; } = data.FlyDuration;

        public bool IsFlying { get; set; } = true;
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reindeers = new List<Reindeer>();
        foreach (var lineRange in input.SplitLines())
        {
            reindeers.Add(ParseLine(input[lineRange]));
        }

        var reindeerStates = new ReindeerState[reindeers.Count];
        for (var i = 0; i < reindeerStates.Length; i++)
        {
            reindeerStates[i] = new ReindeerState(reindeers[i]);
        }

        for (var i = 0; i < 2503; i++)
        {
            var furthestDistance = 0;
            for (var j = 0; j < reindeerStates.Length; j++)
            {
                var reindeerState = reindeerStates[j];
                if (reindeerState.IsFlying)
                {
                    reindeerState.DistanceTraveled += reindeerState.Data.Speed;
                }

                var durationLeft = --reindeerState.DurationLeft;
                if (durationLeft == 0)
                {
                    reindeerState.IsFlying = !reindeerState.IsFlying;
                    reindeerState.DurationLeft = reindeerState.IsFlying
                        ? reindeerState.Data.FlyDuration
                        : reindeerState.Data.RestDuration;
                }

                furthestDistance = Math.Max(reindeerState.DistanceTraveled, furthestDistance);
            }

            foreach (var state in reindeerStates)
            {
                if (state.DistanceTraveled == furthestDistance)
                {
                    state.Points++;
                }
            }
        }

        var maxDistance = 0;
        var maxPoints = 0;

        foreach (var state in reindeerStates)
        {
            maxDistance = Math.Max(state.DistanceTraveled, maxDistance);
            maxPoints = Math.Max(state.Points, maxPoints);
        }

        solution.SubmitPart1(maxDistance);
        solution.SubmitPart2(maxPoints);
    }

    private static Reindeer ParseLine(ReadOnlySpan<byte> line)
    {
        var reader = new SpanReader(line);
        reader.SkipUntil(' ');
        reader.SkipLength("can fly ".Length);
        var speed = reader.ReadPosIntUntil(' ');
        reader.SkipLength("km/s for ".Length);
        var flyDuration = reader.ReadPosIntUntil(' ');
        reader.SkipLength("seconds, but then must rest for ".Length);
        var restDuration = reader.ReadPosIntUntil(' ');
        return new Reindeer(speed, flyDuration, restDuration);
    }
}
