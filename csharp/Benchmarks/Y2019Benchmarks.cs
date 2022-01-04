namespace AdventOfCode.CSharp.Benchmarks;

public abstract class Y2019Benchmarks : BenchmarkBase
{
    protected Y2019Benchmarks(int day) : base(2019, day) { }

    public class Y2019_D01 : Y2019Benchmarks { public Y2019_D01() : base(1) { } }
    public class Y2019_D02 : Y2019Benchmarks { public Y2019_D02() : base(2) { } }
}

