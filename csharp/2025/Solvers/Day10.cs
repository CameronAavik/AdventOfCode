using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
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

        var buttonMasks = new List<uint>();
        var joltages = new List<int>();

        var inputIndex = 0;
        while (inputIndex < input.Length)
        {
            buttonMasks.Clear();
            joltages.Clear();

            var indicators = ParseIndicatorLights(input, ref inputIndex);
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

            var buttonMasksSpan = CollectionsMarshal.AsSpan(buttonMasks);
            var joltagesSpan = CollectionsMarshal.AsSpan(joltages);

            part1 += SolvePart1(indicators, buttonMasksSpan);
            part2 += SolvePart2(buttonMasksSpan, joltagesSpan);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    public static int SolvePart1(uint indicators, ReadOnlySpan<uint> buttonMasks)
    {
        if (indicators == 0)
            return 0;

        if (buttonMasks.Contains(indicators))
            return 1;

        ref var start = ref MemoryMarshal.GetReference(buttonMasks);
        ref var end = ref Unsafe.Add(ref start, buttonMasks.Length);

        // Get smallest subset of buttonMask that xors together to indicators
        for (var count = 2; count <= buttonMasks.Length; count++)
        {
            if (CanMakeTargetWithCount(indicators, count, ref start, ref Unsafe.Subtract(ref end, count)))
                return count;
        }

        return -1;
    }

    public static bool CanMakeTargetWithCount(uint target, int count, ref uint start, ref uint end)
    {
        if (count == 1)
        {
            for (ref var ptr = ref start; Unsafe.IsAddressLessThanOrEqualTo(ref ptr, ref end); ptr = ref Unsafe.Add(ref ptr, 1))
            {
                if (ptr == target)
                    return true;
            }

            return false;
        }

        if (Unsafe.AreSame(ref start, ref end))
        {
            ref var ptr = ref start;
            target ^= ptr;
            for (var i = 1; i < count; i++)
            {
                ptr = ref Unsafe.Add(ref ptr, 1);
                target ^= ptr;
            }

            return target == 0;
        }

        ref var nextStart = ref Unsafe.Add(ref start, 1);

        return CanMakeTargetWithCount(target ^ start, count - 1, ref nextStart, ref Unsafe.Add(ref end, 1)) ||
               CanMakeTargetWithCount(target, count, ref nextStart, ref end);
    }

    public static int SolvePart2(ReadOnlySpan<uint> buttonMasks, ReadOnlySpan<int> joltages)
    {
        var m = joltages.Length;     // equations (rows)
        var n = buttonMasks.Length;  // variables (cols)

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

        var particularSolution = new int[n];
        var nullSpaceBasis = new int[3][]; // max nullity of 3
        for (var i = 0; i < 3; i++)
            nullSpaceBasis[i] = new int[n];

        var nullity = GetIntegerSolutionAndBasis(matrix, joltages, particularSolution, nullSpaceBasis);

        var baseSum = 0;
        foreach (var x in particularSolution)
            baseSum += x;

        // If d == 0, there is only one solution
        if (nullity == 0)
            return baseSum;

        var delta = new int[nullity];
        for (var k = 0; k < nullity; k++)
        {
            var v = nullSpaceBasis[k];
            var s = 0;
            for (var i = 0; i < n; i++)
                s += v[i];

            // Negate vectors with negative delta so later we can assume all deltas are non-negative later
            if (s < 0)
            {
                s = -s;
                for (var i = 0; i < n; i++)
                    v[i] = -v[i];
            }

            delta[k] = s;
        }

        // For each button, there is a maximum number of times it can be pressed before it exceeds the joltage requirement.
        var buttonUpperBounds = new int[buttonMasks.Length];
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

        if (nullity == 1)
        {
            if (delta[0] == 0)
                return baseSum;
            return MinimiseSingleDimension(particularSolution, baseSum, delta[0], nullSpaceBasis[0]);
        }

        if (nullity == 2)
        {
            if (delta[0] == 0 && delta[1] == 0)
                return baseSum;
            return MinimiseTwoDimensions(particularSolution, baseSum, delta[0], delta[1], nullSpaceBasis[0], nullSpaceBasis[1], buttonUpperBounds);
        }

        Debug.Assert(nullity == 3);
        return MinimiseThreeDimensions(particularSolution, baseSum, delta[0], delta[1], delta[2], nullSpaceBasis[0], nullSpaceBasis[1], nullSpaceBasis[2], buttonUpperBounds);
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

        return baseSum + min0 * delta;
    }

    [SkipLocalsInit]
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

        if (range0 <= range1)
        {
            if (range0 < 0)
                return int.MaxValue;

            Debug.Assert(min0 != int.MinValue || max0 != int.MaxValue);

            for (var i = 0; i < newX.Length; i++)
                newX[i] += min0 * v0[i];

            var best = MinimiseSingleDimension(newX, baseSum + min0 * delta0, delta1, v1);
            for (var i = min0 + 1; i <= max0; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v0[j];
                var subSum = MinimiseSingleDimension(newX, baseSum + i * delta0, delta1, v1);
                if (subSum > best)
                    break;
                best = subSum;
            }

            return best;
        }
        else
        {
            if (range1 < 0)
                return int.MaxValue;

            Debug.Assert(min1 != int.MinValue || max1 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min1 * v1[i];

            var best = MinimiseSingleDimension(newX, baseSum + min1 * delta1, delta0, v0);
            for (var i = min1 + 1; i <= max1; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v1[j];

                var subSum = MinimiseSingleDimension(newX, baseSum + i * delta1, delta0, v0);
                if (subSum > best)
                    break;
                best = subSum;
            }

            return best;
        }
    }

    [SkipLocalsInit]
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

        if (range0 <= range1 && range0 <= range2)
        {
            Debug.Assert(range0 >= 0);
            Debug.Assert(min0 != int.MinValue || max0 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min0 * v0[i];

            var best = MinimiseTwoDimensions(newX, baseSum + min0 * delta0, delta1, delta2, v1, v2, upperBounds);
            for (var i = min0 + 1; i <= max0; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v0[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta0, delta1, delta2, v1, v2, upperBounds));
            }

            return best;
        }
        else if (range1 <= range2)
        {
            Debug.Assert(range1 >= 0);
            Debug.Assert(min1 != int.MinValue || max1 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min1 * v1[i];

            var best = MinimiseTwoDimensions(newX, baseSum + min1 * delta1, delta0, delta2, v0, v2, upperBounds);
            for (var i = min1 + 1; i <= max1; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v1[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta1, delta0, delta2, v0, v2, upperBounds));
            }

            return best;
        }
        else
        {
            Debug.Assert(range2 >= 0);
            Debug.Assert(min2 != int.MinValue || max2 != int.MaxValue);
            for (var i = 0; i < newX.Length; i++)
                newX[i] += min2 * v2[i];

            var best = MinimiseTwoDimensions(newX, baseSum + min2 * delta2, delta0, delta1, v0, v1, upperBounds);
            for (var i = min2 + 1; i <= max2; i++)
            {
                for (var j = 0; j < newX.Length; j++)
                    newX[j] += v2[j];
                best = Math.Min(best, MinimiseTwoDimensions(newX, baseSum + i * delta2, delta0, delta1, v0, v1, upperBounds));
            }

            return best;
        }
    }

    // Finds an integer solution to Ax = rhs, a basis for null space of A, and returns the nullity
    [SkipLocalsInit]
    private static int GetIntegerSolutionAndBasis(int[][] A, ReadOnlySpan<int> rhs, Span<int> particularSolution, int[][] nullspace)
    {
        var m = A.Length;
        var n = A[0].Length;

        // 1. Store transposition of A in H and initialize U as identity matrix
        var H = new int[n][];
        var U = new int[n][];

        for (var i = 0; i < n; i++)
        {
            var hRow = H[i] = new int[m];
            var uRow = U[i] = new int[n];
            uRow[i] = 1;
            for (var j = 0; j < m; j++)
                hRow[j] = A[j][i];
        }

        // 2. Compute Hermite Normal Form
        var rank = 0;
        Span<int> pivotCols = stackalloc int[16];
        for (var col = 0; col < m && rank < n; col++)
        {
            var pivotRow = rank;
            while (pivotRow < n && H[pivotRow][col] == 0)
                pivotRow++;

            if (pivotRow == n)
                continue;

            pivotCols[rank] = col;

            if (rank != pivotRow)
            {
                (H[rank], H[pivotRow]) = (H[pivotRow], H[rank]);
                (U[rank], U[pivotRow]) = (U[pivotRow], U[rank]);
            }

            var h_rank = H[rank];
            var u_rank = U[rank];
            for (var i = rank + 1; i < n; i++)
            {
                var h_i = H[i];
                if (h_i[col] == 0)
                    continue;

                var a = h_rank[col];
                var b = h_i[col];
                var gcd = ExtendedGcd(a, b, out var x, out var y);
                var u = a / gcd;
                var v = b / gcd;

                CombineRows(h_rank, h_i, x, y, -v, u);
                CombineRows(u_rank, U[i], x, y, -v, u);
            }

            rank++;
        }

        // 3. Null Space Extraction
        var nullity = n - rank;
        for (var i = 0; i < nullity; i++)
            nullspace[i] = U[rank + i];

        // 4. Particular Solution
        Span<int> y_p = stackalloc int[16];
        for (var r = 0; r < rank; r++)
        {
            var colIdx = pivotCols[r];
            var sum = rhs[colIdx];

            for (var k = 0; k < r; k++)
                sum -= H[k][colIdx] * y_p[k];

            var y_r = sum / H[r][colIdx];
            y_p[r] = y_r;
            AddScaledRow(particularSolution, U[r], y_r);
        }

        return nullity;
    }

    private static void CombineRows(Span<int> row1, Span<int> row2, int a, int b, int c, int d)
    {
        var i = 0;
        if (row1.Length >= Vector256<int>.Count)
        {
            var v1 = Vector256.Create(row1);
            var v2 = Vector256.Create(row2);

            var res1 = Vector256.Add(Vector256.Multiply(v1, a), Vector256.Multiply(v2, b));
            res1.CopyTo(row1);

            var res2 = Vector256.Add(Vector256.Multiply(v1, c), Vector256.Multiply(v2, d));
            res2.CopyTo(row2);

            i = Vector256<int>.Count;
        }

        for (; i < row1.Length; i++)
        {
            var v1 = row1[i];
            var v2 = row2[i];
            row1[i] = a * v1 + b * v2;
            row2[i] = c * v1 + d * v2;
        }
    }

    private static void AddScaledRow(Span<int> target, ReadOnlySpan<int> source, int scale)
    {
        var i = 0;
        if (target.Length >= Vector256<int>.Count)
        {
            var vTarget = Vector256.Create(target);
            var vSource = Vector256.Create(source);

            var res = Vector256.Add(vTarget, Vector256.Multiply(vSource, scale));
            res.CopyTo(target);

            i = Vector256<int>.Count;
        }

        for (; i < target.Length; i++)
            target[i] += source[i] * scale;
    }

    private static int ExtendedGcd(int a, int b, out int x, out int y)
    {
        x = 1;
        y = 0;
        var x1 = 0;
        var y1 = 1;
        var a1 = a;
        var b1 = b;
        while (b1 != 0)
        {
            var q = a1 / b1;
            (x, x1) = (x1, x - q * x1);
            (y, y1) = (y1, y - q * y1);
            (a1, b1) = (b1, a1 - q * b1);
        }
        return a1;
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
