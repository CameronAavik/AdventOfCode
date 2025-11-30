using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day24 : ISolver
{
    public readonly record struct Vec3(long X, long Y, long Z)
    {
        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public Vec3 Cross(Vec3 other) => new(
            (Y * other.Z) - (Z * other.Y),
            (Z * other.X) - (X * other.Z),
            (X * other.Y) - (Y * other.X));

        public static Vec3 operator -(Vec3 a) => new(-a.X, -a.Y, -a.Z);
    }

    public readonly record struct Line(Vec3 P, Vec3 V);
    public readonly record struct LineSegment(long X0, long Y0, long X1, long Y1, double Gradient);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lines = new Line[3];
        var part1Segments = new LineSegment[300];
        var numLines = 0;
        var part1 = 0;
        while (!input.IsEmpty)
        {
            var line = ParseLine(ref input);
            var a = GetPart1Segment(line);

            // GetPart1Segment will set values to zero if the line does not enter the region
            if (a.X0 == 0)
                continue;

            if (numLines < 3)
                lines[numLines] = line;

            for (var j = 0; j < numLines; j++)
            {
                var b = part1Segments[j];

                // using approach from https://math.stackexchange.com/a/1342439
                // modified slightly to prevent long overflow
                var abx0Diff = b.X0 - a.X0;
                var abY0Diff = b.Y0 - a.Y0;
                var intersects =
                    (abx0Diff > abY0Diff * a.Gradient) != ((b.X1 - a.X0) > (b.Y1 - a.Y0) * a.Gradient) &&
                    (-abx0Diff > -abY0Diff * b.Gradient) != ((a.X1 - b.X0) > (a.Y1 - b.Y0) * b.Gradient);

                part1 += intersects ? 1 : 0;
            }

            part1Segments[numLines++] = a;
        }

        solution.SubmitPart1(part1);

        var part2 = SolvePart2(lines[0], lines[1], lines[2]);
        solution.SubmitPart2(part2);
    }

    private static LineSegment GetPart1Segment(Line line)
    {
        const long start = 200000000000000;
        const long end = 400000000000000;

        (var aPos, var aVel) = line;

        var startTX = (start - aPos.X) / aVel.X;
        var endTX = (end - aPos.X) / aVel.X;
        if (aVel.X < 0)
            (startTX, endTX) = (endTX, startTX);

        var startTY = (start - aPos.Y) / aVel.Y;
        var endTY = (end - aPos.Y) / aVel.Y;
        if (aVel.Y < 0)
            (startTY, endTY) = (endTY, startTY);

        var startT = Math.Max(Math.Max(startTX, startTY), 0);
        var endT = Math.Min(endTX, endTY);

        if (endT < startT)
            return new(0, 0, 0, 0, 0);

        var x0 = aVel.X * startT + aPos.X;
        var y0 = aVel.Y * startT + aPos.Y;
        var x1 = aVel.X * endT + aPos.X;
        var y1 = aVel.Y * endT + aPos.Y;

        return new(x0, y0, x1, y1, (double)(x1 - x0) / (y1 - y0));
    }

    private static long SolvePart2(Line a, Line b, Line c)
    {
        var aCrossed = -a.P.Cross(a.V);
        var crossOne = aCrossed + b.P.Cross(b.V);
        var crossTwo = aCrossed + c.P.Cross(c.V);

        // right hand side of linear equations
        decimal[] rhs = [crossOne.X, crossOne.Y, crossOne.Z, crossTwo.X, crossTwo.Y, crossTwo.Z];

        var matrix = new decimal[6][];
        for (var i = 0; i < 6; i++)
        {
            matrix[i] = new decimal[7];
            matrix[i][6] = rhs[i];
        }

        InsertIntoMatrix(a.V - b.V, 0, 0);
        InsertIntoMatrix(a.P - b.P, 0, 3);
        InsertIntoMatrix(a.V - c.V, 3, 0);
        InsertIntoMatrix(a.P - c.P, 3, 3);

        var solution = new decimal[6];

        GaussianElimination(matrix, solution);

        return (long)Math.Round(solution[0] + solution[1] + solution[2]);

        void InsertIntoMatrix(Vec3 v, int i, int j)
        {
            matrix[i][j + 1] = -v.Z;
            matrix[i][j + 2] = v.Y;
            matrix[i + 1][j] = v.Z;
            matrix[i + 1][j + 2] = -v.X;
            matrix[i + 2][j] = -v.Y;
            matrix[i + 2][j + 1] = v.X;
        }

        static void GaussianElimination(decimal[][] m, decimal[] solution)
        {
            var n = m.Length;
            for (var i = 0; i < n; i++)
            {
                var row = m[i];

                // find largest row and swap it with the top
                var maxValue = row[i];
                var maxRow = row;
                for (var j = i + 1; j < n; j++)
                {
                    var pivotRow = m[j];
                    if (Math.Abs(pivotRow[i]) > maxValue)
                    {
                        maxValue = pivotRow[i];
                        maxRow = pivotRow;
                    }
                }

                for (var k = i; k < n + 1; k++)
                    (row[k], maxRow[k]) = (maxRow[k], row[k]);

                // scale down rows by the value in the diagonal so that the lower triangle is zeroes
                for (var k = i + 1; k < n; k++)
                {
                    var kRow = m[k];
                    var c = -kRow[i] / row[i];
                    for (var j = i; j < n + 1; j++)
                        kRow[j] = i == j ? 0 : kRow[j] + c * row[j];
                }
            }

            for (var i = n - 1; i >= 0; i--)
            {
                solution[i] = m[i][n] / m[i][i];
                for (var k = i - 1; k >= 0; k--)
                    m[k][n] = m[k][n] - m[k][i] * solution[i];
            }
        }
    }

    private static Line ParseLine(ref ReadOnlySpan<byte> input)
    {
        var px = ReadLongUntil(ref input, (byte)',');
        input = input[1..];
        var py = ReadLongUntil(ref input, (byte)',');
        input = input[1..];
        var pz = ReadLongUntil(ref input, (byte)' ');
        input = input[2..];
        var vx = ReadLongUntil(ref input, (byte)',');
        input = input[1..];
        var vy = ReadLongUntil(ref input, (byte)',');
        input = input[1..];
        var vz = ReadLongUntil(ref input, (byte)'\n');

        return new Line(new Vec3(px, py, pz), new Vec3(vx, vy, vz));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long ReadLongUntil(ref ReadOnlySpan<byte> input, byte c)
        {
            var cur = input[0];
            long n;
            bool isNeg;
            if (cur == '-')
            {
                n = 0;
                isNeg = true;
            }
            else
            {
                n = cur - '0';
                isNeg = false;
            }

            var i = 1;
            while ((cur = input[i++]) != c)
                n = n * 10 + cur - '0';

            if (isNeg)
                n = -n;

            input = input[i..];
            return n;
        }
    }
}
