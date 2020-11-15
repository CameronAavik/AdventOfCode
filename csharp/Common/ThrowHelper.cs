using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common
{
    /// <summary>
    /// A helper class for throwing exceptions which helps make the callsite more inline-able
    /// </summary>
    public static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowException(string message) => throw new Exception(message);

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowArgumentException(string message, string arg) => throw new ArgumentException(message, arg);

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException(string arg) => throw new ArgumentOutOfRangeException(arg);

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        public static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);



    }
}
