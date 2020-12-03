using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
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

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var instructions = new List<Instruction>();
            foreach (ReadOnlySpan<char> line in input.SplitLines())
            {
                static int ParseReg(ReadOnlySpan<char> line) => line[4] == 'a' ? 0 : 1;

                Instruction instruction = line[2] switch
                {
                    'f' => new Instruction(InstructionType.Half, ParseReg(line)),
                    'l' => new Instruction(InstructionType.Triple, ParseReg(line)),
                    'c' => new Instruction(InstructionType.Increment, ParseReg(line)),
                    'p' => new Instruction(InstructionType.Jump, -1, int.Parse(line[4..])),
                    'e' => new Instruction(InstructionType.JumpIfEven, ParseReg(line), int.Parse(line[7..])),
                    'o' => new Instruction(InstructionType.JumpIfOne, ParseReg(line), int.Parse(line[7..])),
                    _ => default!
                };

                instructions.Add(instruction);
            }

            Instruction[] instructionArr = instructions.ToArray();

            int part1 = Simulate(instructionArr, 0);
            int part2 = Simulate(instructionArr, 1);
            return new Solution(part1, part2);
        }

        private static int Simulate(Instruction[] instructions, int aStart)
        {
            int[] variables = new int[2];
            variables[0] = aStart;

            int i = 0;
            while (i >= 0 && i < instructions.Length)
            {
                Instruction instruction = instructions[i];
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
}
