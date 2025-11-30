using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day21 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ParseInput(input, out var player1Start, out var player2Start);

        // make everything 0-based rather than 1-based
        player1Start--;
        player2Start--;

        var part1 = SolvePart1(player1Start, player2Start);
        solution.SubmitPart1(part1);

        var part2 = SolvePart2(player1Start, player2Start);
        solution.SubmitPart2(part2);
    }

    static int SolvePart1(int player1Start, int player2Start)
    {
        var player1Score = 0;
        var player2Score = 0;
        var rolls = 2;

        while (true)
        {
            player1Start = (player1Start + rolls * 3) % 10;
            rolls += 3;
            player1Score += player1Start + 1;
            if (player1Score >= 1000)
                return player2Score * (rolls - 2);

            player2Start = (player2Start + rolls * 3) % 10;
            rolls += 3;
            player2Score += player2Start + 1;
            if (player2Score >= 1000)
                return player1Score * (rolls - 2);
        }
    }

    [SkipLocalsInit]
    static long SolvePart2(byte player1Start, byte player2Start)
    {
        Span<(long Wins, long Losses)> m = stackalloc (long Wins, long Losses)[10 * 10 * 20 * 21];

        for (var combinedScore = 39; combinedScore >= 0; combinedScore--)
        {
            var minScore1 = Math.Max(combinedScore - 20, 0);
            var maxScore1 = Math.Min(19, combinedScore);

            for (var place1 = 0; place1 < 10; place1++)
            {
                var dpIndex = place1 * 10;
                for (var place2 = 0; place2 < 10; place2++)
                {
                    var dpIndex2 = (dpIndex + place2) * 20;
                    for (var score1 = minScore1; score1 <= maxScore1; score1++)
                    {
                        var score2 = combinedScore - score1;
                        long wins1 = 0;
                        long wins2 = 0;

                        AddRoll(3, 1, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(4, 3, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(5, 6, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(6, 7, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(7, 6, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(8, 3, place1, place2, score1, score2, ref wins1, ref wins2, m);
                        AddRoll(9, 1, place1, place2, score1, score2, ref wins1, ref wins2, m);

                        m[(dpIndex2 + score1) * 21 + score2] = (wins1, wins2);
                    }
                }
            }
        }

        (var player1Wins, var player2Wins) = m[(player1Start * 10 + player2Start) * 20 * 21];
        return Math.Max(player1Wins, player2Wins);
    }

    private static void AddRoll(int rolls, int ways, int place1, int place2, int score1, int score2, ref long wins1, ref long wins2, Span<(long Wins1, long Wins2)> m)
    {
        var newPlace = (place1 + rolls) % 10;

        var newScore = score1 + newPlace + 1;
        if (newScore >= 21)
        {
            wins1 += ways;
            return;
        }

        if (score2 == 20)
        {
            wins2 += ways * 27;
            return;
        }

        (var w1, var w2) = m[((place2 * 10 + newPlace) * 20 + score2) * 21 + newScore];
        wins1 += ways * w2;
        wins2 += ways * w1;
    }

    private static void ParseInput(ReadOnlySpan<byte> input, out byte Player1Start, out byte Player2Start)
    {
        var i = "Player 1 starting position: ".Length;
        Player1Start = (byte)(input[i++] - '0');
        if (Player1Start == 1 && input[i] == '0')
        {
            Player1Start = 10;
            i++;
        }

        i += "\nPlayer 2 starting position: ".Length;

        Player2Start = (byte)(input[i++] - '0');
        if (Player2Start == 1 && input[i] == '0')
        {
            Player2Start = 10;
        }
    }
}
