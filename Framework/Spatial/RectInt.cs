using System;
using System.Runtime.CompilerServices;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Integer Rectangle
    /// </summary>
    public struct RectInt
    {

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Point2 Position
        {
            get => new Point2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Point2 Size
        {
            get => new Point2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public int MinX
        {
            get => X;
            set
            {
                Width += (X - value);
                X = value;
            }
        }

        public int MaxX
        {
            get => X + Width;
            set => Width = value - X;
        }

        public int MinY
        {
            get => Y;
            set
            {
                Height += (Y - value);
                Y = value;
            }
        }

        public int MaxY
        {
            get => Y + Height;
            set => Height = value - Y;
        }

        public Point2 A
        {
            get => new Point2(MinX, MinY);
            set
            {
                MinX = value.X;
                MinY = value.Y;
            }
        }

        public Point2 B
        {
            get => new Point2(MaxX, MinY);
            set
            {
                MaxX = value.X;
                MinY = value.Y;
            }
        }

        public Point2 C
        {
            get => new Point2(MaxX, MaxY);
            set
            {
                MaxX = value.X;
                MaxY = value.Y;
            }
        }

        public Point2 D
        {
            get => new Point2(MinX, MaxY);
            set
            {
                MinX = value.X;
                MaxY = value.Y;
            }
        }

        public Point2 Center => new Point2(X + Width / 2, Y + Height / 2);

        public int Area => Width * Height;

        public RectInt(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public RectInt(Point2 position, Point2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Point2 point)
        {
            return (point.X >= X && point.Y >= Y && point.X < X + Width && point.Y < Y + Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in RectInt rect)
        {
            return (MinX < rect.MinX && MinY < rect.MinY && MaxY > rect.MaxY && MaxX > rect.MaxX);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in RectInt against)
        {
            return X + Width > against.X && Y + Height > against.Y && X < against.X + against.Width && Y < against.Y + against.Height;
        }

        public RectInt CropTo(in RectInt other)
        {
            if (MinX < other.MinX)
                MinX = other.MinX;
            if (MinY < other.MinY)
                MinY = other.MinY;
            if (MaxX > other.MaxX)
                MaxX = other.MaxX;
            if (MaxY > other.MaxY)
                MaxY = other.MaxY;

            return this;
        }

        public RectInt Inflate(int by)
        {
            return new RectInt(X - by, Y - by, Width + by * 2, Height + by * 2);
        }

        public RectInt Scale(float scale)
        {
            return new RectInt((int)(X * scale), (int)(Y * scale), (int)(Width * scale), (int)(Height * scale));
        }

        public RectInt OverlapRect(in RectInt against)
        {
            if (Overlaps(against))
            {
                return new RectInt
                {
                    MinX = Math.Max(MinX, against.MinX),
                    MinY = Math.Max(MinY, against.MinY),
                    MaxX = Math.Min(MaxX, against.MaxX),
                    MaxY = Math.Min(MaxY, against.MaxY)
                };
            }

            return new RectInt(0, 0, 0, 0);
        }

        public override bool Equals(object? obj) => (obj is RectInt other) && (this == other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X;
            hash = hash * 23 + Y;
            hash = hash * 23 + Width;
            hash = hash * 23 + Height;
            return hash;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Width}, {Height}]";
        }

        public static bool operator ==(RectInt a, RectInt b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(RectInt a, RectInt b)
        {
            return !(a == b);
        }

        public static RectInt operator *(RectInt a, int b)
        {
            return new RectInt(a.X * b, a.Y * b, a.Width * b, a.Height * b);
        }
    }
}
