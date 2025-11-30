using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        Span<int> diffSignCounts = stackalloc int[3]; // [0] stores decreasing, [1] stores zero, [2] stores increasing
        Span<int> lastDiffWithSignIndex = stackalloc int[3]; // For each sign type, stores the index of the last difference we saw with it
        Span<int> levelDiffs = stackalloc int[32]; // Stores the pairwise differences of the numbers in the list
        Span<int> largeDiffIndexes = stackalloc int[32]; // Stores the indexes of any differences that are too large

        var i = 0;
        while (i < input.Length)
        {
            diffSignCounts.Clear();
            var numDiffs = 0;
            var largeDiffs = 0;

            var c = input[i++];
            var prev = c - '0';
            while (i < input.Length && (c = input[i++]) >= '0')
                prev = prev * 10 + c - '0';

            while (c != '\n')
            {
                var number = input[i++] - '0';
                while (i < input.Length && (c = input[i++]) >= '0')
                    number = number * 10 + c - '0';

                var diff = number - prev;
                var diffSign = Math.Sign(diff) + 1;
                diffSignCounts[diffSign]++;
                lastDiffWithSignIndex[diffSign] = numDiffs;
                if (diff is < -3 or > 3)
                    largeDiffIndexes[largeDiffs++] = numDiffs;

                levelDiffs[numDiffs++] = diff;
                prev = number;
            }

            var decreaseCount = diffSignCounts[0];
            var increaseCount = diffSignCounts[2];

            // Handle the case where all numbers are increasing or decreasing
            if (decreaseCount == numDiffs || increaseCount == numDiffs)
            {
                if (largeDiffs == 0)
                {
                    part1++;
                    part2++;
                }
                else if (largeDiffs == 1)
                {
                    // Only can work if it starts or ends with the large difference
                    var largeDiffIndex = largeDiffIndexes[0];
                    if (largeDiffIndex == 0 || largeDiffIndex == numDiffs - 1)
                        part2++;
                }
                continue;
            }

            // Get the index of the single difference going in the wrong direction
            int expectedSign;
            int indexToRemove;
            if (decreaseCount == numDiffs - 1)
            {
                expectedSign = -1;
                indexToRemove = increaseCount == 1 ? lastDiffWithSignIndex[2] : lastDiffWithSignIndex[1]; // either increasing or zero
            }
            else if (increaseCount == numDiffs - 1)
            {
                expectedSign = 1;
                indexToRemove = decreaseCount == 1 ? lastDiffWithSignIndex[0] : lastDiffWithSignIndex[1]; // either decreasing or zero
            }
            else
            {
                // Too many differences in the wrong direction, skip
                continue;
            }

            // Our next goal is to identify which side of the difference should be removed to ensure that the resulting sequence is valid
            // We can easily calculate the new difference are removal by adding together adjacent differences: (a - b) + (b - c) == a - c

            // If there are large differences involved, we may be forced to choose either the left or right side
            var forcedMergeIndex = -1;
            if (largeDiffs == 1)
            {
                // Must be either the same as indexToRemove or immediately next to merge
                var distance = largeDiffIndexes[0] - indexToRemove;
                if (distance is -1 or 1)
                    forcedMergeIndex = indexToRemove + distance;
                else if (distance != 0)
                    continue;
            }
            else if (largeDiffs == 2)
            {
                // one must be the indexToRemove, the other must be immediately before or after it
                var distance1 = largeDiffIndexes[0] - indexToRemove;
                var distance2 = largeDiffIndexes[1] - indexToRemove;
                if (distance1 == 0 && distance2 == 1)
                    forcedMergeIndex = indexToRemove + 1;
                else if (distance1 == -1 && distance2 == 0)
                    forcedMergeIndex = indexToRemove - 1;
                else
                    continue;
            }
            else if (largeDiffs > 2)
            {
                continue;
            }

            // Now test the left and right sides to see if the new sequence is valid
            var diffToRemove = levelDiffs[indexToRemove];
            if (forcedMergeIndex != -1) // handle forced case
            {
                if (expectedSign * (diffToRemove + levelDiffs[forcedMergeIndex]) is > 0 and <= 3)
                    part2++;
            }
            else if (
                indexToRemove == 0
                || indexToRemove == numDiffs - 1
                || (expectedSign * (diffToRemove + levelDiffs[indexToRemove - 1]) is > 0 and <= 3)
                || (expectedSign * (diffToRemove + levelDiffs[indexToRemove + 1]) is > 0 and <= 3))
            {
                part2++;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
