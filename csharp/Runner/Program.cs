using System;
using System.IO;
using System.Reflection;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Runner
{
    class Program
    {
        private static Type? GetSolverType(int year, int day)
        {
            Assembly? assembly = year switch
            {
                2015 => typeof(Y2015.Solvers.Day01).Assembly,
                2016 => typeof(Y2016.Solvers.Day01).Assembly,
                2017 => typeof(Y2017.Solvers.Day01).Assembly,
                2018 => typeof(Y2018.Solvers.Day01).Assembly,
                2019 => typeof(Y2019.Solvers.Day01).Assembly,
                2020 => typeof(Y2020.Solvers.Day01).Assembly,
                _ => null
            };

            if (assembly != null)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.Name == $"Day{day:D2}")
                    {
                        return t;
                    }
                }
            }

            return null;
        }

        static void Main(string[] args)
        {
            int year = 2020;
            int day = 4;

            var solver = (ISolver)Activator.CreateInstance(GetSolverType(year, day)!)!;
            var input = File.ReadAllText($"input/{year}/day{day:D2}.txt");

            // uncomment for profiling
            //for (int i = 0; i < 100000; i++)
            //{
            //    _ = solver.Solve(input);
            //}

            var soln = solver.Solve(input);
            Console.WriteLine(soln.Part1);
            Console.WriteLine(soln.Part2);
        }
    }
}
