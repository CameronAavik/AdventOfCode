using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day08: ISolver
{
    /**
     * This solver does not solve the general case, but will work for all actual AoC inputs.
     * It seems that there are a number of properties that are in common among all the inputs that make this problem much simpler:
     * 
     * 1. Each ghost goes in separate loops.
     * 2. Each loop contains only one __Z node.
     * 3. The number of nodes traversed to get into the loop from the __A node is the same number of nodes needed to get to that same node from the __Z node.
     * 4. The number of steps taken to get to the __Z node always aligns with the number of steps in the input
     * 5. All __Z nodes are part of loops
     **/

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int stepsEndIndex = input.IndexOf((byte)'\n');
        ReadOnlySpan<byte> stepsSpan = input.Slice(0, stepsEndIndex);
        ReadOnlySpan<byte> mapsSpan = input.Slice(stepsEndIndex + 2);
        int numMaps = mapsSpan.Count((byte)'\n');
        int lineLength = "AAA = (BBB, CCC)\n".Length;

        ulong[] steps = new ulong[stepsSpan.Length / 64 + 1];
        int stepsInLastBlock = stepsSpan.Length % 64;
        for (int i = 0; i < stepsSpan.Length; i++)
        {
            if (stepsSpan[i] == 'R')
                steps[i / 64] |= 1UL << (i % 64);
        }

        var startNodes = new List<uint>(8);
        var mappings = new Dictionary<uint, uint>(numMaps);
        for (int i = 0; i < numMaps; i++)
        {
            ReadOnlySpan<byte> nodeSpan = mapsSpan.Slice(i * lineLength, 3);
            uint nodeId = NodeSpanToId(nodeSpan);
            mappings[nodeId] = (uint)i;
            if (nodeSpan[2] == 'Z')
                startNodes.Add(nodeId);
        }

        uint[] leftPaths = new uint[numMaps];
        uint[] rightPaths = new uint[numMaps];
        uint[][] paths = [leftPaths, rightPaths];

        for (int i = 0; i < numMaps; i++)
        {
            ReadOnlySpan<byte> lineSpan = mapsSpan.Slice(i * lineLength, lineLength);

            uint leftNodeId = NodeSpanToId(lineSpan.Slice("AAA = (".Length, 3));
            leftPaths[i] = mappings[leftNodeId];

            uint rightNodeId = NodeSpanToId(lineSpan.Slice("AAA = (BBB, ".Length, 3));
            rightPaths[i] = mappings[rightNodeId];
        }

        uint part1StartId = NodeSpanToId("ZZZ"u8);
        ulong part2 = 1;
        foreach (uint startNode in startNodes)
        {
            uint startNodeIndex = mappings[startNode];
            ulong stepsToLoop = GetStepsToLoop(steps, paths, startNodeIndex, stepsInLastBlock);
            if (startNode == part1StartId)
                solution.SubmitPart1(stepsToLoop);
            part2 = LeastCommonMultiple(part2, stepsToLoop);
        }

        solution.SubmitPart2(part2);
    }

    private static uint NodeSpanToId(ReadOnlySpan<byte> nodeSpan) => (uint)((nodeSpan[0] << 16) | (nodeSpan[1] << 8) | nodeSpan[2]);

    private static ulong GetStepsToLoop(ulong[] steps, uint[][] paths, uint startNodeIndex, int stepsInLastBlock)
    {
        uint endNodeIndex = startNodeIndex;
        ulong i = 0;
        while (true)
        {
            for (int j = 0; j < steps.Length - 1; j++)
            {
                ulong stepsBlock = steps[j];
                for (int step = 0; step < 64; step++)
                {
                    startNodeIndex = paths[stepsBlock & 1][startNodeIndex];
                    stepsBlock >>= 1;
                    i++;
                }
            }

            ulong lastStepsBlock = steps[steps.Length - 1];
            for (int step = 0; step < stepsInLastBlock; step++)
            {
                startNodeIndex = paths[lastStepsBlock & 1][startNodeIndex];
                lastStepsBlock >>= 1;
                i++;
            }

            if (startNodeIndex == endNodeIndex)
                return i;
        }
    }

    private static ulong LeastCommonMultiple(ulong a, ulong b)
    {
        static ulong Gcd(ulong left, ulong right)
        {
            while (right != 0)
            {
                ulong temp = left % right;
                left = right;
                right = temp;
            }

            return left;
        }

        return (a / Gcd(a, b)) * b;
    }
}
