using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day08 : ISolver
{
    public readonly struct Instruction(byte operation, int arg)
    {
        public readonly byte Operation = operation;
        public readonly int Arg = arg;
    }

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lines = input.Count((byte)'\n');
        var instructions = new Instruction[lines];
        var values = new int[lines];

        var lineNumber = 0;
        var reader = new SpanReader(input);
        while (!reader.Done)
        {
            var op = reader[0]; // store op by getting the first character. n = nop, a = acc, j = jmp
            var mul = reader[4] == '+' ? 1 : -1;
            reader.SkipLength("jmp +".Length);
            var arg = mul * reader.ReadPosIntUntil('\n');
            instructions[lineNumber] = new(op, arg);
            values[lineNumber] = int.MinValue; // initial value that indicates we have not got a value yet
            lineNumber++;
        }

        var ipsToFlipLen = 0;
        var ipsToFlip = new int[lines];

        // simulate without flipping any instructions
        var ip = 0;
        var acc = 0;
        while (values[ip] == int.MinValue)
        {
            var instruction = instructions[ip];
            switch (instruction.Operation)
            {
                case (byte)'n': // nop
                    ipsToFlip[ipsToFlipLen++] = ip;
                    values[ip++] = acc;
                    break;
                case (byte)'a': // acc
                    acc += instruction.Arg;
                    values[ip++] = acc;
                    break;
                case (byte)'j': // jmp
                    ipsToFlip[ipsToFlipLen++] = ip;
                    values[ip] = acc;
                    ip += instruction.Arg;
                    break;
            }
        }

        var part1 = acc;
        var part2 = 0;

        for (var i = 0; i < ipsToFlipLen; i++)
        {
            ip = ipsToFlip[i];
            acc = values[ip];

            var instruction = instructions[ip];
            ip += instruction.Operation == 'n' // nop
                ? instruction.Arg // nop becomes jmp
                : 1; // jmp becomes nop

            while (ip < lines && values[ip] == int.MinValue)
            {
                instruction = instructions[ip];
                switch (instruction.Operation)
                {
                    case (byte)'n':
                        values[ip++] = acc;
                        break;
                    case (byte)'a':
                        acc += instruction.Arg;
                        values[ip++] = acc;
                        break;
                    case (byte)'j':
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

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
