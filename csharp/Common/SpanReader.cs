﻿using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common
{
    public ref struct SpanReader
    {
        private ReadOnlySpan<char> _input;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanReader(ReadOnlySpan<char> input)
        {
            _input = input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipLength(int length)
        {
            _input = _input.Slice(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadUntil(char c)
        {
            int len = _input.IndexOf(c);
            if (len == -1)
            {
                var ret = _input;
                _input = ReadOnlySpan<char>.Empty;
                return ret;
            }
            else
            {
                var ret = _input.Slice(0, len);
                _input = _input.Slice(len + 1);
                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadPosIntUntil(char c)
        {
            int ret = 0;
            for (int i = 0; i < _input.Length; i++)
            {
                char cur = _input[i];
                if (cur == c)
                {
                    _input = _input.Slice(i + 1);
                    return ret;
                }

                ret = ret * 10 + (cur - '0');
            }

            _input = ReadOnlySpan<char>.Empty;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Peek() => _input[0];

        public bool Done
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _input.IsEmpty;
            }
        }
    }
}