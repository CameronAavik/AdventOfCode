using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2021;
int day = 1;

string input = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);

// process lines, edit selector if data needs to be processed
var lines = input.TrimEnd('\n').Split('\n').Select(x => x).ToArray();

int c = 0;

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    c += 1;
}

Console.WriteLine(c);
