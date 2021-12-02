using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2021;
int day = 2;

string input = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);

var lines = input.TrimEnd('\n').Split('\n').Select(x =>
{
    // process line here
    return x;
}).ToArray();

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
}

Console.WriteLine();
