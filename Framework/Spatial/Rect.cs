using System;
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

        public float MinX
        {
            get => X;
            set
            {
                Width += (X - value);
                X = value;
            }
        }

        public float MaxX
        {
            get => X + Width;
            set => Width = value - X;
        }

        public float MinY
        {
            get => Y;
            set
            {
                Height += (Y - value);
                Y = value;
            }
        }

        public float MaxY
        {
            get => Y + Height;
            set => Height = value - Y;
        }

        public Vector2 A
        {
            get => new Vector2(MinX, MinY);
            set
            {
                MinX = value.X;
                MinY = value.Y;
            }
        }

        public Vector2 B
        {
            get => new Vector2(MaxX, MinY);
            set
            {
                MaxX = value.X;
                MinY = value.Y;
            }
        }

        public Vector2 C
        {
            get => new Vector2(MaxX, MaxY);
            set
            {
                MaxX = value.X;
                MaxY = value.Y;
            }
        }

        public Vector2 D
        {
            get => new Vector2(MinX, MaxY);
            set
            {
                MinX = value.X;
                MaxY = value.Y;
            }
        }

        public float Area => Math.Abs(Width) * Math.Abs(Height);

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Center => new Vector2(X + Width / 2, Y + Height / 2);

        public Rect(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

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
            return (MinX <= rect.MinX && MinY <= rect.MinY && MaxY >= rect.MaxY && MaxX >= rect.MaxX);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in Rect against)
        {
            return X + Width > against.X && Y + Height > against.Y && X < against.X + against.Width && Y < against.Y + against.Height;
        }

        public Rect OverlapRect(in Rect against)
        {
            if (Overlaps(against))
            {
                return new Rect
                {
                    MinX = Math.Max(MinX, against.MinX),
                    MinY = Math.Max(MinY, against.MinY),
                    MaxX = Math.Min(MaxX, against.MaxX),
                    MaxY = Math.Min(MaxY, against.MaxY)
                };
            }

            return new Rect(0, 0, 0, 0);
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
            rect.MinX -= left;
            rect.MinY -= top;
            rect.MaxX += right;
            rect.MaxY += bottom;
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

        public int Points => 4;

        public Vector2 GetPoint(int index)
        {
            return index switch
            {
                0 => A,
                1 => B,
                2 => C,
                3 => D,
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

        public static implicit operator Rect(RectInt rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
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
