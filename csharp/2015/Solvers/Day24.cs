using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day24 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var weightList = new List<int>();
        var totalWeight = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            var weight = reader.ReadPosIntUntil('\n');
            weightList.Add(weight);
            totalWeight += weight;
        }

        int[] weightArr = [.. weightList];
        Array.Sort(weightArr);

        solution.SubmitPart1(Solve(weightArr, totalWeight / 3));
        solution.SubmitPart2(Solve(weightArr, totalWeight / 4));
    }

    private static BigInteger Solve(int[] weights, int targetWeight)
    {
        for (var count = 1; count < weights.Length; count++)
        {
            // Algorithm adapted from the python itertools.combinations documentation
            // https://docs.python.org/3/library/itertools.html#itertools.combinations
            var indices = new int[count];
            for (var i = 0; i < count; i++)
            {
                indices[i] = i;
            }

            BigInteger? minQE = null;
            while (true)
            {
                var hasFinished = true;
                int i;
                for (i = count - 1; i >= 0; i--)
                {
                    if (indices[i] != i + weights.Length - count)
                    {
                        hasFinished = false;
                        break;
                    }
                }

                if (hasFinished)
                {
                    break;
                }

                indices[i]++;
                for (var j = i + 1; j < count; j++)
                {
                    indices[j] = indices[j - 1] + 1;
                }

                var totalWeight = 0;
                foreach (var index in indices)
                {
                    totalWeight += weights[index];
                }

                if (totalWeight == targetWeight)
                {
                    BigInteger qe = 1;
                    foreach (var index in indices)
                    {
                        qe *= weights[index];
                    }

                    if (minQE == null || qe < minQE)
                    {
                        minQE = qe;
                    }
                }
            }

            if (minQE != null)
            {
                return minQE.Value;
            }
        }

        return 0;
    }
}
