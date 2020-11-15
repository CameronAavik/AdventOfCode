using System;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace AdventOfCode.CSharp.Benchmarks
{
    public class CustomColumn : IColumn
    {
        private readonly Func<BenchmarkCase, string> _valueProvider;

        public CustomColumn(string columnName, ColumnCategory category, Func<BenchmarkCase, string> valueProvider)
        {
            _valueProvider = valueProvider;
            ColumnName = columnName;
            Category = category;
        }

        public string Id => nameof(CustomColumn) + "." + ColumnName;
        public string ColumnName { get; }
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => _valueProvider(benchmarkCase);
        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        public ColumnCategory Category { get; }
        public int PriorityInCategory => 0;
        public bool IsNumeric => false;
        public UnitType UnitType => UnitType.Dimensionless;
        public string Legend => $"The {ColumnName} column";
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);
        public override string ToString() => ColumnName;
        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    }
}
