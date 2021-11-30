using System;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2020;
int day = 9;

string input = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);

ISolver solver = AdventRunner.GetSolver(year, day)!;

Solution soln = solver.Solve(input);
Console.WriteLine(soln.Part1);
Console.WriteLine(soln.Part2);
