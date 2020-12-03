﻿using System;
using System.IO;
using AdventOfCode.CSharp.Y2020.Solvers;

namespace AdventOfCode.CSharp.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = File.ReadAllText("input/2020/day03.txt");
            var solver = new Day03();
            var soln = solver.Solve(inputFile);
            Console.WriteLine(soln.Part1);
            Console.WriteLine(soln.Part2);
        }
    }
}
