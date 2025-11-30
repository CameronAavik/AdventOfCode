using System;
using System.Globalization;

namespace AdventOfCode.CSharp.Common;

public static class SolverUtils
{
    public static (int Year, int Day) GetYearAndDay<TSolver>() where TSolver : ISolver
    {
        return GetYearAndDay(typeof(TSolver));
    }

    public static (int Year, int Day) GetYearAndDay(Type solverType)
    {
        var year = int.Parse(solverType.Namespace!.Split('.')[2][1..], CultureInfo.InvariantCulture);
        var dayNumber = int.Parse(solverType.Name[3..], CultureInfo.InvariantCulture);
        return (year, dayNumber);
    }
}
