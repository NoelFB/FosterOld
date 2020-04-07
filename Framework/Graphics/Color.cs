using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Foster.Framework
{    
    /// <summary>
    /// Color Data
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 4)]
    public struct Color
    {

        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        public static readonly Color White = new Color(0xffffff);
        public static readonly Color Black = new Color(0x000000);
        public static readonly Color Red = new Color(0xff0000);
        public static readonly Color Green = new Color(0x00ff00);
        public static readonly Color Blue = new Color(0x0000ff);
        public static readonly Color Yellow = new Color(0xffff00);

        /// <summary>
        /// The Color Value in a ABGR 32-bit unsigned integer
        /// </summary>
        public uint ABGR;

        /// <summary>
        /// Gets the Color Value in a RGBA 32-bit unsigned integer
        /// </summary>
        public uint RGBA => new Color(A, B, G, R).ABGR;

        /// <summary>
        /// The Red Component
        /// </summary>
        public byte R
        {
            get => (byte)ABGR;
            set => ABGR = (ABGR & 0xffffff00) | value;
        }

        /// <summary>
        /// The Green Component
        /// </summary>
        public byte G
        {
            get => (byte)(ABGR >> 8);
            set => ABGR = (ABGR & 0xffff00ff) | ((uint)value << 8);
        }

        /// <summary>
        /// The Blue Component
        /// </summary>
        public byte B
        {
            get => (byte)(ABGR >> 16);
            set => ABGR = (ABGR & 0xff00ffff) | ((uint)value << 16);
        }

        /// <summary>
        /// The Alpha Component
        /// </summary>
        public byte A
        {
            get => (byte)(ABGR >> 24);
            set => ABGR = (ABGR & 0x00ffffff) | ((uint)value << 24);
        }

        /// <summary>
        /// Creates a color given the int32 RGB data
        /// </summary>
        public Color(int rgb, byte alpha = 255)
        {
            ABGR = 0;

            R = (byte)(rgb >> 16);
            G = (byte)(rgb >> 08);
            B = (byte)(rgb);
            A = alpha;
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
            ABGR = 0;
            R = r;
            G = g;
            B = b;
            A = a;
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

        /// <summary>
        /// Premultiplies the color value based on its Alpha component
        /// </summary>
        /// <returns></returns>
        public Color Premultiply()
        {
            byte a = A;
            return new Color((byte)(R * a / 255), (byte)(G * a / 255), (byte)(B * a / 255), a);
        }

        /// <summary>
        /// Converts the Color to a Vector4
        /// </summary>
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
        /// Returns a Hex String representation of the Color's given components
        /// </summary>
        /// <param name="components">The Components, in any order. ex. "RGBA" or "RGB" or "ARGB"</param>
        /// <returns></returns>
        public string ToHexString(string components)
        {
            const string HEX = "0123456789ABCDEF";
            Span<char> result = stackalloc char[components.Length * 2];

            for (int i = 0; i < components.Length; i++)
            {
                switch (components[i])
                {
                    case 'R':
                    case 'r':
                        result[i * 2 + 0] = HEX[(R & 0xf0) >> 4];
                        result[i * 2 + 1] = HEX[(R & 0x0f)];
                        break;
                    case 'G':
                    case 'g':
                        result[i * 2 + 0] = HEX[(G & 0xf0) >> 4];
                        result[i * 2 + 1] = HEX[(G & 0x0f)];
                        break;
                    case 'B':
                    case 'b':
                        result[i * 2 + 0] = HEX[(B & 0xf0) >> 4];
                        result[i * 2 + 1] = HEX[(B & 0x0f)];
                        break;
                    case 'A':
                    case 'a':
                        result[i * 2 + 0] = HEX[(A & 0xf0) >> 4];
                        result[i * 2 + 1] = HEX[(A & 0x0f)];
                        break;
                }
            }

            return new string(result);
        }

        /// <summary>
        /// Returns an RGB Hex string representation of the Color
        /// </summary>
        public string ToHexStringRGB()
        {
            return ToHexString("RGB");
        }

        /// <summary>
        /// Returns an RGBA Hex string representation of the Color
        /// </summary>
        public string ToHexStringRGBA()
        {
            return ToHexString("RGBA");
        }

        /// <summary>
        /// Creates a new Color with the given components from the given string value
        /// </summary>
        /// <param name="components">The components to parse in order, ex. "RGBA"</param>
        /// <param name="value">The Hex value to parse</param>
        /// <returns></returns>
        public static Color FromHexString(string components, ReadOnlySpan<char> value)
        {
            // skip past useless string data (ex. if the string was 0xffffff or #ffffff)
            if (value.Length > 0 && value[0] == '#')
                value = value.Slice(1);
            if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
                value = value.Slice(2);

            var color = Black;

            for (int i = 0; i < components.Length && i * 2 + 2 <= value.Length; i++)
            {
                switch (components[i])
                {
                    case 'R':
                    case 'r':
                        if (byte.TryParse(value.Slice(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r))
                            color.R = r;
                        break;
                    case 'G':
                    case 'g':
                        if (byte.TryParse(value.Slice(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g))
                            color.G = g;
                        break;
                    case 'B':
                    case 'b':
                        if (byte.TryParse(value.Slice(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                            color.B = b;
                        break;
                    case 'A':
                    case 'a':
                        if (byte.TryParse(value.Slice(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a))
                            color.A = a;
                        break;
                }
            }

            return color;
        }

        /// <summary>
        /// Creates a new Color from the given RGB Hex value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color FromHexStringRGB(string value)
        {
            return FromHexString("RGB", value);
        }

        /// <summary>
        /// Creates a new Color from the given RGBA Hex value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color FromHexStringRGBA(string value)
        {
            return FromHexString("RGBA", value);
        }

        /// <summary>
        /// Linearly interpolates between two colors
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Implicitely converts an int32 to a Color, ex 0xffffff
        /// This does not include Alpha values
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator Color(int color)
        {
            return new Color(color);
        }

        /// <summary>
        /// Multiplies a Color by a scaler
        /// </summary>
        public static Color operator *(Color value, float scaler)
        {
            return new Color(
                (int)(value.R * scaler),
                (int)(value.G * scaler),
                (int)(value.B * scaler),
                (int)(value.A * scaler)
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
