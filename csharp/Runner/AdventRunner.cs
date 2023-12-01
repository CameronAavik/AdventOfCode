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
            return await File.ReadAllBytesAsync(filename);

        if (!fetchIfMissing || s_cookie == null || s_inputCacheFolder == null)
            throw new Exception("Unable to load input for year and day");

        var baseAddress = new Uri("https://adventofcode.com");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler) { BaseAddress = baseAddress };
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("github.com/CameronAavik/AdventOfCode");
        cookieContainer.Add(baseAddress, new Cookie("session", s_cookie));
        byte[] inputData = await client.GetByteArrayAsync($"/{year}/day/{day}/input");

        await File.WriteAllBytesAsync(Path.Combine(s_inputCacheFolder, filename), inputData);

        return inputData;
    }

    public static ISolver? GetSolver(int year, int day)
    {
        if (GetSolverType(year, day) is Type type)
        {
            return (ISolver?)Activator.CreateInstance(type);
        }

        return null;
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
