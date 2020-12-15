﻿using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day14 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var mem1 = new Dictionary<long, long>();
            var mem2 = new Dictionary<long, long>();

            long maskXs = 0; // ['X'] -> 1, ['0' or '1'] -> 0
            long mask1s = 0; // ['X' or '0'] -> 0, ['1'] -> 1

            var reader = new SpanReader(input);
            while (!reader.Done)
            {
                if (reader[1] == 'a') // mask
                {
                    maskXs = 0;
                    mask1s = 0;

                    reader.SkipLength("mask = ".Length);
                    for (int i = 0; i < 36; i++)
                    {
                        char c = reader[i];
                        switch (c)
                        {
                            case '1':
                                mask1s |= 1L << (35 - i);
                                break;
                            case 'X':
                                maskXs |= 1L << (35 - i);
                                break;
                        }
                    }

                    reader.SkipLength(37); // 36 digits + newline
                }
                else // mem
                {
                    reader.SkipLength("mem[".Length);
                    long addr = reader.ReadPosLongUntil(']');
                    reader.SkipLength(" = ".Length);
                    long val = reader.ReadPosLongUntil('\n');

                    // Part 1
                    mem1[addr] = (val & maskXs) | mask1s;

                    // Part 2
                    addr |= mask1s; // any 1's in the mask should be set to 1 in the address
                    addr &= ~maskXs; // any x's need to be set to 0 (since we will be iterating through permutations of bits in X)

                    // iterate through submasks: https://cp-algorithms.com/algebra/all-submasks.html
                    long mask = maskXs;
                    mem2[addr] = val; // handle mask = 0 case
                    while (mask != 0)
                    {
                        mem2[addr | mask] = val;
                        mask = (mask - 1) & maskXs;
                    }
                }
            }

            long part1 = 0;
            foreach ((_, long v) in mem1)
            {
                part1 += v;
            }

            long part2 = 0;
            foreach ((_, long v) in mem2)
            {
                part2 += v;
            }

            return new Solution(part1.ToString(), part2.ToString());
        }
    }
}
