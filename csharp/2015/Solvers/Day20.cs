using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day20 : ISolver
{
    // first 10 primes, add more primes if needed?
    private static readonly int[] s_primes = new int[]
    {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29
    };

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int target = int.Parse(input);
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
        int best = int.MaxValue;
        int[] counts = new int[s_primes.Length];
        int[] totals = new int[s_primes.Length];
        int[] muls = new int[s_primes.Length];

        for (int i = 0; i < s_primes.Length; i++)
        {
            totals[i] = 1;
            muls[i] = 1;
        }

        int cur = 0;
        int prod = 1;
        int mulProd = 1;
        while (cur < s_primes.Length)
        {
            if (prod < target && mulProd < best)
            {
                int prime = s_primes[cur];
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
        int[] initPresents = new int[batchSize];
        int[] countPresents = new int[batchSize];
        for (int i = 1; i <= 50; i++)
        {
            if (batchSize % i != 0)
            {
                continue;
            }

            for (int j = 0; j < batchSize; j += i)
            {
                initPresents[j] += j / i;
                countPresents[j] += batchSize / i;
            }
        }

        int[] presents = new int[batchSize];
        int start = 0;
        int batchNumber = 0;
        while (true)
        {
            Array.Copy(initPresents, presents, batchSize);
            for (int i = 2; i <= 50; i++)
            {
                if (batchSize % i == 0)
                {
                    continue;
                }

                int jStart = ((-start % i) + i) % i;
                for (int j = jStart; j < batchSize; j += i)
                {
                    presents[j] += (j + start) / i;
                }
            }

            for (int i = 0; i < batchSize; i++)
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
