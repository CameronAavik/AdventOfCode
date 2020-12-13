using System;
using System.Collections.Generic;
using System.Linq;
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
            public readonly int NW;
            public readonly int N;
            public readonly int NE;
            public readonly int W;
            public readonly int E;
            public readonly int SW;
            public readonly int S;
            public readonly int SE;

            public Seat(short seatData, int nw, int n, int ne, int w, int e, int sw, int s, int se)
            {
                SeatData = seatData;
                NW = nw;
                N = n;
                NE = ne;
                W = w;
                E = e;
                SW = sw;
                S = s;
                SE = se;
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
            var activeSeats = new List<int>();

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
                    activeSeats.Add(seatsIndex);
                }

                seatsIndex++;
            }

            // make a copy of the seats array and active seats since we can reuse it for part 2
            var seatsPart2 = new Seat[seatsPart1.Length];
            Array.Copy(seatsPart1, seatsPart2, seatsPart1.Length);

            foreach (var seat in activeSeats)
            {
                seatsPart1[seat] = GetSeatPart1(seat, width);
            }

            int part1 = Solve(seatsPart1, width, 4, activeSeats);

            foreach (var seat in activeSeats)
            {
                seatsPart2[seat] = GetSeatPart2(seatsPart2, seat, width, height);
            }

            int part2 = Solve(seatsPart2, width, 5, activeSeats);

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

        private static Seat GetSeatPart1(int seat, int width)
        {
            int prev = seat - width;
            int next = seat + width;
            return new Seat(seatData: 0,
                nw: prev - 1,
                n: prev,
                ne: prev + 1,
                w: seat - 1,
                e: seat + 1,
                sw: next - 1,
                s: next,
                se: next + 1);
        }

        private static Seat GetSeatPart2(Seat[] seats, int seat, int width, int height)
        {
            static int GetNeighbour(Seat[] seats, int seat, int maxDist, int delta)
            {
                seat += delta;
                for (byte i = 1; i < maxDist; i++)
                {
                    if ((seats[seat].SeatData & (1 << Finalised)) == 0)
                    {
                        break;
                    }
                    seat += delta;
                }
                return seat;
            }

            int rowsAbove = Math.DivRem(seat, width, out int colsLeft);
            int rowsBelow = height - rowsAbove - 1;
            int colsRight = width - colsLeft - 1;

            return new Seat(
                seatData: 0,
                nw: GetNeighbour(seats, seat, Math.Min(rowsAbove, colsLeft), -width - 1),
                n: GetNeighbour(seats, seat, rowsAbove, -width),
                ne: GetNeighbour(seats, seat, Math.Min(rowsAbove, colsRight), -width + 1),
                w: GetNeighbour(seats, seat, colsLeft, -1),
                e: GetNeighbour(seats, seat, colsRight, 1),
                sw: GetNeighbour(seats, seat, Math.Min(rowsBelow, colsLeft), width - 1),
                s: GetNeighbour(seats, seat, rowsBelow, width),
                se: GetNeighbour(seats, seat, Math.Min(rowsBelow, colsRight), width + 1));
        }

        private static int Solve(Seat[] seats, int width, int neighboursForVacant, List<int> activeSeats)
        {
            var seatsToProcess = activeSeats.ToArray();
            var seatsToProcessLen = seatsToProcess.Length;
            var seatsToFlip = new int[activeSeats.Count];
            var seatsToFlipLen = 0;
            var seatsToFinalise = new int[activeSeats.Count];
            var seatsToFinaliseLen = 0;
            while (true)
            {
                //DebugSeats(seats, width, seats.Length / width);
                int newSeatsToProcessLen = 0;
                for (int i = 0; i < seatsToProcessLen; i++)
                {
                    var seat = seatsToProcess[i];
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
                            seatsToFlip[seatsToFlipLen++] = seat;
                        }
                        else if (neighboursForVacant < finalisedTotal - occAndFinalTotal)
                        {
                            seatsToFinalise[seatsToFinaliseLen++] = seat;
                            continue;
                        }
                    }
                    else
                    {
                        if (occupiedTotal == 0)
                        {
                            seatsToFlip[seatsToFlipLen++] = seat;
                        }
                        else if (occAndFinalTotal >= 1 || finalisedTotal == 8)
                        {
                            seatsToFinalise[seatsToFinaliseLen++] = seat;
                            continue;
                        }
                    }

                    seatsToProcess[newSeatsToProcessLen++] = seat;
                }

                // if there are no seats to flip, we have finished
                if (seatsToFlipLen == 0)
                {
                    break;
                }

                for (int i = 0; i < seatsToFlipLen; i++)
                {
                    seats[seatsToFlip[i]].FlipOccupiedFlag();
                }

                for (int i = 0; i < seatsToFinaliseLen; i++)
                {
                    seats[seatsToFinalise[i]].Finalise();
                }

                seatsToFlipLen = 0;
                seatsToFinaliseLen = 0;
                seatsToProcessLen = newSeatsToProcessLen;
            }

            return CountSeats(seats);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNeighbourTotals(Seat[] seats, Seat cur, int width, int seat)
        {
            return
                seats[cur.NW].SeatData +
                seats[cur.N].SeatData +
                seats[cur.NE].SeatData +
                seats[cur.W].SeatData +
                seats[cur.E].SeatData +
                seats[cur.SW].SeatData +
                seats[cur.S].SeatData +
                seats[cur.SE].SeatData;
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
