using System;

namespace AdventOfCode.CSharp.Y2019.Common;

public static class IntCode
{
    public static ReadOnlySpan<int> ParseFromInput(ReadOnlySpan<byte> program)
    {
        // initialise an int array which the intcode will be read into.
        var code = new int[program.Length / 2 + 1];
        var size = 0;
        var isNegative = false;
        var n = 0;
        foreach (var c in program)
        {
            if (c == ',')
            {
                if (isNegative)
                {
                    n = -n;
                    isNegative = false;
                }
                code[size++] = n;
                n = 0;
            }
            else if (c == '-')
            {
                isNegative = true;
            }
            else
            {
                var digit = c - '0';
                n = n * 10 + digit;
            }
        }

        return new ReadOnlySpan<int>(code, 0, size);
    }
}
