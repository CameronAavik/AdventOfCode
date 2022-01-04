using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day22 : ISolver
{
    const int MaxCardValue = 50;

    // 50 randomly generated numbers to use with buzhash
    private static readonly uint[] s_buzHashTable = new uint[MaxCardValue + 1]
    {
            119564030, 1443776239, 730079727, 637016013, 1609741144, 1628178104, 959411748, 277699449, 1095401195, 2141407359,
            1854243163, 1004465043, 470634801, 415392255, 1337243970, 517336758, 346301782, 1862853160, 1038920193, 1537224577,
            397161517, 415214760, 1927010737, 1563126965, 464456291, 1600929911, 432269658, 1937776014, 339596328, 941709070,
            1801487444, 666151923, 1222063719, 698053827, 737686581, 1185001291, 934247867, 327989044, 1178439112, 1417667540,
            514509190, 1930851386, 1488309314, 1865466052, 1522586288, 631476122, 289764488, 1651811359, 480085310, 2142599053,
            1870472761
    };

    class Deck
    {
        private readonly byte[] _data;
        private int _start;
        private int _end;
        private readonly byte _maxCard;
        private int _length;
        private uint _hash;

        public Deck()
        {
            _data = new byte[MaxCardValue + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Deck(Deck deck, int length)
        {
            _data = new byte[deck._data.Length];
            Array.Copy(deck._data, _data, _data.Length);

            _start = deck._start;
            _end = _start + length;
            if (_end >= _data.Length)
                _end -= _data.Length;

            _length = length;

            byte maxCard = 0;
            uint hash = 0;
            int j = _start;
            for (int i = 0; i < length; i++)
            {
                byte card = _data[j++];

                maxCard = Math.Max(card, maxCard);
                hash ^= BitOperations.RotateLeft(s_buzHashTable[card], i);

                if (j == _data.Length)
                    j = 0;
            }

            _maxCard = maxCard;
            _hash = hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte DrawCard()
        {
            byte card = _data[_start++];
            if (_start == _data.Length)
                _start = 0;
            _length--;
            _hash = BitOperations.RotateRight(_hash ^ s_buzHashTable[card], 1);
            return card;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlaceCard(byte card)
        {
            _data[_end++] = card;
            if (_end == _data.Length)
                _end = 0;
            _hash ^= BitOperations.RotateLeft(s_buzHashTable[card], _length);
            _length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetMaxCard() => _maxCard;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLength() => _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetHash() => _hash;

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

    readonly struct DeckState
    {
        private readonly uint _hash1;
        private readonly uint _hash2;

        public DeckState(Deck player1, Deck player2)
        {
            _hash1 = player1.GetHash();
            _hash2 = player2.GetHash();
        }

        public override bool Equals(object? obj) => obj is DeckState state && _hash1 == state._hash1 && _hash2 == state._hash2;
        public override int GetHashCode() => HashCode.Combine(_hash1, _hash2);
    }

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int player2Index = input.IndexOf(new[] { (byte)'\n', (byte)'\n' });
        ReadOnlySpan<byte> player1Input = input.Slice(0, player2Index + 1);
        ReadOnlySpan<byte> player2Input = input.Slice(player2Index + 2);

        var part1Deck1 = new Deck();
        var part1Deck2 = new Deck();

        ParsePlayerCards(player1Input, part1Deck1);
        ParsePlayerCards(player2Input, part1Deck2);

        // make a copy of the deck to use for part 2
        var part2Deck1 = new Deck(part1Deck1, part1Deck1.GetLength());
        var part2Deck2 = new Deck(part1Deck2, part1Deck2.GetLength());

        int part1 = Solve(part1Deck1, part1Deck2, isPart2: false);
        int part2 = Solve(part2Deck1, part2Deck2, isPart2: true);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParsePlayerCards(ReadOnlySpan<byte> input, Deck deck)
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
        while (deck1.GetLength() > 0 && deck2.GetLength() > 0)
        {
            byte card1 = deck1.DrawCard();
            byte card2 = deck2.DrawCard();

            int winner;
            if (isPart2 && card1 <= deck1.GetLength() && card2 <= deck2.GetLength())
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

        Deck winningDeck = deck1.GetLength() == 0 ? deck2 : deck1;
        return winningDeck.GetScore();
    }

    private static int RecursiveCombat(Deck deck1, Deck deck2)
    {
        if (deck1.GetMaxCard() > deck2.GetMaxCard())
            return 1;

        var seenStates = new HashSet<DeckState>();
        while (deck1.GetLength() > 0 && deck2.GetLength() > 0)
        {
            byte card1 = deck1.DrawCard();
            byte card2 = deck2.DrawCard();

            // check to see if we have seen this deck state before
            if (card2 == deck2.GetMaxCard() && !seenStates.Add(new DeckState(deck1, deck2)))
                return 1;

            int winner;
            if (card1 <= deck1.GetLength() && card2 <= deck2.GetLength())
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

        return deck1.GetLength() == 0 ? 2 : 1;
    }
}
