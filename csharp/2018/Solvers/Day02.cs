using AdventOfCode.CSharp.Common;
using System;
using System.Text;

namespace AdventOfCode.CSharp.Y2018.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lineLength = input.IndexOf((byte)'\n') + 1;

        var part1 = SolvePart1(input, lineLength);

        Span<char> solutionBuffer = stackalloc char[lineLength - 2];
        SolvePart2(input, lineLength, solutionBuffer);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(solutionBuffer);
    }

    private static int SolvePart1(ReadOnlySpan<byte> input, int lineLength)
    {
        var idsWithPairs = 0;
        var idsWithTriples = 0;

        var lineOffset = 0;
        while (lineOffset < input.Length)
        {
            // maintain 4 bitsets that keep track of how many times we have seen a certain letter.
            // there are only 26 letters which fits in a 32 bit uint.
            uint seenMoreThanThrice = 0;
            uint seenThrice = 0;
            uint seenTwice = 0;
            uint seenOnce = 0;

            // loop over characters in ID
            for (var i = lineOffset; i < lineOffset + lineLength - 1; i++)
            {
                var flag = 1U << (input[i] - 'a');
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

    private static void SolvePart2(ReadOnlySpan<byte> input, int lineLength, Span<char> solutionBuffer)
    {
        var idLength = lineLength - 1;
        var halfLength = idLength / 2;

        // get the hash of the first half of each ID and the offset where the line starts in the input
        var halfHashes = new (int Hash, int Offset)[input.Length / lineLength];
        for (var i = 0; i < halfHashes.Length; i++)
        {
            var offset = i * lineLength;
            var hash = HashBytes(input.Slice(offset, halfLength));
            halfHashes[i] = (hash, offset);
        }

        // for all pairs with the same hash, see if they differ by one character
        // this will only happen if the answer to part 2 occurs when the differing character is in the 2nd half
        if (TryFindPart2(input, idLength, halfHashes, solutionBuffer))
            return;

        // repeat the process but this time use the 2nd half of each ID
        for (var i = 0; i < halfHashes.Length; i++)
        {
            var offset = i * lineLength;
            var hash = HashBytes(input.Slice(offset + halfLength, halfLength));
            halfHashes[i] = (hash, offset);
        }

        if (TryFindPart2(input, idLength, halfHashes, solutionBuffer))
            return;

        "INVALID\n".AsSpan().CopyTo(solutionBuffer);
    }

    private static bool TryFindPart2(ReadOnlySpan<byte> input, int idLength, (int Hash, int Offset)[] halfHashes, Span<char> solutionBuffer)
    {
        // sort by the hash
        Array.Sort(halfHashes, (l, r) => l.Hash.CompareTo(r.Hash));

        // for each pair of IDs that have the same hash, check if they differ by one character
        var hashStartIndex = 0;
        var curHash = halfHashes[0].Hash;
        for (var i = 1; i < halfHashes.Length; i++)
        {
            (var Hash, var Offset) = halfHashes[i];
            if (Hash == curHash)
            {
                var curSpan = input.Slice(Offset, idLength);
                for (var j = hashStartIndex; j < i; j++)
                {
                    var otherSpan = input.Slice(halfHashes[j].Offset, idLength);
                    var diffIndex = GetIndexOfDifferentChar(curSpan, otherSpan);
                    if (diffIndex >= 0)
                    {
                        Encoding.ASCII.GetChars(curSpan[..diffIndex], solutionBuffer);
                        Encoding.ASCII.GetChars(otherSpan[(diffIndex + 1)..], solutionBuffer[diffIndex..]);
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

        return false;
    }

    /// <summary>
    /// Gets the index of the single character that is different between the two strings.
    /// If strings are the same, or there is more than one difference, then -1 is returned.
    /// </summary>
    private static int GetIndexOfDifferentChar(ReadOnlySpan<byte> s1, ReadOnlySpan<byte> s2)
    {
        for (var k = 0; k < s1.Length; k++)
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

    private static int HashBytes(ReadOnlySpan<byte> bytes)
    {
        var h = new HashCode();
        foreach (var b in bytes)
            h.Add(b);

        return h.ToHashCode();
    }
}
