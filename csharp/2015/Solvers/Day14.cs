using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day14 : ISolver
    {
        public record Reindeer(int Speed, int FlyDuration, int RestDuration);

        public class ReindeerState
        {
            public ReindeerState(Reindeer data)
            {
                Data = data;
                DistanceTraveled = 0;
                Points = 0;
                DurationLeft = data.FlyDuration;
                IsFlying = true;
            }

            public Reindeer Data { get; init; }

            public int DistanceTraveled { get; set; }

            public int Points { get; set; }

            public int DurationLeft { get; set; }

            public bool IsFlying { get; set; }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var reindeers = new List<Reindeer>();
            foreach (ReadOnlySpan<char> line in input.SplitLines())
            {
                reindeers.Add(ParseLine(line));
            }

            var reindeerStates = new ReindeerState[reindeers.Count];
            for (int i = 0; i < reindeerStates.Length; i++)
            {
                reindeerStates[i] = new ReindeerState(reindeers[i]);
            }

            for (int i = 0; i < 2503; i++)
            {
                int furthestDistance = 0;
                for (int j = 0; j < reindeerStates.Length; j++)
                {
                    ReindeerState reindeerState = reindeerStates[j];
                    if (reindeerState.IsFlying)
                    {
                        reindeerState.DistanceTraveled += reindeerState.Data.Speed;
                    }

                    int durationLeft = --reindeerState.DurationLeft;
                    if (durationLeft == 0)
                    {
                        reindeerState.IsFlying = !reindeerState.IsFlying;
                        reindeerState.DurationLeft = reindeerState.IsFlying
                            ? reindeerState.Data.FlyDuration
                            : reindeerState.Data.RestDuration;
                    }

                    furthestDistance = Math.Max(reindeerState.DistanceTraveled, furthestDistance);
                }

                foreach (ReindeerState state in reindeerStates)
                {
                    if (state.DistanceTraveled == furthestDistance)
                    {
                        state.Points++;
                    }
                }
            }

            int maxDistance = 0;
            int maxPoints = 0;

            foreach (ReindeerState state in reindeerStates)
            {
                maxDistance = Math.Max(state.DistanceTraveled, maxDistance);
                maxPoints = Math.Max(state.Points, maxPoints);
            }

            return new Solution(maxDistance, maxPoints);
        }

        private static Reindeer ParseLine(ReadOnlySpan<char> line)
        {
            int speed = 0;
            int flyDuration = 0;
            int restDuration = 0;

            int tokenIndex = 0;
            foreach (ReadOnlySpan<char> token in line.Split(' '))
            {
                switch (tokenIndex++)
                {
                    case 3:
                        speed = int.Parse(token);
                        break;
                    case 6:
                        flyDuration = int.Parse(token);
                        break;
                    case 13:
                        restDuration = int.Parse(token);
                        break;
                }
            }

            return new Reindeer(speed, flyDuration, restDuration);
        }
    }
}
