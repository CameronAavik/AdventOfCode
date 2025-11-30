using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day17 : ISolver
{
    public record Element(int MinContainers, int AllCount, int MinimizedCount)
    {
        public static readonly Element Zero = new(0, 0, 0);
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var sizes = new List<int>();
        var reader = new SpanReader(input);
        while (!reader.Done)
            sizes.Add(reader.ReadPosIntUntil('\n'));

        var M = new Element[151, sizes.Count];

        // initialize first row
        for (var i = 0; i <= 150; i++)
        {
            M[i, 0] = Element.Zero;
        }
        M[0, 0] = new Element(MinContainers: 0, AllCount: 1, MinimizedCount: 1);
        M[sizes[0], 0] = new Element(MinContainers: 1, AllCount: 1, MinimizedCount: 1);

        for (var i = 1; i < sizes.Count; i++)
        {
            var size = sizes[i];
            for (var capacity = 0; capacity <= 150; capacity++)
            {
                var containerNotUsed = M[capacity, i - 1];
                var containerUsed = capacity >= size ? M[capacity - size, i - 1] : null;

                // if the the size is bigger than the capacity, then assume the container isn't used.
                if (containerUsed is null || containerUsed.AllCount == 0)
                {
                    M[capacity, i] = containerNotUsed;
                    continue;
                }

                // if it's impossible to get the desired size by not using the container, then assume it is used.
                if (containerNotUsed.AllCount == 0)
                {
                    M[capacity, i] = containerUsed with { MinContainers = containerUsed.MinContainers + 1 };
                    continue;
                }

                var allCount = containerUsed.AllCount + containerNotUsed.AllCount;

                var countIfUsed = containerUsed.MinContainers + 1;
                var countIfNotUsed = containerNotUsed.MinContainers;

                var minContainers = Math.Min(countIfUsed, countIfNotUsed);
                var minimizedCount = countIfUsed.CompareTo(countIfNotUsed) switch
                {
                    < 0 => containerUsed.MinimizedCount,
                    > 0 => containerNotUsed.MinimizedCount,
                    0 => containerUsed.MinimizedCount + containerNotUsed.MinimizedCount,
                };

                M[capacity, i] = new Element(minContainers, allCount, minimizedCount);
            }
        }

        var solutionElement = M[150, sizes.Count - 1];
        solution.SubmitPart1(solutionElement.AllCount);
        solution.SubmitPart2(solutionElement.MinimizedCount);
    }
}
