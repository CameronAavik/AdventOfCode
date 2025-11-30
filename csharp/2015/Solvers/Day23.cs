using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day23 : ISolver
{
    public enum InstructionType
    {
        Half,
        Triple,
        Increment,
        Jump,
        JumpIfEven,
        JumpIfOne
    }

    public record Instruction(InstructionType Type, int Arg1, int Arg2 = 0);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var instructions = new List<Instruction>();
        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            static int ParseReg(ReadOnlySpan<byte> line)
            {
                return line[4] == 'a' ? 0 : 1;
            }

            static int ParseNumber(ReadOnlySpan<byte> str)
            {
                var mul = str[0] == '+' ? 1 : -1;
                var c = str[1] - '0';
                for (var i = 2; i < str.Length; i++)
                    c = c * 10 + str[i] - '0';
                return mul * c;
            }

            var instruction = line[2] switch
            {
                (byte)'f' => new Instruction(InstructionType.Half, ParseReg(line)),
                (byte)'l' => new Instruction(InstructionType.Triple, ParseReg(line)),
                (byte)'c' => new Instruction(InstructionType.Increment, ParseReg(line)),
                (byte)'p' => new Instruction(InstructionType.Jump, -1, ParseNumber(line[4..])),
                (byte)'e' => new Instruction(InstructionType.JumpIfEven, ParseReg(line), ParseNumber(line[7..])),
                (byte)'o' => new Instruction(InstructionType.JumpIfOne, ParseReg(line), ParseNumber(line[7..])),
                _ => default!
            };

            instructions.Add(instruction);
        }

        Instruction[] instructionArr = [.. instructions];

        var part1 = Simulate(instructionArr, 0);
        var part2 = Simulate(instructionArr, 1);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int Simulate(Instruction[] instructions, int aStart)
    {
        var variables = new int[2];
        variables[0] = aStart;

        var i = 0;
        while (i >= 0 && i < instructions.Length)
        {
            var instruction = instructions[i];
            switch (instruction.Type)
            {
                case InstructionType.Half:
                    variables[instruction.Arg1] /= 2;
                    i++;
                    break;
                case InstructionType.Triple:
                    variables[instruction.Arg1] *= 3;
                    i++;
                    break;
                case InstructionType.Increment:
                    variables[instruction.Arg1]++;
                    i++;
                    break;
                case InstructionType.Jump:
                    i += instruction.Arg2;
                    break;
                case InstructionType.JumpIfEven:
                    i += variables[instruction.Arg1] % 2 == 0 ? instruction.Arg2 : 1;
                    break;
                case InstructionType.JumpIfOne:
                    i += variables[instruction.Arg1] == 1 ? instruction.Arg2 : 1;
                    break;
            }
        }

        return variables[1];
    }
}
