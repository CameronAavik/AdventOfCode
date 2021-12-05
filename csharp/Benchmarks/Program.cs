using AdventOfCode.CSharp.Benchmarks;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

var config = DefaultConfig.Instance
    .AddJob(Job.Default)
    .AddDiagnoser(MemoryDiagnoser.Default)
    .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond))
    .ClearColumns()
    .AddColumnProvider(DefaultColumnProviders.Descriptor)
    .AddColumnProvider(DefaultColumnProviders.Params)
    .AddColumnProvider(DefaultColumnProviders.Metrics)
    .AddColumn(StatisticColumn.P0)
    .AddColumn(StatisticColumn.P50)
    .AddColumn(StatisticColumn.P100);

BenchmarkRunner.Run<Benchmarks>(config);
