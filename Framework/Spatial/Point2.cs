using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Integer Point
    /// </summary>
    public struct Point2 : IEquatable<Point2>
    {
        public static readonly Point2 Zero = new Point2(0, 0);
        public static readonly Point2 UnitX = new Point2(1, 0);
        public static readonly Point2 UnitY = new Point2(0, 1);
        public static readonly Point2 One = new Point2(1, 1);
        public static readonly Point2 Right = new Point2(1, 0);
        public static readonly Point2 Left = new Point2(-1, 0);
        public static readonly Point2 Up = new Point2(0, -1);
        public static readonly Point2 Down = new Point2(0, 1);

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

        public static implicit operator Point2((int X, int Y) tuple) => new Point2(tuple.X, tuple.Y);

        public static Point2 operator -(Point2 point) => new Point2(-point.X, -point.Y);
        public static Point2 operator /(Point2 point, int scaler) => new Point2(point.X / scaler, point.Y / scaler);
        public static Point2 operator *(Point2 point, int scaler) => new Point2(point.X * scaler, point.Y * scaler);
        public static Point2 operator %(Point2 point, int scaler) => new Point2(point.X % scaler, point.Y % scaler);

        public static Vector2 operator /(Point2 point, float scaler) => new Vector2(point.X / scaler, point.Y / scaler);
        public static Vector2 operator *(Point2 point, float scaler) => new Vector2(point.X * scaler, point.Y * scaler);
        public static Vector2 operator %(Point2 point, float scaler) => new Vector2(point.X % scaler, point.Y % scaler);

        public static Vector2 operator /(Point2 point, Vector2 vector) => new Vector2(point.X / vector.X, point.Y / vector.Y);
        public static Vector2 operator *(Point2 point, Vector2 vector) => new Vector2(point.X * vector.X, point.Y * vector.Y);
        public static Vector2 operator %(Point2 point, Vector2 vector) => new Vector2(point.X % vector.X, point.Y % vector.Y);

        public static Point2 operator +(Point2 a, Point2 b) => new Point2(a.X + b.X, a.Y + b.Y);
        public static Point2 operator -(Point2 a, Point2 b) => new Point2(a.X - b.X, a.Y - b.Y);

        public static Rect operator +(Point2 a, Rect b) => new Rect(a.X + b.X, a.Y + b.Y, b.Width, b.Height);
        public static Rect operator +(Rect a, Point2 b) => new Rect(b.X + a.X, b.Y + a.Y, a.Width, a.Height);

        public static Rect operator -(Rect a, Point2 b) => new Rect(a.X - b.X, a.Y - b.Y, a.Width, a.Height);

        public static RectInt operator +(Point2 a, RectInt b) => new RectInt(a.X + b.X, a.Y + b.Y, b.Width, b.Height);
        public static RectInt operator +(RectInt a, Point2 b) => new RectInt(b.X + a.X, b.Y + a.Y, a.Width, a.Height);

        public static RectInt operator -(RectInt a, Point2 b) => new RectInt(a.X - b.X, a.Y - b.Y, a.Width, a.Height);

        public static bool operator ==(Point2 a, Point2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point2 a, Point2 b) => a.X != b.X || a.Y != b.Y;

        public static explicit operator Point2(Vector2 vector) => new Point2((int)vector.X, (int)vector.Y);
        public static implicit operator Vector2(Point2 point) => new Vector2(point.X, point.Y);

    }
}
