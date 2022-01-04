using System;
using System.Linq;
using System.Text;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day04 : ISolver
{
    private static readonly byte[][] s_rotations =
        Enumerable.Range(0, 26).Select(i => Encoding.ASCII.GetBytes(Rotate("north", -i))).ToArray();

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = -1;

        int[] letterCounts = new int[26];
        foreach (ReadOnlySpan<byte> line in input.SplitLines())
        {
            int nameLength = line.LastIndexOf((byte)'-');
            ReadOnlySpan<byte> name = line.Slice(0, nameLength);
            var reader = new SpanReader(line.Slice(nameLength + 1));
            int sectorId = reader.ReadPosIntUntil('[');
            ReadOnlySpan<byte> checkSum = reader.ReadUntil(']');

            foreach (byte c in name)
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
        int prev = -1;
        int prevCount = int.MaxValue;
        foreach (byte c in checksum)
        {
            int letter = c - 'a';
            int count = letterCounts[letter];

            if (prevCount < count || (prevCount == count && letter < prev))
            {
                return false;
            }

            prev = letter;
            prevCount = count;
            letterCounts[letter] = -1;
        }

        for (int i = 0; i < 26; i++)
        {
            int count = letterCounts[i];
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
            for (int i = 0; i < str.Length; i++)
            {
                chars[i] = (char)(((str[i] - 'a' + amount + 26) % 26) + 'a');
            }
        });
    }
}
