using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 4)]
    public struct Color
    {

        public static readonly Color Transparent = new Color(0x000000) * 0.0f;
        public static readonly Color White = new Color(0xffffff);
        public static readonly Color Black = new Color(0x000000);
        public static readonly Color Red = new Color(0xff0000);
        public static readonly Color Green = new Color(0x00ff00);
        public static readonly Color Blue = new Color(0x0000ff);
        public static readonly Color Yellow = new Color(0xffff00);

        public uint ABGR;

        public byte R
        {
            get { unchecked { return (byte)(ABGR); } }
            set => ABGR = (ABGR & 0xffffff00) | value;
        }

        public byte G
        {
            get { unchecked { return (byte)(ABGR >> 8); } }
            set => ABGR = (ABGR & 0xffff00ff) | ((uint)value << 8);
        }

        public byte B
        {
            get { unchecked { return (byte)(ABGR >> 16); } }
            set => ABGR = (ABGR & 0xff00ffff) | ((uint)value << 16);
        }

        public byte A
        {
            get { unchecked { return (byte)(ABGR >> 24); } }
            set => ABGR = (ABGR & 0x00ffffff) | ((uint)value << 24);
        }

        public Color(int rgb)
        {
            ABGR = 0;

            R = (byte)(rgb >> 16);
            G = (byte)(rgb >> 08);
            B = (byte)(rgb);
            A = 255;
        }

        public Color(int rgb, float alpha)
        {
            ABGR = 0;

            R = (byte)((rgb >> 16) * alpha);
            G = (byte)((rgb >> 08) * alpha);
            B = (byte)(rgb * alpha);
            A = (byte)(255 * alpha);
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            unchecked
            {
                ABGR = 0;
                R = r;
                G = g;
                B = b;
                A = a;
            }
        }

        public Color(int r, int g, int b, int a)
        {
            ABGR = 0;
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }

        public Color(float r, float g, float b, float a)
        {
            ABGR = 0;
            R = (byte)(r * 255);
            G = (byte)(g * 255);
            B = (byte)(b * 255);
            A = (byte)(a * 255);
        }

        public Color Premultiply()
        {
            byte a = A;
            return new Color((byte)(R * a / 255), (byte)(G * a / 255), (byte)(B * a / 255), a);
        }

        public Vector4 ToVector4()
        {
            return new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
        }

        public override bool Equals(object? obj)
        {
            return (obj is Color other) && (this == other);
        }

        public override int GetHashCode()
        {
            return (int)ABGR;
        }

        public override string ToString()
        {
            return ($"[{R}, {G}, {B}, {A}]");
        }

        /// <summary>
        /// The input string is the Components, R, G, B, and A. Ex. "RGBA" returns a hex string with those components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public string ToHexString(string components)
        {
            string result = "";

            for (int i = 0; i < components.Length; i++)
            {
                if (char.ToUpperInvariant(components[i]) == 'R')
                {
                    result += R.ToString("X2");
                }
                else if (char.ToUpperInvariant(components[i]) == 'G')
                {
                    result += G.ToString("X2");
                }
                else if (char.ToUpperInvariant(components[i]) == 'B')
                {
                    result += B.ToString("X2");
                }
                else if (char.ToUpperInvariant(components[i]) == 'A')
                {
                    result += A.ToString("X2");
                }
            }

            return result;
        }

        public string ToHexStringRGB()
        {
            return ToHexString("RGB");
        }

        public string ToHexStringRGBA()
        {
            return ToHexString("RGBA");
        }

        public static Color FromHexString(string components, ReadOnlySpan<char> value)
        {
            Color color;
            color.ABGR = 0xff000000;

            for (int i = 0; i < components.Length && i * 2 + 2 <= value.Length; i++)
            {
                if (char.ToUpperInvariant(components[i]) == 'R')
                {
                    color.R = byte.Parse(value.Slice(i * 2, 2), NumberStyles.HexNumber);
                }
                else if (char.ToUpperInvariant(components[i]) == 'G')
                {
                    color.G = byte.Parse(value.Slice(i * 2, 2), NumberStyles.HexNumber);
                }
                else if (char.ToUpperInvariant(components[i]) == 'B')
                {
                    color.B = byte.Parse(value.Slice(i * 2, 2), NumberStyles.HexNumber);
                }
                else if (char.ToUpperInvariant(components[i]) == 'A')
                {
                    color.A = byte.Parse(value.Slice(i * 2, 2), NumberStyles.HexNumber);
                }
            }

            return color;
        }

        public static Color FromHexStringRGB(string value)
        {
            return FromHexString("RGB", value);
        }

        public static Color FromHexStringRGBA(string value)
        {
            return FromHexString("RGBA", value);
        }

        public static Color Lerp(Color a, Color b, float amount)
        {
            amount = Math.Max(0, Math.Min(1, amount));

            return new Color(
                (int)(a.R + (b.R - a.R) * amount),
                (int)(a.G + (b.G - a.G) * amount),
                (int)(a.B + (b.B - a.B) * amount),
                (int)(a.A + (b.A - a.A) * amount)
            );
        }

        public static implicit operator Color(int color)
        {
            return new Color(color);
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color(
                (int)(value.R * scale),
                (int)(value.G * scale),
                (int)(value.B * scale),
                (int)(value.A * scale)
            );
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.ABGR == b.ABGR;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.ABGR != b.ABGR;
        }
    }
}
