using System;

namespace AdventOfCode.CSharp.Y2019.Common
{
    public static class IntCode
    {
        public static ReadOnlySpan<int> ParseFromInput(ReadOnlySpan<char> program)
        {
            // initialise an int array which the intcode will be read into.
            int[] code = new int[program.Length / 2 + 1];
            int size = 0;
            bool isNegative = false;
            int n = 0;
            foreach (char c in program)
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
                    int digit = c - '0';
                    n = n * 10 + digit;
                }
            }

            return new ReadOnlySpan<int>(code, 0, size);
        }
    }
}
