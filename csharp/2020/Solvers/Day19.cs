using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day19 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // split input into the two sections
        var messagesStart = input.IndexOf("\n\n"u8);
        var rulesSpan = input[..(messagesStart + 1)];
        var messagesSpan = input[(messagesStart + 2)..];

        // rules[n][i][j] returns the jth element of the ith subrule for rule n
        var rules = ParseRules(rulesSpan);

        // this only works for the AoC input, but all rules will always reduce to the same number of terminals
        var ruleLengths = GetRuleLengths(rules);

        static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        var rule0Len = ruleLengths[0];
        var rule42Len = ruleLengths[42];
        var rule11Len = ruleLengths[11];
        var rule31Len = ruleLengths[31];

        var part2Multiple = GCD(rule42Len, rule31Len);

        var part1 = 0;
        var part2 = 0;

        foreach (var messageRange in messagesSpan.SplitLines())
        {
            var message = messagesSpan[messageRange];
            if (message.Length == rule0Len && MatchesRule(message, 0))
            {
                part1++;
            }
            else if (message.Length > rule0Len && message.Length % part2Multiple == 0)
            {
                // we take advantage of the fact that the input always contains the rule "0: 8 11"
                // and there are no other rules that use 8 or 11.
                //
                // rule 8 is just rule 42 repeating
                // rule 11 is rule 42 n times, then rule 31 n times.
                //
                // this means that we are looking for 42 * (a + b) + 31 * b where a >= 1 and b >= 1

                var num31s = 0;
                for (var i = message.Length - rule31Len; i >= 0; i -= rule31Len)
                {
                    if (!MatchesRule(message.Slice(i, rule31Len), 31))
                    {
                        break;
                    }

                    num31s++;
                }

                if (num31s == 0)
                {
                    continue;
                }

                var num42s = (message.Length - (num31s * rule31Len)) / rule42Len;
                if (num42s <= num31s)
                {
                    continue;
                }

                var isValid = true;
                for (var i = 0; i < num42s * rule42Len; i += rule42Len)
                {
                    if (!MatchesRule(message.Slice(i, rule42Len), 42))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    part2++;
                }
            }
        }

        // all messages in part 1 are valid for part 2
        part2 += part1;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);

        bool MatchesRule(ReadOnlySpan<byte> str, int ruleNumber)
        {
            if (ruleNumber < 0)
            {
                var c = str[0];
                return (ruleNumber == -1 && c == 'a') || (ruleNumber == -2 && c == 'b');
            }

            foreach (var subRule in rules[ruleNumber])
            {
                if (MatchesSubRule(str, subRule))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool MatchesSubRule(ReadOnlySpan<byte> str, int[] subRule)
        {
            var i = 0;

            foreach (var subRuleNumber in subRule)
            {
                var ruleLen = subRuleNumber < 0 ? 1 : ruleLengths[subRuleNumber];

                if (!MatchesRule(str.Slice(i, ruleLen), subRuleNumber))
                {
                    return false;
                }

                i += ruleLen;
            }

            return true;
        }
    }

    private static int[][][] ParseRules(ReadOnlySpan<byte> rules)
    {
        var numRules = rules.Count((byte)'\n');
        var rulesArr = new int[numRules][][];

        var reader = new SpanReader(rules);
        while (!reader.Done)
        {
            var ruleId = reader.ReadIntUntil(':');
            reader.SkipLength(1); // skip the space

            if (reader.Peek() == '"')
            {
                var rule = reader[1] == 'a' ? -1 : -2;
                rulesArr[ruleId] = [[rule]];
                reader.SkipLength("\"a\"\n".Length);
            }
            else
            {
                var ruleValueReader = new SpanReader(reader.ReadUntil('\n'));

                var n1 = ruleValueReader.ReadPosIntUntil(' ');
                int[] group1 = ruleValueReader.Done || ruleValueReader.Peek() == '|'
                    ? [n1]
                    : [n1, ruleValueReader.ReadPosIntUntil(' ')];

                if (ruleValueReader.Done)
                {
                    rulesArr[ruleId] = [group1];
                }
                else
                {
                    ruleValueReader.SkipLength("| ".Length);

                    var n3 = ruleValueReader.ReadPosIntUntil(' ');
                    int[] group2 = ruleValueReader.Done
                        ? [n3]
                        : [n3, ruleValueReader.ReadPosIntUntilEnd()];

                    rulesArr[ruleId] = [group1, group2];
                }
            }
        }

        return rulesArr;
    }

    private static int[] GetRuleLengths(int[][][] rules)
    {
        var lengths = new int[rules.Length];

        // ensure whole array is cached
        for (var i = 0; i < rules.Length; i++)
        {
            _ = GetRuleLength(i);
        }

        return lengths;

        int GetRuleLength(int ruleNumber)
        {
            var cachedLen = lengths[ruleNumber];
            if (cachedLen != 0)
            {
                return cachedLen;
            }

            // rules always have the same length regardless of which alternative is taken
            // so we can just take the first rule
            var rule = rules[ruleNumber][0];

            var len = 0;
            foreach (var subRuleNumber in rule)
            {
                // negative sub-rule means it is a terminal of length 1
                len += subRuleNumber < 0 ? 1 : GetRuleLength(subRuleNumber);
            }

            return lengths[ruleNumber] = len;
        }
    }
}
