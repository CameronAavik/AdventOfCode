using System;
using System.IO;
using System.Reflection;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Runner
{
    public static class AdventRunner
    {
        public static string GetInput(int year, int day)
        {
            return File.ReadAllText($"input/{year}/day{day:D2}.txt");
        }

        public static ISolver? GetSolver(int year, int day)
        {
            if (GetSolverType(year, day) is Type type)
            {
                return (ISolver?)Activator.CreateInstance(type);
            }

            return null;
        }

        public static Type? GetSolverType(int year, int day)
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
    }
}
