using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day23 : ISolver
{
    const byte AmphipodA = 0;
    const byte AmphipodB = 1;
    const byte AmphipodC = 2;
    const byte AmphipodD = 3;

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        static byte CharToAmphipodType(byte c) => (byte)(c - 'A');

        const int InputRowWidth = 14;
        byte slotA1 = CharToAmphipodType(input[InputRowWidth * 2 + 3]);
        byte slotB1 = CharToAmphipodType(input[InputRowWidth * 2 + 5]);
        byte slotC1 = CharToAmphipodType(input[InputRowWidth * 2 + 7]);
        byte slotD1 = CharToAmphipodType(input[InputRowWidth * 2 + 9]);
        byte slotA2 = CharToAmphipodType(input[InputRowWidth * 3 + 3]);
        byte slotB2 = CharToAmphipodType(input[InputRowWidth * 3 + 5]);
        byte slotC2 = CharToAmphipodType(input[InputRowWidth * 3 + 7]);
        byte slotD2 = CharToAmphipodType(input[InputRowWidth * 3 + 9]);

        // Part 1 is simulated by assuming the bottom two slots are already filled with the correct amphipods
        ulong part1InitialState = 
            CreateSlot(AmphipodA, slotA1, slotA2, AmphipodA, AmphipodA) |
            ((uint)CreateSlot(AmphipodB, slotB1, slotB2, AmphipodB, AmphipodB) << 8) |
            ((uint)CreateSlot(AmphipodC, slotC1, slotC2, AmphipodC, AmphipodC) << 16) |
            ((uint)CreateSlot(AmphipodD, slotD1, slotD2, AmphipodD, AmphipodD) << 24);

        ulong part2InitialState =
            CreateSlot(AmphipodA, slotA1, AmphipodD, AmphipodD, slotA2) |
            ((uint)CreateSlot(AmphipodB, slotB1, AmphipodC, AmphipodB, slotB2) << 8) |
            ((uint)CreateSlot(AmphipodC, slotC1, AmphipodB, AmphipodA, slotC2) << 16) |
            ((uint)CreateSlot(AmphipodD, slotD1, AmphipodA, AmphipodC, slotD2) << 24);

        uint part1 = Solve(part1InitialState);
        uint part2 = Solve(part2InitialState);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static uint Solve(ulong initialState)
    {
        uint minimumCost = MinimumCost(initialState);

        var seen = new HashSet<ulong>(8192);
        var pq = new PriorityQueue<ulong, uint>(8192);
        pq.Enqueue(initialState, minimumCost * 16 + 16);

        while (pq.TryDequeue(out ulong state, out uint distance))
        {
            if (seen.Contains(state))
                continue;

            seen.Add(state);

            uint slots = (uint)(state & 0xFFFFFFFFU);
            uint topRow = (uint)(state >> 32);

            // Try see if any amphipods in the top row can go straight to their slot
            if (TryMoveAmphipodFromTopRowToSlot(slots, topRow, out ulong newState))
            {
                // If any amphipods moved into their slot, then there is no point considering further moves as it is an
                // optimal decision to make
                pq.Enqueue(newState, distance - 1);
                continue;
            }

            bool isFinalState = true;
            for (byte amph = AmphipodA; amph <= AmphipodD; amph++)
            {
                if (!CanMoveToSlot(amph, slots))
                {
                    isFinalState = false;

                    uint newSlots = PopFromSlot(amph, slots, out byte newAmphipod);

                    if (CanMoveToSlot(newAmphipod, newSlots) && IsPathFromSlotToSlotClear(topRow, amph, newAmphipod))
                    {
                        pq.Enqueue((ulong)topRow << 32 | newSlots, distance - 1);
                        break;
                    }

                    uint moveCost = 16 * GetMoveCost(newAmphipod);

                    // We know that an amphipod can't stop directly outside it's spot, so we have already added moveCost * 2
                    // when determining the minimum distance, so we subtract it here to counteract that.
                    uint newDistanceStart = (uint)(distance + (amph == newAmphipod ? -moveCost * 2 : 0));

                    // Try move left
                    uint newDistance = newDistanceStart;
                    for (int i = amph + 1; i >= 0 && ((topRow & (0xFU << (4 * i))) == 0); i--)
                    {
                        if (i < (newAmphipod + 2))
                            newDistance += (i != amph + 1 && i != newAmphipod + 1 && i > 0 ? 4U : 2U) * moveCost;

                        uint newTopRow = topRow | ((8U + newAmphipod) << (4 * i));
                        pq.Enqueue((ulong)newTopRow << 32 | newSlots, newDistance);
                    }

                    // Try move right
                    newDistance = newDistanceStart;
                    for (int i = amph + 2; i < 7 && ((topRow & (0xFU << (4 * i))) == 0); i++)
                    {
                        if (i > (newAmphipod + 1))
                            newDistance += (i != amph + 2 && i != newAmphipod + 2 && i < 6 ? 4U : 2U) * moveCost;

                        uint newTopRow = topRow | ((8U + newAmphipod) << (4 * i));
                        pq.Enqueue((ulong)newTopRow << 32 | newSlots, newDistance);
                    }
                }
            }

            if (isFinalState)
                return distance / 16;
        }

        return 0;
    }

    private static byte CreateSlot(byte expectedAmphipod, byte a1, byte a2, byte a3, byte a4)
    {
        a1 = (byte)((a1 + 4 - expectedAmphipod) & 3);
        a2 = (byte)((a2 + 4 - expectedAmphipod) & 3);
        a3 = (byte)((a3 + 4 - expectedAmphipod) & 3);
        a4 = (byte)((a4 + 4 - expectedAmphipod) & 3);
        return (byte)(a1 | (a2 << 2) | (a3 << 4) | (a4 << 6));
    }

    private static uint MinimumCost(ulong state)
    {
        int totalCost = 0;
        uint slots = (uint)(state & 0xFFFFFFFFU);
        for (byte expectedAmphipod = 0; expectedAmphipod < 4; expectedAmphipod++)
        {
            byte slot = (byte)(slots & 0xFFU);
            for (int j = 0; j < 4; j++)
            {
                if (slot == 0)
                    break;

                byte amphipod = (byte)((slot + expectedAmphipod) & 3);

                int distanceBetweenSlots =
                    amphipod == expectedAmphipod
                        ? 2 // We must move twice even if the slot is the same
                        : Math.Abs(amphipod - expectedAmphipod) * 2;

                // Cost to move incorrect amphipod to space above its correct slot
                totalCost += (j + 1 + distanceBetweenSlots) * (int)GetMoveCost(amphipod);

                // Cost to move amphipod from above its slot into this position 
                totalCost += (j + 1) * (int)GetMoveCost(expectedAmphipod);
                slot >>= 2;
            }

            slots >>= 8;
        }

        return (uint)totalCost;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanMoveToSlot(byte amphipod, uint slots) => (slots & (0xFFU << (8 * amphipod))) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint PopFromSlot(byte amphipod, uint slots, out byte newAmphipod)
    {
        int slotStart = 8 * amphipod;
        uint slot = (slots >> slotStart) & 0xFFU;
        newAmphipod = (byte)((slot + amphipod) & 3);
        uint slotMask = 0xFFU << slotStart;
        return ((slot >> 2) << slotStart) | (slots & ~slotMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint GetMoveCost(byte amph) => (uint)(((1UL | (10UL << 16) | (100UL << 32) | (1000UL << 48)) >> (amph * 16)) & 0xFFFF);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint GetTopRowMask(int length, int start) => ((1U << (4 * length)) - 1) << (4 * start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsPathFromTopToSlotClear(uint topRow, int start, byte slot)
    {
        int leftOfSlot = slot + 1;
        int diff = start - leftOfSlot;
        uint pathMask = diff <= 0 ? GetTopRowMask(-diff, start + 1) : GetTopRowMask(diff - 1, leftOfSlot + 1);
        return (pathMask & topRow) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsPathFromSlotToSlotClear(uint topRow, byte slot1, byte slot2)
    {
        int diff = slot1 - slot2;
        uint pathMask = diff < 0 ? GetTopRowMask(-diff, slot1 + 2) : GetTopRowMask(diff, slot2 + 2);
        return (pathMask & topRow) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryMoveAmphipodFromTopRowToSlot(uint slots, uint topRow, out ulong newState)
    {
        uint t = topRow;
        int rowIndex = 0;
        while (t != 0)
        {
            uint cell = t & 0xF;
            if (cell != 0)
            {
                byte amph = (byte)(cell & 3);
                if (CanMoveToSlot(amph, slots) && IsPathFromTopToSlotClear(topRow, rowIndex, amph))
                {
                    uint newTopRow = topRow ^ (cell << (rowIndex * 4));
                    newState = ((ulong)newTopRow) << 32 | slots;
                    return true;
                }
            }

            t >>= 4;
            rowIndex++;
        }

        newState = default;
        return false;
    }
}
