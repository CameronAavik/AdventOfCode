using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
            int c = X.CompareTo(other.X);
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

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        List<ScannerData> scanners = ParseInput(input);
        int numScanners = scanners.Count;

        var scannerCoordinates = new Coord[scanners.Count];
        var foundScannersQueue = new Queue<FixedScanner>();
        var beacons = new HashSet<Coord>();
        int part2 = 0;

        int foundScanners = 1;
        ulong foundScannersBitset = 1;
        scannerCoordinates[0] = Coord.Zero;
        foundScannersQueue.Enqueue(new FixedScanner(Coord.Zero, scanners[0]));
        foreach (Coord coord in scanners[0].Coords)
            beacons.Add(coord);

        while (foundScanners < numScanners && foundScannersQueue.TryDequeue(out FixedScanner? fixedScanner))
        {
            List<Coord> fixedCoords = fixedScanner.Data.Coords;
            fixedCoords.Sort();

            for (int i = 0; i < numScanners; i++)
            {
                if ((foundScannersBitset & (1UL << i)) != 0)
                    continue;

                ScannerData scanner = scanners[i];
                List<Coord> scannerCoords = scanner.Coords;
                if (TryFindOverlap(fixedCoords, scannerCoords, out Coord relativePosition))
                {
                    Coord center = fixedScanner.Center + relativePosition;

                    foreach (Coord coord in scannerCoords)
                        beacons.Add(coord + center);

                    for (int j = 0; j < foundScanners; j++)
                    {
                        int distance = (center - scannerCoordinates[j]).GetManhattanDistance();
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

    private static List<ScannerData> ParseInput(ReadOnlySpan<char> input)
    {
        int inputIndex = 0;
        var scanners = new List<ScannerData>();
        while (inputIndex < input.Length)
            scanners.Add(ReadScannerFromInput(input, ref inputIndex));

        return scanners;
    }

    private static ScannerData ReadScannerFromInput(ReadOnlySpan<char> input, ref int inputIndex)
    {
        // Skip first line containing scanner number
        while (input[inputIndex++] != '\n')
            continue;

        var coords = new List<Coord>();
        while (inputIndex < input.Length && input[inputIndex] != '\n')
        {
            int x = ReadIntegerFromInput(input, ',', ref inputIndex);
            int y = ReadIntegerFromInput(input, ',', ref inputIndex);
            int z = ReadIntegerFromInput(input, '\n', ref inputIndex);
            coords.Add(new(x, y, z));
        }

        inputIndex++;

        return new(coords);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerFromInput(ReadOnlySpan<char> span, char until, ref int i)
    {
        // Assume that the first character is always a digit
        char c = span[i++];

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

        char cur;
        while ((cur = span[i++]) != until)
            ret = ret * 10 + (cur - '0');

        return mul * ret;
    }

    private static bool TryFindOverlap(List<Coord> fixedCoords, List<Coord> newCoords, out Coord relativePosition)
    {
        for (int axis = 0; axis < 6; axis++)
        {
            if (TryFindOverlapWithGivenXAxis(fixedCoords, newCoords, (Axis)axis, out relativePosition))
                return true;
        }

        relativePosition = default;
        return false;
    }

    private static bool TryFindOverlapWithGivenXAxis(List<Coord> fixedCoords, List<Coord> newCoords, Axis axis, [NotNullWhen(returnValue: true)] out Coord relativePosition)
    {
        Span<byte> differenceCounts = stackalloc byte[4096];

        int maxDifferenceCount = 1;
        for (int newIndex = 0; newIndex < newCoords.Count - (12 - maxDifferenceCount); newIndex++)
        {
            int x2 = newCoords[newIndex].GetValueOnAxis(axis);

            for (int fixedIndex = 0; fixedIndex < fixedCoords.Count; fixedIndex++)
            {
                int diff = x2 - fixedCoords[fixedIndex].X;

                int diffIndex = diff + 2048;
                byte differenceCount = ++differenceCounts[diffIndex];
                if (differenceCount == 6)
                {
                    if (TryFindOverlapWithGivenXAxisAndOffset(fixedCoords, newCoords, axis, diff, out relativePosition))
                        return true;
                }

                if (differenceCount > maxDifferenceCount)
                    maxDifferenceCount = differenceCount;
            }
        }

        relativePosition = default;
        return false;
    }

    private static bool TryFindOverlapWithGivenXAxisAndOffset(List<Coord> fixedCoords, List<Coord> newCoords, Axis axis, int offset, out Coord relativePosition)
    {
        // This span will store the pairs of coordinates that have the same offset away on the x axis
        Span<(Coord Fixed, Coord New)> matches = stackalloc (Coord Fixed, Coord New)[20];
        int matchIndex = 0;

        // Then, look through the remainder of the newCoords list to find fixed coords that are at the same offset
        for (int newIndex = 0; newIndex < newCoords.Count; newIndex++)
        {
            int possibleMatchesLeft = newCoords.Count - newIndex;
            if (matchIndex + possibleMatchesLeft < 12)
                break;

            Coord newCoord = newCoords[newIndex];
            int newX = newCoord.GetValueOnAxis(axis);
            int expectedX = newX - offset;
            FindFixedCoordWithExpectedX(fixedCoords, expectedX, newCoord, matches, ref matchIndex);
        }

        if (matchIndex < 12)
        {
            relativePosition = default;
            return false;
        }

        return TryFindOverlapFromMatchesOnGivenXAxis(matches, axis, newCoords, out relativePosition);
    }

    private static bool TryFindOverlapFromMatchesOnGivenXAxis(Span<(Coord Fixed, Coord New)> matches, Axis axis, List<Coord> newCoords, out Coord relativePosition)
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

    private static int FindFixedCoordWithExpectedX(List<Coord> fixedCoords, int expectedX, Coord otherCoord, Span<(Coord Fixed, Coord New)> matches, ref int matchIndex)
    {
        int lo = 0;
        int hi = fixedCoords.Count - 1;

        while (lo <= hi)
        {
            int midIndex = (lo + hi) / 2;
            Coord fixedCoord = fixedCoords[midIndex];
            int x = fixedCoord.X;
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

                for (int i = midIndex - 1; i >= lo; i++)
                {
                    fixedCoord = fixedCoords[i];
                    if (fixedCoord.X != expectedX)
                        break;

                    matches[matchIndex++] = (fixedCoord, otherCoord);
                }

                for (int i = midIndex + 1; i <= hi; i++)
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

    private static bool TryFindOverlapFromMatchesOnGivenAxesAndFixCoords(Span<(Coord Fixed, Coord New)> matches, Axis xAxis, Axis yAxis, Axis zAxis, List<Coord> allCoords, out Coord center)
    {
        center = default;

        // Use a faster solution if there are exactly 12 matches
        if (matches.Length == 12)
        {
            (Coord firstMatchFixed, Coord firstMatchNew) = matches[0];
            int expectedYDiff = firstMatchNew.GetValueOnAxis(yAxis) - firstMatchFixed.Y;
            int expectedZDiff = firstMatchNew.GetValueOnAxis(zAxis) - firstMatchFixed.Z;

            for (int i = 1; i < matches.Length; i++)
            {
                (Coord matchFixed, Coord matchNew) = matches[i];
                int yDiff = matchNew.GetValueOnAxis(yAxis) - matchFixed.Y;
                if (yDiff != expectedYDiff)
                    return false;

                int zDiff = matchNew.GetValueOnAxis(zAxis) - matchFixed.Z;
                if (zDiff != expectedZDiff)
                    return false;
            }

            center = firstMatchFixed - firstMatchNew.ApplyAxes(xAxis, yAxis, zAxis);
        }
        else
        {
            int matchesLeft = matches.Length;
            uint matchesLeftBitset = (1U << (1 + matchesLeft)) - 1;
            while (matchesLeft > 0)
            {
                int matchIndex = 0;
                while ((matchesLeftBitset & (1U << matchIndex)) == 0)
                    matchIndex++;

                int maxMatched = matchesLeft;
                matchesLeft--;

                (Coord firstMatchFixed, Coord firstMatchNew) = matches[matchIndex];

                int expectedYDiff = firstMatchNew.GetValueOnAxis(yAxis) - firstMatchFixed.Y;
                int expectedZDiff = firstMatchNew.GetValueOnAxis(zAxis) - firstMatchFixed.Z;

                for (int i = matchIndex + 1; i < matches.Length; i++)
                {
                    uint matchBit = 1U << matchIndex;
                    if ((matchesLeftBitset & matchBit) == 0)
                        continue;

                    (Coord matchFixed, Coord matchNew) = matches[i];
                    int yDiff = matchNew.GetValueOnAxis(yAxis) - matchFixed.Y;
                    if (yDiff != expectedYDiff)
                    {
                        if (--maxMatched < 12)
                            break;
                        continue;
                    }

                    int zDiff = matchNew.GetValueOnAxis(zAxis) - matchFixed.Z;
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

        for (int i = 0; i < allCoords.Count; i++)
            allCoords[i] = allCoords[i].ApplyAxes(xAxis, yAxis, zAxis);

        return true;
    }
}
