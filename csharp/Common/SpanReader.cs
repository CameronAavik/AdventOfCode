using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common;

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
    public void SkipUntil(char c)
    {
        _input = _input.Slice(_input.IndexOf(c) + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> ReadUntil(char c)
    {
        int len = _input.IndexOf(c);
        if (len == -1)
        {
            ReadOnlySpan<char> ret = _input;
            _input = ReadOnlySpan<char>.Empty;
            return ret;
        }
        else
        {
            ReadOnlySpan<char> ret = _input.Slice(0, len);
            _input = _input.Slice(len + 1);
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> ReadUntil(ReadOnlySpan<char> str)
    {
        int len = _input.IndexOf(str);
        if (len == -1)
        {
            ReadOnlySpan<char> ret = _input;
            _input = ReadOnlySpan<char>.Empty;
            return ret;
        }
        else
        {
            ReadOnlySpan<char> ret = _input.Slice(0, len);
            _input = _input.Slice(len + str.Length);
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadIntUntil(char c)
    {
        int i = 0;
        bool isNeg = false;
        if (_input[i] == '-')
        {
            isNeg = true;
            i++;
        }

        // we assume the first char is always a digit
        int ret = _input[i++] - '0';
        for (; i < _input.Length; i++)
        {
            char cur = _input[i];
            if (cur == c)
            {
                _input = _input.Slice(i + 1);
                if (isNeg)
                    ret = -ret;
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = ReadOnlySpan<char>.Empty;
        if (isNeg)
            ret = -ret;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPosIntUntil(char c)
    {
        // we assume the first char is always a digit
        int ret = _input[0] - '0';
        for (int i = 1; i < _input.Length; i++)
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
    public long ReadPosLongUntil(char c)
    {
        // we assume the first char is always a digit
        long ret = _input[0] - '0';
        for (int i = 1; i < _input.Length; i++)
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

    public char this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _input[i];
        }
    }

    public bool Done
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _input.IsEmpty;
        }
    }
}
