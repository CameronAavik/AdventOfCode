namespace AdventOfCode.CSharp.Benchmarks;

public abstract class Y2018Benchmarks : BenchmarkBase
{
    protected Y2018Benchmarks(int day) : base(2018, day) { }

    public class Y2018_D01 : Y2018Benchmarks { public Y2018_D01() : base(1) { } }
    public class Y2018_D02 : Y2018Benchmarks { public Y2018_D02() : base(2) { } }
}

