using System;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    /// <summary>
    /// A structure encapsulating a 3x2 matrix.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2D
    {

        public float M11;
        public float M12;
        public float M21;
        public float M22;
        public float M31;
        public float M32;

        public Matrix2D(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
        }

        public Vector2 Translation
        {
            get => new Vector2(M31, M32);
            set
            {
                M31 = value.X;
                M32 = value.Y;
            }
        }

        public Vector2 Scale
        {
            get => new Vector2(M11, M22);
            set
            {
                M11 = value.X;
                M22 = value.Y;
            }
        }

        public Matrix2D Invert()
        {
            var det = (M11 * M22) - (M21 * M12);
            if (det != 0)
            {
                var invDet = 1.0f / det;

                return new Matrix2D()
                {
                    M11 = M22 * invDet,
                    M12 = -M12 * invDet,
                    M21 = -M21 * invDet,
                    M22 = M11 * invDet,
                    M31 = (M21 * M32 - M31 * M22) * invDet,
                    M32 = (M31 * M12 - M11 * M32) * invDet
                };
            }
            else
            {
                return Identity;
            }
        }

        public override bool Equals(object? obj) => (obj is Matrix2D other) && (this == other);

        public override int GetHashCode()
        {
            return M11.GetHashCode() + M12.GetHashCode() +
                   M21.GetHashCode() + M22.GetHashCode() +
                   M31.GetHashCode() + M32.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{M11:{M11}, M12:{M12}, M21:{M21}, M22:{M22}, M31:{M31}, M32:{M32}}}";
        }

        public static readonly Matrix2D Identity = new Matrix2D()
        {
            M11 = 1,
            M12 = 0,
            M21 = 0,
            M22 = 1,
            M31 = 0,
            M32 = 0
        };

        public static Matrix2D Add(in Matrix2D a, in Matrix2D b)
        {
            return new Matrix2D()
            {
                M11 = a.M11 + b.M11,
                M12 = a.M12 + b.M12,
                M21 = a.M21 + b.M21,
                M22 = a.M22 + b.M22,
                M31 = a.M31 + b.M31,
                M32 = a.M21 + b.M32
            };
        }

        public static Matrix2D Subtract(in Matrix2D a, in Matrix2D b)
        {
            return new Matrix2D()
            {
                M11 = a.M11 - b.M11,
                M12 = a.M12 - b.M12,
                M21 = a.M21 - b.M21,
                M22 = a.M22 - b.M22,
                M31 = a.M31 - b.M31,
                M32 = a.M21 - b.M32
            };
        }

        public static Matrix2D Multiply(in Matrix2D a, in Matrix2D b)
        {
            return new Matrix2D()
            {
                M11 = a.M11 * b.M11 + a.M12 * b.M21,
                M12 = a.M11 * b.M12 + a.M12 * b.M22,
                M21 = a.M21 * b.M11 + a.M22 * b.M21,
                M22 = a.M21 * b.M12 + a.M22 * b.M22,
                M31 = a.M31 * b.M11 + a.M32 * b.M21 + b.M31,
                M32 = a.M31 * b.M12 + a.M32 * b.M22 + b.M32
            };
        }

        public static Matrix2D CreateTransform(in Vector2 position, in Vector2 origin, in Vector2 scale, in float rotation)
        {
            Matrix2D matrix;

            if (origin != Vector2.Zero)
                matrix = CreateTranslation(-origin.X, -origin.Y);
            else
                matrix = Identity;

            if (scale != Vector2.One)
                matrix *= CreateScale(scale.X, scale.Y);

            if (rotation != 0)
                matrix *= CreateRotation(rotation);

            if (position != Vector2.Zero)
                matrix *= CreateTranslation(position.X, position.Y);

            return matrix;
        }

        public static Matrix2D CreateTransform(in ITransform2D transform) => CreateTransform(transform.Position, transform.Origin, transform.Scale, transform.Rotation);

        public static Matrix2D CreateTranslation(Vector2 vec2) => CreateTranslation(vec2.X, vec2.Y);
        public static Matrix2D CreateTranslation(float x, float y) => new Matrix2D(1, 0, 0, 1, x, y);

        public static Matrix2D CreateScale(float scale) => CreateScale(scale, scale);
        public static Matrix2D CreateScale(Vector2 vec2) => CreateScale(vec2.X, vec2.Y);
        public static Matrix2D CreateScale(float x, float y) => new Matrix2D(x, 0, 0, y, 0, 0);

        public static Matrix2D CreateRotation(float radians)
        {
            var c = Calc.Cos(radians);
            var s = Calc.Sin(radians);

            return new Matrix2D(c, s, -s, c, 0, 0);
        }

        public static Matrix2D operator *(Matrix2D a, Matrix2D b) => Multiply(a, b);
        public static Matrix2D operator +(Matrix2D a, Matrix2D b) => Add(a, b);
        public static Matrix2D operator -(Matrix2D a, Matrix2D b) => Subtract(a, b);

        public static bool operator ==(Matrix2D a, Matrix2D b)
        {
            return (a.M11 == b.M11 && a.M12 == b.M12 && a.M21 == b.M21 && a.M22 == b.M22 && a.M31 == b.M31 && a.M32 == b.M32);
        }

        public static bool operator !=(Matrix2D a, Matrix2D b)
        {
            return (a.M11 != b.M11 || a.M12 != b.M12 || a.M21 != b.M21 || a.M22 != b.M22 || a.M31 != b.M31 || a.M32 != b.M32);

        }
    }
}
