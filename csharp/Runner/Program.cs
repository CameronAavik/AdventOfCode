using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.CSharp.Runner;

int year = 2023;
int day = 16;

byte[] inputBytes = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);
string input = Encoding.ASCII.GetString(inputBytes);

var parse = (string s) =>
{
    var ints = ExtractInts(s);
    return s;
};

var lines = input.TrimEnd('\n').Split("\n").Select(parse).ToArray();


int Count(int xS, int yS, int dxS, int dyS)
{
    var beams = new List<(int X, int Y, int Dx, int Dy)>();
    beams.Add((xS, yS, dxS, dyS));

    var energised = new HashSet<(int, int)>();
    var seen = new HashSet<(int X, int Y, int Dx, int Dy)>();
    while (beams.Count > 0)
    {
        var newBeams = new List<(int X, int Y, int Dx, int Dy)>();
        foreach (var beam in beams)
        {
            if (seen.Contains(beam))
            {
                continue;
            }

            seen.Add(beam);

            (var x, var y, var dx, var dy) = beam;
            if (x >= 0 && x < lines[0].Length && y >= 0 && y < lines.Length)
            {
                energised.Add((x, y));
            }
            else
            {
                continue;
            }

            if (lines[y][x] == '.')
            {
            }
            else if (lines[y][x] == '\\')
            {
                (dx, dy) = (dy, dx);
            }
            else if (lines[y][x] == '/')
            {
                (dx, dy) = (-dy, -dx);
            }
            else if (lines[y][x] == '|')
            {
                if (dy == 0)
                {
                    newBeams.Add((x, y - 1, 0, -1));
                    newBeams.Add((x, y + 1, 0, 1));
                    continue;
                }
            }
            else if (lines[y][x] == '-')
            {
                if (dx == 0)
                {
                    newBeams.Add((x - 1, y, -1, 0));
                    newBeams.Add((x + 1, y, 1, 0));
                    continue;
                }
            }

            x += dx;
            y += dy;
            newBeams.Add((x, y, dx, dy));
        }

        beams = newBeams;
    }

    return energised.Count;
}

var max = 0;
for (int y = 0; y < lines.Length; y++)
{
    max = Math.Max(max, Count(0, y, 1, 0));
    max = Math.Max(max, Count(lines[0].Length - 1, y, -1, 0));
}

for (int x = 0; x < lines[0].Length; x++)
{
    max = Math.Max(max, Count(x, 0, 0, 1));
    max = Math.Max(max, Count(x, lines.Length - 1, 0, -1));
}
Console.WriteLine(max);

List<int> ExtractInts(string s)
{
    var ints = new List<int>();
    foreach (var match in Regex.Matches(s, @"(?:(?<!\d)-)?\d+").ToList())
    {
        ints.Add(int.Parse(match.Value));
    }
    return ints;
}
