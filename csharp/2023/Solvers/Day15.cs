using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day15 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;

        List<ulong>[] boxes = new List<ulong>[256];
        for (int i = 0; i < boxes.Length; i++)
            boxes[i] = new(24); // upper bound on length of box based on inputs

        while (input.Length > 0)
        {
            byte c = input[0];
            ulong part2Label = (ulong)c - 'a';
            uint hash = c;
            hash += hash << 4;

            int i = 1;
            while ((c = input[i++]) >= 'a')
            {
                hash += c;
                hash += hash << 4;
                part2Label += (ulong)(c - 'a') << (i * 4);
            }

            List<ulong> box = boxes[(byte)hash];

            if (c == '-')
            {
                hash += (hash << 4) + ('-' << 4) + '-';
                i += 1; // skip comma

                HandleMinus(part2Label, box);
            }
            else
            {
                hash += (hash << 4) + ('=' << 4) + '=';

                c = input[i++];
                uint num = (uint)c - '0';
                hash += c;
                hash += hash << 4;
                while ((c = input[i++]) >= '0')
                {
                    num = num * 10 + c - '0';
                    hash += c;
                    hash += hash << 4;
                }

                HandleEquals(part2Label, box, num);
            }

            part1 += (byte)hash;
            input = input.Slice(i);
        }

        solution.SubmitPart1(part1);

        int part2 = 0;
        for (int i = 0; i < boxes.Length; i++)
        {
            int index = 1;
            foreach (ulong element in boxes[i])
            {
                if (element != 0)
                    part2 += (i + 1) * index++ * (int)(element & uint.MaxValue);
            }
        }

        solution.SubmitPart2(part2);
    }

    private static void HandleMinus(ulong part2Label, List<ulong> box)
    {
        for (int j = 0; j < box.Count; j++)
        {
            if (box[j] >> 32 == part2Label)
            {
                box[j] = 0;
                return;
            }
        }
    }

    private static void HandleEquals(ulong part2Label, List<ulong> box, uint num)
    {
        for (int j = 0; j < box.Count; j++)
        {
            if (box[j] >> 32 == part2Label)
            {
                box[j] = (part2Label << 32) + num;
                return;
            }
        }

        box.Add((part2Label << 32) + num);
    }
}
