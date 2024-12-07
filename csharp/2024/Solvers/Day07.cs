using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

public class Day07 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int BufferSize = 512;
        ReadOnlySpan<ulong> powersOf10 = [
            1, 10, 100, 1000, 1_000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000, 1_000_000_000,
            10_000_000_000, 100_000_000_000, 1_000_000_000_000, 10_000_000_000_000, 100_000_000_000_000
        ];

        ulong part1 = 0;
        ulong part2 = 0;
        int i = 0;
        Span<ulong> buffer = new ulong[BufferSize];
        Span<int> operandLengths = stackalloc int[16];
        Span<ulong> operands = stackalloc ulong[16];
        while (i < input.Length)
        {
            byte c;
            ulong finalTarget = input[i++] - (ulong)'0';
            while ((c = input[i++]) != (byte)':')
                finalTarget = finalTarget * 10UL + (c - (ulong)'0');

            i++;
            int operandsStartIndex = i;

            int numOperands = 0;
            while (c != '\n')
            {
                int prevI = i;
                ulong operand = input[i++] - (ulong)'0';
                while ((c = input[i++]) >= '0')
                    operand = operand * 10 + (c - (ulong)'0');

                operandLengths[numOperands] = i - prevI - 1;
                operands[numOperands++] = operand;
            }

            int totalOperandsLength = i - operandsStartIndex - numOperands;

            Span<ulong> prevTargets = buffer;
            prevTargets[0] = finalTarget;
            int part1TargetCount = 1;
            int part2TargetCount = 0;
            int offset = 0;
            for (int j = numOperands - 1; j > 1 && part1TargetCount + part2TargetCount > 0; j--)
            {
                ulong operand = operands[j];
                int operandLength = operandLengths[j];
                
                prevTargets = buffer.Slice(offset, BufferSize / 2);

                Span<ulong> newTargets = buffer.Slice(BufferSize / 2 - offset, BufferSize / 2);
                Span<ulong> part1Targets = newTargets.Slice(0, BufferSize / 4);
                Span<ulong> part2Targets = newTargets.Slice(BufferSize / 4);
                ulong powerOfTen = powersOf10[operandLength];
                int remainingOperandLength = totalOperandsLength - operandLength;
                ulong maximum = remainingOperandLength < powersOf10.Length
                    ? powersOf10[remainingOperandLength] * 10 - 1
                    : ulong.MaxValue; // maximum if we concatenated all the remaining operands

                int newPart1TargetCount = 0;
                int newPart2TargetCount = 0;
                foreach (ulong target in prevTargets.Slice(0, part1TargetCount))
                {
                    if (target - operand <= maximum)
                        part1Targets[newPart1TargetCount++] = target - operand;
                    if (target % operand == 0)
                        part1Targets[newPart1TargetCount++] = target / operand;
                    if (target % powerOfTen == operand)
                        part2Targets[newPart2TargetCount++] = target / powerOfTen;
                }

                foreach (ulong target in prevTargets.Slice(BufferSize / 4, part2TargetCount))
                {
                    if (target - operand <= maximum)
                        part2Targets[newPart2TargetCount++] = target - operand;
                    if (target % operand == 0)
                        part2Targets[newPart2TargetCount++] = target / operand;
                    if (target % powerOfTen == operand)
                        part2Targets[newPart2TargetCount++] = target / powerOfTen;
                }

                offset = BufferSize / 2 - offset;
                part1TargetCount = newPart1TargetCount;
                part2TargetCount = newPart2TargetCount;
                totalOperandsLength -= operandLength;
            }

            if (part1TargetCount + part2TargetCount > 0)
            {
                ulong op1 = operands[0];
                ulong op2 = operands[1];

                ulong add = op1 + op2;
                ulong mul = op1 * op2;
                ulong concat = op1 * powersOf10[operandLengths[1]] + op2;

                prevTargets = buffer.Slice(offset);

                if (part1TargetCount > 0)
                {
                    Span<ulong> part1Targets = prevTargets.Slice(0, part1TargetCount);
                    int part1Index = part1Targets.IndexOfAny(add, mul, concat);
                    if (part1Index >= 0)
                    {
                        part2 += finalTarget;
                        if (part1Targets[part1Index] != concat || part1Targets.Slice(part1Index).ContainsAny(add, mul))
                            part1 += finalTarget;
                        continue;
                    }
                }

                if (part2TargetCount > 0)
                {
                    Span<ulong> part2Targets = prevTargets.Slice(BufferSize / 4, part2TargetCount);
                    if (part2Targets.ContainsAny(add, mul, concat))
                        part2 += finalTarget;
                }
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
