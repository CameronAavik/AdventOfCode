using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Runner
{
    class Program
    {
        static void Main()
        {
            int year = 2020;
            int day = 9;

            ISolver solver = AdventRunner.GetSolver(year, day)!;
            string input = AdventRunner.GetInput(year, day);

            Solution soln = solver.Solve(input);
            Console.WriteLine(soln.Part1);
            Console.WriteLine(soln.Part2);
        }
    }
}
