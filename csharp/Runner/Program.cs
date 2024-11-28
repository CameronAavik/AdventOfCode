using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2024;
int day = 1;

byte[] inputBytes = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);
string input = Encoding.ASCII.GetString(inputBytes);

var parse = (string s) =>
{
    List<int> ints = ExtractInts(s);
    return s;
};

var lines = input.TrimEnd('\n').Split("\n").Select(parse).ToArray();

int ans = 0;
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
}

Console.WriteLine(ans);

List<int> ExtractInts(string s)
{
    var ints = new List<int>();
    foreach (Match? match in Regex.Matches(s, @"(?:(?<!\d)-)?\d+").ToList())
    {
        ints.Add(int.Parse(match.Value));
    }
    return ints;
}
