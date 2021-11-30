using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace AdventOfCode.CSharp.Benchmarks;

public static class ManualConfigExtensions
{
    public static ManualConfig ClearColumns(this ManualConfig config)
    {
        var columnProviders =
            typeof(ManualConfig)!
                .GetField("columnProviders", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(config) as List<IColumnProvider>;

        columnProviders!.Clear();

        return config;
    }
}
