using System;

namespace AdventOfCode.CSharp.Common
{
    public readonly struct Vec2 : IEquatable<Vec2>
    {
        public static readonly Vec2 Zero = new Vec2 { X = 0, Y = 0 };

        public int X { get; init; }

        public int Y { get; init; }

        public override bool Equals(object? obj) => obj is Vec2 pos && Equals(pos);

        public bool Equals(Vec2 other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Vec2 left, Vec2 right) => left.Equals(right);

        public static bool operator !=(Vec2 left, Vec2 right) => !(left == right);

        public static Vec2 operator +(Vec2 left, Vec2 right) =>
            new Vec2 { X = left.X + right.X, Y = left.Y + right.Y };

        public static Vec2 operator *(Vec2 left, int right) =>
            new Vec2 { X = left.X * right, Y = left.Y * right };
    }
}
