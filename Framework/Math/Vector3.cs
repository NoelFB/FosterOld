using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public struct Vector3
    {

        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Left = new Vector3(-1, 0, 0);
        public static readonly Vector3 Up = new Vector3(0, -1, 0);
        public static readonly Vector3 Down = new Vector3(0, 1, 0);
        public static readonly Vector3 Forward = new Vector3(0, 0, -1);
        public static readonly Vector3 Backward = new Vector3(0, 0, 1);

        public float X;
        public float Y;
        public float Z;

        public Vector3(float xyz)
        {
            X = Y = Z = xyz;
        }

        public Vector3(float x, float y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector2 vec2, float z = 0f)
        {
            X = vec2.X;
            Y = vec2.Y;
            Z = z;
        }

        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public float Length => Calc.Sqrt(LengthSquared);
        public float LengthSquared => (X * X + Y * Y + Z * Z);
        public Vector3 Normalized => Normalize(this);

        public override bool Equals(object? obj) => (obj is Vector3 other) && (this == other);

        public Point3 Floor()
        {
            return new Point3((int)Math.Floor(X), (int)Math.Floor(Y), (int)Math.Floor(Z));
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            return hash;
        }

        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.X * vector2.X +
                   vector1.Y * vector2.Y +
                   vector1.Z * vector2.Z;
        }

        public static Vector3 Normalize(Vector3 vec)
        {
            var len = vec.Length;
            if (len > 0)
                return vec / len;
            return Zero;
        }

        public static Vector3 Project(Vector3 a, Vector3 b)
        {
            return b * (Dot(a, b) / b.Length);
        }

        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(
                vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                vector1.Z * vector2.X - vector1.X * vector2.Z,
                vector1.X * vector2.Y - vector1.Y * vector2.X);
        }

        public static Vector3 Transform(Vector3 position, Matrix4x4 matrix)
        {
            return new Vector3(
                position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41,
                position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42,
                position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43);
        }

        public static Vector3 Transform(Vector3 value, Quaternion rotation)
        {
            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;

            float wx2 = rotation.W * x2;
            float wy2 = rotation.W * y2;
            float wz2 = rotation.W * z2;
            float xx2 = rotation.X * x2;
            float xy2 = rotation.X * y2;
            float xz2 = rotation.X * z2;
            float yy2 = rotation.Y * y2;
            float yz2 = rotation.Y * z2;
            float zz2 = rotation.Z * z2;

            return new Vector3(
                value.X * (1.0f - yy2 - zz2) + value.Y * (xy2 - wz2) + value.Z * (xz2 + wy2),
                value.X * (xy2 + wz2) + value.Y * (1.0f - xx2 - zz2) + value.Z * (yz2 - wx2),
                value.X * (xz2 - wy2) + value.Y * (yz2 + wx2) + value.Z * (1.0f - xx2 - yy2));
        }

        public static implicit operator Vector3(Point2 point) => new Vector3(point.X, point.Y, 0);
        public static implicit operator Vector3(Point3 point) => new Vector3(point.X, point.Y, point.Z);
        public static implicit operator Vector3(Vector2 vec) => new Vector3(vec.X, vec.Y, 0);
        public static implicit operator Vector3(Vector4 vec) => new Vector3(vec.X, vec.Y, vec.Z);

        public static Vector3 operator -(Vector3 a) => new Vector3(-a.X, -a.Y, -a.Z);
        public static Vector3 operator /(Vector3 vec, float scaler) => new Vector3(vec.X / scaler, vec.Y / scaler, vec.Z / scaler);
        public static Vector3 operator *(float scaler, Vector3 vec) => new Vector3(vec.X * scaler, vec.Y * scaler, vec.Z * scaler);
        public static Vector3 operator *(Vector3 vec, float scaler) => new Vector3(vec.X * scaler, vec.Y * scaler, vec.Z * scaler);
        public static Vector3 operator %(Vector3 vec, float scaler) => new Vector3(vec.X % scaler, vec.Y % scaler, vec.Z % scaler);

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Vector3 a, Vector3 b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }
}
