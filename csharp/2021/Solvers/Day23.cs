using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day23 : ISolver
{
    const byte AmphipodAValue = 1 << 0;
    const byte AmphipodBValue = 1 << 1;
    const byte AmphipodCValue = 1 << 2;
    const byte AmphipodDValue = 1 << 3;

    interface IAmphipod { static abstract byte Value { get; } }
    readonly struct AmphipodA : IAmphipod { public static byte Value => AmphipodAValue; }
    readonly struct AmphipodB : IAmphipod { public static byte Value => AmphipodBValue; }
    readonly struct AmphipodC : IAmphipod { public static byte Value => AmphipodCValue; }
    readonly struct AmphipodD : IAmphipod { public static byte Value => AmphipodDValue; }

    readonly record struct Slot<TAmphipod>(ushort Data) where TAmphipod : IAmphipod
    {
        public Slot(byte a1, byte a2, byte a3, byte a4) : this((ushort)(a1 | (a2 << 4) | (a3 << 8) | (a4 << 12))) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slot<TAmphipod> Pop(out byte amphipod)
        {
            amphipod = (byte)(Data & 0xF);
            return new((ushort)(Data >> 4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slot<TAmphipod> Push() => new((ushort)(Data << 4 | TAmphipod.Value));

        public int MinimumCostToCorrectStack()
        {
            int totalCost = 0;
            bool needsToMove = false;
            for (int i = 3; i >= 0; i--)
            {
                byte amphipod = (byte)((Data >> (4 * i)) & 0xF);
                needsToMove = needsToMove || amphipod != TAmphipod.Value;
                if (needsToMove)
                {
                    int distanceBetweenSlots = (amphipod | TAmphipod.Value) switch
                    {
                        AmphipodAValue | AmphipodBValue or AmphipodBValue | AmphipodCValue or AmphipodCValue | AmphipodDValue => 2,
                        AmphipodAValue | AmphipodCValue or AmphipodBValue | AmphipodDValue => 4,
                        AmphipodAValue | AmphipodDValue => 6,
                        AmphipodAValue or AmphipodBValue or AmphipodCValue or AmphipodDValue or _ => 2, // We must move twice even if the slot is the same
                    };

                    // Cost to move incorrect amphipod to space above its correct slot
                    totalCost += (i + 1 + distanceBetweenSlots) * GetMoveCost(amphipod);

                    // Cost to move amphipod from above its slot into this position 
                    totalCost += (i + 1) * GetMoveCost(TAmphipod.Value);
                }
            }

            return totalCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsInvalidAmphipods()
        {
            ushort fullSlot = (ushort)(TAmphipod.Value | TAmphipod.Value << 4 | TAmphipod.Value << 8 | TAmphipod.Value << 12);
            return (Data & ~fullSlot) != 0;
        }
    }

    readonly record struct State(ulong TopRow, Slot<AmphipodA> SlotA, Slot<AmphipodB> SlotB, Slot<AmphipodC> SlotC, Slot<AmphipodD> SlotD)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoesSlotContainInvalid(byte amphipod) => amphipod switch
        {
            AmphipodAValue => SlotA.ContainsInvalidAmphipods(),
            AmphipodBValue => SlotB.ContainsInvalidAmphipods(),
            AmphipodCValue => SlotC.ContainsInvalidAmphipods(),
            AmphipodDValue or _ => SlotD.ContainsInvalidAmphipods(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State PopFromSlot(byte amphipod, out byte newAmphipod) => amphipod switch
        {
            AmphipodAValue => this with { SlotA = SlotA.Pop(out newAmphipod) },
            AmphipodBValue => this with { SlotB = SlotB.Pop(out newAmphipod) },
            AmphipodCValue => this with { SlotC = SlotC.Pop(out newAmphipod) },
            AmphipodDValue or _ => this with { SlotD = SlotD.Pop(out newAmphipod) },
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State PushIntoSlot(byte amphipod) => amphipod switch
        {
            AmphipodAValue => this with { SlotA = SlotA.Push() },
            AmphipodBValue => this with { SlotB = SlotB.Push() },
            AmphipodCValue => this with { SlotC = SlotC.Push() },
            AmphipodDValue or _ => this with { SlotD = SlotD.Push() },
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetMoveCost(byte AmphipodType) =>
        AmphipodType switch { AmphipodAValue => 1, AmphipodBValue => 10, AmphipodCValue => 100, AmphipodDValue or _ => 1000 };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetDestination(byte AmphipodType) =>
        AmphipodType switch { AmphipodAValue => 2, AmphipodBValue => 4, AmphipodCValue => 6, AmphipodDValue or _ => 8 };

    static bool IsPathClear(ulong topRow, int start, int destination)
    {
        static ulong GetPathMask(int from, int to) => ((1UL << ((to - from + 1) * 4)) - 1) << (from * 4);
        ulong pathMask = start < destination ? GetPathMask(start + 1, destination) : GetPathMask(destination, start - 1);
        return (pathMask & topRow) == 0;
    }

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        static byte CharToAmphipodType(byte c) => (byte)(1 << (c - 'A'));

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
            SlotA: new(slotA1, slotA2, AmphipodAValue, AmphipodAValue),
            SlotB: new(slotB1, slotB2, AmphipodBValue, AmphipodBValue),
            SlotC: new(slotC1, slotC2, AmphipodCValue, AmphipodCValue),
            SlotD: new(slotD1, slotD2, AmphipodDValue, AmphipodDValue));

        var part2InitialState = new State(
            TopRow: 0,
            SlotA: new(slotA1, AmphipodDValue, AmphipodDValue, slotA2),
            SlotB: new(slotB1, AmphipodCValue, AmphipodBValue, slotB2),
            SlotC: new(slotC1, AmphipodBValue, AmphipodAValue, slotC2),
            SlotD: new(slotD1, AmphipodAValue, AmphipodCValue, slotD2));

        int part1 = Solve(in part1InitialState);
        int part2 = Solve(in part2InitialState);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int Solve(in State initialState)
    {
        int minimumDistance =
            initialState.SlotA.MinimumCostToCorrectStack() +
            initialState.SlotB.MinimumCostToCorrectStack() +
            initialState.SlotC.MinimumCostToCorrectStack() +
            initialState.SlotD.MinimumCostToCorrectStack();

        var seen = new HashSet<State>();
        var pq = new PriorityQueue<State, int>();
        pq.Enqueue(initialState, minimumDistance);

        while (pq.TryDequeue(out State state, out int distance))
        {
            if (seen.Contains(state))
                continue;

            seen.Add(state);

            // Try see if any amphipods in the top row can go straight to their slot
            if (TryMoveAmphipodFromTopRowToSlot(in state, out State newState))
            {
                // If any amphipods moved into their slot, then there is no point considering further moves as it is an
                // optimal decision to make
                pq.Enqueue(newState, distance);
                continue;
            }

            ulong topRow = state.TopRow;
            bool isFinalState = true;
            for (byte amph = 1; amph <= AmphipodDValue; amph <<= 1)
            {
                if (state.DoesSlotContainInvalid(amph))
                {
                    isFinalState = false;

                    State stateAfterPopping = state.PopFromSlot(amph, out byte newAmphipod);

                    int start = GetDestination(amph);
                    int destination = GetDestination(newAmphipod);
                    if (!stateAfterPopping.DoesSlotContainInvalid(newAmphipod) && IsPathClear(topRow, start, destination))
                    {
                        pq.Enqueue(stateAfterPopping.PushIntoSlot(newAmphipod), distance);
                        break;
                    }

                    int moveCost = GetMoveCost(newAmphipod);

                    static bool IsSpotVacant(ulong topRow, int i) => (topRow & (0xFUL << (i * 4))) == 0;
                    static bool CanStopOnSpot(int i) => ((1U << i) & 0b00101010100U) == 0;

                    // We know that an amphipod can't stop directly outside it's spot, so we have already added moveCost * 2
                    // when determining the minimum distance, so we subtract it here to counteract that.
                    int newDistanceStart = distance + (amph == newAmphipod ? -moveCost * 2 : 0);

                    // Try move left
                    int newDistance = newDistanceStart;
                    for (int i = start - 1; i >= 0 && IsSpotVacant(topRow, i); i--)
                    {
                        if (i < destination)
                            newDistance += 2 * moveCost;

                        if (CanStopOnSpot(i))
                            pq.Enqueue(stateAfterPopping with { TopRow = topRow | ((ulong)newAmphipod << (i * 4)) }, newDistance);
                    }

                    // Try move right
                    newDistance = newDistanceStart;
                    for (int i = start + 1; i < 11 && IsSpotVacant(topRow, i); i++)
                    {
                        if (i > destination)
                            newDistance += 2 * moveCost;

                        if (CanStopOnSpot(i))
                            pq.Enqueue(stateAfterPopping with { TopRow = topRow | ((ulong)newAmphipod << (i * 4)) }, newDistance);
                    }
                }
            }

            if (isFinalState)
                return distance;
        }

        return -1;
    }

    private static bool TryMoveAmphipodFromTopRowToSlot(in State state, out State newState)
    {
        ulong topRow = state.TopRow;
        ulong t = topRow;
        int rowIndex = 0;
        while (t != 0)
        {
            byte amph = (byte)(t & 0xF);
            if (amph != 0 && !state.DoesSlotContainInvalid(amph) && IsPathClear(topRow, rowIndex, GetDestination(amph)))
            {
                ulong newTopRow = topRow ^ ((ulong)amph << (4 * rowIndex));
                newState = state.PushIntoSlot(amph) with { TopRow = newTopRow };
                return true;
            }

            t >>= 4;
            rowIndex++;
        }

        newState = default;
        return false;
    }
}
