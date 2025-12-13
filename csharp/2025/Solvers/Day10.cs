using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

/// <summary>
/// At some point I may add some more explanations here about how this all works
/// </summary>
public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        var inputIndex = 0;
        while (inputIndex < input.Length)
        {
            var indicators = ParseIndicatorLights(input, ref inputIndex);
            var buttonMasks = new List<uint>();
            var joltages = new List<int>();

            while (true)
            {
                inputIndex++; // ' '
                if (input[inputIndex++] == '(')
                {
                    uint buttonMask = 0;
                    while (true)
                    {
                        var value = input[inputIndex++] - '0';
                        byte c;
                        while ((c = input[inputIndex++]) >= '0') // end on ',' or ')'
                            value = value * 10 + (c - '0');
                        buttonMask |= 1U << value;
                        if (c == ')')
                            break;
                    }

                    buttonMasks.Add(buttonMask);
                }
                else // '{'
                {
                    while (true)
                    {
                        var value = input[inputIndex++] - '0';
                        byte c;
                        while (((c = input[inputIndex++]) & 0xF) <= 9) // end on ',' or '}'
                            value = value * 10 + (c - '0');

                        joltages.Add(value);
                        if (c == '}')
                            break;
                    }

                    inputIndex++; // '\n'
                    break;
                }
            }

            part1 += SolvePart1(indicators, buttonMasks);
            part2 += SolvePart2(buttonMasks, joltages);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    public static int SolvePart1(uint indicators, List<uint> buttonMask)
    {
        // Get smallest subset of buttonMask that xors together to indicators
        for (var count = 1; count <= buttonMask.Count; count++)
        {
            if (CanMakeTargetWithCount(indicators, 0, count))
                return count;
        }

        return -1;

        bool CanMakeTargetWithCount(uint target, int i, int count)
        {
            if (count == 0)
                return target == 0;

            if (i >= buttonMask.Count)
                return false;

            // Try including buttonMask[i]
            if (CanMakeTargetWithCount(target ^ buttonMask[i], i + 1, count - 1))
                return true;

            // Try excluding buttonMask[i]
            return CanMakeTargetWithCount(target, i + 1, count);
        }
    }

    public static int SolvePart2(List<uint> buttonMasks, List<int> joltages)
    {
        var m = joltages.Count;     // equations (rows)
        var n = buttonMasks.Count;  // variables (cols)

        var matrix = new int[m][];
        for (var i = 0; i < m; i++)
            matrix[i] = new int[n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                if (((buttonMasks[i] >> j) & 1) != 0)
                {
                    matrix[j][i] = 1;
                }
            }
        }

        var X0 = new int[n];
        ToColumnHermiteNormalForm(matrix, joltages, X0, out var V);
        var d = V.Length;

        var baseSum = 0;
        for (var i = 0; i < n; i++)
            baseSum += X0[i];

        // If d == 0, there is only one solution
        if (d == 0)
            return baseSum;

        var delta = new int[d];
        for (var k = 0; k < d; k++)
        {
            var v = V[k];
            var s = 0;
            for (var i = 0; i < n; i++)
                s += v[i];
            delta[k] = s;
        }

        // For each button, there is a maximum number of times it can be pressed before it exceeds the joltage requirement.
        var buttonUpperBounds = new int[buttonMasks.Count];
        for (var i = 0; i < n; i++)
        {
            var buttonMask = buttonMasks[i];
            var buttonUpper = int.MaxValue;
            for (var j = 0; j < m; j++)
            {
                if ((buttonMask & (1U << j)) != 0)
                {
                    buttonUpper = Math.Min(buttonUpper, joltages[j]);
                }
            }

            buttonUpperBounds[i] = buttonUpper;
        }

        if (d == 1)
        {
            if (delta[0] == 0)
                return baseSum;
            return MinimiseSingleDimension(X0, baseSum, delta[0], V[0]);
        }

        if (d == 2)
        {
            if (delta[0] == 0 && delta[1] == 0)
                return baseSum;
            return MinimiseTwoDimensions(X0, baseSum, delta[0], delta[1], V[0], V[1], buttonUpperBounds);
        }

        Debug.Assert(d == 3);
        return MinimiseThreeDimensions(X0, baseSum, delta[0], delta[1], delta[2], V[0], V[1], V[2], buttonUpperBounds);
    }

    private static int MinimiseSingleDimension(ReadOnlySpan<int> x, int baseSum, int delta, int[] v)
    {
        var min0 = int.MinValue;
        var max0 = int.MaxValue;

        for (var i = 0; i < x.Length; i++)
        {
            var vi = v[i];
            if (vi > 0)
            {
                min0 = Math.Max(min0, CeilDiv(-x[i], vi));
            }
            else if (vi < 0)
            {
                max0 = Math.Min(max0, FloorDiv(x[i], -vi));
            }
        }

        if (min0 > max0)
            return int.MaxValue;

        var k0 = delta >= 0 ? min0 : max0;
        return baseSum + k0 * delta;
    }

    private static int MinimiseTwoDimensions(ReadOnlySpan<int> X0, int baseSum, int delta0, int delta1, int[] v0, int[] v1, ReadOnlySpan<int> upperBounds)
    {
        var min0 = int.MinValue / 2;
        var max0 = int.MaxValue / 2;
        var min1 = int.MinValue / 2;
        var max1 = int.MaxValue / 2;

        for (var i = 0; i < X0.Length; i++)
        {
            var x = X0[i];
            var n0 = v0[i];
            var n1 = v1[i];
            var upper = upperBounds[i];

            if (n0 != 0 && n1 == 0)
            {
                // 0 <= x + k0 * n0 <= upper[i]
                if (n0 > 0)
                {
                    min0 = Math.Max(min0, CeilDiv(-x, n0));
                    max0 = Math.Min(max0, FloorDiv(upper - x, n0));
                }
                else
                {
                    min0 = Math.Max(min0, CeilDiv(x - upper, -n0));
                    max0 = Math.Min(max0, FloorDiv(x, -n0));
                }
            }
            else if (n0 == 0 && n1 != 0) // Can set bounds on k1
            {
                // 0 <= x + k1 * n1 <= upper[i]
                if (n1 > 0)
                {
                    min1 = Math.Max(min1, CeilDiv(-x, n1));
                    max1 = Math.Min(max1, FloorDiv(upper - x, n1));
                }
                else
                {
                    min1 = Math.Max(min1, CeilDiv(x - upper, -n1));
                    max1 = Math.Min(max1, FloorDiv(x, -n1));
                }
            }
        }

        var newX = X0.Length < 16 ? stackalloc int[16] : new int[X0.Length];
        newX = newX[..X0.Length];
        X0.CopyTo(newX);

        var range0 = max0 - min0;
        var range1 = max1 - min1;

        var best = int.MaxValue;
        if (range0 <= range1)
        {
            if (range0 < 0)
                return int.MaxValue;

            Debug.Assert(min0 != int.MinValue || max0 != int.MaxValue);

            for (var i = 0; i < newX.Length; i++)
                newX[i] += min0 * v0[i];

            best = Math.Min(best, MinimiseSingleDimension(newX, baseSum + min0 * delta0, delta1, v1));
            for (var i = min0 + 1; i <= max0; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v0[j];
                best = Math.Min(best, MinimiseSingleDimension(newX, baseSum + i * delta0, delta1, v1));
            }
        }
        else
        {
            if (range1 < 0)
                return int.MaxValue;

            Debug.Assert(min1 != int.MinValue || max1 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min1 * v1[i];

            best = Math.Min(best, MinimiseSingleDimension(newX, baseSum + min1 * delta1, delta0, v0));
            for (var i = min1 + 1; i <= max1; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v1[j];
                best = Math.Min(best, MinimiseSingleDimension(newX, baseSum + i * delta1, delta0, v0));
            }
        }

        return best;
    }

    private static int MinimiseThreeDimensions(ReadOnlySpan<int> X0, int baseSum, int delta0, int delta1, int delta2, int[] v0, int[] v1, int[] v2, ReadOnlySpan<int> upperBounds)
    {
        var min0 = int.MinValue / 2;
        var max0 = int.MaxValue / 2;
        var min1 = int.MinValue / 2;
        var max1 = int.MaxValue / 2;
        var min2 = int.MinValue / 2;
        var max2 = int.MaxValue / 2;

        for (var i = 0; i < X0.Length; i++)
        {
            var x = X0[i];
            var n0 = v0[i];
            var n1 = v1[i];
            var n2 = v2[i];
            var upper = upperBounds[i];

            // Can set bounds on k0
            if (n0 != 0 && n1 == 0 && n2 == 0)
            {
                // 0 <= x + k0 * n0 <= upper[i]
                if (n0 > 0)
                {
                    min0 = Math.Max(min0, CeilDiv(-x, n0));
                    max0 = Math.Min(max0, FloorDiv(upper - x, n0));
                }
                else
                {
                    min0 = Math.Max(min0, CeilDiv(x - upper, -n0));
                    max0 = Math.Min(max0, FloorDiv(x, -n0));
                }
            }
            else if (n0 == 0 && n1 != 0 && n2 == 0)
            {
                // x + k1 * n1 >= 0
                if (n1 > 0)
                {
                    min1 = Math.Max(min1, CeilDiv(-x, n1));
                    max1 = Math.Min(max1, FloorDiv(upper - x, n1));
                }
                else
                {
                    min1 = Math.Max(min1, CeilDiv(x - upper, -n1));
                    max1 = Math.Min(max1, FloorDiv(x, -n1));
                }
            }
            else if (n0 == 0 && n1 == 0 && n2 != 0)
            {
                // x + k2 * n2 >= 0
                if (n2 > 0)
                {
                    min2 = Math.Max(min2, CeilDiv(-x, n2));
                    max2 = Math.Min(max2, FloorDiv(upper - x, n2));
                }
                else
                {
                    min2 = Math.Max(min2, CeilDiv(x - upper, -n2));
                    max2 = Math.Min(max2, FloorDiv(x, -n2));
                }
            }
        }

        var newX = X0.Length < 16 ? stackalloc int[16] : new int[X0.Length];
        newX = newX[..X0.Length];
        X0.CopyTo(newX);

        var range0 = max0 - min0;
        var range1 = max1 - min1;
        var range2 = max2 - min2;

        var best = int.MaxValue;
        if (range0 <= range1 && range0 <= range2)
        {
            Debug.Assert(range0 >= 0);
            Debug.Assert(min0 != int.MinValue || max0 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min0 * v0[i];

            best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + min0 * delta0, delta1, delta2, v1, v2, upperBounds));
            for (var i = min0 + 1; i <= max0; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v0[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta0, delta1, delta2, v1, v2, upperBounds));
            }
        }
        else if (range1 <= range2)
        {
            Debug.Assert(range1 >= 0);
            Debug.Assert(min1 != int.MinValue || max1 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min1 * v1[i];

            best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + min1 * delta1, delta0, delta2, v0, v2, upperBounds));
            for (var i = min1 + 1; i <= max1; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v1[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta1, delta0, delta2, v0, v2, upperBounds));
            }
        }
        else
        {
            Debug.Assert(range2 >= 0);
            Debug.Assert(min2 != int.MinValue || max2 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min2 * v2[i];
            best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + min2 * delta2, delta0, delta1, v0, v1, upperBounds));
            for (var i = min2 + 1; i <= max2; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v2[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta2, delta0, delta1, v0, v1, upperBounds));
            }
        }

        return best;
    }

    private static void ToColumnHermiteNormalForm(int[][] matrix, List<int> joltages, Span<int> particularSolution, out int[][] nullspace)
    {
        var m = matrix.Length;
        var n = matrix[0].Length;

        // We are going to essentially perform the HNF on the transpose of the matrix. However to avoid redundantly creating a transposed version of the matrix,
        // we just reuse matrix but interact with it as if it was transposed.
        var H_T = matrix;

        // We also keep track of the transposition of the U matrix from the HNF decomposition U * A^T = H, where U is unimodular (integer inverse).
        var U_T = new int[n][];
        for (var i = 0; i < n; i++)
        {
            var u_i = U_T[i] = new int[n];
            u_i[i] = 1;
        }

        var maxRank = Math.Min(m, n);
        Span<int> pivots = new int[maxRank];
        var rank = 0;

        var lead = 0;
        for (var c = 0; c < maxRank; c++)
        {
            var i = c;
            while (lead < m && H_T[lead][i] == 0)
            {
                i++;
                if (i == n)
                {
                    i = c;
                    lead++;
                }
            }

            if (lead >= m)
            {
                for (var cc = c; cc < maxRank; cc++)
                    pivots[cc] = -1;
                break;
            }

            pivots[c] = lead;
            rank++;

            if (i != c)
            {
                // Swap cols i and c
                foreach (var row in H_T)
                    (row[c], row[i]) = (row[i], row[c]);

                foreach (var row in U_T)
                    (row[c], row[i]) = (row[i], row[c]);
            }

            var h_lead = H_T[lead];

            // Make pivot positive
            if (h_lead[c] < 0)
            {
                foreach (var row in H_T)
                    row[c] = -row[c];
                foreach (var row in U_T)
                    row[c] = -row[c];
            }

            for (var i2 = c + 1; i2 < n; i2++)
            {
                if (h_lead[i2] != 0)
                {
                    var a = h_lead[c];
                    var b = h_lead[i2];
                    var gcd = ExtendedGcd(a, b, out var x, out var y);

                    var u = a / gcd;
                    var v = b / gcd;

                    for (var j = lead; j < m; j++)
                    {
                        var h_j = H_T[j];
                        var temp1 = h_j[c];
                        var temp2 = h_j[i2];
                        h_j[c] = x * temp1 + y * temp2;
                        h_j[i2] = -v * temp1 + u * temp2;
                    }

                    for (var j = 0; j < n; j++)
                    {
                        var u_j = U_T[j];
                        var temp1 = u_j[c];
                        var temp2 = u_j[i2];
                        u_j[c] = x * temp1 + y * temp2;
                        u_j[i2] = -v * temp1 + u * temp2;
                    }

                    // Make pivot positive
                    if (h_lead[c] < 0)
                    {
                        foreach (var row in H_T)
                            row[c] = -row[c];
                        foreach (var row in U_T)
                            row[c] = -row[c];
                    }
                }
            }

            lead++;
        }

        Debug.Assert(rank <= n);
        var nullity = n - rank;
        // Compute nullspace, will be last n - m rows of U (columns of U_T)
        nullspace = new int[nullity][];
        for (var i = 0; i < nullity; i++)
        {
            var n_i = nullspace[i] = new int[n];
            for (var j = 0; j < n; j++)
                n_i[j] = U_T[j][rank + i];
        }

        // Compute particular solution, Let first m rows of H be H_top, solve for y in H_top^T * y = joltages
        // Since we have H_T, H_top is just the first m columns of H_T, so H_top^T is just the first m rows of H_T
        // This will be lower triangular and can be solved using forward subsitution
        Span<int> y_p = new int[rank];
        for (var r = 0; r < rank; r++)
        {
            var p = pivots[r];
            Debug.Assert(p != -1);

            var h_p = H_T[p];
            var sum = joltages[p];
            for (var c = 0; c < r; c++)
                sum -= h_p[c] * y_p[c];

            Debug.Assert(h_p[r] > 0, "Expected positive pivot coefficient in HNF");
            Debug.Assert(sum % h_p[r] == 0, "Expected integer solution for particular solution");
            y_p[r] = sum / h_p[r];
        }

        // Now compute x_p = U_T * y_p
        for (var i = 0; i < n; i++)
        {
            var u_i = U_T[i];
            var sum = 0;
            for (var j = 0; j < rank; j++)
                sum += u_i[j] * y_p[j];
            particularSolution[i] = sum;
        }
    }

    private static int ExtendedGcd(int a, int b, out int x, out int y)
    {
        if (b == 0)
        {
            x = 1;
            y = 0;
            return a;
        }

        var gcd = ExtendedGcd(b, a % b, out var x1, out var y1);
        x = y1;
        y = x1 - (a / b) * y1;
        return gcd;
    }

    private static int CeilDiv(int a, int b)
    {
        Debug.Assert(b >= 0);
        if (a >= 0)
            return (a + b - 1) / b;
        return a / b;
    }

    private static int FloorDiv(int a, int b)
    {
        Debug.Assert(b >= 0);
        if (a >= 0)
            return a / b;
        return (a - (b - 1)) / b;
    }

    private static uint ParseIndicatorLights(ReadOnlySpan<byte> input, ref int inputIndex)
    {
        // [.##.#] => b10110
        inputIndex++; // '['
        uint target = 0;
        var bit = 1U;
        while (true)
        {
            var c = input[inputIndex++];
            if (c == ']')
                break;

            if (c == '#')
                target |= bit;

            bit <<= 1;
        }

        return target;
    }
}
