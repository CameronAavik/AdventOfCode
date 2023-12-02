using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2023;
int day = 2;
byte[] inputBytes = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);
string input = Encoding.ASCII.GetString(inputBytes);

List<int> ExtractInts(string s)
{
    var ints = new List<int>();
    foreach (var match in Regex.Matches(s, @"(?:(?<!\d)-)?\d+").ToList())
    {
        ints.Add(int.Parse(match.Value));
    }
    return ints;
}

var parse = (string s) =>
{
    var ints = ExtractInts(s);
    var t = s.Split(':');
    s = t[1];
    var rounds = s.Split(';');
    return rounds.Select(r => r.Split(',').Select(u => u.Trim().Split(' ')));
};

var lines = input.TrimEnd('\n').Split("\n").Select(parse).ToArray();

int ans = 0;
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    var maxs = new Dictionary<string, int>();

    foreach (var round in line)
    {
        foreach (var go in round)
        {
            maxs[go[1]] = Math.Max(maxs.GetValueOrDefault(go[1], 0), int.Parse(go[0]));
        }
    }

    var score = 1;
    foreach (var value in maxs)
    {
        score *= value.Value;
    }

    ans += score;

    if (maxs["red"] <= 12 && maxs["green"] <= 13 && maxs["blue"] <= 14)
    {

        //ans += i + 1;
    }
}

Console.WriteLine(ans);
