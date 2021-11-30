using System;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day08 : ISolver
{
    public readonly struct Instruction
    {
        public readonly char Operation;
        public readonly int Arg;

        public Instruction(char operation, int arg)
        {
            Operation = operation;
            Arg = arg;
        }
    }

    public Solution Solve(ReadOnlySpan<char> input)
    {
        int lines = input.Count('\n');
        Instruction[] instructions = new Instruction[lines];
        int[] values = new int[lines];

        int lineNumber = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            char op = reader[0]; // store op by getting the first character. n = nop, a = acc, j = jmp
            int mul = reader[4] == '+' ? 1 : -1;
            reader.SkipLength("jmp +".Length);
            int arg = mul * reader.ReadPosIntUntil('\n');
            instructions[lineNumber] = new(op, arg);
            values[lineNumber] = int.MinValue; // initial value that indicates we have not got a value yet
            lineNumber++;
        }

        int ipsToFlipLen = 0;
        int[] ipsToFlip = new int[lines];

        // simulate without flipping any instructions
        int ip = 0;
        int acc = 0;
        while (values[ip] == int.MinValue)
        {
            Instruction instruction = instructions[ip];
            switch (instruction.Operation)
            {
                case 'n': // nop
                    ipsToFlip[ipsToFlipLen++] = ip;
                    values[ip++] = acc;
                    break;
                case 'a': // acc
                    acc += instruction.Arg;
                    values[ip++] = acc;
                    break;
                case 'j': // jmp
                    ipsToFlip[ipsToFlipLen++] = ip;
                    values[ip] = acc;
                    ip += instruction.Arg;
                    break;
            }
        }

        int part1 = acc;
        int part2 = 0;

        for (int i = 0; i < ipsToFlipLen; i++)
        {
            ip = ipsToFlip[i];
            acc = values[ip];

            Instruction instruction = instructions[ip];
            ip += instruction.Operation == 'n' // nop
                ? instruction.Arg // nop becomes jmp
                : 1; // jmp becomes nop

            while (ip < lines && values[ip] == int.MinValue)
            {
                instruction = instructions[ip];
                switch (instruction.Operation)
                {
                    case 'n':
                        values[ip++] = acc;
                        break;
                    case 'a':
                        acc += instruction.Arg;
                        values[ip++] = acc;
                        break;
                    case 'j':
                        values[ip] = acc;
                        ip += instruction.Arg;
                        break;
                }
            }

            if (ip == lines)
            {
                part2 = acc;
                break;
            }
        }

        return new Solution(part1, part2);
    }
}
