using System;
using System.Linq;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day04 : ISolver
    {
        private static readonly string[] s_rotations =
            Enumerable.Range(0, 26).Select(i => Rotate("north", -i)).ToArray();

        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1 = 0;
            int part2 = -1;

            int[] letterCounts = new int[26];
            foreach (ReadOnlySpan<char> line in input.Split('\n'))
            {
                int nameLength = line.LastIndexOf('-');
                ReadOnlySpan<char> name = line.Slice(0, nameLength);
                int sectorId = int.Parse(line.Slice(nameLength + 1, line.Length - 8 - nameLength));
                ReadOnlySpan<char> checkSum = line.Slice(line.Length - 6, 5);

                foreach (char c in name)
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

            return new Solution(part1, part2);
        }

        private static bool IsChecksumCorrect(int[] letterCounts, ReadOnlySpan<char> checksum)
        {
            int prev = -1;
            int prevCount = int.MaxValue;
            foreach (char c in checksum)
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
}
