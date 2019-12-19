using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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

        public static Rect Bounds(Rect viewport, Matrix2D matrix)
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

        public static float AngleApproach(float val, float target, float maxMove)
        {
            var diff = AngleDiff(val, target);
            if (Math.Abs(diff) < maxMove)
                return target;
            return val + Clamp(diff, -maxMove, maxMove);
        }

        public static float AngleLerp(float startAngle, float endAngle, float percent)
        {
            return startAngle + AngleDiff(startAngle, endAngle) * percent;
        }

        public static float AngleDiff(float radiansA, float radiansB)
        {
            float diff = radiansB - radiansA;

            while (diff > MathF.PI)
                diff -= MathF.PI * 2;
            while (diff <= -MathF.PI)
                diff += MathF.PI * 2;

            return diff;
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
        private static uint Adler32_Naive(uint value, Span<byte> buf)
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

        public static void Triangulate(IList<Vector2> points, List<int> populate)
        {
            float Area()
            {
                var area = 0f;

                for (int p = points.Count - 1, q = 0; q < points.Count; p = q++)
                {
                    var pval = points[p];
                    var qval = points[q];

                    area += pval.X * qval.Y - qval.X * pval.Y;
                }

                return area * 0.5f;
            }

            bool Snip(int u, int v, int w, int n, Span<int> list)
            {
                var a = points[list[u]];
                var b = points[list[v]];
                var c = points[list[w]];

                if (float.Epsilon > (((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X))))
                    return false;

                for (int p = 0; p < n; p++)
                {
                    if ((p == u) || (p == v) || (p == w))
                        continue;

                    if (InsideTriangle(a, b, c, points[list[p]]))
                        return false;
                }

                return true;
            }

            if (points.Count < 3)
                return;

            Span<int> list = (points.Count < 1000 ? stackalloc int[points.Count] : new int[points.Count]);

            if (Area() > 0)
            {
                for (int v = 0; v < points.Count; v++)
                    list[v] = v;
            }
            else
            {
                for (int v = 0; v < points.Count; v++)
                    list[v] = (points.Count - 1) - v;
            }

            var nv = points.Count;
            var count = 2 * nv;

            for (int v = nv - 1; nv > 2;)
            {
                if ((count--) <= 0)
                    return;

                var u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                var w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, list))
                {
                    populate.Add(list[u]);
                    populate.Add(list[v]);
                    populate.Add(list[w]);

                    for (int s = v, t = v + 1; t < nv; s++, t++)
                        list[s] = list[t];

                    nv--;
                    count = 2 * nv;
                }
            }

            populate.Reverse();
        }

        public static List<int> Triangulate(IList<Vector2> points)
        {
            var indices = new List<int>();
            Triangulate(points, indices);
            return indices;
        }

        public static bool InsideTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
        {
            var p0 = c - b;
            var p1 = a - c;
            var p2 = b - a;

            var ap = point - a;
            var bp = point - b;
            var cp = point - c;

            return (p0.X * bp.Y - p0.Y * bp.X >= 0.0f) &&
                   (p2.X * ap.Y - p2.Y * ap.X >= 0.0f) &&
                   (p1.X * cp.Y - p1.Y * cp.X >= 0.0f);
        }

        #endregion

        #region Parsing

        public static Vector2 ParseVector2(ReadOnlySpan<char> span, char delimiter = ',')
        {
            var index = span.IndexOf(delimiter);

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

        #region Utils

        public static string NormalizePath(string path)
        {
            unsafe
            {
                Span<char> temp = stackalloc char[path.Length];
                for (int i = 0; i < path.Length; i++)
                    temp[i] = path[i];
                return NormalizePath(temp).ToString();
            }
        }

        public static Span<char> NormalizePath(Span<char> path)
        {
            for (int i = 0; i < path.Length; i++)
                if (path[i] == '\\') path[i] = '/';

            int length = path.Length;
            for (int i = 1, t = 1, l = length; t < l; i++, t++)
            {
                if (path[t - 1] == '/' && path[t] == '/')
                {
                    i--;
                    length--;
                }
                else
                    path[i] = path[t];
            }

            return path.Slice(0, length);
        }

        public static string RelativePath(string root, string path)
        {
            var start = 0;
            while (start < path.Length && start < root.Length && char.ToLower(path[start]) == char.ToLower(root[start]))
                start++;
            path = path.Substring(0, start);

            return path.Trim('/');
        }

        public static Span<char> RelativePath(Span<char> root, Span<char> path)
        {
            var start = 0;
            while (start < path.Length && start < root.Length && char.ToLower(path[start]) == char.ToLower(root[start]))
                start++;
            path = path.Slice(start);

            return path.Trim('/');
        }

        public static Stream EmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                throw new Exception();

            var path = assembly.GetName().Name + "." + resourceName.Replace('/', '.').Replace('\\', '.');

            var stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
                throw new Exception($"Embedded Resource '{resourceName}' doesn't exist");

            return stream;
        }

        public static string EmbeddedResourceText(string resourceName)
        {
            using var stream = EmbeddedResource(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        #endregion Utils
    }
}
