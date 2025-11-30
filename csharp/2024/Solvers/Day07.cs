using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

public class Day07 : ISolver
{
    [SkipLocalsInit]
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ulong part1 = 0, part2 = 0;
        var i = 0;
        Span<ulong> operands = stackalloc ulong[16];
        Span<int> operandLengths = stackalloc int[16];
        Span<ulong> part1Maximums = stackalloc ulong[16];

        while (i < input.Length)
        {
            var finalTarget = ParseTargetNumber(input, ref i);
            var numOperands = ParseOperands(input, ref i, operandLengths, operands, part1Maximums);
            var lineOperands = operands[..numOperands];

            if (TryFindSolutionPart1(finalTarget, lineOperands, part1Maximums))
            {
                part1 += finalTarget;
                part2 += finalTarget;
                continue;
            }

            if (TryFindSolutionPart2(finalTarget, lineOperands, operandLengths))
                part2 += finalTarget;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ParseTargetNumber(ReadOnlySpan<byte> input, ref int i)
    {
        var target = input[i++] - (ulong)'0';
        byte c;
        while ((c = input[i++]) != (byte)':')
            target = target * 10UL + (c - (ulong)'0');
        return target;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ParseOperands(ReadOnlySpan<byte> input, ref int i, Span<int> operandLengths, Span<ulong> operands, Span<ulong> part1Maximums)
    {
        byte c;

        var numOperands = 0;
        ulong prevMaxPart1 = 0;
        do
        {
            var startIndex = ++i;
            var operand = input[i] - (ulong)'0';
            while ((c = input[++i]) >= '0')
                operand = operand * 10 + (c - (ulong)'0');

            operandLengths[numOperands] = i - startIndex;
            operands[numOperands] = operand;
            prevMaxPart1 = part1Maximums[numOperands] = Math.Max(prevMaxPart1 + operand, prevMaxPart1 * operand);
            numOperands++;
        } while (c != (byte)'\n');

        i++;
        return numOperands;
    }

    private readonly record struct StackEntry(ulong Target, int OperandIndex);

    [SkipLocalsInit]
    private static bool TryFindSolutionPart1(ulong target, Span<ulong> operands, Span<ulong> maximums)
    {
        var op0 = operands[0];
        var op1 = operands[1];
        var terminalAdd = op0 + op1;
        var terminalMul = op0 * op1;

        Span<StackEntry> stack = stackalloc StackEntry[32];
        var stackPtr = 0;
        stack[stackPtr++] = new(target, operands.Length - 1);

        while (stackPtr > 0)
        {
            var entry = stack[--stackPtr];
            if (entry.OperandIndex == 1)
            {
                if (entry.Target == terminalAdd || entry.Target == terminalMul)
                    return true;
                continue;
            }

            var op = operands[entry.OperandIndex];
            var nextIndex = entry.OperandIndex - 1;

            var diff = entry.Target - op;
            if (diff <= maximums[nextIndex])
                stack[stackPtr++] = new(diff, nextIndex);

            (var div, var rem) = Math.DivRem(entry.Target, op);
            if (rem == 0)
                stack[stackPtr++] = new(div, nextIndex);
        }

        return false;
    }

    [SkipLocalsInit]
    private static bool TryFindSolutionPart2(ulong target, Span<ulong> operands, Span<int> operandLengths)
    {
        var op0 = operands[0];
        var op1 = operands[1];
        var terminalAdd = op0 + op1;
        var terminalMul = op0 * op1;
        var terminalConcat = op0 * GetPowerOfTen(operandLengths[1]) + op1;

        Span<StackEntry> stack = stackalloc StackEntry[32];
        Span<ulong> maximums = stackalloc ulong[16];
        Span<ulong> concatMultipliers = stackalloc ulong[16];
        var totalLength = 0;
        for (var i = 0; i < operands.Length; i++)
        {
            var opLength = operandLengths[i];
            totalLength += opLength;
            maximums[i] = totalLength < 20 ? GetPowerOfTen(totalLength) : ulong.MaxValue;
            concatMultipliers[i] = GetPowerOfTen(opLength);
        }

        var stackPtr = 0;
        stack[stackPtr++] = new(target, operands.Length - 1);

        while (stackPtr > 0)
        {
            var entry = stack[--stackPtr];
            if (entry.OperandIndex == 1)
            {
                if (entry.Target == terminalAdd || entry.Target == terminalMul || entry.Target == terminalConcat)
                    return true;
                continue;
            }

            var op = operands[entry.OperandIndex];
            var nextIndex = entry.OperandIndex - 1;

            var diff = entry.Target - op;
            if (diff <= maximums[nextIndex])
                stack[stackPtr++] = new(diff, nextIndex);

            (var div, var rem) = Math.DivRem(entry.Target, op);
            if (rem == 0)
                stack[stackPtr++] = new(div, nextIndex);

            (div, rem) = Math.DivRem(entry.Target, concatMultipliers[entry.OperandIndex]);
            if (rem == op)
                stack[stackPtr++] = new(div, nextIndex);
        }

        return false;
    }

    private static ulong GetPowerOfTen(int n)
    {
        // These powers of ten will be compiled into the binary
        ReadOnlySpan<ulong> PowersOf10 = [
            1, 10, 100, 1_000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000, 1_000_000_000, 10_000_000_000,
            100_000_000_000, 1_000_000_000_000, 10_000_000_000_000, 100_000_000_000_000, 1_000_000_000_000_000,
            10_000_000_000_000_000, 100_000_000_000_000_000, 1_000_000_000_000_000_000, 10_000_000_000_000_000_000
        ];
        return PowersOf10[n];
    }
}
