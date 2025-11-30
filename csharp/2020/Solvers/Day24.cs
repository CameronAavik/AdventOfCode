using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day24Grid
{
    // We store the grid using axial coordinates as described in:
    // https://www.redblobgames.com/grids/hexagons/#coordinates-axial

    // A (Q, R) coordinate is packed into a single integer
    const int Q = 1 << 0;
    const int R = 1 << 8;

    // All 6 neighbours can then be represented as offsets like so
    const int SE = R;
    const int SW = -Q + R;
    const int NE = Q - R;
    const int NW = -R;
    const int W = -Q;
    const int E = Q;

    // Each cell has 5 bits
    // - The low 4 bits store the count of active neighbours.
    // - The 5th bit stores the current state.
    const int CountMask = 0xF;
    const int StateBit = 4;
    const int StateMask = 1 << StateBit;

    // We store all the cells in a 1D array which is enough to fit any (Q, R) coordinate where Q, R are between 0 and 255.
    private readonly byte[] _cells = new byte[256 * 256];

    // To speed up the time spent on each step, we keep a bounding rectangle which contains the tiles that we should check.
    private int _minQ = int.MaxValue;
    private int _maxQ = int.MinValue;
    private int _minR = int.MaxValue;
    private int _maxR = int.MinValue;

    public int AliveCount { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Step()
    {
        var tilesToMakeAliveCount = 0;
        var tilesToMakeAlive = ArrayPool<int>.Shared.Rent(AliveCount * 8);

        var tilesToMakeDeadCount = 0;
        var tilesToMakeDead = ArrayPool<int>.Shared.Rent(AliveCount);

        // check all the tiles that are within the current bounds where flipped tiles exist
        for (var r = _minR; r <= _maxR; r += R)
        {
            for (var q = _minQ; q <= _maxQ; q += Q)
            {
                CheckTile(r | q);
            }
        }

        // check all the tiles just one step outside the bounds, and extend the bounds if necessary
        var newMinR = _minR - R;
        var newMaxR = _maxR + R;
        for (var q = _minQ; q <= _maxQ; q += Q)
        {
            if (CheckTileOutOfCurrentBounds(q | newMinR))
                _minR = newMinR;

            if (CheckTileOutOfCurrentBounds(q | newMaxR))
                _maxR = newMaxR;
        }

        var newMinQ = _minQ - Q;
        var newMaxQ = _maxQ + Q;
        for (var r = _minR; r <= _maxR; r += R)
        {
            if (CheckTileOutOfCurrentBounds(r | newMinQ))
                _minQ = newMinQ;

            if (CheckTileOutOfCurrentBounds(r | newMaxQ))
                _maxQ = newMaxQ;
        }

        for (var i = 0; i < tilesToMakeAliveCount; i++)
            BecomeAlive(tilesToMakeAlive[i]);

        for (var i = 0; i < tilesToMakeDeadCount; i++)
            BecomeDead(tilesToMakeDead[i]);

        ArrayPool<int>.Shared.Return(tilesToMakeAlive);
        ArrayPool<int>.Shared.Return(tilesToMakeDead);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckTile(int pos)
        {
            int cell = _cells[pos];

            var count = cell & CountMask;
            var state = (cell & StateMask) != 0;
            var newState = count == 2 | count == 1 & state;
            if (state & !newState)
            {
                tilesToMakeDead[tilesToMakeDeadCount++] = pos;
            }
            else if (!state & newState)
            {
                tilesToMakeAlive[tilesToMakeAliveCount++] = pos;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool CheckTileOutOfCurrentBounds(int pos)
        {
            // we know that the tile can't be flipped, so assume the state is false and it only flips
            // if the count is 2
            if ((_cells[pos] & CountMask) == 2)
            {
                tilesToMakeAlive[tilesToMakeAliveCount++] = pos;
                return true;
            }

            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BecomeAlive(int pos)
    {
        if ((_cells[pos] & StateMask) != 0)
            return;

        _cells[pos + SE]++;
        _cells[pos + SW]++;
        _cells[pos + NE]++;
        _cells[pos + NW]++;
        _cells[pos + W]++;
        _cells[pos + E]++;

        AliveCount++;
        _cells[pos] |= StateMask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BecomeDead(int pos)
    {
        if ((_cells[pos] & StateMask) == 0)
            return;

        _cells[pos + SE]--;
        _cells[pos + SW]--;
        _cells[pos + NE]--;
        _cells[pos + NW]--;
        _cells[pos + W]--;
        _cells[pos + E]--;

        AliveCount--;
        _cells[pos] &= unchecked((byte)~StateMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FlipTile(int q, int r)
    {
        r <<= 8;

        _minQ = Math.Min(q, _minQ);
        _maxQ = Math.Max(q, _maxQ);
        _minR = Math.Min(r, _minR);
        _maxR = Math.Max(r, _maxR);

        var pos = r | q;
        if ((_cells[pos] & StateMask) == 0)
            BecomeAlive(pos);
        else
            BecomeDead(pos);
    }
}

public class Day24 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var grid = new Day24Grid();

        PopulateInitialBlackTiles(input, grid);

        var part1 = grid.AliveCount;

        for (var i = 0; i < 100; i++)
        {
            grid.Step();
        }

        var part2 = grid.AliveCount;
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void PopulateInitialBlackTiles(ReadOnlySpan<byte> input, Day24Grid grid)
    {
        var q = 128;
        var r = 128;

        var i = 0;
        while (i < input.Length)
        {
            var c = input[i++];
            switch (c)
            {
                case (byte)'\n':
                    grid.FlipTile(q, r);
                    q = 128;
                    r = 128;
                    break;
                case (byte)'w':
                    q--;
                    break;
                case (byte)'e':
                    q++;
                    break;
                case (byte)'n':
                    r--;
                    if (input[i++] == 'e')
                        q++;
                    break;
                case (byte)'s':
                    r++;
                    if (input[i++] == 'w')
                        q--;
                    break;
            }
        }
    }
}
