using System;
using System.Linq;
using System.Text;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day04 : ISolver
{
    private static readonly byte[][] s_rotations =
        [.. Enumerable.Range(0, 26).Select(i => Encoding.ASCII.GetBytes(Rotate("north", -i)))];

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = -1;

        var letterCounts = new int[26];
        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];

            var nameLength = line.LastIndexOf((byte)'-');
            var name = line[..nameLength];
            var reader = new SpanReader(line[(nameLength + 1)..]);
            var sectorId = reader.ReadPosIntUntil('[');
            var checkSum = reader.ReadUntil(']');

            foreach (var c in name)
            {
                if (c != '-')
                {
                    letterCounts[c - 'a']++;
                }
            }

            if (IsChecksumCorrect(letterCounts, checkSum))
            {
                part1 += sectorId;
                if (part2 == -1 && name.IndexOf(s_rotations[sectorId % 26]) != -1)
                {
                    part2 = sectorId;
                }
            }

            Array.Clear(letterCounts, 0, 26);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static bool IsChecksumCorrect(int[] letterCounts, ReadOnlySpan<byte> checksum)
    {
        var prev = -1;
        var prevCount = int.MaxValue;
        foreach (var c in checksum)
        {
            var letter = c - 'a';
            var count = letterCounts[letter];

            if (prevCount < count || (prevCount == count && letter < prev))
            {
                return false;
            }

            prev = letter;
            prevCount = count;
            letterCounts[letter] = -1;
        }

        for (var i = 0; i < 26; i++)
        {
            var count = letterCounts[i];
            if (count > prevCount || (count == prevCount && i < prev))
            {
                return false;
            }
        }

        return true;
    }

    private static string Rotate(string str, int amount)
    {
        return string.Create(str.Length, str, (chars, str) =>
        {
            for (var i = 0; i < str.Length; i++)
            {
                chars[i] = (char)(((str[i] - 'a' + amount + 26) % 26) + 'a');
            }
        });
    }
}
