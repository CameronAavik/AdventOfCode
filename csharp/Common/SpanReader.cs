using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct SpanReader(ReadOnlySpan<byte> input)
{
    private ReadOnlySpan<byte> _input = input;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read()
    {
        var ret = _input[0];
        _input = _input[1..];
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        var ret = _input[..length];
        _input = _input[length..];
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipLength(int length)
    {
        _input = _input[length..];
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
        _input = _input[(_input.IndexOf(c) + 1)..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(char c) => ReadUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(byte c)
    {
        var len = _input.IndexOf(c);
        if (len == -1)
        {
            var ret = _input;
            _input = [];
            return ret;
        }
        else
        {
            var ret = _input[..len];
            _input = _input[(len + 1)..];
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadUntil(ReadOnlySpan<byte> str)
    {
        var len = _input.IndexOf(str);
        if (len == -1)
        {
            var ret = _input;
            _input = [];
            return ret;
        }
        else
        {
            var ret = _input[..len];
            _input = _input[(len + str.Length)..];
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadIntUntil(char c) => ReadIntUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadIntUntil(byte c)
    {
        var i = 0;
        var isNeg = false;
        if (_input[i] == '-')
        {
            isNeg = true;
            i++;
        }

        // we assume the first char is always a digit
        var ret = _input[i++] - '0';
        for (; i < _input.Length; i++)
        {
            var cur = _input[i];
            if (cur == c)
            {
                _input = _input[(i + 1)..];
                if (isNeg)
                    ret = -ret;
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = [];
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
        var ret = _input[0] - '0';
        for (var i = 1; i < _input.Length; i++)
        {
            var cur = _input[i];
            if (cur == c)
            {
                _input = _input[(i + 1)..];
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = [];
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadPosIntUntilEnd()
    {
        // we assume the first char is always a digit
        var ret = _input[0] - '0';
        for (var i = 1; i < _input.Length; i++)
            ret = ret * 10 + (_input[i] - '0');

        _input = [];
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadPosLongUntil(char c) => ReadPosLongUntil((byte)c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadPosLongUntil(byte c)
    {
        // we assume the first char is always a digit
        long ret = _input[0] - '0';
        for (var i = 1; i < _input.Length; i++)
        {
            var cur = _input[i];
            if (cur == c)
            {
                _input = _input[(i + 1)..];
                return ret;
            }

            ret = ret * 10 + (cur - '0');
        }

        _input = [];
        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly byte Peek() => _input[0];

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
