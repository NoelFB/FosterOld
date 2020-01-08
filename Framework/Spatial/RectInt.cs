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

        public int Left
        {
            get => X;
            set
            {
                Width += (X - value);
                X = value;
            }
        }

        public int Right
        {
            get => X + Width;
            set => Width = value - X;
        }

        public int Top
        {
            get => Y;
            set
            {
                Height += (Y - value);
                Y = value;
            }
        }

        public int Bottom
        {
            get => Y + Height;
            set => Height = value - Y;
        }

        public Point2 TopLeft
        {
            get => new Point2(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Point2 TopRight
        {
            get => new Point2(Right, Top);
            set
            {
                Right = value.X;
                Top = value.Y;
            }
        }

        public Point2 BottomLeft
        {
            get => new Point2(Left, Bottom);
            set
            {
                Left = value.X;
                Bottom = value.Y;
            }
        }

        public Point2 BottomRight
        {
            get => new Point2(Right, Bottom);
            set
            {
                Right = value.X;
                Bottom = value.Y;
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

        public bool Contains(Point2 point)
        {
            return (point.X >= X && point.Y >= Y && point.X < X + Width && point.Y < Y + Height);
        }

        public bool Contains(RectInt rect)
        {
            return (Left < rect.Left && Top < rect.Top && Bottom > rect.Bottom && Right > rect.Right);
        }

        public bool Intersects(RectInt against)
        {
            return X + Width >= against.X && Y + Height >= against.Y && X < against.X + against.Width && Y < against.Y + against.Height;
        }

        public RectInt CropTo(RectInt other)
        {
            if (Left < other.Left)
                Left = other.Left;
            if (Top < other.Top)
                Top = other.Top;
            if (Right > other.Right)
                Right = other.Right;
            if (Bottom > other.Bottom)
                Bottom = other.Bottom;

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

    }
}
