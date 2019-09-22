using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public static class Calc
    {

        public const float PI = 3.14159265359f;
        public const float HalfPI = 3.14159265359f / 2f;
        public const float TAU = 6.28318530718f;

        public const float RadiansLeft = PI;
        public const float RadiansDown = PI / 2;
        public const float RadiansRight = 0;
        public const float RadiansUp = PI + PI / 2;

        #region Binary  Operations

        public static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static bool IsBitSet(int b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        #endregion

        #region Give Me

        public static T GiveMe<T>(int index, T a, T b)
        {
            return index switch
            {
                0 => a,
                1 => b,
                _ => throw new Exception("Index was out of range!"),
            };
        }

        public static T GiveMe<T>(int index, T a, T b, T c)
        {
            return index switch
            {
                0 => a,
                1 => b,
                2 => c,
                _ => throw new Exception("Index was out of range!"),
            };
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d)
        {
            return index switch
            {
                0 => a,
                1 => b,
                2 => c,
                3 => d,
                _ => throw new Exception("Index was out of range!"),
            };
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e)
        {
            return index switch
            {
                0 => a,
                1 => b,
                2 => c,
                3 => d,
                4 => e,
                _ => throw new Exception("Index was out of range!"),
            };
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
        {
            return index switch
            {
                0 => a,
                1 => b,
                2 => c,
                3 => d,
                4 => e,
                5 => f,
                _ => throw new Exception("Index was out of range!"),
            };
        }

        #endregion

        #region Math

        public static float Approach(float from, float target, float amount)
        {
            if (from > target)
                return Math.Max(from - amount, target);
            else
                return Math.Min(from + amount, target);
        }

        public static float Lerp(float a, float b, float percent)
        {
            return (a + (b - a) * percent);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float YoYo(float value)
        {
            if (value <= .5f)
                return value * 2;
            else
                return 1 - ((value - .5f) * 2);
        }

        public static float Map(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return ((val - min) / (max - min)) * (newMax - newMin) + newMin;
        }

        public static float SineMap(float counter, float newMin, float newMax)
        {
            return Map((float)Math.Sin(counter), 0, 1, newMin, newMax);
        }

        public static float ClampedMap(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return Clamp((val - min) / (max - min), 0, 1) * (newMax - newMin) + newMin;
        }

        public static Rect Bounds(Rect viewport, Matrix3x2 matrix)
        {
            var inverse = matrix.Invert();

            var topleft = Vector2.Transform(new Vector2(viewport.X, viewport.Y), inverse);
            var topright = Vector2.Transform(new Vector2(viewport.Width, viewport.Y), inverse);
            var bottomright = Vector2.Transform(new Vector2(viewport.Width, viewport.Height), inverse);
            var bottomleft = Vector2.Transform(new Vector2(viewport.X, viewport.Height), inverse);

            var left = Math.Min(Math.Min(Math.Min(topleft.X, topright.X), bottomright.X), bottomleft.X);
            var right = Math.Max(Math.Max(Math.Max(topleft.X, topright.X), bottomright.X), bottomleft.X);
            var top = Math.Min(Math.Min(Math.Min(topleft.Y, topright.Y), bottomright.Y), bottomleft.Y);
            var bottom = Math.Max(Math.Max(Math.Max(topleft.Y, topright.Y), bottomright.Y), bottomleft.Y);

            return new Rect(left, top, (right - left), (bottom - top));
        }

        public static float Angle(Vector2 vec)
        {
            return Atan2(vec.Y, vec.X);
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Cos(float d)
        {
            return (float)Math.Cos(d);
        }

        public static float Sin(float a)
        {
            return (float)Math.Sin(a);
        }

        public static float Tan(float a)
        {
            return (float)Math.Tan(a);
        }

        public static float Sqrt(float d)
        {
            return (float)Math.Sqrt(d);
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        /// <summary>
        /// Adler32 checksum algorithm taken from zlib format specification: https://tools.ietf.org/html/rfc1950#section-9
        /// </summary>
        static uint Adler32_Naive(uint value, Span<byte> buf)
        {
            uint s1 = value & 0xffff;
            uint s2 = (value >> 16) & 0xffff;

            for (int n = 0; n < buf.Length; n++)
            {
                s1 += buf[n];
                while (s1 > 65520)
                    s1 -= 65521;

                s2 += s1;
                while (s2 > 65520)
                    s2 -= 65521;
            }

            return ((s2 << 16) + s1);
        }

        /// <summary>
        /// Adler32 checksum algorithm from the zlib library, converted to C# code
        /// https://github.com/madler/zlib
        /// </summary>
        public static unsafe uint Adler32_Z(uint adler, Span<byte> buffer)
        {
            const int BASE = 65521;
            const int NMAX = 5552;

            int len = buffer.Length;
            int n;
            uint sum2;

            sum2 = (adler >> 16) & 0xffff;
            adler &= 0xffff;

            fixed (byte* ptr = buffer)
            {
                byte* buf = ptr;

                if (len == 1)
                {
                    adler += buf[0];
                    if (adler >= BASE)
                        adler -= BASE;
                    sum2 += adler;
                    if (sum2 >= BASE)
                        sum2 -= BASE;
                    return adler | (sum2 << 16);
                }

                if (len < 16)
                {
                    while (len-- > 0)
                    {
                        adler += *buf++;
                        sum2 += adler;
                    }
                    if (adler >= BASE)
                        adler -= BASE;
                    sum2 %= BASE;
                    return adler | (sum2 << 16);
                }

                while (len >= NMAX)
                {
                    len -= NMAX;
                    n = NMAX / 16;
                    do
                    {
                        adler += (buf)[0];
                        sum2 += adler;
                        adler += (buf)[0 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 2];
                        sum2 += adler;
                        adler += (buf)[0 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 4];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 2];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[8];
                        sum2 += adler;
                        adler += (buf)[8 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 2];
                        sum2 += adler;
                        adler += (buf)[8 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 4];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 2];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 2 + 1];
                        sum2 += adler;
                        buf += 16;
                    } while (--n > 0);
                    adler %= BASE;
                    sum2 %= BASE;
                }

                if (len > 0)
                {
                    while (len >= 16)
                    {
                        len -= 16;
                        adler += (buf)[0];
                        sum2 += adler;
                        adler += (buf)[0 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 2];
                        sum2 += adler;
                        adler += (buf)[0 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 4];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 1];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 2];
                        sum2 += adler;
                        adler += (buf)[0 + 4 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[8];
                        sum2 += adler;
                        adler += (buf)[8 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 2];
                        sum2 += adler;
                        adler += (buf)[8 + 2 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 4];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 1];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 2];
                        sum2 += adler;
                        adler += (buf)[8 + 4 + 2 + 1];
                        sum2 += adler;
                        buf += 16;
                    }

                    while (len-- > 0)
                    {
                        adler += *buf++;
                        sum2 += adler;
                    }
                    adler %= BASE;
                    sum2 %= BASE;
                }
            }

            return adler | (sum2 << 16);
        }

        #endregion

        #region Triangulation

        public static bool IsTriangleClockwise(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.X * b.Y + c.X * a.Y + b.X * c.Y - a.X * c.Y - c.X * b.Y - b.X * a.Y) <= 0f;
        }
        
        public static bool IsPointInTriangle(Vector2 t0, Vector2 t1, Vector2 t2, Vector2 p)
        {
            var denominator = ((t1.Y - t2.Y) * (t0.X - t2.X) + (t2.X - t1.X) * (t0.Y - t2.Y));
            var a = ((t1.Y - t2.Y) * (p.X - t2.X) + (t2.X - t1.X) * (p.Y - t2.Y)) / denominator;
            var b = ((t2.Y - t0.Y) * (p.X - t2.X) + (t0.X - t2.X) * (p.Y - t2.Y)) / denominator;
            var c = 1 - a - b;
            
            return (a >= 0f && a < 1f && b >= 0f && b < 1f && c >= 0f && c < 1f);
        }

        public static bool IsPointInPolygon(IList<Vector2> polygon, Vector2 p)
        {
            var result = false;
            var j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < p.Y && polygon[j].Y >= p.Y || 
                    polygon[j].Y < p.Y && polygon[i].Y >= p.Y)
                {
                    if (polygon[i].X + (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < p.X)
                        result = !result;
                }

                j = i;
            }

            return result;
        }

        public static unsafe bool Triangulate(IList<Vector2> polygon, List<Vector2> output)
        {
            // not possible
            if (polygon.Count <= 2)
                return false;

            // a single triangle
            if (polygon.Count == 3)
            {
                output.AddRange(polygon);
                return true;
            }

            // create a temporary list
            var vertsCount = polygon.Count;
            var verts = stackalloc Vector2[vertsCount];
            for (int i = 0; i < vertsCount; i++)
                verts[i] = polygon[i];

            Vector2 Prev(int index) => verts[(index + vertsCount - 1) % vertsCount];
            Vector2 Next(int index) => verts[(index + 1) % vertsCount];

            bool IsEar(int index)
            {
                var a = Prev(index);
                var b = verts[index];
                var c = Next(index);

                if (IsTriangleClockwise(a, b, c))
                    return false;

                for (int i = 0; i < vertsCount; i++)
                {
                    if (IsTriangleClockwise(Prev(i), verts[i], Next(i)))
                    {
                        var p = verts[i];
                        if (IsPointInTriangle(a, b, c, p))
                            return false;
                    }
                }

                return true;
            }

            // build triangles
            while (vertsCount > 3)
            {
                // find the first ear
                int ear = -1;
                for (int i = 0; i < vertsCount; i++)
                {
                    if (IsEar(i))
                    {
                        ear = i;
                        break;
                    }
                }

                // something bad happened
                if (ear < 0)
                    return false;

                // add triangle
                output.Add(Prev(ear));
                output.Add(verts[ear]);
                output.Add(Next(ear));

                // remove this ear
                for (int i = ear; i < vertsCount; i++)
                    verts[i] = verts[i + 1];
                vertsCount--;
            }

            output.Add(verts[0]);
            output.Add(verts[1]);
            output.Add(verts[2]);
            return true;
        }

        public static List<Vector2>? Triangulate(IList<Vector2> polygon)
        {
            var output = new List<Vector2>();
            if (Triangulate(polygon, output))
                return output;
            return null;
        }

        #endregion

        #region Parsing

        public static Vector2 ParseVector2(ReadOnlySpan<char> span, char separator = ',')
        {
            var index = span.IndexOf(separator);

            if (index >= 0)
            {
                var x = span.Slice(0, index);
                var y = span.Slice(index + 1);

                var result = new Vector2();
                if (float.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out result.X) &&
                    float.TryParse(y, NumberStyles.Float, CultureInfo.InvariantCulture, out result.Y))
                    return result;
            }

            return Vector2.Zero;
        }

        #endregion
    }
}
