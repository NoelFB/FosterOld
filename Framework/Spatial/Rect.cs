﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Rectangle
    /// </summary>
    public struct Rect : IConvexShape2D
    {

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public float Area => Math.Abs(Width) * Math.Abs(Height);

        #region Edges

        public float Left
        {
            get => X;
            set => X = value;
        }

        public float Right
        {
            get => X + Width;
            set => X = value - Width;
        }

        public float CenterX
        {
            get => X + Width / 2;
            set => X = value - Width / 2;
        }

        public float Top
        {
            get => Y;
            set => Y = value;
        }

        public float Bottom
        {
            get => Y + Height;
            set => Y = value - Height;
        }

        public float CenterY
        {
            get => Y + Height / 2;
            set => Y = value - Height / 2;
        }

        #endregion

        #region Points

        public Vector2 TopLeft
        {
            get => new Vector2(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopCenter
        {
            get => new Vector2(CenterX, Top);
            set
            {
                CenterX = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopRight
        {
            get => new Vector2(Right, Top);
            set
            {
                Right = value.X;
                Top = value.Y;
            }
        }

        public Vector2 CenterLeft
        {
            get => new Vector2(Left, CenterY);
            set
            {
                Left = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 Center
        {
            get => new Vector2(CenterX, CenterY);
            set
            {
                CenterX = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 CenterRight
        {
            get => new Vector2(Right, CenterY);
            set
            {
                Right = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 BottomLeft
        {
            get => new Vector2(Left, Bottom);
            set
            {
                Left = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomCenter
        {
            get => new Vector2(CenterX, Bottom);
            set
            {
                CenterX = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomRight
        {
            get => new Vector2(Right, Bottom);
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        /// <summary>
        /// Get a position within the rectangle based on x- and y-values 0-1
        /// </summary>
        public Vector2 GetPointIn(float x, float y)
            => new(X + Width * x, Y + Height * y);

        #endregion

        public Rect(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public Rect(float w, float h) : this(0, 0, w, h) { }

        public Rect(Vector2 a, Vector2 b)
        {
            X = Math.Min(a.X, b.X);
            Y = Math.Min(a.Y, b.Y);
            Width = Math.Max(a.X, b.X) - X;
            Height = Math.Max(a.Y, b.Y) - Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Vector2 point)
        {
            return (point.X >= X && point.Y >= Y && point.X < X + Width && point.Y < Y + Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Rect rect)
        {
            return (Left <= rect.Left && Top <= rect.Top && Bottom >= rect.Bottom && Right >= rect.Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in Rect against)
        {
            return X + Width > against.X && Y + Height > against.Y && X < against.X + against.Width && Y < against.Y + against.Height;
        }

        public Rect OverlapRect(in Rect against)
        {
            var overlapX = X + Width > against.X && X < against.X + against.Width;
            var overlapY = Y + Height > against.Y && Y < against.Y + against.Height;

            Rect r = new Rect();

            if (overlapX)
            {
                r.Left = Math.Max(Left, against.Left);
                r.Width = Math.Min(Right, against.Right) - r.Left;
            }

            if (overlapY)
            {
                r.Top = Math.Max(Top, against.Top);
                r.Height = Math.Min(Bottom, against.Bottom) - r.Top;
            }

            return r;
        }

        public RectInt Int()
        {
            return new RectInt((int)X, (int)Y, (int)Width, (int)Height);
        }

        public Rect Inflate(float by)
        {
            return new Rect(X - by, Y - by, Width + by * 2, Height + by * 2);
        }

        public Rect Inflate(float left, float top, float right, float bottom)
        {
            var rect = new Rect(X, Y, Width, Height);
            rect.Left -= left;
            rect.Top -= top;
            rect.Width += left + right;
            rect.Height += top + bottom;
            return rect;
        }

        public Rect Scale(float by)
        {
            return new Rect(X * by, Y * by, Width * by, Height * by);
        }

        public Rect Scale(in Vector2 by)
        {
            return new Rect(X * by.X, Y * by.Y, Width * by.X, Height * by.Y);
        }

        public void Project(in Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            var dot = Vector2.Dot(new Vector2(X, Y), axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(new Vector2(X + Width, Y), axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(new Vector2(X + Width, Y + Height), axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(new Vector2(X, Y + Height), axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }

        public Vector2 ClosestPointTo(Vector2 p)
        {
            byte sides = 0;
            if (p.X < X)
                sides |= 0b0001;
            else if (p.X >= X + Width)
                sides |= 0b0010;
            if (p.Y < Y)
                sides |= 0b0100;
            else if (p.Y >= Y + Height)
                sides |= 0b1000;

            switch (sides)
            {
                default:
                case 0b0000:    // inside rect
                    return p;

                case 0b0001:    // left of rect
                    return new Vector2(X, p.Y);
                case 0b0010:    // right of rect
                    return new Vector2(X + Width, p.Y);
                case 0b0100:    // above rect
                    return new Vector2(p.X, Y);
                case 0b1000:    // below rect
                    return new Vector2(p.X, Y + Height);

                case 0b0101:    // above & left of rect
                    return TopLeft;
                case 0b0110:    // above & right of rect
                    return TopRight;
                case 0b1001:    // below & left of rect
                    return BottomLeft;
                case 0b1010:    // below & right of rect
                    return BottomRight;
            }
        }

        public int Points => 4;

        public Vector2 GetPoint(int index)
        {
            return index switch
            {
                0 => TopLeft,
                1 => TopRight,
                2 => BottomRight,
                3 => BottomLeft,
                _ => throw new IndexOutOfRangeException(),
            };
        }

        public int Axis => 2;

        public Vector2 GetAxis(int index)
        {
            return index switch
            {
                0 => Vector2.UnitX,
                1 => Vector2.UnitY,
                _ => throw new IndexOutOfRangeException(),
            };
        }

        public override bool Equals(object? obj) => (obj is Rect other) && (this == other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Width.GetHashCode();
            hash = hash * 23 + Height.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Width}, {Height}]";
        }

        public static Rect Between(Vector2 a, Vector2 b)
        {
            Rect rect;

            rect.X = a.X < b.X ? a.X : b.X;
            rect.Y = a.Y < b.Y ? a.Y : b.Y;
            rect.Width = (a.X > b.X ? a.X : b.X) - rect.X;
            rect.Height = (a.Y > b.Y ? a.Y : b.Y) - rect.Y;

            return rect;
        }

        public static implicit operator Rect((float X, float Y, float Width, float Height) tuple) => new Rect(tuple.X, tuple.Y, tuple.Width, tuple.Height);

        public static implicit operator Rect(RectInt rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static implicit operator Rect(Vector4 vec)
        {
            return new Rect(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static implicit operator Vector4(Rect rect)
        {
            return new Vector4(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height);
        }

        public static bool operator !=(Rect a, Rect b)
        {
            return !(a == b);
        }

        public static Rect operator *(Rect a, Vector2 scaler)
        {
            return new Rect(a.X * scaler.X, a.Y * scaler.Y, a.Width * scaler.X, a.Height * scaler.Y);
        }

        public static Rect operator +(Rect a, Vector2 b)
        {
            return new Rect(a.X + b.X, a.Y + b.Y, a.Width, a.Height);
        }

        public static Rect operator -(Rect a, Vector2 b)
        {
            return new Rect(a.X - b.X, a.Y - b.Y, a.Width, a.Height);
        }

        public static Rect operator *(Rect a, float scaler)
        {
            return new Rect(a.X * scaler, a.Y * scaler, a.Width * scaler, a.Height * scaler);
        }

        public static Rect operator /(Rect a, float scaler)
        {
            return new Rect(a.X / scaler, a.Y / scaler, a.Width / scaler, a.Height / scaler);
        }
    }
}
