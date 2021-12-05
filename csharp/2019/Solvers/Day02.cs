using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2019.Common;
using System;

namespace AdventOfCode.CSharp.Y2019.Solvers;

public class Day02 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        ReadOnlySpan<int> intCode = IntCode.ParseFromInput(input);
        int[] memory = intCode.ToArray();

        int part1 = Run(memory, 12, 2);
        solution.SubmitPart1(part1);

        for (int noun = 0; noun < 100; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                // reinitialise memory to intcode
                intCode.CopyTo(memory);

                if (Run(memory, noun, verb) == 19690720)
                {
                    int part2 = 100 * noun + verb;
                    solution.SubmitPart2(part2);
                    return;
                }
            }
        }

        ThrowHelper.ThrowException("Unable to find solution for part 2");
    }

    private static int Run(int[] memory, int noun, int verb)
    {
        memory[1] = noun;
        memory[2] = verb;

        int ip = 0;
        while (memory[ip] != 99)
        {
            int a = memory[ip + 1];
            int b = memory[ip + 2];
            int c = memory[ip + 3];
            switch (memory[ip])
            {
                case 1:
                    memory[c] = memory[b] + memory[a];
                    break;
                case 2:
                    memory[c] = memory[b] * memory[a];
                    break;
            }

            ip += 4;
        }

        return memory[0];
    }
}
