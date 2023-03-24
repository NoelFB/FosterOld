﻿using System;
using System.Numerics;

namespace Foster.Framework;

/// <summary>
/// A 2D Integer Point
/// </summary>
public struct Point2 : IEquatable<Point2>
{
    public static readonly Point2 Zero = new(0, 0);
    public static readonly Point2 UnitX = new(1, 0);
    public static readonly Point2 UnitY = new(0, 1);
    public static readonly Point2 One = new(1, 1);
    public static readonly Point2 Right = new(1, 0);
    public static readonly Point2 Left = new(-1, 0);
    public static readonly Point2 Up = new(0, -1);
    public static readonly Point2 Down = new(0, 1);

    /// <summary>
    /// The X component of the Point
    /// </summary>
    public int X;

    /// <summary>
    /// The Y component of the Point
    /// </summary>
    public int Y;

    /// <summary>
    /// Creates a Point with the given X and Y components
    /// </summary>
    public Point2(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the Length of the Point
    /// </summary>
    public float Length()
    {
        return new Vector2(X, Y).Length();
    }

    /// <summary>
    /// Gets the Length Squared of the Point
    /// </summary>
    public float LengthSquared()
    {
        return new Vector2(X, Y).LengthSquared();
    }

    /// <summary>
    /// Gets the Normalized Vector of the Point
    /// </summary>
    public Vector2 Normalized()
    {
        return new Vector2(X, Y).Normalized();
    }

    /// <summary>
    /// Floors both axes of the Point2 to the given interval
    /// </summary>
    public Point2 FloorTo(int interval)
    {
        return (this / interval) * interval;
    }

    /// <summary>
    /// Rounds both axes of the Point2 to the given interval
    /// </summary>
    public Point2 RoundTo(int interval)
    {
        return (this / (float)interval).RoundToPoint2() * interval;
    }

    /// <summary>
    /// Returns a Point2 with the X-value of this Point2, but zero Y
    /// </summary>
    public Point2 OnlyX()
    {
        return new(X, 0);
    }

    /// <summary>
    /// Returns a Point2 with the Y-value of this Point2, but zero X
    /// </summary>
    public Point2 OnlyY()
    {
        return new(0, Y);
    }

    /// <summary>
    /// Turns a Point2 to its right perpendicular
    /// </summary>
    public Point2 TurnRight() => new(-Y, X);

    /// <summary>
    /// Turns a Point2 to its left perpendicular
    /// </summary>
    public Point2 TurnLeft() => new(Y, -X);

    /// <summary>
    /// Clamps the point inside the provided range.
    /// </summary>
    public Point2 Clamp(in Point2 min, in Point2 max) =>
        new Point2(Calc.Clamp(X, min.X, max.X), Calc.Clamp(Y, min.Y, max.Y));

    /// <summary>
    /// Clamps the point inside the bounding rectangle.
    /// </summary>
    public Point2 Clamp(in RectInt bounds) =>
        Clamp(bounds.TopLeft, bounds.BottomRight);

    public override bool Equals(object? obj) => (obj is Point2 other) && (other == this);

    public bool Equals(Point2 other) => (X == other.X && Y == other.Y);

    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = hashCode * 23 + X;
        hashCode = hashCode * 23 + Y;
        return hashCode;
    }

    public override string ToString()
    {
        return $"[{X}, {Y}]";
    }

    public static Point2 Min(Point2 a, Point2 b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    public static Point2 Min(Point2 a, Point2 b, Point2 c) => new(Calc.Min(a.X, b.X, c.X), Calc.Min(a.Y, b.Y, c.Y));
    public static Point2 Min(Point2 a, Point2 b, Point2 c, Point2 d) => new(Calc.Min(a.X, b.X, c.X, d.X), Calc.Min(a.Y, b.Y, c.Y, d.Y));

    public static Point2 Max(Point2 a, Point2 b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
    public static Point2 Max(Point2 a, Point2 b, Point2 c) => new(Calc.Max(a.X, b.X, c.X), Calc.Max(a.Y, b.Y, c.Y));
    public static Point2 Max(Point2 a, Point2 b, Point2 c, Point2 d) => new(Calc.Max(a.X, b.X, c.X, d.X), Calc.Max(a.Y, b.Y, c.Y, d.Y));

    public static int ManhattanDist(in Point2 a, in Point2 b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    public static Point2 FromBools(bool left, bool right, bool up, bool down)
    {
        Point2 result = Point2.Zero;
        if (right) result += Point2.Right;
        if (up) result += Point2.Up;
        if (left) result += Point2.Left;
        if (down) result += Point2.Down;
        return result;
    }

    public static implicit operator Point2((int X, int Y) tuple) => new(tuple.X, tuple.Y);

    public static Point2 operator -(Point2 point) => new(-point.X, -point.Y);
    public static Point2 operator /(Point2 point, int scaler) => new(point.X / scaler, point.Y / scaler);
    public static Point2 operator *(Point2 point, int scaler) => new(point.X * scaler, point.Y * scaler);
    public static Point2 operator %(Point2 point, int scaler) => new(point.X % scaler, point.Y % scaler);

    public static Vector2 operator /(Point2 point, float scaler) => new(point.X / scaler, point.Y / scaler);
    public static Vector2 operator *(Point2 point, float scaler) => new(point.X * scaler, point.Y * scaler);
    public static Vector2 operator %(Point2 point, float scaler) => new(point.X % scaler, point.Y % scaler);

    public static Point2 operator /(Point2 point, Point2 vector) => new(point.X / vector.X, point.Y / vector.Y);
    public static Point2 operator *(Point2 point, Point2 vector) => new(point.X * vector.X, point.Y * vector.Y);
    public static Point2 operator %(Point2 point, Point2 vector) => new(point.X % vector.X, point.Y % vector.Y);

    public static Vector2 operator /(Point2 point, Vector2 vector) => new(point.X / vector.X, point.Y / vector.Y);
    public static Vector2 operator *(Point2 point, Vector2 vector) => new(point.X * vector.X, point.Y * vector.Y);
    public static Vector2 operator %(Point2 point, Vector2 vector) => new(point.X % vector.X, point.Y % vector.Y);

    public static Point2 operator +(Point2 a, Point2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Point2 operator -(Point2 a, Point2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Rect operator +(Point2 a, Rect b) => new(a.X + b.X, a.Y + b.Y, b.Width, b.Height);
    public static Rect operator +(Rect a, Point2 b) => new(b.X + a.X, b.Y + a.Y, a.Width, a.Height);

    public static Rect operator -(Rect a, Point2 b) => new(a.X - b.X, a.Y - b.Y, a.Width, a.Height);

    public static RectInt operator +(Point2 a, RectInt b) => new(a.X + b.X, a.Y + b.Y, b.Width, b.Height);
    public static RectInt operator +(RectInt a, Point2 b) => new(b.X + a.X, b.Y + a.Y, a.Width, a.Height);

    public static RectInt operator -(RectInt a, Point2 b) => new(a.X - b.X, a.Y - b.Y, a.Width, a.Height);

    public static bool operator ==(Point2 a, Point2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Point2 a, Point2 b) => a.X != b.X || a.Y != b.Y;

    public static explicit operator Point2(Vector2 vector) => new((int)vector.X, (int)vector.Y);
    public static implicit operator Vector2(Point2 point) => new(point.X, point.Y);
}
