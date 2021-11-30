using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day18 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        long part1 = 0;
        long part2 = 0;

        // initialise a stack and stack pointer for both parts
        // we put -1 on top of the stack to represent a bad value
        long[] stack1 = new long[64];
        stack1[0] = -1;
        int sp1 = 0;

        long[] stack2 = new long[128];
        stack2[0] = -1;
        int sp2 = 0;

        foreach (char c in input)
        {
            if (c == '\n')
            {
                // for part 1 the stack will only have one item which is the answer
                part1 += stack1[1];

                // for part 2 the stack will looks like [-1, n1, *, n2, *, n3, * n4]
                // so multiply every second number
                long p2 = 1;
                for (int i = 1; i <= sp2; i += 2)
                    p2 *= stack2[i];

                part2 += p2;

                // reset the stack pointers
                sp1 = 0;
                sp2 = 0;
            }
            else if (c != ' ')
            {
                ProcessCharPart1(c, ref stack1, ref sp1);
                ProcessCharPart2(c, ref stack2, ref sp2);
            }
        }

        return new Solution(part1.ToString(), part2.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProcessCharPart1(char c, ref long[] stack, ref int sp)
    {
        long n;
        if (c == ')')
        {
            n = stack[sp--];
            sp--; // pop the '(' off the stack
        }
        else
        {
            n = c - '0';
        }

        if (n >= 0)
        {
            switch (stack[sp])
            {
                case '+' - '0':
                    sp--; // pop the '+' off the stack
                    stack[sp] += n;
                    break;
                case '*' - '0':
                    sp--; // pop the '*' off the stack
                    stack[sp] *= n;
                    break;
                default:
                    stack[++sp] = n;
                    break;
            }
        }
        else
        {
            stack[++sp] = n;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProcessCharPart2(char c, ref long[] stack, ref int sp)
    {
        long n;
        if (c == ')')
        {
            n = stack[sp--];
            while (stack[sp--] != '(' - '0')
            {
                n *= stack[sp--];
            }
        }
        else
        {
            n = c - '0';
        }

        if (n >= 0 && stack[sp] == '+' - '0')
        {
            stack[--sp] += n;
        }
        else
        {
            stack[++sp] = n;
        }
    }
}
