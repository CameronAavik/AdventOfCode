using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day22 : ISolver
{
    readonly struct Cube(int x1, int x2, int y1, int y2, int z1, int z2)
    {
        private readonly int _x1 = x1;
        private readonly int _x2 = x2;
        private readonly int _y1 = y1;
        private readonly int _y2 = y2;
        private readonly int _z1 = z1;
        private readonly int _z2 = z2;

        // Since all coordinate values are doubled, we need to divide by 8 to account for this on all 3 axes.
        public long Volume => (_x2 + 1L - _x1) * (_y2 + 1L - _y1) * (_z2 + 1L - _z1) / 8L;

        public bool ContainsCube(Cube other) => _x1 <= other._x1 && _y1 <= other._y1 && _z1 <= other._z1 && other._x2 <= _x2 && other._y2 <= _y2 && other._z2 <= _z2;

        public Cube UnionWith(Cube other)
            => new(
                Math.Min(_x1, other._x1), Math.Max(_x2, other._x2),
                Math.Min(_y1, other._y1), Math.Max(_y2, other._y2),
                Math.Min(_z1, other._z1), Math.Max(_z2, other._z2));

        public Cube IntersectWith(Cube other)
            => new(
                Math.Max(_x1, other._x1), Math.Min(_x2, other._x2),
                Math.Max(_y1, other._y1), Math.Min(_y2, other._y2),
                Math.Max(_z1, other._z1), Math.Min(_z2, other._z2));

        public bool ContainsNegativeSide => _x2 < _x1 || _y2 < _y1 || _z2 < _z1;

        public void GetRangeOnAxis(int axis, out int start, out int end)
        {
            // Abuse the fact that the coordinates are contiguous in memory to turn this into a memory lookup.
            ref var field = ref Unsafe.AsRef(in _x1);
            start = Unsafe.Add(ref field, (nint)(uint)(axis * 2));
            end = Unsafe.Add(ref field, (nint)(uint)(axis * 2 + 1));
        }

        public void SplitOnAxis(int axis, int separator, out Cube left, out Cube right)
        {
            switch (axis)
            {
                case 0:
                    left = new(_x1, separator - 1, _y1, _y2, _z1, _z2);
                    right = new(separator, _x2, _y1, _y2, _z1, _z2);
                    break;
                case 1:
                    left = new(_x1, _x2, _y1, separator - 1, _z1, _z2);
                    right = new(_x1, _x2, separator, _y2, _z1, _z2);
                    break;
                case 2:
                default:
                    left = new(_x1, _x2, _y1, _y2, _z1, separator - 1);
                    right = new(_x1, _x2, _y1, _y2, separator, _z2);
                    break;
            }
        }
    }

    readonly record struct RebootStep(bool IsOn, Cube Cube);

    [SkipLocalsInit]
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<RebootStep> part1Steps = stackalloc RebootStep[1024];
        Span<RebootStep> part2Steps = stackalloc RebootStep[1024];
        var part1StepCount = 0;
        var part2StepCount = 0;

        Cube part1Bounds = new(-100, 99, -100, 99, -100, 99);
        Cube part2Bounds = default;

        var inputIndex = 0;
        while (inputIndex < input.Length)
        {
            // ParseRebootStep will parse the input, but will also modify the starting and ending coordinates
            // such that each value will be doubled, and the ending values will be offset by 1.
            // Example: x=1 to 23 will be stored as 2 to 47.
            var rebootStep = ParseRebootStep(input, ref inputIndex);
            (var isOn, var cube) = rebootStep;

            // Clamp the cube to the bounds from part 1
            var boundToPart1 = cube.IntersectWith(part1Bounds);
            if (!boundToPart1.ContainsNegativeSide)
                part1Steps[part1StepCount++] = new(isOn, boundToPart1);

            part2Bounds = part2Bounds.UnionWith(cube);
            part2Steps[part2StepCount++] = rebootStep;
        }

        part1Steps = part1Steps[..part1StepCount];
        part2Steps = part2Steps[..part2StepCount];

        var part1 = Solve(part1Steps, part1Bounds);
        var part2 = Solve(part2Steps, part2Bounds);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [SkipLocalsInit]
    private static long Solve(ReadOnlySpan<RebootStep> steps, Cube boundingCube, int splitAxis = 0, bool defaultIsOn = false)
    {
        // Skip any steps at the start which set the cube to the same state that is the default
        var newStart = 0;
        while (newStart < steps.Length && steps[newStart].IsOn == defaultIsOn)
            newStart++;
        steps = steps[newStart..];

        // If there are no steps left, then we use the volume of the bounding cube.
        if (steps.Length == 0)
            return defaultIsOn ? boundingCube.Volume : 0;

        // If there is only one step, then we know that it will have the opposite state to the default.
        // If the default is on, then the cube is off, so return the difference in volume with the bounding cube.
        // If the default is off, then the cube is on, so return the volume of the cube.
        if (steps.Length == 1)
        {
            // To get the volume of the cube, we need to get the cube representing the overlap
            var overlap = steps[0].Cube.IntersectWith(boundingCube);
            var cubeVolume = overlap.Volume;
            return defaultIsOn ? boundingCube.Volume - cubeVolume : cubeVolume;
        }

        // Optimise case where there are two cubes left
        if (steps.Length == 2)
        {
            var overlap1 = steps[0].Cube.IntersectWith(boundingCube);
            var overlap2 = steps[1].Cube.IntersectWith(boundingCube);

            var combinedVolumes = overlap1.Volume;

            // If they both have the same state, then add together.
            if (!defaultIsOn == steps[1].IsOn)
                combinedVolumes += overlap2.Volume;

            // If they overlap, subtract the overlap volume.
            var overlaps = overlap1.IntersectWith(overlap2);
            if (!overlaps.ContainsNegativeSide)
                combinedVolumes -= overlaps.Volume;

            return defaultIsOn ? boundingCube.Volume - combinedVolumes : combinedVolumes;
        }

        // Try find an axis and value to split on.
        int separator;
        var axisValues = steps.Length <= 8 ? stackalloc int[16] : new int[steps.Length * 2];
        while (true)
        {
            // Get two coordinates from each cube on the given axis and put it in axisValues.
            // Skip any coordinates that are already touching the bounding cube.
            var numValues = GetAxisValues(steps, boundingCube, splitAxis, axisValues);

            // If there are no values, it means that all the cubes are touching the bounding cube on the axis.
            // We therefore try the next axis
            if (numValues == 0)
            {
                splitAxis = (splitAxis + 1) % 3;
                continue;
            }

            // Determine the value to split on
            separator = GetMedian(axisValues[..numValues]);

            // If the separator is odd, then increment it by 1 so that it represents the start of a range.
            if (separator % 2 != 0)
                separator++;

            break;
        }

        // Generate two new cubes after splitting on the given axis
        boundingCube.SplitOnAxis(splitAxis, separator, out var leftCube, out var rightCube);

        // Build a list of steps that affect the left cube, and a list that affects the right cube.
        // If a step overlaps with both the left and right cube, it will be placed in both lists.
        var leftSteps = steps.Length <= 8 ? stackalloc RebootStep[8] : new RebootStep[steps.Length];
        var rightSteps = steps.Length <= 8 ? stackalloc RebootStep[8] : new RebootStep[steps.Length];
        var leftStepsLength = 0;
        var rightStepsLength = 0;

        // In this process, we may also find cubes that encompass the entirety of the left or right cube.
        // When we find these, we will be able to update the default state to apply for the entire cube.
        var leftDefault = defaultIsOn;
        var rightDefault = defaultIsOn;

        foreach (var step in steps)
        {
            var cube = step.Cube;
            cube.GetRangeOnAxis(splitAxis, out var start, out var end);

            var shouldCheckRightOverlap = true;
            if (start < separator)
            {
                if (separator <= end + 1 && cube.ContainsCube(leftCube))
                {
                    // If a cube encompasses the entirety of the left cube, then we can ignore any prior steps.
                    // We can also set the default value for the entire left cube to that of the encompassing cube.
                    leftStepsLength = 0; // Setting this to zero is how we clear the list of steps
                    leftDefault = step.IsOn;

                    // We also know we don't need to check the right cube for overlap as it can't overlap both.
                    shouldCheckRightOverlap = false;
                }
                else
                {
                    leftSteps[leftStepsLength++] = step;
                }
            }

            if (separator < end)
            {
                if (shouldCheckRightOverlap && start <= separator && cube.ContainsCube(rightCube))
                {
                    rightStepsLength = 0;
                    rightDefault = step.IsOn;
                }
                else
                {
                    rightSteps[rightStepsLength++] = step;
                }
            }
        }

        // Every step down, we split by the next axis
        var nextAxis = (splitAxis + 1) % 3;
        var leftVolume = Solve(leftSteps[..leftStepsLength], leftCube, nextAxis, leftDefault);
        var rightVolume = Solve(rightSteps[..rightStepsLength], rightCube, nextAxis, rightDefault);
        return leftVolume + rightVolume;
    }

    // Implementation of introselect algorithm to find median of a list of values
    private static int GetMedian(Span<int> values)
    {
        var medianIndex = (values.Length - 1) / 2;
        while (values.Length > 1)
        {
            if (medianIndex == 0)
                return FindMin(values);

            if (medianIndex == values.Length - 1)
                return FindMax(values);

            var pivot = values[0];
            var l = 1;
            var r = values.Length - 1;
            while (l <= r)
            {
                var score = values[l];
                if (score <= pivot)
                {
                    l++;
                }
                else
                {
                    values[l] = values[r];
                    values[r] = score;
                    r--;
                }
            }

            if (l <= medianIndex)
            {
                medianIndex -= l;
                values = values[l..];
            }
            else if (l == medianIndex + 1)
            {
                return pivot;
            }
            else
            {
                values = values[1..l];
            }
        }

        return values[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int FindMax(Span<int> scores)
        {
            var max = int.MinValue;
            foreach (var score in scores)
                if (score > max)
                    max = score;
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int FindMin(Span<int> scores)
        {
            var min = int.MaxValue;
            foreach (var score in scores)
                if (score < min)
                    min = score;
            return min;
        }
    }

    private static int GetAxisValues(ReadOnlySpan<RebootStep> steps, Cube boundingCube, int axis, Span<int> values)
    {
        boundingCube.GetRangeOnAxis(axis, out var boundingStart, out var boundingEnd);

        var valuesLength = 0;
        foreach (var step in steps)
        {
            var cube = step.Cube;
            cube.GetRangeOnAxis(axis, out var start, out var end);

            if (start > boundingStart)
                values[valuesLength++] = start;

            if (end < boundingEnd)
                values[valuesLength++] = end;
        }

        return valuesLength;
    }

    private static RebootStep ParseRebootStep(ReadOnlySpan<byte> input, ref int inputIndex)
    {
        var isOn = input[inputIndex + 1] == 'n';
        inputIndex += isOn ? "on x=".Length : "off x=".Length;

        var x1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        var x2 = ReadIntegerFromInput(input, ',', ref inputIndex);
        inputIndex += 2;

        var y1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        var y2 = ReadIntegerFromInput(input, ',', ref inputIndex);
        inputIndex += 2;

        var z1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        var z2 = ReadIntegerFromInput(input, '\n', ref inputIndex);

        return new(isOn, new(x1 * 2, x2 * 2 + 1, y1 * 2, y2 * 2 + 1, z1 * 2, z2 * 2 + 1));
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
}
