using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

// This code is a mess, but it is a fast mess
// I will rewrite it to remove all the copy pasting I promise
public class Day23 : ISolver
{
    const byte AmphipodA = 1 << 0;
    const byte AmphipodB = 1 << 1;
    const byte AmphipodC = 1 << 2;
    const byte AmphipodD = 1 << 3;

    readonly record struct Slot(ushort Data)
    {
        public Slot(byte a1, byte a2, byte a3, byte a4)
            : this((ushort)(a1 | (a2 << 4) | (a3 << 8) | (a4 << 12)))
        {
        }

        public byte Amphipod1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (byte)(Data & 0xF);
            }
        }

        public byte Amphipod2 => (byte)((Data >> 4) & 0xF);
        public byte Amphipod3 => (byte)((Data >> 8) & 0xF);
        public byte Amphipod4 => (byte)((Data >> 12) & 0xF);

        public byte AmphipodsInSlot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                byte x = (byte)((Data >> 8) | (Data & 0xFF));
                return (byte)((x >> 4) | (x & 0xF));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slot Pop(out byte amphipod)
        {
            amphipod = Amphipod1;
            return new Slot((ushort)(Data >> 4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slot Push(byte amphipod)
        {
            return new Slot((ushort)(Data << 4 | amphipod));
        }

        public int MinimumCostToCorrectStack(byte ValidAmphipodType)
        {
            int totalCost = 0;

            bool needsToMove = Amphipod4 != ValidAmphipodType;
            if (needsToMove)
            {
                totalCost += (4 + GetDistanceBetweenSlots(ValidAmphipodType, Amphipod4)) * GetMoveCost(Amphipod4);
                totalCost += 4 * GetMoveCost(ValidAmphipodType);
            }

            needsToMove = needsToMove || Amphipod3 != ValidAmphipodType;
            if (needsToMove)
            {
                totalCost += (3 + GetDistanceBetweenSlots(ValidAmphipodType, Amphipod3)) * GetMoveCost(Amphipod3);
                totalCost += 3 * GetMoveCost(ValidAmphipodType);
            }

            needsToMove = needsToMove || Amphipod2 != ValidAmphipodType;
            if (needsToMove)
            {
                totalCost += (2 + GetDistanceBetweenSlots(ValidAmphipodType, Amphipod2)) * GetMoveCost(Amphipod2);
                totalCost += 2 * GetMoveCost(ValidAmphipodType);
            }

            needsToMove = needsToMove || Amphipod1 != ValidAmphipodType;
            if (needsToMove)
            {
                totalCost += (1 + GetDistanceBetweenSlots(ValidAmphipodType, Amphipod1)) * GetMoveCost(Amphipod1);
                totalCost += GetMoveCost(ValidAmphipodType);
            }

            return totalCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsInvalidAmphipods(byte AmphipodType) => (AmphipodsInSlot | AmphipodType) != AmphipodType;
    }

    readonly record struct State(ulong TopRow, Slot SlotA, Slot SlotB, Slot SlotC, Slot SlotD);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetMoveCost(byte AmphipodType) =>
        AmphipodType switch { AmphipodA => 1, AmphipodB => 10, AmphipodC => 100, AmphipodD => 1000, _ => 0 };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetDestination(byte AmphipodType) =>
        AmphipodType switch { AmphipodA => 2, AmphipodB => 4, AmphipodC => 6, AmphipodD => 8, _ => 0 };

    static int GetDistanceBetweenSlots(byte AmphipodType1, byte AmphipodType2) =>
        (AmphipodType1 | AmphipodType2) switch
        {
            AmphipodA | AmphipodB or AmphipodB | AmphipodC or AmphipodC | AmphipodD => 2,
            AmphipodA | AmphipodC or AmphipodB | AmphipodD => 4,
            AmphipodA | AmphipodD => 6,
            AmphipodA or AmphipodB or AmphipodC or AmphipodD or _ => 0,
        };

    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        static byte CharToAmphipodType(char c) => (byte)(1 << (c - 'A'));

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
        var part1InitialState = new State(
            TopRow: 0,
            SlotA: new Slot(slotA1, slotA2, AmphipodA, AmphipodA),
            SlotB: new Slot(slotB1, slotB2, AmphipodB, AmphipodB),
            SlotC: new Slot(slotC1, slotC2, AmphipodC, AmphipodC),
            SlotD: new Slot(slotD1, slotD2, AmphipodD, AmphipodD));

        var part2InitialState = new State(
            TopRow: 0,
            SlotA: new Slot(slotA1, AmphipodD, AmphipodD, slotA2),
            SlotB: new Slot(slotB1, AmphipodC, AmphipodB, slotB2),
            SlotC: new Slot(slotC1, AmphipodB, AmphipodA, slotC2),
            SlotD: new Slot(slotD1, AmphipodA, AmphipodC, slotD2));

        int part1 = Solve(part1InitialState);
        int part2 = Solve(part2InitialState);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int Solve(State initialState)
    {
        var minimumDistance =
            initialState.SlotA.MinimumCostToCorrectStack(AmphipodA) +
            initialState.SlotB.MinimumCostToCorrectStack(AmphipodB) +
            initialState.SlotC.MinimumCostToCorrectStack(AmphipodC) +
            initialState.SlotD.MinimumCostToCorrectStack(AmphipodD);

        var seen = new HashSet<State>();
        var pq = new PriorityQueue<State, int>();
        pq.Enqueue(initialState, minimumDistance);

        while (pq.TryDequeue(out State newState, out int distance))
        {
            if (seen.Contains(newState))
                continue;

            seen.Add(newState);

            (ulong topRow, Slot slotA, Slot slotB, Slot slotC, Slot slotD) = newState;

            bool slotAContainsInvalid = slotA.ContainsInvalidAmphipods(AmphipodA);
            bool slotBContainsInvalid = slotB.ContainsInvalidAmphipods(AmphipodB);
            bool slotCContainsInvalid = slotC.ContainsInvalidAmphipods(AmphipodC);
            bool slotDContainsInvalid = slotD.ContainsInvalidAmphipods(AmphipodD);

            bool isFinalState = true;
            if (topRow != 0)
            {
                isFinalState = false;

                ulong t = topRow;
                bool movedAmphipodIntoSlot = false;
                int rowIndex = 0;
                while (t != 0)
                {
                    byte amph = (byte)(t & 0xF);
                    switch (amph)
                    {
                        case AmphipodA:
                            if (!slotAContainsInvalid)
                            {
                                ulong path = 0xFFF ^ ((0x1UL << (4 * (rowIndex + (rowIndex < 2 ? 1 : 0)))) - 1);
                                bool isPathClear = (path & topRow) == 0;
                                if (isPathClear)
                                {
                                    Slot newSlot = slotA.Push(AmphipodA);
                                    ulong newTopRow = topRow ^ ((ulong)AmphipodA << (4 * rowIndex));
                                    pq.Enqueue(newState with { SlotA = newSlot, TopRow = newTopRow }, distance);
                                    movedAmphipodIntoSlot = true;
                                }
                            }
                            break;
                        case AmphipodB:
                            if (!slotB.ContainsInvalidAmphipods(AmphipodB))
                            {
                                ulong path = 0xFFFFF ^ ((0x1UL << (4 * (rowIndex + (rowIndex < 4 ? 1 : 0)))) - 1);
                                bool isPathClear = (path & topRow) == 0;
                                if (isPathClear)
                                {
                                    Slot newSlot = slotB.Push(AmphipodB);
                                    ulong newTopRow = topRow ^ ((ulong)AmphipodB << (4 * rowIndex));
                                    pq.Enqueue(newState with { SlotB = newSlot, TopRow = newTopRow }, distance);
                                    movedAmphipodIntoSlot = true;
                                }
                            }
                            break;
                        case AmphipodC:
                            if (!slotC.ContainsInvalidAmphipods(AmphipodC))
                            {
                                ulong path = 0xFFFFFFF ^ ((0x1UL << (4 * (rowIndex + (rowIndex < 6 ? 1 : 0)))) - 1);
                                bool isPathClear = (path & topRow) == 0;
                                if (isPathClear)
                                {
                                    Slot newSlot = slotC.Push(AmphipodC);
                                    ulong newTopRow = topRow ^ ((ulong)AmphipodC << (4 * rowIndex));
                                    pq.Enqueue(newState with { SlotC = newSlot, TopRow = newTopRow }, distance);
                                    movedAmphipodIntoSlot = true;
                                }
                            }
                            break;
                        case AmphipodD:
                            if (!slotD.ContainsInvalidAmphipods(AmphipodD))
                            {
                                ulong path = 0xFFFFFFFFF ^ ((0x1UL << (4 * (rowIndex + (rowIndex < 8 ? 1 : 0)))) - 1);
                                bool isPathClear = (path & topRow) == 0;
                                if (isPathClear)
                                {
                                    Slot newSlot = slotD.Push(AmphipodD);
                                    ulong newTopRow = topRow ^ ((ulong)AmphipodD << (4 * rowIndex));
                                    pq.Enqueue(newState with { SlotD = newSlot, TopRow = newTopRow }, distance);
                                    movedAmphipodIntoSlot = true;
                                }
                            }
                            break;
                    }

                    if (movedAmphipodIntoSlot)
                        break;

                    t >>= 4;
                    rowIndex++;
                }

                if (movedAmphipodIntoSlot)
                    continue;
            }

            if (slotAContainsInvalid)
            {
                isFinalState = false;

                Slot newSlot = slotA.Pop(out byte newAmphipod);
                int moveCost = GetMoveCost(newAmphipod);

                switch (newAmphipod)
                {
                    case AmphipodB:
                        if ((topRow & 0xF000) == 0 && !slotBContainsInvalid)
                        {
                            Slot newSlot2 = slotB.Push(AmphipodB);
                            pq.Enqueue(newState with { SlotA = newSlot, SlotB = newSlot2 }, distance);
                            continue;
                        }
                        break;
                    case AmphipodC:
                        if ((topRow & 0xFFF000) == 0 && !slotCContainsInvalid)
                        {
                            Slot newSlot2 = slotC.Push(AmphipodC);
                            pq.Enqueue(newState with { SlotA = newSlot, SlotC = newSlot2 }, distance);
                            continue;
                        }
                        break;
                    case AmphipodD:
                        if ((topRow & 0xFFFFF000) == 0 && !slotDContainsInvalid)
                        {
                            Slot newSlot2 = slotD.Push(AmphipodD);
                            pq.Enqueue(newState with { SlotA = newSlot, SlotD = newSlot2 }, distance);
                            continue;
                        }
                        break;
                }

                int start = 2;
                int destination = GetDestination(newAmphipod);

                int distanceToAdd = 0;
                for (int i = start - 1; i >= 0; i --)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i < destination)
                        distanceToAdd += 2 * moveCost;

                    ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                    pq.Enqueue(newState with { SlotA = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                }

                distanceToAdd = 0;
                for (int i = start + 1; i < 11; i ++)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i > destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not (4 or 6 or 8))
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotA = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }
            }

            if (slotBContainsInvalid)
            {
                isFinalState = false;

                Slot newSlot = slotB.Pop(out byte newAmphipod);
                int moveCost = GetMoveCost(newAmphipod);

                switch (newAmphipod)
                {
                    case AmphipodA:
                        if ((topRow & 0xF000) == 0 && !slotAContainsInvalid)
                        {
                            Slot newSlot2 = slotA.Push(AmphipodA);
                            pq.Enqueue(newState with { SlotB = newSlot, SlotA = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodC:
                        if ((topRow & 0xF00000) == 0 && !slotCContainsInvalid)
                        {
                            Slot newSlot2 = slotC.Push(AmphipodC);
                            pq.Enqueue(newState with { SlotB = newSlot, SlotC = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodD:
                        if ((topRow & 0xFFF00000) == 0 && !slotDContainsInvalid)
                        {
                            Slot newSlot2 = slotD.Push(AmphipodD);
                            pq.Enqueue(newState with { SlotB = newSlot, SlotD = newSlot2 }, distance);
                        }
                        break;
                }

                int start = 4;
                int destination = GetDestination(newAmphipod);

                int distanceToAdd = 0;
                for (int i = start - 1; i >= 0; i--)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i < destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not 2)
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotB = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }

                distanceToAdd = 0;
                for (int i = 5; i < 11; i++)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i > destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not (6 or 8))
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotB = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }
            }

            if (slotCContainsInvalid)
            {
                isFinalState = false;

                Slot newSlot = slotC.Pop(out byte newAmphipod);
                int moveCost = GetMoveCost(newAmphipod);

                switch (newAmphipod)
                {
                    case AmphipodA:
                        if ((topRow & 0xFFF000) == 0 && !slotAContainsInvalid)
                        {
                            Slot newSlot2 = slotA.Push(AmphipodA);
                            pq.Enqueue(newState with { SlotC = newSlot, SlotA = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodB:
                        if ((topRow & 0xF00000) == 0 && !slotBContainsInvalid)
                        {
                            Slot newSlot2 = slotB.Push(AmphipodB);
                            pq.Enqueue(newState with { SlotC = newSlot, SlotB = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodD:
                        if ((topRow & 0xF0000000) == 0 && !slotDContainsInvalid)
                        {
                            Slot newSlot2 = slotD.Push(AmphipodD);
                            pq.Enqueue(newState with { SlotC = newSlot, SlotD = newSlot2 }, distance);
                        }
                        break;
                }

                int start = 6;
                int destination = GetDestination(newAmphipod);

                int distanceToAdd = 0;
                for (int i = start - 1; i >= 0; i--)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i < destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not (2 or 4))
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotC = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }

                distanceToAdd = 0;
                for (int i = 7; i < 11; i++)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i > destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not 8)
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotC = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }
            }

            if (slotDContainsInvalid)
            {
                isFinalState = false;

                Slot newSlot = slotD.Pop(out byte newAmphipod);
                int moveCost = GetMoveCost(newAmphipod);

                switch (newAmphipod)
                {
                    case AmphipodA:
                        if ((topRow & 0xFFFFF000) == 0 && !slotAContainsInvalid)
                        {
                            Slot newSlot2 = slotA.Push(AmphipodA);
                            pq.Enqueue(newState with { SlotD = newSlot, SlotA = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodB:
                        if ((topRow & 0xFFF00000) == 0 && !slotBContainsInvalid)
                        {
                            Slot newSlot2 = slotB.Push(AmphipodB);
                            pq.Enqueue(newState with { SlotD = newSlot, SlotB = newSlot2 }, distance);
                        }
                        break;
                    case AmphipodC:
                        if ((topRow & 0xF0000000) == 0 && !slotCContainsInvalid)
                        {
                            Slot newSlot2 = slotC.Push(AmphipodC);
                            pq.Enqueue(newState with { SlotD = newSlot, SlotC = newSlot2 }, distance);
                        }
                        break;
                }

                int start = 8;
                int destination = GetDestination(newAmphipod);

                int distanceToAdd = 0;
                for (int i = start - 1; i >= 0; i--)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i < destination)
                        distanceToAdd += 2 * moveCost;

                    if (i is not (2 or 4 or 6))
                    {
                        ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                        pq.Enqueue(newState with { SlotD = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                    }
                }

                distanceToAdd = 0;
                for (int i = 9; i < 11; i++)
                {
                    if ((topRow & (0xFUL << (i * 4))) != 0)
                        break;

                    if (i > destination)
                        distanceToAdd += 2 * moveCost;

                    ulong newTopRow = topRow | ((ulong)newAmphipod << (i * 4));
                    pq.Enqueue(newState with { SlotD = newSlot, TopRow = newTopRow }, distance + distanceToAdd);
                }
            }

            if (isFinalState)
                return distance;
        }

        return 0;
    }
}
