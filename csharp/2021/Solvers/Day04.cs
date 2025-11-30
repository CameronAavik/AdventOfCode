using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day04 : ISolver
{
    // This solver assumes that there are no bingo numbers greater than 99, which is true of all the AoC inputs and examples.
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // Create a lookup table where the key is the bingo number and the value is what order it is called out.
        // For example, if the first number called out is 9, then orderIndexLookup[9] == 0.
        Span<byte> orderIndexLookup = stackalloc byte[100];

        // Represents the index into the input which we are currently parsing.
        var inputCursor = 0;

        // Parses the first line of the input and stores the order in the orderIndexLookup.
        ParseNumberOrderLine(input, orderIndexLookup, ref inputCursor);

        // Keep track of the earliest and latest bingos and what their scores were.
        var earliestBingo = int.MaxValue;
        var earliestBingoScore = 0;
        var latestBingo = 0;
        var latestBingoScore = 0;

        // Stores the order of the latest called out number in each column of a given board.
        Span<byte> colMaxs = stackalloc byte[5];

        // Stores the 25 bingo numbers of the current board.
        Span<byte> bingoNumbers = stackalloc byte[25];

        // Each iteration of the loop processes a single bingo board.
        while (inputCursor < input.Length)
        {
            // Stores the order of the earliest bingo among the rows of the board only
            var earliestRowBingo = int.MaxValue;

            // Skips a newline
            inputCursor++;

            // All boards are 5x5 as per the problem statement
            for (var row = 0; row < 5; row++)
            {
                // Stores the order of the latest called out number in the current row.
                var latestInRow = 0;

                for (var col = 0; col < 5; col++)
                {
                    // As we assume no bingo numbers are greater than 99, each number is represented using 2 characters
                    // with a space in the first character for single-digit numbers.
                    var digitOne = input[inputCursor++] switch { (byte)' ' => 0, byte c => c - '0' };
                    var digitTwo = input[inputCursor++] - '0';

                    // Skip the space or newline.
                    inputCursor++;

                    // Calculate the bingo number using the digits and the order it is called out
                    var value = digitOne * 10 + digitTwo;
                    var order = orderIndexLookup[value];

                    // Update latestInRow and colMaxs[col]
                    if (order > latestInRow)
                        latestInRow = order;

                    if (row == 0 || order > colMaxs[col])
                        colMaxs[col] = order;

                    bingoNumbers[row * 5 + col] = (byte)value;
                }

                if (latestInRow < earliestRowBingo)
                    earliestRowBingo = latestInRow;
            }

            // Take the min of all the columns and the earliest row.
            var bingo = Math.Min(Math.Min(Math.Min(colMaxs[0], colMaxs[1]), Math.Min(colMaxs[2], colMaxs[3])), Math.Min(colMaxs[4], earliestRowBingo));

            // Update the earliest and latest bingo values
            if (bingo < earliestBingo)
            {
                earliestBingo = bingo;
                earliestBingoScore = CalculateScore(orderIndexLookup, bingoNumbers, bingo);
            }

            if (bingo > latestBingo)
            {
                latestBingo = bingo;
                latestBingoScore = CalculateScore(orderIndexLookup, bingoNumbers, bingo);
            }
        }

        solution.SubmitPart1(earliestBingoScore);
        solution.SubmitPart2(latestBingoScore);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ParseNumberOrderLine(ReadOnlySpan<byte> input, Span<byte> orderIndexLookup, ref int inputCursor)
    {
        byte order = 0;
        while (true)
        {
            var digitOne = input[inputCursor++] - '0';
            var charTwo = input[inputCursor++];

            switch (charTwo)
            {
                case (byte)',':
                    orderIndexLookup[digitOne] = order;
                    break;
                case (byte)'\n':
                    orderIndexLookup[digitOne] = order;
                    return;
                default:
                    orderIndexLookup[digitOne * 10 + (charTwo - '0')] = order;
                    if (input[inputCursor++] == '\n')
                        return;
                    break;
            }

            order++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateScore(Span<byte> orderIndexLookup, Span<byte> bingoNumbers, int bingo)
    {
        var score = 0;
        var bingoNumber = 0;

        foreach (var number in bingoNumbers)
        {
            var order = orderIndexLookup[number];
            if (order > bingo)
            {
                score += number;
            }
            else if (order == bingo)
            {
                bingoNumber = number;
            }
        }

        return score * bingoNumber;
    }
}
