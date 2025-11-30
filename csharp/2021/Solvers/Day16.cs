using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day16 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reader = new InputReader(input);
        uint part1 = 0;
        var part2 = ParsePacket(ref reader, ref part1);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ulong ParsePacket(ref InputReader reader, ref uint versionSum)
    {
        versionSum += reader.ReadBits(3);
        var typeId = reader.ReadBits(3);
        if (typeId == 4)
        {
            ulong number = 0;
            while (reader.ReadBit())
            {
                number <<= 4;
                number += reader.ReadBits(4);
            }

            number <<= 4;
            number |= reader.ReadBits(4);
            return number;
        }

        var lengthIsNumberOfSubPackets = reader.ReadBit();
        var length = (int)reader.ReadBits(lengthIsNumberOfSubPackets ? 11 : 15);
        switch (typeId)
        {
            case 0:
                ulong sum = 0;
                if (lengthIsNumberOfSubPackets)
                {
                    for (var i = 0; i < length; i++)
                        sum += ParsePacket(ref reader, ref versionSum);
                }
                else
                {
                    var finalIndex = reader.BitIndex + length;
                    while (reader.BitIndex < finalIndex)
                        sum += ParsePacket(ref reader, ref versionSum);
                }
                return sum;
            case 1:
                ulong product = 1;
                if (lengthIsNumberOfSubPackets)
                {
                    for (var i = 0; i < length; i++)
                        product *= ParsePacket(ref reader, ref versionSum);
                }
                else
                {
                    var finalIndex = reader.BitIndex + length;
                    while (reader.BitIndex < finalIndex)
                        product *= ParsePacket(ref reader, ref versionSum);
                }
                return product;
            case 2:
                var min = ulong.MaxValue;
                if (lengthIsNumberOfSubPackets)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var val = ParsePacket(ref reader, ref versionSum);
                        if (val < min)
                            min = val;
                    }
                }
                else
                {
                    var finalIndex = reader.BitIndex + length;
                    while (reader.BitIndex < finalIndex)
                    {
                        var val = ParsePacket(ref reader, ref versionSum);
                        if (val < min)
                            min = val;
                    }
                }
                return min;
            case 3:
                var max = ulong.MinValue;
                if (lengthIsNumberOfSubPackets)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var val = ParsePacket(ref reader, ref versionSum);
                        if (val > max)
                            max = val;
                    }
                }
                else
                {
                    var finalIndex = reader.BitIndex + length;
                    while (reader.BitIndex < finalIndex)
                    {
                        var val = ParsePacket(ref reader, ref versionSum);
                        if (val > max)
                            max = val;
                    }
                }
                return max;
            case 5:
                return ParsePacket(ref reader, ref versionSum) > ParsePacket(ref reader, ref versionSum) ? 1UL : 0UL;
            case 6:
                return ParsePacket(ref reader, ref versionSum) < ParsePacket(ref reader, ref versionSum) ? 1UL : 0UL;
            case 7:
                return ParsePacket(ref reader, ref versionSum) == ParsePacket(ref reader, ref versionSum) ? 1UL : 0UL;
        }

        return 0;
    }

    private ref struct InputReader(ReadOnlySpan<byte> input)
    {
        private readonly ReadOnlySpan<byte> _input = input;
        private int _inputIndex = 0;
        private int _readBitsCount = 0;
        private ulong _readBits = 0;

        public readonly int BitIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (_inputIndex * 4) - _readBitsCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBit()
        {
            bool result;
            if (_readBitsCount > 0)
            {
                result = (_readBits >> 63) != 0;
                _readBits <<= 1;
                _readBitsCount--;
            }
            else
            {
                var next4Bits = CharToInt(_input[_inputIndex++]);
                result = (next4Bits >> 3) != 0;

                _readBitsCount = 3;
                _readBits = next4Bits;

                ConsumeMoreInput();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadBits(int numBits)
        {
            uint number;
            if (numBits <= _readBitsCount)
            {
                number = (uint)(_readBits >> (64 - numBits));
                _readBits <<= numBits;
                _readBitsCount -= numBits;
            }
            else
            {
                number = (uint)(_readBits >> (64 - _readBitsCount));
                numBits -= _readBitsCount;
                while (numBits > 4)
                {
                    number <<= 4;
                    number |= CharToInt(_input[_inputIndex++]);
                    numBits -= 4;
                }

                _readBitsCount = 0;
                if (numBits > 0)
                {
                    var next4Bits = CharToInt(_input[_inputIndex++]);
                    number <<= numBits;
                    number |= (uint)next4Bits >> (4 - numBits);

                    _readBitsCount = 4 - numBits;
                    _readBits = next4Bits;
                }

                ConsumeMoreInput();
            }

            return number;
        }

        private void ConsumeMoreInput()
        {
            var amountToRead = Math.Min((64 - _readBitsCount) / 4, _input.Length - _inputIndex);
            for (var i = 0; i < amountToRead; i++)
            {
                _readBits <<= 4;
                _readBits |= CharToInt(_input[_inputIndex++]);
                _readBitsCount += 4;
            }

            _readBits <<= 64 - _readBitsCount;
        }

        private static byte CharToInt(byte c) => (byte)(c <= '9' ? c - '0' : c - 'A' + 10);
    }
}
