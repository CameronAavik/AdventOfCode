using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day20 : ISolver
{
    /**
     * This solution is quite cheaty to be honest as it directly exploits how the inputs are structured and will not 
     * solve any kind of general case. I suppose a less cheaty implementation would try and guess which conjunction 
     * nodes appear to be cyclic in nature and then perform an LCM (or CRT?) on those cycles to determine what the 
     * value of rx is, but even then that's still assuming a lot of things so I'm just going to stick with this.
     */
    public enum ModuleType : byte { FlipFlop, Conjunction, Broadcast, Final }
    record Module(ModuleType Type, List<int> Destinations, List<int> Senders);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Dictionary<int, Module> modules = new(64);
        var rxModule = new Module(ModuleType.Final, [], new(4));
        modules['r' << 8 | 'x'] = rxModule;

        while (!input.IsEmpty)
        {
            var type = input[0] switch { (byte)'%' => ModuleType.FlipFlop, (byte)'&' => ModuleType.Conjunction, _ => ModuleType.Broadcast };
            int name;
            if (type == ModuleType.Broadcast)
            {
                name = -1;
                input = input["broadcaster -> ".Length..];
            }
            else
            {
                name = input[1] << 8 | input[2];
                input = input["%jp -> ".Length..];
            }

            var destinations = new List<int>(10);

            var i = 0;
            while (true)
            {
                var c1 = input[i++];
                var c2 = input[i++];
                destinations.Add(c1 << 8 | c2);
                if (input[i++] == '\n')
                    break;
                i++;
            }

            modules[name] = new Module(type, destinations, new(10));
            input = input[i..];
        }

        foreach ((var name, var module) in modules)
            foreach (var destination in module.Destinations)
                modules[destination].Senders.Add(name);

        long lowPulses = 1000; // 1000 because of 1000 button presses
        long highPulses = 0;
        long part2 = 1;
        var nodeBeforeLast = rxModule.Senders[0];

        foreach (var inputNode in modules[nodeBeforeLast].Senders)
        {
            lowPulses += 1000; // For each pulse from the broadcast node

            var inputCenter = modules[inputNode].Senders[0];
            var centerModule = modules[inputCenter];
            var start = modules[centerModule.Senders[0]]; // could be in the middle, we don't know

            var number = 1;
            var cur = start;
            var bit = 2;
            while (true)
            {
                var destinations = cur.Destinations;
                var nextId = destinations[0];
                if (nextId == inputCenter)
                {
                    if (destinations.Count == 1)
                    {
                        number |= bit >> 1; // we reached the end, so set the previous bit
                        break;
                    }
                    nextId = destinations[1];
                }

                cur = modules[nextId];

                if (cur.Destinations.Count == 2)
                    number |= bit;

                bit <<= 1;
            }

            cur = start;
            while (true)
            {
                var senders = cur.Senders;
                var prevId = senders[0];
                if (prevId == inputCenter)
                    prevId = senders[1];

                if (prevId == -1)
                    break;

                cur = modules[prevId];

                number = (number << 1) | (cur.Destinations.Count - 1);
            }

            part2 *= number;

            var bitsInNumber = 32 - BitOperations.LeadingZeroCount((uint)number);
            var highPerPulseToCenter = bitsInNumber - BitOperations.PopCount((uint)number) + 3; // each pulse to center, sends a high pulse to each zero, and 3 extra modules

            var flips = 1000;
            while (flips > 0)
            {
                var isOneBit = number & 1;
                var numDestinations = isOneBit + 1;
                var numLowPulses = flips / 2;

                // half of the flips are low pulse, the other high pulse
                lowPulses += numDestinations * numLowPulses;
                highPulses += numDestinations * ((flips + 1) / 2);

                // if this is a 1 bit, then each pulse to the center will cause additional pulses
                lowPulses += isOneBit * flips; // 1 low pulse between the modules that connect the center to rx
                highPulses += isOneBit * flips * highPerPulseToCenter;

                number >>= 1;
                flips >>= 1;
            }
        }

        solution.SubmitPart1(lowPulses * highPulses);
        solution.SubmitPart2(part2);
    }
}
