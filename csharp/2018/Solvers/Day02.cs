using AdventOfCode.CSharp.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.CSharp.Y2018.Solvers;

public class Day02 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int lineLength = input.IndexOf('\n') + 1;

        int part1 = SolvePart1(input, lineLength);
        string part2 = SolvePart2(input, lineLength);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int SolvePart1(ReadOnlySpan<char> input, int lineLength)
    {
        int idsWithPairs = 0;
        int idsWithTriples = 0;

        int lineOffset = 0;
        while (lineOffset < input.Length)
        {
            // maintain 4 bitsets that keep track of how many times we have seen a certain letter.
            // there are only 26 letters which fits in a 32 bit uint.
            uint seenMoreThanThrice = 0;
            uint seenThrice = 0;
            uint seenTwice = 0;
            uint seenOnce = 0;

            // loop over characters in ID
            for (int i = lineOffset; i < lineOffset + lineLength - 1; i++)
            {
                uint flag = 1U << (input[i] - 'a');
                seenMoreThanThrice |= seenThrice & flag;
                seenThrice |= seenTwice & flag;
                seenTwice |= seenOnce & flag;
                seenOnce |= flag;
            }

            // if there was a letter seen twice, but not thrice, then there is a pair
            if ((seenTwice & ~seenThrice) != 0)
            {
                idsWithPairs += 1;
            }

            // if there was a letter seen thrice, but not more than thrice, then there is a triple
            if ((seenThrice & ~seenMoreThanThrice) != 0)
            {
                idsWithTriples += 1;
            }

            lineOffset += lineLength;
        }

        return idsWithPairs * idsWithTriples;
    }

    private static string SolvePart2(ReadOnlySpan<char> input, int lineLength)
    {
        int idLength = lineLength - 1;
        int halfLength = idLength / 2;

        // get the hash of the first half of each ID and the offset where the line starts in the input
        var halfHashes = new (int Hash, int Offset)[input.Length / lineLength];
        for (int i = 0; i < halfHashes.Length; i++)
        {
            int offset = i * lineLength;
            int hash = string.GetHashCode(input.Slice(offset, halfLength));
            halfHashes[i] = (hash, offset);
        }

        // for all pairs with the same hash, see if they differ by one character
        // this will only happen if the answer to part 2 occurs when the differeing character is in the 2nd half
        if (TryFindPart2(input, idLength, halfHashes, out string? answer))
        {
            return answer;
        }

        // repeat the process but this time use the 2nd half of each ID
        for (int i = 0; i < halfHashes.Length; i++)
        {
            int offset = i * lineLength;
            int hash = string.GetHashCode(input.Slice(offset + halfLength, halfLength));
            halfHashes[i] = (hash, offset);
        }

        if (TryFindPart2(input, idLength, halfHashes, out answer))
        {
            return answer;
        }

        return "INVALID";
    }

    private static bool TryFindPart2(
        ReadOnlySpan<char> input,
        int idLength,
        (int Hash, int Offset)[] halfHashes,
        [NotNullWhen(returnValue: true)] out string? answer)
    {
        // sort by the hash
        Array.Sort(halfHashes, (l, r) => l.Hash.CompareTo(r.Hash));

        // for each pair of IDs that have the same hash, check if they differ by one character
        int hashStartIndex = 0;
        int curHash = halfHashes[0].Hash;
        for (int i = 1; i < halfHashes.Length; i++)
        {
            (int Hash, int Offset) = halfHashes[i];
            if (Hash == curHash)
            {
                ReadOnlySpan<char> curSpan = input.Slice(Offset, idLength);
                for (int j = hashStartIndex; j < i; j++)
                {
                    ReadOnlySpan<char> otherSpan = input.Slice(halfHashes[j].Offset, idLength);
                    int diffIndex = GetIndexOfDifferentChar(curSpan, otherSpan);
                    if (diffIndex >= 0)
                    {
                        answer = string.Concat(curSpan.Slice(0, diffIndex), otherSpan.Slice(diffIndex + 1));
                        return true;
                    }
                }
            }
            else
            {
                hashStartIndex = i;
                curHash = Hash;
            }
        }

        answer = default;
        return false;
    }

    /// <summary>
    /// Gets the index of the single character that is different between the two strings.
    /// If strings are the same, or there is more than one difference, then -1 is returned.
    /// </summary>
    private static int GetIndexOfDifferentChar(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        for (int k = 0; k < s1.Length; k++)
        {
            if (s1[k] != s2[k])
            {
                if (s1[(k + 1)..].SequenceEqual(s2[(k + 1)..]))
                {
                    return k;
                }

                break;
            }
        }

        return -1;
    }
}
