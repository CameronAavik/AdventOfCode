using System;

namespace AdventOfCode.CSharp.Common;

public static class SolverUtils
{
    public static (int Year, int Day) GetYearAndDay<TSolver>() where TSolver : ISolver
    {
        return GetYearAndDay(typeof(TSolver));
    }

    public static (int Year, int Day) GetYearAndDay(Type solverType)
    {
        int year = int.Parse(solverType.Namespace!.Split('.')[2][1..]);
        int dayNumber = int.Parse(solverType.Name[3..]);
        return (year, dayNumber);
    }
}
