using System;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day12 : ISolver
{
    readonly struct Param
    {
        public readonly byte Type; // 0 = Regstier, 1 = Immediate
        public readonly int Value;

        public Param(byte type, int value)
        {
            Type = type;
            Value = value;
        }
    }

    readonly struct Instruction
    {
        public readonly byte Operation;
        public readonly Param Param1;
        public readonly Param Param2;

        public Instruction(byte operation, Param param1, Param param2)
        {
            Operation = operation;
            Param1 = param1;
            Param2 = param2;
        }
    }

    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int lines = input.Count((byte)'\n');
        var instrs = new Instruction[lines];

        int i = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            byte op = reader.Peek();
            reader.SkipLength("cpy ".Length);
            Param arg1, arg2;
            if (op is (byte)'c' or (byte)'j')
            {
                arg1 = ParseParamUntil(ref reader, ' ');
                arg2 = ParseParamUntil(ref reader, '\n');
            }
            else
            {
                arg1 = ParseParamUntil(ref reader, '\n');
                arg2 = default;
            }
            instrs[i++] = new Instruction(op, arg1, arg2);
        }

        int part1 = Solve(instrs, 0);
        int part2 = Solve(instrs, 1);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static Param ParseParamUntil(ref SpanReader reader, char until)
    {
        byte c = reader.Peek();
        if (c is >= (byte)'a' and <= (byte)'d')
        {
            reader.SkipLength(2);
            return new Param(0, c - 'a');
        }
        else
        {
            return new Param(1, reader.ReadIntUntil(until));
        }
    }

    private static int Solve(Instruction[] instrs, int c)
    {
        int ip = 0;
        Span<int> regs = stackalloc int[4];
        regs[2] = c;

        while (ip < instrs.Length)
        {
            Instruction op = instrs[ip++];
            Param p1 = op.Param1;
            switch (op.Operation)
            {
                case (byte)'c': // cpy
                    regs[op.Param2.Value] = p1.Type == 0 ? regs[p1.Value] : p1.Value;
                    break;
                case (byte)'i': // inc
                    regs[p1.Value]++;
                    break;
                case (byte)'d': // dec
                    regs[p1.Value]--;
                    break;
                case (byte)'j': // jump not zero
                    int value = p1.Type == 0 ? regs[p1.Value] : p1.Value;
                    if (value != 0)
                        ip += op.Param2.Value - 1;
                    break;
            }
        }

        return regs[0];
    }
}
