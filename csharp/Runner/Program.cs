using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;

int year = 2024;
int day = 1;

byte[] inputBytes = await AdventRunner.GetInputAsync(year, day, fetchIfMissing: true);
string input = Encoding.ASCII.GetString(inputBytes);

var lines = input
    .TrimEnd('\n')
    .Split("\n")
    //.Select(Extensions.ExtractNumbers<int>)
    //.Select(LineData.FromString)
    .ToList();

var ans = 0;
foreach ((var i, var line) in lines.Enumerate()) 
{

}

Console.WriteLine(ans);

// Record for storing parsed line data
record LineData()
{
    public static LineData FromString(string line)
    {
        var sParts = line.Split(' ');
        return new LineData();
    }
}

static class Extensions
{
    public static List<T> ExtractNumbers<T>(this string s) where T : IBinaryInteger<T>
    {
        var numbers = new List<T>();
        foreach (Match? match in Regex.Matches(s, @"(?:(?<!\d)-)?\d+").ToList())
        {
            if (!T.TryParse(match.Value, null, out var number))
                throw new Exception($"Failed to parse number: {match.Value}");
            numbers.Add(number);
        }

        return numbers;
    }

    public static List<int> ExtractIntegers(this string s) => s.ExtractNumbers<int>();
    public static List<long> ExtractLongs(this string s) => s.ExtractNumbers<long>();

    public static IEnumerable<(int Index, T Item)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        int i = 0;
        foreach (var item in enumerable)
            yield return (i++, item);
    }
}
