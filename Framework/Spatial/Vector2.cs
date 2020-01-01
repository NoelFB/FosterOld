using System;

namespace Foster.Framework
{
    public struct Vector2
    {

        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Down = new Vector2(0, 1);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 Up = new Vector2(0, -1);

        public float X;
        public float Y;

        public Vector2(float xy)
        {
            X = Y = xy;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        public float Length => Calc.Sqrt(LengthSquared);
        public float LengthSquared => X * X + Y * Y;

        public Vector2 Normalized => Normalize(this);

        public Vector2 TurnRight => new Vector2(-Y, X);

        public Vector2 TurnLeft => new Vector2(Y, -X);

        public float Angle() => Calc.Atan2(Y, X);

        public Point2 Floor() => new Point2((int)Math.Floor(X), (int)Math.Floor(Y));

        public override bool Equals(object? obj) => (obj is Vector2 other) && (this == other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        public static Vector2 Angle(float angle, float length = 1)
        {
            return new Vector2(Calc.Cos(angle) * length, Calc.Sin(angle) * length);
        }

        public static Vector2 Rotate(Vector2 a, float angle)
        {
            var cos = Calc.Cos(angle);
            var sin = Calc.Sin(angle);
            return new Vector2(cos * a.X - sin * a.Y, sin * a.X + cos * a.Y);
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 Normalize(Vector2 vec)
        {
            var len = vec.Length;
            if (len > 0)
                return vec / len;
            return Zero;
        }

        public static Vector2 Project(Vector2 a, Vector2 b)
        {
            return b * (Dot(a, b) / b.Length);
        }

        public static Vector2 Transform(Vector2 vec, Matrix2D matrix)
        {
            return new Vector2(
                (vec.X * matrix.M11) + (vec.Y * matrix.M21) + matrix.M31,
                (vec.X * matrix.M12) + (vec.Y * matrix.M22) + matrix.M32);
        }

        public static Vector2 Approach(Vector2 start, Vector2 target, float maxDelta)
        {
            var diff = target - start;
            if (diff.Length <= maxDelta)
                return target;
            else
                return start + diff.Normalized * maxDelta;
        }

        public static Vector2 ClampedLerp(Vector2 start, Vector2 end, float t)
        {
            if (t <= 0)
                return start;
            if (t >= 1)
                return end;
            else
                return start + (end - start) * t;
        }

        public static implicit operator Vector2(Point2 point) => new Vector2(point.X, point.Y);
        public static implicit operator Vector2(Point3 point) => new Vector2(point.X, point.Y);
        public static implicit operator Vector2(Vector3 vec) => new Vector2(vec.X, vec.Y);
        public static implicit operator Vector2(Vector4 vec) => new Vector2(vec.X, vec.Y);

        public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);
        public static Vector2 operator /(Vector2 vec, float scaler) => new Vector2(vec.X / scaler, vec.Y / scaler);
        public static Vector2 operator /(float scaler, Vector2 vec) => new Vector2(scaler / vec.X, scaler / vec.Y);
        public static Vector2 operator *(Vector2 vec, float scaler) => new Vector2(vec.X * scaler, vec.Y * scaler);
        public static Vector2 operator *(float scaler, Vector2 vec) => new Vector2(vec.X * scaler, vec.Y * scaler);
        public static Vector2 operator %(Vector2 vec, float scaler) => new Vector2(vec.X % scaler, vec.Y % scaler);

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.X / b.X, a.Y / b.Y);

        public static Rect operator +(Vector2 a, Rect b) => new Rect(a.X + b.X, a.Y + b.Y, b.Width, b.Height);
        public static Rect operator +(Rect b, Vector2 a) => new Rect(a.X + b.X, a.Y + b.Y, b.Width, b.Height);

        public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2 a, Vector2 b) => a.X != b.X || a.Y != b.Y;

    }
}
