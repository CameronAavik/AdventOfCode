using System;
using System.Runtime.CompilerServices;
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

        ushort numWorkflows = 2; // first two reserved for A and R
        byte c;
        while ((c = input[0]) != '\n')
        {
            var id = c - 'a' + 1; // add one to distinguish it from empty
            var i = 1;
            while ((c = input[i++]) != '{')
                id = (id << 5) + c - 'a' + 1;

            ParseRuleList(input, workflowTable.Slice(numWorkflows * 8, 8), ref i);
            workflowNameLookup[id] = numWorkflows++;
            input = input[(i + 1)..];
        }

        for (var i = 17; i < numWorkflows * 8; i += 2)
            workflowTable[i] = workflowNameLookup[workflowTable[i]];

        var startWorkflowId = workflowNameLookup[(('i' - 'a' + 1) << 5) + 'n' - 'a' + 1];

        input = input[1..];

        Span<ushort> xmas = stackalloc ushort[4];
        long part1 = 0;
        while (input.Length > 0)
        {
            var i = "{x=".Length;

            var x = input[i++] - '0';
            while ((c = input[i++]) != ',')
                x = x * 10 + c - '0';

            i += "m=".Length;

            var m = input[i++] - '0';
            while ((c = input[i++]) != ',')
                m = m * 10 + c - '0';

            i += "a=".Length;

            var a = input[i++] - '0';
            while ((c = input[i++]) != ',')
                a = a * 10 + c - '0';

            i += "s=".Length;

            var s = input[i++] - '0';
            while ((c = input[i++]) != '}')
                s = s * 10 + c - '0';

            xmas[0] = (ushort)x;
            xmas[1] = (ushort)m;
            xmas[2] = (ushort)a;
            xmas[3] = (ushort)s;

            var workflowId = startWorkflowId;
            while (true)
            {
                if (workflowId == Accepted)
                {
                    part1 += x + m + a + s;
                    break;
                }

                if (workflowId == Rejected)
                    break;

                var workflowOffset = workflowId * 8;
                var ruleOffset = workflowId * 8;
                for (var j = 0; j < 8; j += 2)
                {
                    var rule = workflowTable[workflowOffset + j];
                    var destination = workflowTable[workflowOffset + j + 1];
                    if (rule == NoRuleRule)
                    {
                        workflowId = destination;
                        break;
                    }

                    var variable = rule & 0b11;
                    var isLessThan = (rule >> 2) & 1;
                    var value = (ushort)(rule >> 3);

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

            input = input[(i + 1)..];
        }

        solution.SubmitPart1(part1);

        var part2 = CountRatings(startWorkflowId, [1, 4000, 1, 4000, 1, 4000, 1, 4000], workflowTable);
        solution.SubmitPart2(part2);

        static long CountRatings(ushort workflowId, Span<ushort> rangeValues, ReadOnlySpan<ushort> workflowTable)
        {
            if (workflowId == Accepted)
                return (long)(rangeValues[1] - rangeValues[0] + 1) * (rangeValues[3] - rangeValues[2] + 1) * (rangeValues[5] - rangeValues[4] + 1) * (rangeValues[7] - rangeValues[6] + 1);

            if (workflowId == Rejected)
                return 0;

            Span<ushort> newRangeValues = stackalloc ushort[8];
            rangeValues.CopyTo(newRangeValues);

            long total = 0;
            var workflowOffset = workflowId * 8;
            for (var i = 0; i < 8; i += 2)
            {
                var rule = workflowTable[workflowOffset + i];
                var destination = workflowTable[workflowOffset + i + 1];
                if (rule == NoRuleRule)
                    return total + CountRatings(destination, newRangeValues, workflowTable);

                var variable = rule & 0b11;
                var isLessThan = (rule >> 2) & 1;
                var value = (ushort)(rule >> 3);

                if (isLessThan != 0)
                {
                    var min = newRangeValues[variable * 2];
                    if (value < min)
                        continue;

                    var max = newRangeValues[variable * 2 + 1];
                    if (max < value)
                        return total + CountRatings(destination, newRangeValues, workflowTable);

                    newRangeValues[variable * 2 + 1] = (ushort)(value - 1);
                    total += CountRatings(destination, newRangeValues, workflowTable);
                    newRangeValues[variable * 2 + 1] = max;
                    newRangeValues[variable * 2] = value;
                }
                else
                {
                    var max = newRangeValues[variable * 2 + 1];
                    if (value > max)
                        continue;

                    var min = newRangeValues[variable * 2];
                    if (min > value)
                        return total + CountRatings(destination, newRangeValues, workflowTable);

                    newRangeValues[variable * 2] = (ushort)(value + 1);
                    total += CountRatings(destination, newRangeValues, workflowTable);
                    newRangeValues[variable * 2] = min;
                    newRangeValues[variable * 2 + 1] = value;
                }
            }

            return total;
        }
    }

    private static void ParseRuleList(ReadOnlySpan<byte> input, Span<ushort> workflowData, ref int i)
    {
        var ruleIndex = 0;
        while (true)
        {
            var c = input[i++];
            var c2 = input[i++];
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
            var value = (uint)input[i++] - '0';
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
