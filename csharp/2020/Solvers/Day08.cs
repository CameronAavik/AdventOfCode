using System;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day08 : ISolver
    {
        enum Operation { Nop, Acc, Jmp }

        record Instruction(Operation Operation, int Arg);

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var lines = input.Count('\n');
            Instruction[] instructions = new Instruction[lines];
            int[] values = new int[lines];

            int lineNumber = 0;
            var reader = new SpanReader(input);
            while (!reader.Done)
            {
                Operation op = reader.Peek() switch { 'a' => Operation.Acc, 'j' => Operation.Jmp, _ => Operation.Nop };
                reader.SkipLength(4);
                int mul = reader.Peek() == '+' ? 1 : -1;
                reader.SkipLength(1);
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
                var instruction = instructions[ip];
                switch (instruction.Operation)
                {
                    case Operation.Nop:
                        ipsToFlip[ipsToFlipLen++] = ip;
                        values[ip++] = acc;
                        break;
                    case Operation.Acc:
                        acc += instruction.Arg;
                        values[ip++] = acc;
                        break;
                    case Operation.Jmp:
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

                var instruction = instructions[ip];
                ip += instruction.Operation == Operation.Nop
                    ? instruction.Arg // nop becomes jmp
                    : 1; // jmp becomes nop

                while (ip < lines && values[ip] == int.MinValue)
                {
                    instruction = instructions[ip];
                    switch (instruction.Operation)
                    {
                        case Operation.Nop:
                            values[ip++] = acc;
                            break;
                        case Operation.Acc:
                            acc += instruction.Arg;
                            values[ip++] = acc;
                            break;
                        case Operation.Jmp:
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
}
