using System;

namespace AdventOfCode.CSharp.Common;

/// <summary>
/// A helper class for throwing exceptions which helps make the callsite more inline-able
/// </summary>
public static class ThrowHelper
{
    public static void ThrowException(string message) => throw new Exception(message);

    public static void ThrowArgumentException(string message, string arg) => throw new ArgumentException(message, arg);

    public static void ThrowArgumentOutOfRangeException(string arg) => throw new ArgumentOutOfRangeException(arg);

    public static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);
}
