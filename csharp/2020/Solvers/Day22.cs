using System;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day22 : ISolver
    {
        struct Deck
        {
            private byte[] _data;
            private int _start;
            private int _end;
            private int _len;

            public Deck(int capacity)
            {
                _data = new byte[capacity];
                _start = 0;
                _end = 0;
                _len = 0;
            }

            public Deck(Deck deck, int length)
            {
                _data = new byte[deck._data.Length];
                Array.Copy(deck._data, _data, _data.Length);

                _start = deck._start;
                _end = _start + length;
                if (_end >= _data.Length)
                    _end -= _data.Length;
                _len = length;
            }

            public byte DrawCard()
            {
                byte card = _data[_start++];
                if (_start >= _data.Length)
                    _start = 0;
                _len--;
                return card;
            }

            public void PlaceCard(byte card)
            {
                _data[_end++] = card;
                if (_end >= _data.Length)
                    _end = 0;
                _len++;
            }

            public int Length => _len;

            public bool IsEmpty => _len == 0;

            public int GetScore()
            {
                int score = 0;
                int multiplier = 1;
                if (_end < _start)
                {
                    for (int i = _end - 1; i >= 0; i--)
                    {
                        score += multiplier++ * _data[i];
                    }

                    _end = _data.Length;
                }

                for (int i = _end - 1; i >= _start; i--)
                {
                    score += multiplier++ * _data[i];
                }

                return score;
            }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            int player2Index = input.IndexOf("\n\n");
            ReadOnlySpan<char> player1Input = input.Slice(0, player2Index + 1);
            ReadOnlySpan<char> player2Input = input.Slice(player2Index + 2);

            int totalCards = player1Input.Count('\n') - 1 + player2Input.Count('\n') - 1;
            var part1Deck1 = new Deck(totalCards + 1);
            var part1Deck2 = new Deck(totalCards + 1);

            ParsePlayerCards(player1Input, ref part1Deck1);
            ParsePlayerCards(player2Input, ref part1Deck2);

            // make a copy of the deck to use for part 2
            var part2Deck1 = new Deck(part1Deck1, part1Deck1.Length);
            var part2Deck2 = new Deck(part1Deck2, part1Deck2.Length);

            int part1 = Solve(part1Deck1, part1Deck2, isPart2: false);
            int part2 = Solve(part2Deck1, part2Deck2, isPart2: true);

            return new Solution(part1, part2);
        }

        private static void ParsePlayerCards(ReadOnlySpan<char> input, ref Deck deck)
        {
            var reader = new SpanReader(input);
            reader.SkipLength("Player 1:\n".Length);
            while (!reader.Done)
            {
                byte card = (byte)reader.ReadPosIntUntil('\n');
                deck.PlaceCard(card);
            }
        }

        private static int Solve(Deck deck1, Deck deck2, bool isPart2)
        {
            while (!deck1.IsEmpty && !deck2.IsEmpty)
            {
                byte card1 = deck1.DrawCard();
                byte card2 = deck2.DrawCard();

                int winner;
                if (isPart2 && card1 <= deck1.Length && card1 <= deck2.Length)
                {
                    winner = RecursiveCombat(new Deck(deck1, card1), new Deck(deck2, card2));
                }
                else if (card1 < card2)
                {
                    winner = 2;
                }
                else
                {
                    winner = 1;
                }

                if (winner == 1)
                {
                    deck1.PlaceCard(card1);
                    deck1.PlaceCard(card2);
                }
                else
                {
                    deck2.PlaceCard(card2);
                    deck2.PlaceCard(card1);
                }
            }

            Deck winningDeck = deck1.IsEmpty ? deck2 : deck1;
            return winningDeck.GetScore();
        }

        private static int RecursiveCombat(Deck deck1, Deck deck2)
        {
            return 1;
        }
    }
}
