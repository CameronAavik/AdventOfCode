using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day09 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var segments = ParseHorizontalSegments(input);
        segments.Sort();
        solution.SubmitPart1(SolvePart1(segments));
        solution.SubmitPart2(SolvePart2(segments));
    }

    private static long SolvePart1(List<HorizontalSegment> segments)
    {
        // TODO: Optimize this O(n^2) solution as it takes about 40% of the total running time
        long maxArea = 0;
        for (var i = 0; i < segments.Count; i++)
        {
            var coord1 = segments[i];
            for (var j = i + 1; j < segments.Count; j++)
                maxArea = Math.Max(maxArea, coord1.GetLargestRectangularArea(segments[j]));
        }

        return maxArea;
    }

    private static long SolvePart2(List<HorizontalSegment> segments)
    {
        // Identify all unique x-coordinates
        var xCoords = GetUniqueXCoordinates(segments);

        // Keep track of the current polygon state at each x-coordinate (column) of the grid. The state is updated as
        // we scan through the horizontal lines to answer queries such as whether a column is currently inside the
        // polygon, and the locations of any corners inside the polygon in that column.
        var columns = new ScanlineColumn[xCoords.Length];
        for (var i = 0; i < columns.Length; i++)
            columns[i] = new ScanlineColumn(xCoords[i]);

        long maxArea = 0;
        var prevY = 0;
        var sameYStart = int.MaxValue;

        for (var i = 0; i < segments.Count; i++)
        {
            var (y, x1, x2) = segments[i];

            // If the segment is on a new row, update all the scanline columns with the segments on the previous row.
            if (y != prevY)
            {
                for (var j = sameYStart; j < i; j++)
                {
                    var (_, prevX1, prevX2) = segments[j];
                    var prevStartXIndex = xCoords.BinarySearch(prevX1);
                    for (var xIndex = prevStartXIndex; xIndex < columns.Length; xIndex++)
                    {
                        var column = columns[xIndex];
                        if (column.X == prevX1 || column.X == prevX2)
                        {
                            column.AddCorner(prevY, isStartOfSegment: column.X == prevX1);
                            if (column.X == prevX2)
                                break;
                        }
                        else
                        {
                            column.AddEdge(prevY);
                        }
                    }
                }

                prevY = y;
                sameYStart = i;
            }

            var startXIndex = xCoords.BinarySearch(x1);
            var endXIndex = -1;

            var startYMin = columns[startXIndex].MinYInsidePolygon;
            var endYMin = 0;

            // Find largest rectangles from (x1, y) between x1 and x2
            var minValidY = startYMin;
            for (var xIndex = startXIndex; xIndex < columns.Length; xIndex++)
            {
                var column = columns[xIndex];

                if (column.GetCornerAfter(minValidY) is int cornerY)
                {
                    var width = column.X - x1 + 1;
                    var height = y - cornerY + 1;
                    maxArea = Math.Max(maxArea, (long)width * height);
                }

                minValidY = Math.Max(minValidY, column.MinYInsidePolygon);

                if (column.X == x2)
                {
                    endXIndex = xIndex;
                    endYMin = column.MinYInsidePolygon;
                    break;
                }
            }

            // Find largest rectangles from (x2, y) between x2 and x1
            minValidY = endYMin;
            for (var xIndex = endXIndex; xIndex >= startXIndex; xIndex--)
            {
                var column = columns[xIndex];
                if (column.GetCornerAfter(minValidY) is int cornerY)
                {
                    var width = x2 - column.X + 1;
                    var height = y - cornerY + 1;
                    maxArea = Math.Max(maxArea, (long)width * height);
                }

                minValidY = Math.Max(minValidY, column.MinYInsidePolygon);
            }

            var otherXYMin = startYMin;

            // Find all rectangles with a top corner < x1
            minValidY = startYMin;
            for (var xIndex = startXIndex; xIndex >= 0; xIndex--)
            {
                var column = columns[xIndex];
                if (!column.IsActive)
                    break;

                if (column.GetCornerAfter(minValidY) is int cornerY)
                {
                    var useX2 = cornerY >= otherXYMin;
                    var x = useX2 ? x2 : x1;

                    var width = x - column.X + 1;
                    var height = y - cornerY + 1;
                    maxArea = Math.Max(maxArea, (long)width * height);

                    if (!useX2 && column.GetCornerAfter(otherXYMin) is int otherCornerY)
                    {
                        width = x2 - column.X + 1;
                        height = y - otherCornerY + 1;
                        maxArea = Math.Max(maxArea, (long)width * height);
                    }
                }

                minValidY = Math.Max(minValidY, column.MinYInsidePolygon);
            }

            // Find all rectanges with a top corner > x2
            minValidY = endYMin;
            for (var xIndex = endXIndex; xIndex < columns.Length; xIndex++)
            {
                var column = columns[xIndex];
                if (!column.IsActive)
                    break;

                if (column.GetCornerAfter(minValidY) is int cornerY)
                {
                    var useX1 = cornerY >= otherXYMin;
                    var x = useX1 ? x1 : x2;

                    var width = column.X - x + 1;
                    var height = y - cornerY + 1;
                    maxArea = Math.Max(maxArea, (long)width * height);

                    if (!useX1 && column.GetCornerAfter(otherXYMin) is int otherCornerY)
                    {
                        width = column.X - x1 + 1;
                        height = y - otherCornerY + 1;
                        maxArea = Math.Max(maxArea, (long)width * height);
                    }
                }

                minValidY = Math.Max(minValidY, column.MinYInsidePolygon);
            }
        }

        return maxArea;
    }

    public class ScanlineColumn(int x)
    {
        private readonly List<int> _cornerLocations = [];
        private bool _lastCornerWasSegmentStart;

        public int X => x;
        public int MinYInsidePolygon { get; private set; } = int.MaxValue;
        public bool IsActive => MinYInsidePolygon != int.MaxValue;

        public int? GetCornerAfter(int y)
        {
            foreach (var cornerY in _cornerLocations)
            {
                if (cornerY >= y)
                    return cornerY;
            }

            return null;
        }

        public void AddEdge(int y)
        {
            if (IsActive)
            {
                _cornerLocations.Clear();
                MinYInsidePolygon = int.MaxValue;
            }
            else
            {
                MinYInsidePolygon = y;
            }
        }

        public void AddCorner(int y, bool isStartOfSegment)
        {
            if (!IsActive)
            {
                MinYInsidePolygon = y;
                _cornerLocations.Add(y);
            }
            else if (_cornerLocations.Count % 2 == 0)
            {
                _cornerLocations.Add(y);
            }
            else
            {
                // If the previous and current corner are both from the same end of their segment, then it means we have not entered or left the polygon
                var matchPreviousStatus = isStartOfSegment == _lastCornerWasSegmentStart;
                var wasActive = MinYInsidePolygon != _cornerLocations[0];

                var newStateIsActive = matchPreviousStatus ? wasActive : !wasActive;
                if (newStateIsActive)
                {
                    _cornerLocations.Add(y);
                }
                else
                {
                    _cornerLocations.Clear();
                    MinYInsidePolygon = int.MaxValue;
                }
            }

            _lastCornerWasSegmentStart = isStartOfSegment;
        }
    }

    private static ReadOnlySpan<int> GetUniqueXCoordinates(List<HorizontalSegment> segments)
    {
        Span<int> xValues = new int[segments.Count * 2];
        for (var i = 0; i < segments.Count; i++)
        {
            xValues[i * 2] = segments[i].X1;
            xValues[i * 2 + 1] = segments[i].X2;
        }

        xValues.Sort();

        var xCount = 1;
        var prevX = xValues[0];
        for (var i = 1; i < xValues.Length; i++)
        {
            if (xValues[i] != prevX)
            {
                xValues[xCount++] = xValues[i];
                prevX = xValues[i];
            }
        }

        return xValues[..xCount];
    }

    public readonly record struct Coordinate(int X, int Y);

    public readonly record struct HorizontalSegment(int Y, int X1, int X2) : IComparable<HorizontalSegment>
    {
        public int CompareTo(HorizontalSegment other)
        {
            var yComp = Y.CompareTo(other.Y);
            return yComp != 0 ? yComp : X1.CompareTo(other.X1);
        }

        public long GetLargestRectangularArea(HorizontalSegment other)
        {
            var height = Math.Abs(Y - other.Y) + 1;
            var minX = Math.Min(X1, other.X1);
            var maxX = Math.Max(X2, other.X2);
            var width = maxX - minX + 1;
            return (long)height * width;
        }
    }

    private static List<HorizontalSegment> ParseHorizontalSegments(ReadOnlySpan<byte> input)
    {
        var inputPtr = 0;
        var segments = new List<HorizontalSegment>(input.Length / 22); // 22 is based on average line length of real inputs
        var firstCoord = ParseCoordinate(input, ref inputPtr);
        var prevCoord = firstCoord;
        while (inputPtr < input.Length)
        {
            var coord = ParseCoordinate(input, ref inputPtr);
            if (coord.Y == prevCoord.Y)
                segments.Add(prevCoord.X < coord.X ? new(coord.Y, prevCoord.X, coord.X) : new(coord.Y, coord.X, prevCoord.X));
            prevCoord = coord;
        }

        if (firstCoord.Y == prevCoord.Y)
            segments.Add(prevCoord.X < firstCoord.X ? new(firstCoord.Y, prevCoord.X, firstCoord.X) : new(firstCoord.Y, firstCoord.X, prevCoord.X));

        return segments;
    }

    private static Coordinate ParseCoordinate(ReadOnlySpan<byte> input, ref int i)
    {
        var x = ParseIntUntil(input, (byte)',', ref i);
        var y = ParseIntUntil(input, (byte)'\n', ref i);
        return new Coordinate(x, y);
    }

    private static int ParseIntUntil(ReadOnlySpan<byte> input, byte endChar, ref int i)
    {
        var value = (int)(input[i++] - (byte)'0');
        while (input[i++] is byte c && c != endChar)
            value = value * 10 + (int)(c - (byte)'0');
        return value;
    }
}
