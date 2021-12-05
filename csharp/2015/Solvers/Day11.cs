using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day11 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        ReadOnlySpan<char> part1 = GetNextPassword(input.TrimEnd('\n'));
        ReadOnlySpan<char> part2 = GetNextPassword(part1);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ReadOnlySpan<char> GetNextPassword(ReadOnlySpan<char> password)
    {
        // increment the password and ensure there are no confusing characters
        char[] newPassword = password.ToArray();
        IncrementPassword(newPassword);

        for (int i = 0; i < newPassword.Length; i++)
        {
            if (newPassword[i] is 'i' or 'o' or 'l')
            {
                newPassword[i]++;
            }
        }

        while (true)
        {
            int doubles = 0;
            bool hasTriple = false;

            bool ignoreDouble = false;

            char prev2 = '\0';
            char prev = '\0';
            foreach (char c in newPassword)
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
                return newPassword;
            }

            IncrementPassword(newPassword, doubles == 0 ? 3 : 1);
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
