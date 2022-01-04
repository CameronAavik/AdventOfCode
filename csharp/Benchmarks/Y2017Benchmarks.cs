namespace AdventOfCode.CSharp.Benchmarks;

public abstract class Y2017Benchmarks : BenchmarkBase
{
    protected Y2017Benchmarks(int day) : base(2017, day) { }

    public class Y2017_D01 : Y2017Benchmarks { public Y2017_D01() : base(1) { } }
    public class Y2017_D02 : Y2017Benchmarks { public Y2017_D02() : base(2) { } }
}

