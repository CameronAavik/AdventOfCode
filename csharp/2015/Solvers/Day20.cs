using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day20 : ISolver
{
    // first 10 primes, add more primes if needed?
    private static readonly int[] s_primes =
    [
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29
    ];

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var target = new SpanReader(input).ReadPosIntUntil('\n');
        solution.SubmitPart1(SolvePart1(target));
        solution.SubmitPart2(SolvePart2(target));
    }

    private static int SolvePart1(int target)
    {
        // divide the target by the present multiplier and round up if not an integer.
        const int presentMultiplier = 10;
        target = target / presentMultiplier + (target % presentMultiplier == 0 ? 0 : 1);

        // essentially we are iterating through different combinations of prime factorisations
        // 
        var best = int.MaxValue;
        var counts = new int[s_primes.Length];
        var totals = new int[s_primes.Length];
        var muls = new int[s_primes.Length];

        for (var i = 0; i < s_primes.Length; i++)
        {
            totals[i] = 1;
            muls[i] = 1;
        }

        var cur = 0;
        var prod = 1;
        var mulProd = 1;
        while (cur < s_primes.Length)
        {
            if (prod < target && mulProd < best)
            {
                var prime = s_primes[cur];
                mulProd *= prime;
                counts[cur]++;
                muls[cur] *= prime;
                prod /= totals[cur];
                totals[cur] += muls[cur];
                prod *= totals[cur];
                cur = 0;
            }
            else
            {
                if (prod >= target)
                {
                    best = Math.Min(best, mulProd);
                }
                counts[cur] = 0;
                mulProd /= muls[cur];
                muls[cur] = 1;
                prod /= totals[cur];
                totals[cur] = 1;
                cur++;
            }
        }

        return best;
    }

    private static int SolvePart2(int target)
    {
        // divide the target by the present multiplier and round up if not an integer.
        const int presentMultiplier = 11;
        target = target / presentMultiplier + (target % presentMultiplier == 0 ? 0 : 1);

        // the batch size is a highly composite number
        const int batchSize = 15120;
        var initPresents = new int[batchSize];
        var countPresents = new int[batchSize];
        for (var i = 1; i <= 50; i++)
        {
            if (batchSize % i != 0)
            {
                continue;
            }

            for (var j = 0; j < batchSize; j += i)
            {
                initPresents[j] += j / i;
                countPresents[j] += batchSize / i;
            }
        }

        var presents = new int[batchSize];
        var start = 0;
        var batchNumber = 0;
        while (true)
        {
            Array.Copy(initPresents, presents, batchSize);
            for (var i = 2; i <= 50; i++)
            {
                if (batchSize % i == 0)
                {
                    continue;
                }

                var jStart = ((-start % i) + i) % i;
                for (var j = jStart; j < batchSize; j += i)
                {
                    presents[j] += (j + start) / i;
                }
            }

            for (var i = 0; i < batchSize; i++)
            {
                if (presents[i] + countPresents[i] * batchNumber >= target)
                {
                    return start + i;
                }
            }

            start += batchSize;
            batchNumber++;
        }
    }
}
