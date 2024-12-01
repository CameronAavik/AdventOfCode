using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Runner;

public static class AdventRunner
{
    private static readonly string? s_cookie;
    private static readonly string? s_inputCacheFolder;

    static AdventRunner()
    {
        if (File.Exists("settings.local.json"))
        {
            string settingsJsonString = File.ReadAllText("settings.local.json");
            JsonElement settings = JsonSerializer.Deserialize<JsonElement>(settingsJsonString);
            s_cookie = settings.GetProperty("SessionCookie").GetString();
            s_inputCacheFolder = settings.GetProperty("InputCacheFolder").GetString();
        }
        else
        {
            s_cookie = null;
        }
    }

    public static async Task<byte[]> GetInputAsync(int year, int day, bool fetchIfMissing = false)
    {
        string filename = $"input/{year}/day{day:D2}.txt";
        if (File.Exists(filename))
        {
            var contents = await File.ReadAllBytesAsync(filename);
            if (contents.Length > 0)
                return contents;
        }

        if (!fetchIfMissing || s_cookie == null || s_inputCacheFolder == null)
            throw new Exception("Unable to load input for year and day");

        var baseAddress = new Uri("https://adventofcode.com");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler) { BaseAddress = baseAddress };
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("https://github.com/CameronAavik/AdventOfCode");
        cookieContainer.Add(baseAddress, new Cookie("session", s_cookie));
        byte[] inputData = await client.GetByteArrayAsync($"/{year}/day/{day}/input");

        await File.WriteAllBytesAsync(Path.Combine(s_inputCacheFolder, filename), inputData);

        return inputData;
    }

    public delegate void SolverDelegate(ReadOnlySpan<byte> input, Solution solution);

    public static void RunSolver(int year, int day, ReadOnlySpan<byte> input, out string solution1, out string solution2)
    {
        SolverDelegate solver = GetSolverMethod(year, day);
        char[] buffer1 = new char[64];
        char[] buffer2 = new char[64];

        var solution = new Solution(buffer1, buffer2);
        solver(input, solution);

        int buffer1Newline = Array.IndexOf(buffer1, '\n');
        solution1 = buffer1Newline == -1 ? string.Empty : buffer1.AsSpan().Slice(0, buffer1Newline).ToString();

        int buffer2Newline = Array.IndexOf(buffer2, '\n');
        solution2 = buffer2Newline == -1 ? string.Empty : buffer2.AsSpan().Slice(0, buffer2Newline).ToString();
    }

    public static SolverDelegate GetSolverMethod(int year, int day)
    {
        Type solverType = GetSolverType(year, day) ?? throw new Exception();
        MethodInfo? method = solverType.GetMethod("Solve", BindingFlags.Static | BindingFlags.Public, [typeof(ReadOnlySpan<byte>), typeof(Solution)]);
        return method?.CreateDelegate<SolverDelegate>() ?? throw new Exception();
    }

    public static Type? GetSolverType(int year, int day)
    {
        Assembly? assembly = year switch
        {
            2015 => typeof(Y2015.Solvers.Day01).Assembly,
            2016 => typeof(Y2016.Solvers.Day01).Assembly,
            2017 => typeof(Y2017.Solvers.Day01).Assembly,
            2018 => typeof(Y2018.Solvers.Day01).Assembly,
            2019 => typeof(Y2019.Solvers.Day01).Assembly,
            2020 => typeof(Y2020.Solvers.Day01).Assembly,
            2021 => typeof(Y2021.Solvers.Day01).Assembly,
            2022 => typeof(Y2022.Solvers.Day01).Assembly,
            2023 => typeof(Y2023.Solvers.Day01).Assembly,
            2024 => typeof(Y2024.Solvers.Day01).Assembly,
            _ => null
        };

        if (assembly != null)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.Name == $"Day{day:D2}")
                {
                    return t;
                }
            }
        }

        return null;
    }
}
