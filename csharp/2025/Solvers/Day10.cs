using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        var buttonMasks = new List<uint>();
        var joltages = new List<short>();

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

                        joltages.Add((short)value);
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

    public static int SolvePart2(ReadOnlySpan<uint> buttonMasks, ReadOnlySpan<short> joltages)
    {
        var m = joltages.Length;     // equations (rows)
        var n = buttonMasks.Length;  // variables (cols)

        Debug.Assert(m <= 16 && n <= 16, "Exceeded maximum supported problem size");

        Span<short> matrix = stackalloc short[16 * 16];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                matrix[16 * j + i] = (short)((buttonMasks[i] >> j) & 1);
            }
        }

        const int maxNullity = 3;
        Span<short> particularSolution = stackalloc short[16];
        Span<short> nullSpaceBasis = stackalloc short[maxNullity * 16];

        var nullity = GetIntegerSolutionAndBasis(matrix, joltages, particularSolution, nullSpaceBasis, m, n);

        var xSum = Vector256.Sum(Vector256.Create(particularSolution));

        // If d == 0, there is only one solution
        if (nullity == 0)
            return xSum;

        var v0 = nullSpaceBasis;
        var v0Sum = SumAndNormalizeVector(v0);

        if (nullity == 1)
            return xSum + MinimiseSingleDimension(particularSolution, v0);

        var v1 = nullSpaceBasis.Slice(16);
        var v1Sum = SumAndNormalizeVector(v1);
        if (v0Sum < v1Sum)
        {
            SwapSpans(ref v0, ref v1);
            (v0Sum, v1Sum) = (v1Sum, v0Sum);
        }

        if (nullity == 2)
            return xSum + MinimiseTwoDimensions(particularSolution, v0, v0Sum, v1, v1Sum, n);

        Debug.Assert(nullity == 3);

        var v2 = nullSpaceBasis.Slice(32);
        var v2Sum = SumAndNormalizeVector(v2);
        if (v0Sum < v2Sum)
        {
            SwapSpans(ref v0, ref v2);
            (v0Sum, v2Sum) = (v2Sum, v0Sum);
        }

        return xSum + MinimiseThreeDimensions(particularSolution, v0, v0Sum, v1, v1Sum, v2, v2Sum, n);

        static int SumAndNormalizeVector(Span<short> v0)
        {
            var v0Vec = Vector256.Create(v0);
            int v0Sum = Vector256.Sum(v0Vec);
            if (v0Sum < 0)
            {
                v0Vec = Vector256.Negate(v0Vec);
                v0Vec.CopyTo(v0);
                v0Sum = -v0Sum;
            }

            return v0Sum;
        }

        static void SwapSpans(ref Span<short> a, ref Span<short> b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }

    private static int MinimiseSingleDimension(ReadOnlySpan<short> x, Span<short> v)
    {
        var vVec = Vector256.Create(v);
        int vSum = Vector256.Sum(vVec);

        // Make sum negative so that we just need to find a max
        if (vSum > 0)
        {
            vVec = Vector256.Negate(vVec);
            vVec.CopyTo(v);
            vSum = -vSum;
        }

        var negativeBits = vVec.ExtractMostSignificantBits();
        var i = BitOperations.TrailingZeroCount(negativeBits);
        int numerator = x[i];
        var denominator = -v[i];
        negativeBits &= negativeBits - 1;

        while (negativeBits != 0)
        {
            i = BitOperations.TrailingZeroCount(negativeBits);
            var vi = -v[i];
            int xi = x[i];
            if (xi * denominator < numerator * vi)
            {
                numerator = xi;
                denominator = vi;
            }

            // Clear lowest bit
            negativeBits &= negativeBits - 1;
        }

        return FloorDiv(numerator, denominator) * vSum;
    }

    private static int MinimiseTwoDimensions(ReadOnlySpan<short> x, ReadOnlySpan<short> v0, int v0Sum, ReadOnlySpan<short> v1, int v1Sum, int n, int best = short.MaxValue)
    {
        // Find the minimum k0
        var minK0 = GetFourierMotzkinBounds2D(x, v0, v1, n);
        var minK1 = GetFourierMotzkinBounds2D(x, v1, v0, n);

        // Find the minimum k1 for minK0 (the upper bound)
        int minK1ForMinK0 = short.MinValue;
        int minK0ForMinK1 = short.MinValue;
        for (var i = 0; i < n; i++)
        {
            var v0i = v0[i];
            var v1i = v1[i];
            if (v0i > 0)
            {
                var rem = x[i] + minK1 * v1i;
                if (minK0ForMinK1 * v0i < -rem)
                    minK0ForMinK1 = CeilDiv(-rem, v0i);
            }

            if (v1i > 0)
            {
                var rem = x[i] + minK0 * v0i;
                if (minK1ForMinK0 * v1i < -rem)
                    minK1ForMinK0 = CeilDiv(-rem, v1i);
            }
        }

        var sumAtMinK0 = minK0 * v0Sum + minK1ForMinK0 * v1Sum;
        var sumAtMinK1 = minK0ForMinK1 * v0Sum + minK1 * v1Sum;

        // If it is better to start at max k0, swap v0 and v1
        if (sumAtMinK1 < sumAtMinK0)
        {
            if (sumAtMinK1 >= best)
                return best;

            var temp = v0;
            v0 = v1;
            v1 = temp;
            (v0Sum, v1Sum) = (v1Sum, v0Sum);
            (minK0, minK1) = (minK1, minK0);
            minK1ForMinK0 = minK0ForMinK1;
        }
        else
        {
            if (sumAtMinK0 >= best)
                return best;
        }

        var v0Vec = Vector256.Create(v0);
        var v1Vec = Vector256.Create(v1);

        var mustAddV0IfNegativeMask = Vector256.GreaterThan(v0Vec, Vector256<short>.Zero) & ~Vector256.LessThan(v1Vec, Vector256<short>.Zero);

        // A mask of any components that once they are negative, can't be made positive again
        var unrecoverableMask = ~Vector256.GreaterThan(v0Vec, Vector256<short>.Zero) & ~Vector256.LessThan(v1Vec, Vector256<short>.Zero);

        var xVec = Vector256.Create(x) + Vector256.Multiply(v0Vec, (short)minK0) + Vector256.Multiply(v1Vec, (short)minK1ForMinK0);

        var curSum = minK0 * v0Sum + minK1ForMinK0 * v1Sum;
        var k0 = minK0;
        if (Vector256.LessThan(xVec, Vector256<short>.Zero) == Vector256<short>.Zero)
        {
            if (curSum < best)
                best = curSum;

            k0++;
            xVec += v0Vec - v1Vec;
            curSum += v0Sum - v1Sum;
        }

        while (true)
        {
            var bestPossible = k0 * v0Sum + minK1 * v1Sum;
            if (bestPossible >= best)
                break;

            // Find the next feasible solution
            var negativeMask = Vector256.LessThan(xVec, Vector256<short>.Zero);
            if (negativeMask != Vector256<short>.Zero)
            {
                if ((negativeMask & unrecoverableMask) != Vector256<short>.Zero)
                    return best;

                if ((negativeMask & mustAddV0IfNegativeMask) != Vector256<short>.Zero)
                {
                    k0++;
                    xVec += v0Vec;
                    curSum += v0Sum;
                }
                else
                {
                    xVec -= v1Vec;
                    curSum -= v1Sum;
                }

                continue;
            }

            // Subtract v1 until it is minimal
            while (true)
            {
                var resultAfterSubtracting = xVec - v1Vec;
                if (Vector256.LessThan(resultAfterSubtracting, Vector256<short>.Zero) != Vector256<short>.Zero)
                    break;

                xVec = resultAfterSubtracting;
                curSum -= v1Sum;
            }

            if (curSum < best)
                best = curSum;

            k0++;
            xVec += v0Vec - v1Vec;
            curSum += v0Sum - v1Sum;
        }

        return best;
    }

    [SkipLocalsInit]
    private static int MinimiseThreeDimensions(ReadOnlySpan<short> x, ReadOnlySpan<short> v0, int v0Sum, ReadOnlySpan<short> v1, int v1Sum, ReadOnlySpan<short> v2, int v2Sum, int n)
    {
        var (minK0, maxK0) = GetFourierMotzkinBounds3D(x, v0, v1, v2, n);

        var v0Vec = Vector256.Create(v0);
        var xVec = Vector256.Create(x) + v0Vec * Vector256.Create((short)minK0);

        Span<short> newXSpan = stackalloc short[16];
        xVec.CopyTo(newXSpan);

        var curV0Sum = v0Sum * minK0;
        var best = MinimiseTwoDimensions(newXSpan, v1, v1Sum, v2, v2Sum, n) + v0Sum * minK0;
        for (var k = minK0 + 1; k <= maxK0; k++)
        {
            xVec += v0Vec;
            curV0Sum += v0Sum;
            xVec.CopyTo(newXSpan);

            var innerBest = best - curV0Sum;
            var newMin = MinimiseTwoDimensions(newXSpan, v1, v1Sum, v2, v2Sum, n, innerBest);
            if (newMin < innerBest)
                best = newMin + curV0Sum;
        }

        return best;
    }

    // Finds an integer solution to Ax = rhs, a basis for null space of A, and returns the nullity
    // This is achieved by computing the Hermite Normal Form of A^T https://en.wikipedia.org/wiki/Hermite_normal_form
    private static int GetIntegerSolutionAndBasis(Span<short> A, ReadOnlySpan<short> rhs, Span<short> particularSolution, Span<short> nullspace, int m, int n)
    {
        // 1. Initialize H and U matrices that correspond with H = U * A^T
        Span<short> H = stackalloc short[16 * 16];
        Span<short> U = stackalloc short[16 * 16];

        for (var i = 0; i < n; i++)
        {
            U[i * 16 + i] = 1;
            for (var j = 0; j < m; j++)
                H[i * 16 + j] = A[j * 16 + i];
        }

        // 2. Compute Hermite Normal Form
        var rank = 0;
        Span<int> pivotCols = stackalloc int[16];
        for (var col = 0; col < m && rank < n; col++)
        {
            var pivotRow = rank;
            while (pivotRow < n && H[pivotRow * 16 + col] == 0)
                pivotRow++;

            if (pivotRow == n)
                continue;

            pivotCols[rank] = col;

            var h_rank = H.Slice(rank * 16, 16);
            var u_rank = U.Slice(rank * 16, 16);
            if (rank != pivotRow)
            {
                SwapRows(h_rank, H.Slice(pivotRow * 16, 16));
                SwapRows(u_rank, U.Slice(pivotRow * 16, 16));
            }

            for (var i = rank + 1; i < n; i++)
            {
                var h_i = H.Slice(i * 16, 16);
                if (h_i[col] == 0)
                    continue;

                var a = h_rank[col];
                var b = h_i[col];
                var gcd = ExtendedGcd(a, b, out var x, out var y);
                var u = (short)(a / gcd);
                var v = (short)-(b / gcd);

                CombineRows(h_rank, h_i, x, y, v, u);
                CombineRows(u_rank, U.Slice(i * 16, 16), x, y, v, u);
            }

            rank++;
        }

        // 3. Null Space Extraction
        var nullity = n - rank;
        for (var i = 0; i < nullity; i++)
        {
            var uSrc = U.Slice((rank + i) * 16, 16);
            var nullSpaceDst = nullspace.Slice(i * 16, 16);
            Vector256.Create(uSrc).CopyTo(nullSpaceDst);
        }

        // 4. Particular Solution
        Span<short> y_p = stackalloc short[16];
        for (var r = 0; r < rank; r++)
        {
            var colIdx = pivotCols[r];
            var sum = (int)rhs[colIdx];

            for (var k = 0; k < r; k++)
                sum -= H[k * 16 + colIdx] * y_p[k];

            var y_r = (short)(sum / H[r * 16 + colIdx]);
            y_p[r] = y_r;
            AddScaledRow(particularSolution, U.Slice(r * 16, 16), y_r);
        }

        return nullity;
    }

    private static void SwapRows(Span<short> row1, Span<short> row2)
    {
        var v1 = Vector256.Create(row1);
        var v2 = Vector256.Create(row2);
        v2.CopyTo(row1);
        v1.CopyTo(row2);
    }

    private static void CombineRows(Span<short> row1, Span<short> row2, short a, short b, short c, short d)
    {
        var v1 = Vector256.Create(row1);
        var v2 = Vector256.Create(row2);
        var res1 = Vector256.Add(Vector256.Multiply(v1, a), Vector256.Multiply(v2, b));
        var res2 = Vector256.Add(Vector256.Multiply(v1, c), Vector256.Multiply(v2, d));
        res1.CopyTo(row1);
        res2.CopyTo(row2);
    }

    private static void AddScaledRow(Span<short> target, ReadOnlySpan<short> source, short scale)
    {
        var vTarget = Vector256.Create(target);
        var vSource = Vector256.Create(source);
        var res = Vector256.Add(vTarget, Vector256.Multiply(vSource, scale));
        res.CopyTo(target);
    }

    private static void ApplyBound(ref int min, ref int max, int numerator, int denominator)
    {
        if (denominator > 0)
        {
            if (min * denominator < numerator)
                min = CeilDiv(numerator, denominator);
        }
        else if (denominator < 0)
        {
            if (max * denominator < numerator)
                max = FloorDiv(-numerator, -denominator);
        }
    }

    // Given constraints of the form x + k0*v0 + k1*v1 >= 0, finds lower bounds on k0
    // This is done using Fourier-Motzkin elimination https://en.wikipedia.org/wiki/Fourier%E2%80%93Motzkin_elimination
    [SkipLocalsInit]
    private static int GetFourierMotzkinBounds2D(ReadOnlySpan<short> x, ReadOnlySpan<short> v0, ReadOnlySpan<short> v1, int n)
    {
        Span<(int x, int v0, int v1)> positiveConstraints = stackalloc (int x, int v0, int v1)[16];
        var posLen = 0;

        Span<(int x, int v0, int v1)> negativeConstraints = stackalloc (int x, int v0, int v1)[16];
        var negLen = 0;

        int minK0 = short.MinValue;
        for (var i = 0; i < n; i++)
        {
            var v1i = v1[i];
            if (v1i == 0)
            {
                var v0i = v0[i];
                if (v0i > 0)
                {
                    var xi = -x[i];
                    if (minK0 * v0i < xi)
                        minK0 = CeilDiv(xi, v0i);
                }
            }
            else if (v1i > 0)
            {
                positiveConstraints[posLen++] = (x[i], v0[i], v1i);
            }
            else
            {
                negativeConstraints[negLen++] = (x[i], v0[i], v1i);
            }
        }

        foreach (var (xi, v0i, v1i) in positiveConstraints[..posLen])
        {
            foreach (var (xj, v0j, v1j) in negativeConstraints[..negLen])
            {
                var A = v0j * v1i - v0i * v1j;
                if (A > 0)
                {
                    var B = xi * v1j - xj * v1i;
                    if (minK0 * A < B)
                        minK0 = CeilDiv(B, A);
                }
            }
        }

        return minK0;
    }

    // Given constraints of the form x + k0*v0 + k1*v1 + k2*v2 >= 0, finds lower and upper bounds on k0
    [SkipLocalsInit]
    private static (int, int) GetFourierMotzkinBounds3D(ReadOnlySpan<short> x, ReadOnlySpan<short> v0, ReadOnlySpan<short> v1, ReadOnlySpan<short> v2, int n)
    {
        int minK0 = short.MinValue;
        int maxK0 = short.MaxValue;

        Span<(int x, int v0, int v1, int v2)> positiveK2Constraints = stackalloc (int x, int v0, int v1, int v2)[16];
        var posK2Len = 0;

        Span<(int x, int v0, int v1, int v2)> negativeK2Constraints = stackalloc (int x, int v0, int v1, int v2)[16];
        var negK2Len = 0;

        Span<(int x, int v0, int v1)> positiveK1Constraints = stackalloc (int x, int v0, int v1)[128];
        var posK1Len = 0;

        Span<(int x, int v0, int v1)> negativeK1Constraints = stackalloc (int x, int v0, int v1)[128];
        var negK1Len = 0;

        for (var i = 0; i < n; i++)
        {
            var v2i = v2[i];

            if (v2i == 0)
            {
                var v1i = v1[i];
                if (v1i == 0)
                {
                    ApplyBound(ref minK0, ref maxK0, -x[i], v0[i]);
                }
                else if (v1i < 0)
                {
                    negativeK1Constraints[negK1Len++] = (x[i], v0[i], v1i);
                }
                else
                {
                    positiveK1Constraints[posK1Len++] = (x[i], v0[i], v1i);
                }
            }
            else if (v2i > 0)
            {
                positiveK2Constraints[posK2Len++] = (x[i], v0[i], v1[i], v2i);
            }
            else
            {
                negativeK2Constraints[negK2Len++] = (x[i], v0[i], v1[i], v2i);
            }
        }

        foreach (var (xi, v0i, v1i, v2i) in positiveK2Constraints[..posK2Len])
        {
            foreach (var (xj, v0j, v1j, v2j) in negativeK2Constraints[..negK2Len])
            {
                var newX = -v2j * xi + v2i * xj;
                var newV0 = -v2j * v0i + v2i * v0j;
                var newV1 = -v2j * v1i + v2i * v1j;

                if (newV1 == 0)
                {
                    ApplyBound(ref minK0, ref maxK0, -newX, newV0);
                }
                else if (newV1 < 0)
                {
                    negativeK1Constraints[negK1Len++] = (newX, newV0, newV1);
                }
                else
                {
                    positiveK1Constraints[posK1Len++] = (newX, newV0, newV1);
                }
            }
        }

        foreach (var (xi, v0i, v1i) in positiveK1Constraints[..posK1Len])
        {
            foreach (var (xj, v0j, v1j) in negativeK1Constraints[..negK1Len])
            {
                var A = v0j * v1i - v0i * v1j;
                var B = xi * v1j - xj * v1i;
                ApplyBound(ref minK0, ref maxK0, B, A);
            }
        }

        return (minK0, maxK0);
    }

    private static short ExtendedGcd(short a, short b, out short x, out short y)
    {
        x = 1;
        y = 0;
        var x1 = 0;
        var y1 = 1;
        var a1 = (int)a;
        var b1 = (int)b;
        while (b1 != 0)
        {
            var q = a1 / b1;
            (x, x1) = ((short)x1, x - q * x1);
            (y, y1) = ((short)y1, y - q * y1);
            (a1, b1) = (b1, a1 - q * b1);
        }
        return (short)a1;
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
