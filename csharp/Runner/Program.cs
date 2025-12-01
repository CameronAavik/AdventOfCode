using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
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
        var parts = line.Split(' ');
        return new LineData();
    }
}

static class Extensions
{
    extension(string s)
    {
        public List<T> ExtractNumbers<T>() where T : IBinaryInteger<T>
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

        public List<int> ExtractIntegers() => s.ExtractNumbers<int>();
        public List<long> ExtractLongs() => s.ExtractNumbers<long>();
    }

    extension<T>(IEnumerable<T> enumerable)
    {
        public IEnumerable<(int Index, T Item)> Enumerate()
        {
            int i = 0;
            foreach (var item in enumerable)
                yield return (i++, item);
        }
    }
}
