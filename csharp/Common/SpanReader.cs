using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common;

public ref struct SpanReader
{
    private ReadOnlySpan<byte> _input;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReader(ReadOnlySpan<byte> input)
    {
        _input = input;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read()
    {
        byte ret = _input[0];
        _input = _input.Slice(1);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        ReadOnlySpan<byte> ret = _input.Slice(0, length);
        _input = _input.Slice(length);
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipLength(int length)
    {
        _input = _input.Slice(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipWhile(char c) => SkipWhile((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipWhile(byte c)
    {
        _input = _input.TrimStart(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipUntil(char c) => SkipUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipUntil(byte c)
    {
        _input = _input.Slice(_input.IndexOf(c) + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(char c) => ReadUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(byte c)
    {
        int len = _input.IndexOf(c);
        if (len == -1)
        {
            ReadOnlySpan<byte> ret = _input;
            _input = ReadOnlySpan<byte>.Empty;
            return ret;
        }
        else
        {
            ReadOnlySpan<byte> ret = _input.Slice(0, len);
            _input = _input.Slice(len + 1);
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(ReadOnlySpan<byte> str)
    {
        int len = _input.IndexOf(str);
        if (len == -1)
        {
            ReadOnlySpan<byte> ret = _input;
            _input = ReadOnlySpan<byte>.Empty;
            return ret;
        }
        else
        {
            ReadOnlySpan<byte> ret = _input.Slice(0, len);
            _input = _input.Slice(len + str.Length);
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadIntUntil(char c) => ReadIntUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadIntUntil(byte c)
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
            byte cur = _input[i];
            if (cur == c)
            {
                _input = _input.Slice(i + 1);
                if (isNeg)
                    ret = -ret;
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = ReadOnlySpan<byte>.Empty;
        if (isNeg)
            ret = -ret;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPosIntUntil(char c) => ReadPosIntUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPosIntUntil(byte c)
    {
        // we assume the first char is always a digit
        int ret = _input[0] - '0';
        for (int i = 1; i < _input.Length; i++)
        {
            byte cur = _input[i];
            if (cur == c)
            {
                _input = _input.Slice(i + 1);
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = ReadOnlySpan<byte>.Empty;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPosIntUntilEnd()
    {
        // we assume the first char is always a digit
        int ret = _input[0] - '0';
        for (int i = 1; i < _input.Length; i++)
            ret = ret * 10 + (_input[i] - '0');

        _input = ReadOnlySpan<byte>.Empty;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadPosLongUntil(char c) => ReadPosLongUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadPosLongUntil(byte c)
    {
        // we assume the first char is always a digit
        long ret = _input[0] - '0';
        for (int i = 1; i < _input.Length; i++)
        {
            byte cur = _input[i];
            if (cur == c)
            {
                _input = _input.Slice(i + 1);
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = ReadOnlySpan<byte>.Empty;
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Peek() => _input[0];

    public readonly byte this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _input[i];
        }
    }

    public readonly bool Done
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _input.IsEmpty;
        }
    }
}
