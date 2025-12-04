using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

/// <summary>
/// This solution treats the grid of paper rolls as a graph where each '@' cell is a vertex connected to its eight
/// neighbours, and uses an 8-bit mask per cell to track which of its 8 neighbours are still present. For part 2,
/// the problem is essentially finding all k-cores of the graph where k=4. A k-core is a subgraph where every vertex
/// has at least k connections to other vertices in the subgraph. The algorithm works by maintaining a stack of all
/// cells that have fewer than 4 neighbours. Each time a cell is removed from the graph, the neighbour masks of its
/// neighbours are updated to remove the edge, and if removing an edge causes a neighbour to drop below 4 connections,
/// it will be added to the stack. Once the stack is empty, all remaining cells in the graph will be in a 4-core.
/// </summary>
public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var height = input.Length / (width + 1);

        // Padded neighbour masks
        var paddedWidth = width + 2;
        var paddedHeight = height + 2;
        Span<byte> cellNeighboursBitsets = new byte[paddedWidth * paddedHeight];

        // Directions (0 - 7): NW, N, NE, W, E, SW, S, SE
        // Returns what to add to an index in cellNeighboursBitsets to get the neighbour in that direction
        ReadOnlySpan<int> directionDeltas = [-paddedWidth - 1, -paddedWidth, -paddedWidth + 1, -1, 1, paddedWidth - 1, paddedWidth, paddedWidth + 1];

        Span<int> paperRolls = new int[width * height];
        var numPaperRolls = 0;
        for (var row = 0; row < height; row++)
        {
            var inputRowStart = row * (width + 1);
            var maskRowStart = (row + 1) * paddedWidth + 1;
            for (var col = 0; col < width; col++)
            {
                if (input[inputRowStart + col] == '@')
                {
                    var maskIndex = maskRowStart + col;
                    for (var i = 0; i < 8; i++)
                    {
                        var inverseNeighbourBit = (byte)(1 << (7 - i)); // e.g converts NE to SW
                        cellNeighboursBitsets[maskIndex + directionDeltas[i]] |= inverseNeighbourBit;
                    }

                    paperRolls[numPaperRolls++] = maskIndex;
                }
            }
        }

        var accessibleStack = paperRolls; // Reuse paperRolls array to avoid extra allocations
        var stackPtr = 0;
        for (var i = 0; i < numPaperRolls; i++)
        {
            var cellMaskIndex = paperRolls[i];
            if (BitOperations.PopCount(cellNeighboursBitsets[cellMaskIndex]) < 4)
                accessibleStack[stackPtr++] = cellMaskIndex;
        }

        solution.SubmitPart1(stackPtr);

        var part2 = 0;
        while (stackPtr > 0)
        {
            part2++;
            var cellMaskIndex = accessibleStack[--stackPtr];
            var cellNeighbours = cellNeighboursBitsets[cellMaskIndex];

            // iterate over set bits using https://lemire.me/blog/2018/02/21/iterating-over-set-bits-quickly/
            while (cellNeighbours != 0)
            {
                var neighbourBit = cellNeighbours & (~cellNeighbours + 1);
                var direction = BitOperations.TrailingZeroCount(neighbourBit);
                var inverseNeighbourBit = (byte)(1 << (7 - direction));

                var neighbourIndex = cellMaskIndex + directionDeltas[direction];
                var neighbourNeighbours = cellNeighboursBitsets[neighbourIndex] ^= inverseNeighbourBit;
                if (BitOperations.PopCount(neighbourNeighbours) == 3)
                    accessibleStack[stackPtr++] = neighbourIndex;

                cellNeighbours ^= (byte)neighbourBit;
            }
        }

        solution.SubmitPart2(part2);
    }
}
