using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

// TODO: Rewrite GetTotalLength to use an interval tree
public class Day22 : ISolver
{
    readonly record struct RebootStep(bool IsOn, int X1, int X2, int Y1, int Y2, int Z1, int Z2);

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<RebootStep> part1Steps = stackalloc RebootStep[1024];
        Span<RebootStep> part2Steps = stackalloc RebootStep[1024];
        int part1StepCount = 0;
        int part2StepCount = 0;

        int minZ = int.MaxValue;
        int maxZ = int.MinValue;

        int inputIndex = 0;
        while (inputIndex < input.Length)
        {
            RebootStep rebootStep = ParseRebootStep(input, ref inputIndex);

            (bool isOn, int x1, int x2, int y1, int y2, int z1, int z2) = rebootStep;
            if (x1 <= 50 && x2 >= -50 && y1 <= 50 && y2 >= -50 && z1 <= 50 && z2 >= -50)
                part1Steps[part1StepCount++] = new(isOn, Math.Clamp(x1, -50, 50), Math.Clamp(x2, -50, 50), Math.Clamp(y1, -50, 50), Math.Clamp(y2, -50, 50), Math.Clamp(z1, -50, 50), Math.Clamp(z2, -50, 50));

            part2Steps[part2StepCount++] = rebootStep;

            minZ = Math.Min(minZ, rebootStep.Z1);
            maxZ = Math.Max(maxZ, rebootStep.Z2);
        }

        part1Steps = part1Steps.Slice(0, part1StepCount);
        part2Steps = part2Steps.Slice(0, part2StepCount);

        long part1 = Solve(part1Steps);
        long part2 = Solve(part2Steps);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    readonly record struct SweepEvent(int Value, short EventId) : IComparable<SweepEvent>
    {
        public int CompareTo(SweepEvent other) => Value.CompareTo(other.Value);
    }

    readonly record struct SweepIndexes(short StartIndex, short EndIndex);

    private static long Solve(ReadOnlySpan<RebootStep> steps)
    {
        Span<ulong> onEvents = stackalloc ulong[((steps.Length + 63) / 64)];
        Span<SweepEvent> xSweepEvents = stackalloc SweepEvent[steps.Length * 2];
        Span<SweepEvent> ySweepEvents = stackalloc SweepEvent[steps.Length * 2];
        Span<SweepEvent> zSweepEvents = stackalloc SweepEvent[steps.Length * 2];

        ulong currentIsOnValue = 0;
        for (short i = 0; i < steps.Length; i++)
        {
            RebootStep step = steps[i];
            xSweepEvents[i * 2] = new(step.X1, i);
            xSweepEvents[i * 2 + 1] = new(step.X2 + 1, i);
            ySweepEvents[i * 2] = new(step.Y1, i);
            ySweepEvents[i * 2 + 1] = new(step.Y2 + 1, i);
            zSweepEvents[i * 2] = new(step.Z1, i);
            zSweepEvents[i * 2 + 1] = new(step.Z2 + 1, i);

            if (step.IsOn)
                currentIsOnValue |= 1UL << (i % 64);

            if (i % 64 == 63)
            {
                onEvents[i / 64] = currentIsOnValue;
                currentIsOnValue = 0;
            }
        }

        if (steps.Length % 64 != 0)
            onEvents[onEvents.Length - 1] = currentIsOnValue;

        xSweepEvents.Sort();
        ySweepEvents.Sort();
        zSweepEvents.Sort();

        Span<SweepIndexes> ySweepIndexes = stackalloc SweepIndexes[steps.Length];
        PopulateSweepIndexes(ySweepEvents, ySweepIndexes);

        Span<SweepIndexes> zSweepIndexes = stackalloc SweepIndexes[steps.Length];
        PopulateSweepIndexes(zSweepEvents, zSweepIndexes);


        Span<ulong> yEvents = stackalloc ulong[(ySweepEvents.Length + 63) / 64];

        long totalVolume = 0;
        int lastX = int.MaxValue;
        foreach ((int x, short eventId) in xSweepEvents)
        {
            if (lastX < x)
                totalVolume += (x - lastX) * GetArea(onEvents, ySweepEvents, zSweepEvents, zSweepIndexes, yEvents);

            lastX = x;

            (short yStart, short yEnd) = ySweepIndexes[eventId];
            yEvents[yStart / 64] ^= 1UL << (yStart % 64);
            yEvents[yEnd / 64] ^= 1UL << (yEnd % 64);
        }

        return totalVolume;
    }

