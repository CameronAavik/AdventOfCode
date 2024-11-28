using System;
using System.IO;
using System.Linq;
using AdventOfCode.CSharp.Benchmarks;
using AdventOfCode.CSharp.Common;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

#if DEBUG
var config = new DebugInProcessConfig();
#else
IConfig config = DefaultConfig.Instance;
#endif
System.Collections.Generic.IEnumerable<Summary> results = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

var reports = results.Select(ConvertSummaryToReport).ToList();

using var writer = new StreamWriter(File.Create("Benchmarks.md"));

foreach (IGrouping<int, BenchmarkReport> group in reports.GroupBy(r => r.Year))
{
    writer.WriteLine($"## {group.Key}");
    writer.WriteLine();
    writer.WriteLine("| Day | P0 | P50 | P100 | Allocations |");
    writer.WriteLine("|--- |---:|---:|---:|---:|");
    foreach (BenchmarkReport? r in group)
    {
        writer.WriteLine($"| **{r.Day?.ToString() ?? "All"}** | **{r.P0:N2} μs** | **{r.P50:N2} μs** | **{r.P100:N2} μs** | **{FormatAllocations(r.Allocations)}** |");
    }

    writer.WriteLine();
}

BenchmarkReport ConvertSummaryToReport(Summary summary)
{
    BenchmarkDotNet.Reports.BenchmarkReport report = summary.Reports[0];
    Type benchmarkClassType = report.BenchmarkCase.Descriptor.Type;

    int year;
    int? day;
    if ((benchmarkClassType.BaseType?.IsGenericType ?? false) && (benchmarkClassType.BaseType.GetGenericTypeDefinition() == typeof(SolverBenchmarkBase<>) || benchmarkClassType.BaseType.GetGenericTypeDefinition() == typeof(MultiInputSolverBenchmarkBase<>)))
    {
        Type solverType = benchmarkClassType.GenericTypeArguments[0];
        (year, day) = SolverUtils.GetYearAndDay(solverType);
    }
    else
    {
        // assume that otherwise it is the 2023 All Days (I will fix this to support other years later)
        year = 2023;
        day = null;
    }

    BenchmarkDotNet.Mathematics.PercentileValues percentiles = report.ResultStatistics!.Percentiles;
    double allocations = report.Metrics.Single(m => m.Key.Equals("Allocated Memory")).Value.Value;
    return new BenchmarkReport(year, day, percentiles.P0 / 1000, percentiles.P50 / 1000, percentiles.P100 / 1000, allocations);
}

string FormatAllocations(double allocations) => allocations switch
{
    0 => "--",
    < 1_000 => $"{(int)allocations} B",
    < 1_000_000 => $"{allocations / 1_000:F1} KB",
    _ => $"{allocations / 1_000_000:N1} MB",
};

record BenchmarkReport(int Year, int? Day, double P0, double P50, double P100, double Allocations);
