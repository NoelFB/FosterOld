using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// A 3D Quad
    /// </summary>
    public struct Quad : IProjectable
    {

        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 D;

        public Vector3 Center => (A + B + C + D) / 4f;

        public Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Quad Translate(Vector3 amount)
        {
            A += amount;
            B += amount;
            C += amount;
            D += amount;
            return this;
        }

        public void Project(Vector3 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            var dot = Vector3.Dot(A, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector3.Dot(B, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector3.Dot(C, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector3.Dot(D, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }

        public Box BoundingBox()
        {
            var box = new Box();

            box.Position.X = Math.Min(A.X, Math.Min(B.X, Math.Min(C.X, D.X)));
            box.Position.Y = Math.Min(A.Y, Math.Min(B.Y, Math.Min(C.Y, D.Y)));
            box.Position.Z = Math.Min(A.Z, Math.Min(B.Z, Math.Min(C.Z, D.Z)));

            box.Size.X = Math.Max(A.X, Math.Max(B.X, Math.Max(C.X, D.X))) - box.Position.X;
            box.Size.Y = Math.Max(A.Y, Math.Max(B.Y, Math.Max(C.Y, D.Y))) - box.Position.Y;
            box.Size.Z = Math.Max(A.Z, Math.Max(B.Z, Math.Max(C.Z, D.Z))) - box.Position.Z;

            return box;
        }

        public override bool Equals(object? obj) => (obj is Quad other) && (this == other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + A.GetHashCode();
            hash = hash * 23 + B.GetHashCode();
            hash = hash * 23 + C.GetHashCode();
            hash = hash * 23 + D.GetHashCode();
            return hash;
        }

        public static Quad Transform(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Matrix4x4 matrix)
        {
            return new Quad(
                Vector3.Transform(a, matrix),
                Vector3.Transform(b, matrix),
                Vector3.Transform(c, matrix),
                Vector3.Transform(d, matrix));
        }

        public static Quad Transform(Quad quad, Matrix4x4 matrix)
        {
            return new Quad(
                Vector3.Transform(quad.A, matrix),
                Vector3.Transform(quad.B, matrix),
                Vector3.Transform(quad.C, matrix),
                Vector3.Transform(quad.D, matrix));
        }

        public static bool operator ==(Quad a, Quad b)
        {
            return a.A == b.A && a.B == b.B && a.C == b.C && a.D == b.D;
        }

        public static bool operator !=(Quad a, Quad b)
        {
            return a.A != b.A || a.B != b.B || a.C != b.C || a.D != b.D;
        }
    }
}