    private static void PopulateSweepIndexes(Span<SweepEvent> ySweepEvents, Span<SweepIndexes> sweepIndexes)
    {
        Span<short> firstIndexes = stackalloc short[sweepIndexes.Length];
        firstIndexes.Fill(-1);

        for (short i = 0; i < ySweepEvents.Length; i++)
        {
            SweepEvent sweepEvent = ySweepEvents[i];
            short firstIndex = firstIndexes[sweepEvent.EventId];
            if (firstIndex < 0)
                firstIndexes[sweepEvent.EventId] = i;
            else
                sweepIndexes[sweepEvent.EventId] = new(firstIndex, i);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long GetArea(ReadOnlySpan<ulong> onEvents, ReadOnlySpan<SweepEvent> ySweepEvents, ReadOnlySpan<SweepEvent> zSweepEvents, ReadOnlySpan<SweepIndexes> zSweepIndexes, ReadOnlySpan<ulong> yEvents)
    {
        Span<ulong> zEvents = stackalloc ulong[(zSweepEvents.Length + 63) / 64];

        long totalArea = 0;

        int lastY = int.MaxValue;
        for (int i = 0; i < yEvents.Length; i++)
        {
            ulong yEventBits = yEvents[i];
            int yEventIndexStart = i * 64;
            while (yEventBits != 0)
            {
                ulong t = yEventBits & (~yEventBits + 1UL);
                int yEventIndex = yEventIndexStart + BitOperations.TrailingZeroCount(t);
                yEventBits ^= t;

                (int y, short eventId) = ySweepEvents[yEventIndex];
                if (lastY < y)
                    totalArea += (y - lastY) * GetTotalLength(onEvents, zSweepEvents, zEvents);

                lastY = y;

                (short zStart, short zEnd) = zSweepIndexes[eventId];
                zEvents[zStart / 64] ^= 1UL << (zStart % 64);
                zEvents[zEnd / 64] ^= 1UL << (zEnd % 64);
            }
        }

        return totalArea;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long GetTotalLength(ReadOnlySpan<ulong> onEvents, ReadOnlySpan<SweepEvent> zSweepEvents, ReadOnlySpan<ulong> zEvents)
    {
        Span<ulong> enabledEvents = stackalloc ulong[onEvents.Length];

        long totalLength = 0;
        int lastZ = int.MaxValue;
        bool isOn = false;
        int maxEventId = -1;
        for (int i = 0; i < zEvents.Length; i++)
        {
            ulong zEventBits = zEvents[i];
            int zEventIndexStart = i * 64;
            while (zEventBits != 0)
            {
                ulong t = zEventBits & (~zEventBits + 1UL);
                int zEventIndex = zEventIndexStart + BitOperations.TrailingZeroCount(t);
                zEventBits ^= t;

                (int z, short eventId) = zSweepEvents[zEventIndex];
                if (lastZ < z && isOn)
                    totalLength += z - lastZ;

                lastZ = z;

                enabledEvents[eventId / 64] ^= 1UL << (eventId % 64);

                if (maxEventId < eventId)
                {
                    maxEventId = eventId;
                    isOn = ((1UL << (eventId % 64)) & onEvents[eventId / 64]) != 0;
                }
                else if (maxEventId == eventId)
                {
                    maxEventId = -1;
                    isOn = false;

                    for (int j = enabledEvents.Length - 1; j >= 0; j--)
                    {
                        ulong enabledEventsBits = enabledEvents[j];
                        if (enabledEventsBits != 0)
                        {
                            int highestEventBit = 63 - BitOperations.LeadingZeroCount(enabledEventsBits);
                            isOn = ((1UL << highestEventBit) & onEvents[j]) != 0;
                            maxEventId = highestEventBit + 64 * j;
                            break;
                        }
                    }
                }
            }
        }

        return totalLength;
    }

    private static RebootStep ParseRebootStep(ReadOnlySpan<byte> input, ref int inputIndex)
    {
        bool isOn = input[inputIndex + 1] == 'n';
        inputIndex += isOn ? "on x=".Length : "off x=".Length;

        int x1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        int x2 = ReadIntegerFromInput(input, ',', ref inputIndex);
        inputIndex += 2;

        int y1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        int y2 = ReadIntegerFromInput(input, ',', ref inputIndex);
        inputIndex += 2;

        int z1 = ReadIntegerFromInput(input, '.', ref inputIndex);
        inputIndex++;
        int z2 = ReadIntegerFromInput(input, '\n', ref inputIndex);

        return new(isOn, x1, x2, y1, y2, z1, z2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerFromInput(ReadOnlySpan<byte> span, char until, ref int i)
    {
        // Assume that the first character is always a digit
        byte c = span[i++];

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
