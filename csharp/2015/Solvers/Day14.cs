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
            public Reindeer Data { get; init; }

            public int DistanceTraveled { get; set; }

            public int Points { get; set; }

            public int DurationLeft { get; set; }

            public bool IsFlying { get; set; }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var reindeers = new List<Reindeer>();
            foreach (var line in input.Split('\n'))
            {
                reindeers.Add(ParseLine(line));
            }

            var reindeerStates = new ReindeerState[reindeers.Count];
            for (int i = 0; i < reindeerStates.Length; i++)
            {
                reindeerStates[i] = new ReindeerState
                {
                    Data = reindeers[i],
                    DurationLeft = reindeers[i].FlyDuration,
                    IsFlying = true
                };
            }

            for (int i = 0; i < 2503; i++)
            {
                int furthestDistance = 0;
                for (int j = 0; j < reindeerStates.Length; j++)
                {
                    var reindeerState = reindeerStates[j];
                    if (reindeerState.IsFlying)
                    {
                        reindeerState.DistanceTraveled += reindeerState.Data.Speed;
                    }

                    if (--reindeerState.DurationLeft == 0)
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

            foreach (var state in reindeerStates)
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
                        speed = Int32.Parse(token);
                        break;
                    case 6:
                        flyDuration = Int32.Parse(token);
                        break;
                    case 13:
                        restDuration = Int32.Parse(token);
                        break;
                }
            }

            return new Reindeer(speed, flyDuration, restDuration);
        }
    }
}
