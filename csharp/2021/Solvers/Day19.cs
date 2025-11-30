using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

/// <summary>
/// Please come back later once I have made this better/more readable, but for now it's fast-ish
/// </summary>
public class Day19 : ISolver
{
    public enum Axis { PosX, NegX, PosY, NegY, PosZ, NegZ }

    readonly record struct Coord(int X, int Y, int Z) : IComparable<Coord>
    {
        public static readonly Coord Zero = new(0, 0, 0);

        public int GetValueOnAxis(Axis axis) => axis switch
        {
            Axis.PosX => X,
            Axis.NegX => -X,
            Axis.PosY => Y,
            Axis.NegY => -Y,
            Axis.PosZ => Z,
            Axis.NegZ => -Z,
            _ => default
        };

        public Coord ApplyAxes(Axis XAxis, Axis YAxis, Axis ZAxis)
            => new(GetValueOnAxis(XAxis), GetValueOnAxis(YAxis), GetValueOnAxis(ZAxis));

        public int GetManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public int CompareTo(Coord other)
        {
            var c = X.CompareTo(other.X);
            if (c != 0) return c;

            c = Y.CompareTo(other.Y);
            if (c != 0) return c;

            return Z.CompareTo(other.Z);
        }

        public static Coord operator +(Coord left, Coord right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        public static Coord operator -(Coord left, Coord right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    record ScannerData(List<Coord> Coords);

    record FixedScanner(Coord Center, ScannerData Data);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var scanners = ParseInput(input);
        var numScanners = scanners.Count;

        var scannerCoordinates = new Coord[scanners.Count];
        var foundScannersQueue = new Queue<FixedScanner>();
        var beacons = new HashSet<Coord>();
        var part2 = 0;

        var foundScanners = 1;
        ulong foundScannersBitset = 1;
        scannerCoordinates[0] = Coord.Zero;
        foundScannersQueue.Enqueue(new FixedScanner(Coord.Zero, scanners[0]));
        foreach (var coord in scanners[0].Coords)
            beacons.Add(coord);

        while (foundScanners < numScanners && foundScannersQueue.TryDequeue(out var fixedScanner))
        {
            var fixedCoords = CollectionsMarshal.AsSpan(fixedScanner.Data.Coords);
            fixedCoords.Sort();

            for (var i = 0; i < numScanners; i++)
            {
                if ((foundScannersBitset & (1UL << i)) != 0)
                    continue;

                var scanner = scanners[i];
                var scannerCoords = CollectionsMarshal.AsSpan(scanner.Coords);
                if (TryFindOverlap(fixedCoords, scannerCoords, out var relativePosition))
                {
                    var center = fixedScanner.Center + relativePosition;

                    foreach (var coord in scannerCoords)
                        beacons.Add(coord + center);

                    for (var j = 0; j < foundScanners; j++)
                    {
                        var distance = (center - scannerCoordinates[j]).GetManhattanDistance();
                        if (distance > part2)
                            part2 = distance;
                    }

                    scannerCoordinates[foundScanners++] = center;
                    foundScannersBitset |= 1UL << i;
                    foundScannersQueue.Enqueue(new FixedScanner(center, scanner));
                }
            }
        }

        solution.SubmitPart1(beacons.Count);
        solution.SubmitPart2(part2);
    }

    private static List<ScannerData> ParseInput(ReadOnlySpan<byte> input)
    {
        var inputIndex = 0;
        var scanners = new List<ScannerData>();
        while (inputIndex < input.Length)
            scanners.Add(ReadScannerFromInput(input, ref inputIndex));

        return scanners;
    }

    private static ScannerData ReadScannerFromInput(ReadOnlySpan<byte> input, ref int inputIndex)
    {
        // Skip first line containing scanner number
        while (input[inputIndex++] != '\n')
            continue;

        var coords = new List<Coord>();
        while (inputIndex < input.Length && input[inputIndex] != '\n')
        {
            var x = ReadIntegerFromInput(input, ',', ref inputIndex);
            var y = ReadIntegerFromInput(input, ',', ref inputIndex);
            var z = ReadIntegerFromInput(input, '\n', ref inputIndex);
            coords.Add(new(x, y, z));
        }

        inputIndex++;

        return new(coords);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerFromInput(ReadOnlySpan<byte> span, char until, ref int i)
    {
        // Assume that the first character is always a digit
        var c = span[i++];

        int mul;
        int ret;
        if (c == '-')
        {
            mul = -1;
            ret = 0;
        }
        else
        {
            mul = 1;
            ret = c - '0';
        }

        byte cur;
        while ((cur = span[i++]) != until)
            ret = ret * 10 + (cur - '0');

        return mul * ret;
    }

    private static bool TryFindOverlap(ReadOnlySpan<Coord> fixedCoords, Span<Coord> newCoords, out Coord relativePosition)
    {
        for (var axis = 0; axis < 6; axis++)
        {
            if (TryFindOverlapWithGivenXAxis(fixedCoords, newCoords, (Axis)axis, out relativePosition))
                return true;
        }

        relativePosition = default;
        return false;
    }

    private static bool TryFindOverlapWithGivenXAxis(ReadOnlySpan<Coord> fixedCoords, Span<Coord> newCoords, Axis axis, [NotNullWhen(returnValue: true)] out Coord relativePosition)
    {
        Span<byte> differenceCounts = stackalloc byte[4096];

        var maxDifferenceCount = 1;
        for (var newIndex = 0; newIndex < newCoords.Length - (12 - maxDifferenceCount); newIndex++)
        {
            var x2 = newCoords[newIndex].GetValueOnAxis(axis);

            foreach (var fixedCoord in fixedCoords)
            {
                var diff = x2 - fixedCoord.X;

                var diffIndex = diff + 2048;
                var differenceCount = differenceCounts[diffIndex]++;

                if (differenceCount == 6 && TryFindOverlapWithGivenXAxisAndOffset(fixedCoords, newCoords, axis, diff, out relativePosition))
                    return true;

                if (differenceCount > maxDifferenceCount)
                    maxDifferenceCount = differenceCount;
            }
        }

        relativePosition = default;
        return false;
    }

    private static bool TryFindOverlapWithGivenXAxisAndOffset(ReadOnlySpan<Coord> fixedCoords, Span<Coord> newCoords, Axis axis, int offset, out Coord relativePosition)
    {
        // This span will store the pairs of coordinates that have the same offset away on the x axis
        Span<(Coord Fixed, Coord New)> matches = stackalloc (Coord Fixed, Coord New)[20];
        var matchIndex = 0;

        // Then, look through the remainder of the newCoords list to find fixed coords that are at the same offset
        for (var newIndex = 0; newIndex < newCoords.Length; newIndex++)
        {
            var possibleMatchesLeft = newCoords.Length - newIndex;
            if (matchIndex + possibleMatchesLeft < 12)
                break;

            var newCoord = newCoords[newIndex];
            var newX = newCoord.GetValueOnAxis(axis);
            var expectedX = newX - offset;
            FindFixedCoordWithExpectedX(fixedCoords, expectedX, newCoord, matches, ref matchIndex);
        }

        if (matchIndex < 12)
        {
            relativePosition = default;
            return false;
        }

        return TryFindOverlapFromMatchesOnGivenXAxis(matches, axis, newCoords, out relativePosition);
    }

    private static bool TryFindOverlapFromMatchesOnGivenXAxis(ReadOnlySpan<(Coord Fixed, Coord New)> matches, Axis axis, Span<Coord> newCoords, out Coord relativePosition)
    {
        switch (axis)
        {
            case Axis.PosX:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosX, Axis.PosY, Axis.PosZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosX, Axis.NegY, Axis.NegZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosX, Axis.NegZ, Axis.PosY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosX, Axis.PosZ, Axis.NegY, newCoords, out relativePosition);
            case Axis.NegX:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegX, Axis.PosZ, Axis.PosY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegX, Axis.NegZ, Axis.NegY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegX, Axis.NegY, Axis.PosZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegX, Axis.PosY, Axis.NegZ, newCoords, out relativePosition);
            case Axis.PosY:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosY, Axis.PosZ, Axis.PosX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosY, Axis.NegZ, Axis.NegX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosY, Axis.NegX, Axis.PosZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosY, Axis.PosX, Axis.NegZ, newCoords, out relativePosition);
            case Axis.NegY:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegY, Axis.PosX, Axis.PosZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegY, Axis.NegX, Axis.NegZ, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegY, Axis.NegZ, Axis.PosX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegY, Axis.PosZ, Axis.NegX, newCoords, out relativePosition);
            case Axis.PosZ:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosZ, Axis.PosX, Axis.PosY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosZ, Axis.NegX, Axis.NegY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosZ, Axis.NegY, Axis.PosX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.PosZ, Axis.PosY, Axis.NegX, newCoords, out relativePosition);
            case Axis.NegZ:
                return
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegZ, Axis.PosY, Axis.PosX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegZ, Axis.NegY, Axis.NegX, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegZ, Axis.NegX, Axis.PosY, newCoords, out relativePosition) ||
                    TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(matches, Axis.NegZ, Axis.PosX, Axis.NegY, newCoords, out relativePosition);
            default:
                relativePosition = default;
                return false;
        }
    }

    private static int FindFixedCoordWithExpectedX(ReadOnlySpan<Coord> fixedCoords, int expectedX, Coord otherCoord, Span<(Coord Fixed, Coord New)> matches, ref int matchIndex)
    {
        var lo = 0;
        var hi = fixedCoords.Length - 1;

        while (lo <= hi)
        {
            var midIndex = (lo + hi) / 2;
            var fixedCoord = fixedCoords[midIndex];
            var x = fixedCoord.X;
            if (x > expectedX)
            {
                hi = midIndex - 1;
            }
            else if (x < expectedX)
            {
                lo = midIndex + 1;
            }
            else
            {
                matches[matchIndex++] = (fixedCoord, otherCoord);

                for (var i = midIndex - 1; i >= lo; i++)
                {
                    fixedCoord = fixedCoords[i];
                    if (fixedCoord.X != expectedX)
                        break;

                    matches[matchIndex++] = (fixedCoord, otherCoord);
                }

                for (var i = midIndex + 1; i <= hi; i++)
                {
                    fixedCoord = fixedCoords[i];
                    if (fixedCoord.X != expectedX)
                        break;

                    matches[matchIndex++] = (fixedCoord, otherCoord);
                }

                break;
            }
        }

        return matchIndex;
    }

    private static bool TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(ReadOnlySpan<(Coord Fixed, Coord New)> matches, Axis xAxis, Axis yAxis, Axis zAxis, Span<Coord> allCoords, out Coord center)
    {
        center = default;

        // Use a faster solution if there are exactly 12 matches
        if (matches.Length == 12)
        {
            (var firstMatchFixed, var firstMatchNew) = matches[0];
            var expectedYDiff = firstMatchNew.GetValueOnAxis(yAxis) - firstMatchFixed.Y;
            var expectedZDiff = firstMatchNew.GetValueOnAxis(zAxis) - firstMatchFixed.Z;

            for (var i = 1; i < matches.Length; i++)
            {
                (var matchFixed, var matchNew) = matches[i];
                var yDiff = matchNew.GetValueOnAxis(yAxis) - matchFixed.Y;
                if (yDiff != expectedYDiff)
                    return false;

                var zDiff = matchNew.GetValueOnAxis(zAxis) - matchFixed.Z;
                if (zDiff != expectedZDiff)
                    return false;
            }

            center = firstMatchFixed - firstMatchNew.ApplyAxes(xAxis, yAxis, zAxis);
        }
        else
        {
            var matchesLeft = matches.Length;
            var matchesLeftBitset = (1U << (1 + matchesLeft)) - 1;
            while (matchesLeft > 0)
            {
                var matchIndex = 0;
                while ((matchesLeftBitset & (1U << matchIndex)) == 0)
                    matchIndex++;

                var maxMatched = matchesLeft;
                matchesLeft--;

                (var firstMatchFixed, var firstMatchNew) = matches[matchIndex];

                var expectedYDiff = firstMatchNew.GetValueOnAxis(yAxis) - firstMatchFixed.Y;
                var expectedZDiff = firstMatchNew.GetValueOnAxis(zAxis) - firstMatchFixed.Z;

                for (var i = matchIndex + 1; i < matches.Length; i++)
                {
                    var matchBit = 1U << matchIndex;
                    if ((matchesLeftBitset & matchBit) == 0)
                        continue;

                    (var matchFixed, var matchNew) = matches[i];
                    var yDiff = matchNew.GetValueOnAxis(yAxis) - matchFixed.Y;
                    if (yDiff != expectedYDiff)
                    {
                        if (--maxMatched < 12)
                            break;
                        continue;
                    }

                    var zDiff = matchNew.GetValueOnAxis(zAxis) - matchFixed.Z;
                    if (zDiff != expectedZDiff)
                    {
                        if (--maxMatched < 12)
                            break;
                        continue;
                    }

                    matchesLeft--;
                    matchesLeftBitset &= ~matchBit;
                }

                if (maxMatched >= 12)
                {
                    center = firstMatchFixed - firstMatchNew.ApplyAxes(xAxis, yAxis, zAxis);
                    break;
                }
                else if (matchesLeft < 12)
                {
                    return false;
                }
            }
        }

        for (var i = 0; i < allCoords.Length; i++)
            allCoords[i] = allCoords[i].ApplyAxes(xAxis, yAxis, zAxis);

        return true;
    }
}
