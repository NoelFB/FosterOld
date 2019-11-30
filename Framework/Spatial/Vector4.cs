using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public struct Vector4
    {
        public static readonly Vector4 Zero = new Vector4(0);

        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float xyzw)
        {
            X = Y = Z = W = xyzw;
        }

        public Vector4(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 0;
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector2 vec2, float z = 0, float w = 0)
        {
            X = vec2.X;
            Y = vec2.Y;
            Z = z;
            W = w;
        }


        public Vector4(Vector3 vec3, float w = 0)
        {
            X = vec3.X;
            Y = vec3.Y;
            Z = vec3.Z;
            W = w;
        }

        public void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        public override bool Equals(object? obj) => (obj is Vector4 other) && (this == other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            hash = hash * 23 + W.GetHashCode();
            return hash;
        }

        public static implicit operator Vector4(Vector2 vec) => new Vector4(vec.X, vec.Y, 0, 0);
        public static implicit operator Vector4(Vector3 vec) => new Vector4(vec.X, vec.Y, vec.Z, 0);
        public static implicit operator Vector4(Point2 point) => new Vector4(point.X, point.Y, 0, 0);
        public static implicit operator Vector4(Point3 point) => new Vector4(point.X, point.Y, point.Z, 0);

        public static bool operator ==(Vector4 a, Vector4 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        public static bool operator !=(Vector4 a, Vector4 b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;

    }
}
