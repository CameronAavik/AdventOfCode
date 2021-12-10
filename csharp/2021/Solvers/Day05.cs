using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day05 : ISolver
{
    public interface IBinarySearchable
    {
        public int BinarySearchAxis { get; }
    }

    public readonly record struct HorizontalLine(int Y, int X1, int X2) : IBinarySearchable, IComparable<HorizontalLine>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HorizontalLine Create(int x1, int y1, int x2, int y2) => x1 < x2 ? new(y1, x1, x2) : new(y1, x2, x1);

        public int BinarySearchAxis => Y;

        public int CompareTo(HorizontalLine other)
        {
            int c = Y.CompareTo(other.Y);
            if (c != 0) return c;

            c = X1.CompareTo(other.X1);
            if (c != 0) return c;

            return X2.CompareTo(other.X2);
        }
    }

    public readonly record struct VerticalLine(int X, int Y1, int Y2) : IBinarySearchable, IComparable<VerticalLine>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VerticalLine Create(int x1, int y1, int x2, int y2) => y1 < y2 ? new(x1, y1, y2) : new(x1, y2, y1);

        public int BinarySearchAxis => X;

        public int CompareTo(VerticalLine other)
        {
            int c = X.CompareTo(other.X);
            if (c != 0) return c;

            c = Y1.CompareTo(other.Y1);
            if (c != 0) return c;

            return Y2.CompareTo(other.Y2);
        }
    }

    public readonly record struct PositiveDiagonalLine(int YIntersect, int X1, int X2) : IBinarySearchable, IComparable<PositiveDiagonalLine>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PositiveDiagonalLine Create(int x1, int y1, int x2, int y2) => x1 < x2 ? new(y1 - x1, x1, x2) : new(y1 - x1, x2, x1);
        public int BinarySearchAxis => YIntersect;
        public int Y1 => X1 + YIntersect;
        public int Y2 => X2 + YIntersect;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetYAtX(int x) => x + YIntersect;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetXAtY(int y) => y - YIntersect;

        public int CompareTo(PositiveDiagonalLine other)
        {
            int c = YIntersect.CompareTo(other.YIntersect);
            if (c != 0) return c;

            c = X1.CompareTo(other.X1);
            if (c != 0) return c;

            return X2.CompareTo(other.X2);
        }
    }

    public readonly record struct NegativeDiagonalLine(int YIntersect, int X1, int X2) : IBinarySearchable, IComparable<NegativeDiagonalLine>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NegativeDiagonalLine Create(int x1, int y1, int x2, int y2) => x1 < x2 ? new(y1 + x1, x1, x2) : new(y1 + x1, x2, x1);
        public int BinarySearchAxis => YIntersect;
        public int Y1 => YIntersect - X1;
        public int Y2 => YIntersect - X2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetYAtX(int x) => YIntersect - x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetXAtY(int y) => YIntersect - y;
        public int CompareTo(NegativeDiagonalLine other)
        {
            int c = YIntersect.CompareTo(other.YIntersect);
            if (c != 0) return c;

            c = X1.CompareTo(other.X1);
            if (c != 0) return c;

            return X2.CompareTo(other.X2);
        }
    }

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        var horizontals = new List<HorizontalLine>();
        var verticals = new List<VerticalLine>();
        var posDiags = new List<PositiveDiagonalLine>();
        var negDiags = new List<NegativeDiagonalLine>();

        int inputCursor = 0;
        while (inputCursor < input.Length)
        {
            int x1 = ReadIntegerUntil(input, ',', ref inputCursor);
            int y1 = ReadIntegerUntil(input, ' ', ref inputCursor);
            inputCursor += "-> ".Length;
            int x2 = ReadIntegerUntil(input, ',', ref inputCursor);
            int y2 = ReadIntegerUntil(input, '\n', ref inputCursor);

            int xDiff = x2 - x1;
            int yDiff = y2 - y1;

            if (y1 == y2)
            {
                horizontals.Add(HorizontalLine.Create(x1, y1, x2, y2));
            }
            else if (x1 == x2)
            {
                verticals.Add(VerticalLine.Create(x1, y1, x2, y2));
            }
            else if (yDiff == xDiff)
            {
                // y = x + yIntersect -> yIntersect = y - x
                posDiags.Add(PositiveDiagonalLine.Create(x1, y1, x2, y2));
            }
            else if (yDiff == -xDiff)
            {
                // y = -x + yIntersect -> yIntersect = y + x
                negDiags.Add(NegativeDiagonalLine.Create(x1, y1, x2, y2));
            }
        }

        horizontals.Sort();
        verticals.Sort();
        posDiags.Sort();
        negDiags.Sort();

        var overlaps = new HashSet<int>();
        AddOverlapsFromHorizontalsAndVerticals(horizontals, verticals, overlaps);
        solution.SubmitPart1(overlaps.Count);

        AddOverlapsFromDiagonal(horizontals, verticals, posDiags, negDiags, overlaps);
        solution.SubmitPart2(overlaps.Count);
    }

    private static void AddOverlapsFromHorizontalsAndVerticals(List<HorizontalLine> horizontals, List<VerticalLine> verticals, HashSet<int> overlaps)
    {
        for (int i = 0; i < horizontals.Count; i++)
        {
            HorizontalLine horizontal = horizontals[i];

            // Check for overlaps with other horizontals
            for (int j = i + 1; j < horizontals.Count; j++)
            {
                HorizontalLine otherHorizontal = horizontals[j];
                if (horizontal.Y != otherHorizontal.Y || horizontal.X2 < otherHorizontal.X1)
                    break;

                for (int x = otherHorizontal.X1; x <= Math.Min(otherHorizontal.X2, horizontal.X2); x++)
                    overlaps.Add(x << 16 | horizontal.Y);
            }

            // Check for overlaps with verticals
            int startVerticalIndex = BinarySearchForAxis(verticals, horizontal.X1);
            for (int j = startVerticalIndex; j < verticals.Count; j++)
            {
                VerticalLine vertical = verticals[j];
                if (vertical.X > horizontal.X2)
                    break;

                if (vertical.Y1 <= horizontal.Y && horizontal.Y <= vertical.Y2)
                    overlaps.Add(vertical.X << 16 | horizontal.Y);
            }
        }

        for (int i = 0; i < verticals.Count; i++)
        {
            VerticalLine vertical = verticals[i];

            // Check if it overlaps with any other verticals
            for (int j = i + 1; j < verticals.Count; j++)
            {
                VerticalLine otherVertical = verticals[j];
                if (vertical.X != otherVertical.X || vertical.Y2 < otherVertical.Y1)
                    break;

                for (int y = otherVertical.Y1; y <= Math.Min(otherVertical.Y2, vertical.Y2); y++)
                    overlaps.Add(vertical.X << 16 | y);
            }
        }
    }

    private static void AddOverlapsFromDiagonal(
        List<HorizontalLine> horizontals,
        List<VerticalLine> verticals,
        List<PositiveDiagonalLine> posDiags,
        List<NegativeDiagonalLine> negDiags,
        HashSet<int> overlaps)
    {
        for (int i = 0; i < posDiags.Count; i++)
        {
            PositiveDiagonalLine diag = posDiags[i];

            // Check for overlaps with other positive diagonals
            for (int j = i + 1; j < posDiags.Count; j++)
            {
                PositiveDiagonalLine otherDiag = posDiags[j];
                if (diag.YIntersect != otherDiag.YIntersect || diag.X2 < otherDiag.X1)
                    break;

                for (int x = otherDiag.X1; x <= Math.Min(otherDiag.X2, diag.X2); x++)
                    overlaps.Add(x << 16 | diag.GetYAtX(x));
            }

            // Check for overlaps with horizontals
            int startHorizontalIndex = BinarySearchForAxis(horizontals, diag.Y1);
            for (int j = startHorizontalIndex; j < horizontals.Count; j++)
            {
                HorizontalLine horizontal = horizontals[j];
                if (horizontal.Y > diag.Y2)
                    break;

                int xIntersect = diag.GetXAtY(horizontal.Y);
                if (horizontal.X1 <= xIntersect && xIntersect <= horizontal.X2)
                    overlaps.Add(xIntersect << 16 | horizontal.Y);
            }

            // Check for overlaps with verticals
            int startVerticalIndex = BinarySearchForAxis(verticals, diag.X1);
            for (int j = startVerticalIndex; j < verticals.Count; j++)
            {
                VerticalLine vertical = verticals[j];
                if (vertical.X > diag.X2)
                    break;

                int yIntersect = diag.GetYAtX(vertical.X);
                if (vertical.Y1 <= yIntersect && yIntersect <= vertical.Y2)
                    overlaps.Add(vertical.X << 16 | yIntersect);
            }

            // Check for overlaps with negative diagonals
            int startNegativeDiagIndex = BinarySearchForAxis(negDiags, diag.X1 + diag.Y1);
            for (int j = startNegativeDiagIndex; j < negDiags.Count; j++)
            {
                NegativeDiagonalLine negDiag = negDiags[j];
                if (negDiag.YIntersect > diag.X2 + diag.Y2)
                    break;

                // If the Y intersect of the positive diag is even, then the Y intersect of the negative diag must also be even
                var c = diag.YIntersect + negDiag.YIntersect;
                if (c % 2 == 1)
                    continue;

                // This is the x and y value where the two diagonals meet
                int yIntersect = c / 2;
                int xIntersect = diag.GetXAtY(yIntersect);

                if (diag.X1 <= xIntersect && xIntersect <= diag.X2 && negDiag.X1 <= xIntersect && xIntersect <= negDiag.X2)
                    overlaps.Add(xIntersect << 16 | yIntersect);
            }
        }

        for (int i = 0; i < negDiags.Count; i++)
        {
            NegativeDiagonalLine diag = negDiags[i];

            // Check for overlaps with other negative diagonals
            for (int j = i + 1; j < negDiags.Count; j++)
            {
                NegativeDiagonalLine otherDiag = negDiags[j];
                if (diag.YIntersect != otherDiag.YIntersect || diag.X2 < otherDiag.X1)
                    break;

                for (int x = otherDiag.X1; x <= Math.Min(otherDiag.X2, diag.X2); x++)
                    overlaps.Add(x << 16 | diag.GetYAtX(x));
            }

            // Check for overlaps with horizontals
            int startHorizontalIndex = BinarySearchForAxis(horizontals, diag.Y2);
            for (int j = startHorizontalIndex; j < horizontals.Count; j++)
            {
                HorizontalLine horizontal = horizontals[j];
                if (horizontal.Y > diag.Y1)
                    break;

                int xIntersect = diag.GetXAtY(horizontal.Y);
                if (horizontal.X1 <= xIntersect && xIntersect <= horizontal.X2)
                    overlaps.Add(xIntersect << 16 | horizontal.Y);
            }

            // Check for overlaps with verticals
            int startVerticalIndex = BinarySearchForAxis(verticals, diag.X1);
            for (int j = startVerticalIndex; j < verticals.Count; j++)
            {
                VerticalLine vertical = verticals[j];
                if (vertical.X > diag.X2)
                    break;

                int yIntersect = diag.GetYAtX(vertical.X);
                if (vertical.Y1 <= yIntersect && yIntersect <= vertical.Y2)
                    overlaps.Add(vertical.X << 16 | yIntersect);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerUntil(ReadOnlySpan<char> span, char c, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = span[i++] - '0';

        char cur;
        while ((cur = span[i++]) != c)
            ret = ret * 10 + (cur - '0');

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearchForAxis<T>(List<T> segments, int searchAxis)
        where T : IBinarySearchable
    {
        int lo = 0;
        int hi = segments.Count - 1;
        while (lo < hi)
        {
            int i = lo + ((hi - lo) >> 1);
            int axis = segments[i].BinarySearchAxis;
            if (axis < searchAxis)
            {
                lo = i + 1;
            }
            else
            {
                hi = i;
            }
        }

        return lo;
    }
}
