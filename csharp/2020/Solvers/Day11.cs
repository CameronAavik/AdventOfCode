using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day11 : ISolver
    {
        const short Occupied = 0;
        const short Finalised = 4;
        const short OccupiedAndFinalised = 8;

        private struct Seat
        {
            public short SeatData;
            public readonly byte DistanceNW;
            public readonly byte DistanceN;
            public readonly byte DistanceNE;
            public readonly byte DistanceW;
            public readonly byte DistanceE;
            public readonly byte DistanceSW;
            public readonly byte DistanceS;
            public readonly byte DistanceSE;

            public Seat(short seatData, byte distNW, byte distN, byte distNE, byte distW, byte distE, byte distSW, byte distS, byte distSE)
            {
                SeatData = seatData;
                DistanceNW = distNW;
                DistanceN = distN;
                DistanceNE = distNE;
                DistanceW = distW;
                DistanceE = distE;
                DistanceSW = distSW;
                DistanceS = distS;
                DistanceSE = distSE;
            }

            public void FlipOccupiedFlag()
            {
                SeatData ^= 1 << Occupied;
            }

            public void Finalise()
            {
                SeatData |= (short)(SeatData << (OccupiedAndFinalised - Occupied));
                SeatData |= 1 << Finalised;
            }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var cols = input.IndexOf('\n');
            var rows = input.Length / (cols + 1);

            // padding row and column is added
            var height = rows + 2;
            var width = cols + 2;
            var seatsPart1 = new Seat[height * width];
            MarkPaddingAsFinalised(seatsPart1, height, width);

            // store a set of all seats that are not finalised (we will be iterating over this)
            var activeSeatsPart1 = new HashSet<int>();

            // populate input into seats
            int seatsIndex = width + 1; // index in seats where input starts
            foreach (var c in input)
            {
                if (c == '\n')
                {
                    seatsIndex++; // skip a padding column
                }
                else if (c == '.')
                {
                    seatsPart1[seatsIndex].Finalise(); // floors start out as finalised
                }
                else
                {
                    activeSeatsPart1.Add(seatsIndex);
                }

                seatsIndex++;
            }

            // make a copy of the seats array and active seats since we can reuse it for part 2
            var seatsPart2 = new Seat[seatsPart1.Length];
            Array.Copy(seatsPart1, seatsPart2, seatsPart1.Length);
            var activeSeatsPart2 = new HashSet<int>(activeSeatsPart1, activeSeatsPart1.Comparer);

            foreach (var seat in activeSeatsPart1)
            {
                seatsPart1[seat] = new Seat(0, 1, 1, 1, 1, 1, 1, 1, 1); // all distances are 1 for part 1
            }

            int part1 = Solve(seatsPart1, width, 4, activeSeatsPart1);

            foreach (var seat in activeSeatsPart2)
            {
                seatsPart2[seat] = GetSeatWithDistsToNearestSeat(seatsPart2, seat, width, height);
            }

            int part2 = Solve(seatsPart2, width, 5, activeSeatsPart2);

            return new Solution(part1, part2);
        }

        private static void MarkPaddingAsFinalised(Seat[] seats, int height, int width)
        {
            // mark padding rows and columns as finalised
            for (int i = 0; i < seats.Length; i += width) // first col
                seats[i].Finalise();
            for (int i = width - 1; i < seats.Length; i += width) // last col
                seats[i].Finalise();
            for (int i = 0; i < width; i++) // first row
                seats[i].Finalise();
            for (int i = (height - 1) * width; i < seats.Length; i++) // last row
                seats[i].Finalise();
        }

        private static Seat GetSeatWithDistsToNearestSeat(Seat[] seats, int seat, int width, int height)
        {
            static byte GetDistance(Seat[] seats, int seat, int maxDist, int delta)
            {
                for (byte i = 1; i < maxDist; i++)
                {
                    seat += delta;
                    if ((seats[seat].SeatData & (1 << Finalised)) == 0)
                    {
                        return i;
                    }
                }
                return (byte)maxDist;
            }

            int rowsAbove = Math.DivRem(seat, width, out int colsLeft);
            int rowsBelow = height - rowsAbove - 1;
            int colsRight = width - colsLeft - 1;

            return new Seat(
                seatData: 0,
                distNW: GetDistance(seats, seat, Math.Min(rowsAbove, colsLeft), -width - 1),
                distN: GetDistance(seats, seat, rowsAbove, -width),
                distNE: GetDistance(seats, seat, Math.Min(rowsAbove, colsRight), -width + 1),
                distW: GetDistance(seats, seat, colsLeft, -1),
                distE: GetDistance(seats, seat, colsRight, 1),
                distSW: GetDistance(seats, seat, Math.Min(rowsBelow, colsLeft), width - 1),
                distS: GetDistance(seats, seat, rowsBelow, width),
                distSE: GetDistance(seats, seat, Math.Min(rowsBelow, colsRight), width + 1));
        }

        private static int Solve(Seat[] seats, int width, int neighboursForVacant, HashSet<int> activeSeats)
        {
            var seatsToFlip = new List<int>(activeSeats.Count);
            var seatsToFinalise = new List<int>(activeSeats.Count);
            while (true)
            {
                // DebugSeats(seats, width, seats.Length / width);

                foreach (var seat in activeSeats)
                {
                    var cur = seats[seat];
                    int totals = GetNeighbourTotals(seats, cur, width, seat);
                    int occupiedTotal = (totals >> Occupied) & 0xF;
                    int finalisedTotal = (totals >> Finalised) & 0xF;
                    int occAndFinalTotal = (totals >> OccupiedAndFinalised) & 0xF;

                    bool isSeatOccupied = (cur.SeatData & (1 << Occupied)) != 0;
                    if (isSeatOccupied)
                    {
                        if (occupiedTotal >= neighboursForVacant)
                        {
                            seatsToFlip.Add(seat);
                        }
                        else if (neighboursForVacant < finalisedTotal - occAndFinalTotal)
                        {
                            seatsToFinalise.Add(seat);
                        }
                    }
                    else
                    {
                        if (occupiedTotal == 0)
                        {
                            seatsToFlip.Add(seat);
                        }
                        else if (occAndFinalTotal >= 1 || finalisedTotal == 8)
                        {
                            seatsToFinalise.Add(seat);
                        }
                    }
                }

                // if there are no seats to flip, we have finished
                if (seatsToFlip.Count == 0)
                {
                    break;
                }

                foreach (var seat in seatsToFlip)
                {
                    seats[seat].FlipOccupiedFlag();
                }

                foreach (var seat in seatsToFinalise)
                {
                    activeSeats.Remove(seat);
                    seats[seat].Finalise();
                }

                seatsToFlip.Clear();
                seatsToFinalise.Clear();
            }

            return CountSeats(seats);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNeighbourTotals(Seat[] seats, Seat cur, int width, int seat)
        {
            return
                seats[seat - (width + 1) * cur.DistanceNW].SeatData +
                seats[seat - width * cur.DistanceN].SeatData +
                seats[seat - (width - 1) * cur.DistanceNE].SeatData +
                seats[seat - cur.DistanceW].SeatData +
                seats[seat + cur.DistanceE].SeatData +
                seats[seat + (width - 1) * cur.DistanceSW].SeatData +
                seats[seat + width * cur.DistanceS].SeatData +
                seats[seat + (width + 1) * cur.DistanceSE].SeatData;
        }

        private static int CountSeats(Seat[] grid)
        {
            int occupied = 0;
            foreach (var seat in grid)
            {
                occupied += seat.SeatData & (1 << Occupied);
            }

            return occupied;
        }

        private static void DebugSeats(Seat[] grid, int width, int height)
        {
            for (int row = 1; row < height - 1; row++)
            {
                for (int col = 1; col < width - 1; col++)
                {
                    var seat = grid[row * width + col].SeatData;
                    var isVacant = (seat & (1 << Occupied)) == 0;
                    var isFinalised = (seat & (1 << Finalised)) != 0;
                    Console.Write(isVacant ? (isFinalised ? '.' : 'L') : '#');
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
