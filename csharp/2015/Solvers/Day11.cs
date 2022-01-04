using AdventOfCode.CSharp.Common;
using System;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day11 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ReadOnlySpan<byte> trimmed = input.TrimEnd((byte)'\n');
        Span<char> password = stackalloc char[trimmed.Length];
        Encoding.ASCII.GetChars(trimmed, password);

        GetNextPassword(password);
        solution.SubmitPart1(password);

        GetNextPassword(password);
        solution.SubmitPart2(password);
    }

    private static void GetNextPassword(Span<char> password)
    {
        // increment the password and ensure there are no confusing characters
        IncrementPassword(password);

        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] is 'i' or 'o' or 'l')
            {
                password[i]++;
            }
        }

        while (true)
        {
            int doubles = 0;
            bool hasTriple = false;

            bool ignoreDouble = false;

            char prev2 = '\0';
            char prev = '\0';
            foreach (char c in password)
            {
                if (c == prev && !ignoreDouble)
                {
                    doubles++;
                    ignoreDouble = true;
                }
                else
                {
                    ignoreDouble = false;
                }

                if (prev2 + 2 == c && prev + 1 == c)
                {
                    hasTriple = true;
                }

                prev2 = prev;
                prev = c;
            }

            if (doubles >= 2 && hasTriple)
            {
                return;
            }

            IncrementPassword(password, doubles == 0 ? 3 : 1);
        }
    }

    private static void IncrementPassword(Span<char> password, int minDigits = 1)
    {
        for (int i = password.Length - 1; i >= 0; i--)
        {
            ref char cur = ref password[i];
            if (cur == 'z' || minDigits > 1)
            {
                cur = 'a';
                minDigits--;
            }
            else
            {
                cur++;
                if (cur is 'i' or 'o' or 'l')
                {
                    cur++;
                }
                break;
            }
        }
    }
}
