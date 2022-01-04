using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2021;
int day = 3;

byte[] inputBytes = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);
string input = Encoding.ASCII.GetString(inputBytes);

var parse = (string s) =>
{
    return s;
};

var lines = input.TrimEnd('\n').Split("\n\n").Select(parse).ToArray();

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
}

Console.WriteLine();
