using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2021;
int day = 3;

string input = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);

var parse = (string s) =>
{
    return s;
};

var lines = input.TrimEnd('\n').Split('\n').Select(parse).ToArray();

var width = lines[0].Length;
var oxygen = "";
var co2 = "";

var oxygens = lines;
var co2s = lines.ToArray();

for (int i = 0; i < width; i++)
{
    var oxygenOnes = 0;
    var total = 0;
    foreach (var line in lines)
    {
        if (line.StartsWith(oxygen))
        {
            if (line[i] == '1')
                oxygenOnes++;
            total++;
        }
    }

    if (total == 1)
    {
        oxygen += oxygenOnes == 1 ? "1" : "0";
    }
    else if (oxygenOnes >= total - oxygenOnes)
    {
        oxygen += "1";
    }
    else
    {
        oxygen += "0";
    }

    var c02Ones = 0;
    total = 0;
    foreach (var line in lines)
    {
        if (line.StartsWith(co2))
        {
            if (line[i] == '1')
                c02Ones++;
            total++;
        }
    }

    if (total == 1)
    {
        co2 += c02Ones == 1 ? "1" : "0";
    }
    else if (c02Ones * 2 == total)
    {
        co2 += "0";
    }
    else if (c02Ones < total - c02Ones)
    {
        co2 += "1";
    }
    else
    {
        co2 += "0";
    }
}

var gammaVal = Convert.ToInt32(oxygen, 2);
var epsilonVal = Convert.ToInt32(co2, 2);
Console.WriteLine(gammaVal * epsilonVal);


for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
}

Console.WriteLine(lines.Length);
