using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day19 : ISolver
{
    private const ushort NoRuleRule = 0;
    private const ushort Accepted = 0;
    private const ushort Rejected = 1;

    [SkipLocalsInit]
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int MaxWorkflowCount = 600;
        Span<ushort> workflowNameLookup = stackalloc ushort[32 * 32 * 32]; // I know, I know.. too much memory
        workflowNameLookup[0] = 0;
        workflowNameLookup[1] = 1;
        Span<ushort> workflowTable = stackalloc ushort[MaxWorkflowCount * 4 * 2]; // up to 600 workflows, each with 4 rules using 2 ushorts
        workflowTable.Clear();
        ref ushort workflowTableRef = ref MemoryMarshal.GetReference(workflowTable);

        ushort numWorkflows = 2; // first two reserved for A and R
        byte c;
        while ((c = input[0]) != '\n')
        {
            int id = c - 'a' + 1; // add one to distinguish it from empty
            int i = 1;
            while ((c = input[i++]) != '{')
                id = (id << 5) + c - 'a' + 1;

            ParseRuleList(input, workflowTable.Slice(numWorkflows * 8, 8), ref i);
            workflowNameLookup[id] = numWorkflows++;
            input = input.Slice(i + 1);
        }

        for (int i = 17; i < numWorkflows * 8; i += 2)
            workflowTable[i] = workflowNameLookup[workflowTable[i]];

        ushort startWorkflowId = workflowNameLookup[(('i' - 'a' + 1) << 5) + 'n' - 'a' + 1];

        input = input.Slice(1);

        Span<ushort> xmas = stackalloc ushort[4];
        long part1 = 0;
        while (input.Length > 0)
        {
            int i = "{x=".Length;

            int x = input[i++] - '0';
            while ((c = input[i++]) != ',')
                x = x * 10 + c - '0';

            i += "m=".Length;

            int m = input[i++] - '0';
            while ((c = input[i++]) != ',')
                m = m * 10 + c - '0';

            i += "a=".Length;

            int a = input[i++] - '0';
            while ((c = input[i++]) != ',')
                a = a * 10 + c - '0';

            i += "s=".Length;

            int s = input[i++] - '0';
            while ((c = input[i++]) != '}')
                s = s * 10 + c - '0';

            xmas[0] = (ushort)x;
            xmas[1] = (ushort)m;
            xmas[2] = (ushort)a;
            xmas[3] = (ushort)s;

            ushort workflowId = startWorkflowId;
            while (true)
            {
                if (workflowId == Accepted)
                {
                    part1 += x + m + a + s;
                    break;
                }

                if (workflowId == Rejected)
                    break;

                ref ushort workflowRef = ref Unsafe.Add(ref workflowTableRef, workflowId * 8);
                int ruleOffset = workflowId * 8;
                for (int j = 0; j < 8; j += 2)
                {
                    ushort rule = Unsafe.Add(ref workflowRef, j);
                    ushort destination = Unsafe.Add(ref workflowRef, j + 1);
                    if (rule == NoRuleRule)
                    {
                        workflowId = destination;
                        break;
                    }

                    int variable = rule & 0b11;
                    int isLessThan = (rule >> 2) & 1;
                    ushort value = (ushort)(rule >> 3);

                    if (isLessThan != 0)
                    {
                        if (xmas[variable] < value)
                        {
                            workflowId = destination;
                            break;
                        }
                    }
                    else
                    {
                        if (xmas[variable] > value)
                        {
                            workflowId = destination;
                            break;
                        }
                    }
                }
            }

            input = input.Slice(i + 1);
        }

        solution.SubmitPart1(part1);

        long part2 = CountRatings(startWorkflowId, [1, 4000, 1, 4000, 1, 4000, 1, 4000], ref workflowTableRef);
        solution.SubmitPart2(part2);

        static long CountRatings(ushort workflowId, Span<ushort> rangeValues, ref ushort workflowTableRef)
        {
            if (workflowId == Accepted)
                return (long)(rangeValues[1] - rangeValues[0] + 1) * (rangeValues[3] - rangeValues[2] + 1) * (rangeValues[5] - rangeValues[4] + 1) * (rangeValues[7] - rangeValues[6] + 1);

            if (workflowId == Rejected)
                return 0;

            Span<ushort> newRangeValues = stackalloc ushort[8];
            rangeValues.CopyTo(newRangeValues);

            long total = 0;
            ref ushort workflowRef = ref Unsafe.Add(ref workflowTableRef, workflowId * 8);
            int ruleOffset = workflowId * 8;
            for (int i = 0; i < 8; i += 2)
            {
                ushort rule = Unsafe.Add(ref workflowRef, i);
                ushort destination = Unsafe.Add(ref workflowRef, i + 1);
                if (rule == NoRuleRule)
                    return total + CountRatings(destination, newRangeValues, ref workflowTableRef);

                int variable = rule & 0b11;
                int isLessThan = (rule >> 2) & 1;
                ushort value = (ushort)(rule >> 3);

                if (isLessThan != 0)
                {
                    ushort min = newRangeValues[variable * 2];
                    if (value < min)
                        continue;

                    ushort max = newRangeValues[variable * 2 + 1];
                    if (max < value)
                        return total + CountRatings(destination, newRangeValues, ref workflowTableRef);

                    newRangeValues[variable * 2 + 1] = (ushort)(value - 1);
                    total += CountRatings(destination, newRangeValues, ref workflowTableRef);
                    newRangeValues[variable * 2 + 1] = max;
                    newRangeValues[variable * 2] = value;
                }
                else
                {
                    ushort max = newRangeValues[variable * 2 + 1];
                    if (value > max)
                        continue;

                    ushort min = newRangeValues[variable * 2];
                    if (min > value)
                        return total + CountRatings(destination, newRangeValues, ref workflowTableRef);

                    newRangeValues[variable * 2] = (ushort)(value + 1);
                    total += CountRatings(destination, newRangeValues, ref workflowTableRef);
                    newRangeValues[variable * 2] = min;
                    newRangeValues[variable * 2 + 1] = value;
                }
            }

            return total;
        }
    }

    private static void ParseRuleList(ReadOnlySpan<byte> input, Span<ushort> workflowData, ref int i)
    {
        int ruleIndex = 0;
        while (true)
        {
            byte c = input[i++];
            byte c2 = input[i++];
            uint isLessThan;
            uint destinationId;
            if (c2 == '<')
                isLessThan = 1;
            else if (c2 == '>')
                isLessThan = 0;
            else
            {
                if (c <= 'Z')
                {
                    destinationId = c == 'A' ? Accepted : Rejected;
                }
                else
                {
                    destinationId = (uint)(c - 'a' + 1);
                    if (c2 != '}')
                    {
                        destinationId = (destinationId << 5) + (uint)(c2 - 'a' + 1);
                        while ((c = input[i++]) != '}')
                            destinationId = (destinationId << 5) + (uint)(c - 'a' + 1);
                    }
                }

                workflowData[ruleIndex++] = NoRuleRule;
                workflowData[ruleIndex++] = (ushort)destinationId;
                break;
            }

            uint variable = c switch { (byte)'x' => 0, (byte)'m' => 1, (byte)'a' => 2, _ => 3 };
            uint value = (uint)input[i++] - '0';
            while ((c = input[i++]) != ':')
                value = 10 * value + c - '0';

            if ((c = input[i++]) <= 'Z')
            {
                destinationId = c == 'A' ? Accepted : Rejected;
                i++;
            }
            else
            {
                destinationId = (uint)(c - 'a' + 1);
                while ((c = input[i++]) != ',')
                    destinationId = (destinationId << 5) + (uint)(c - 'a' + 1);
            }

            workflowData[ruleIndex++] = (ushort)(variable | (isLessThan << 2) | (value << 3));
            workflowData[ruleIndex++] = (ushort)destinationId;
        }
    }
}
