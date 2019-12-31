using StbTrueTypeSharp;
using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    /// <summary>
    /// Contains the Data of a Font at a given size
    /// </summary>
    public class FontSize
    {
        /// <summary>
        /// A single Font Character
        /// </summary>
        public class Character
        {
            /// <summary>
            /// The Unicode Value of the Character
            /// </summary>
            public char Unicode;

            /// <summary>
            /// The Associated Glyph of the Character
            /// </summary>
            public int Glyph;

            /// <summary>
            /// The Width (in pixels) of the Character
            /// </summary>
            public int Width;

            /// <summary>
            /// The Height (in pixels) of the Character
            /// </summary>
            public int Height;

            /// <summary>
            /// The Horizontal Advance (in pixels) of the Character
            /// </summary>
            public float Advance;

            /// <summary>
            /// The X-Offset (in pixels) of the Character
            /// </summary>
            public float OffsetX;

            /// <summary>
            /// The Y-Offset (in pixels) of the Character
            /// </summary>
            public float OffsetY;

            /// <summary>
            /// Whether the Character has a Glyph. If not, this character cannot be rendered
            /// </summary>
            public bool HasGlyph;
        };

        /// <summary>
        /// The Font associated with this Font Size
        /// </summary>
        public readonly Font Font;

        /// <summary>
        /// The Size of the Font
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The Ascent of the Font. This is the Font.Ascent * our Scale
        /// </summary>
        public readonly float Ascent;

        /// <summary>
        /// The Descent of the Font. This is the Font.Descent * our Scale
        /// </summary>
        public readonly float Descent;

        /// <summary>
        /// The LineGap of the Font. This is the Font.LineGap * our Scale
        /// </summary>
        public readonly float LineGap;

        /// <summary>
        /// The Height of the Font. This is the Font.Height * our Scale
        /// </summary>
        public readonly float Height;

        /// <summary>
        /// The LineHeight of the Font. This is the Font.LineHeight * our Scale
        /// </summary>
        public readonly float LineHeight;

        /// <summary>
        /// The Scale of the Font Size
        /// </summary>
        public readonly float Scale;

        /// <summary>
        /// The Character Set of the Font Size
        /// </summary>
        public readonly Dictionary<char, Character> Charset = new Dictionary<char, Character>();

        public FontSize(Font font, int size, string charset)
        {
            if (font.Disposed)
                throw new Exception("Cannot get Font data as it is disposed");

            Font = font;
            Size = size;
            Scale = font.GetScale(size);
            Ascent = font.Ascent * Scale;
            Descent = font.Descent * Scale;
            LineGap = font.LineGap * Scale;
            Height = Ascent - Descent;
            LineHeight = Height + LineGap;

            for (int i = 0; i < charset.Length; i++)
            {
                // get font info
                var unicode = charset[i];
                var glyph = font.GetGlyph(unicode);

                if (glyph > 0)
                {
                    unsafe
                    {
                        int advance, offsetX, x0, y0, x1, y1;

                        StbTrueType.stbtt_GetGlyphHMetrics(font.fontInfo, glyph, &advance, &offsetX);
                        StbTrueType.stbtt_GetGlyphBitmapBox(font.fontInfo, glyph, Scale, Scale, &x0, &y0, &x1, &y1);

                        int w = (x1 - x0);
                        int h = (y1 - y0);

                        // define character
                        var ch = new Character
                        {
                            Unicode = unicode,
                            Glyph = glyph,
                            Width = w,
                            Height = h,
                            Advance = advance * Scale,
                            OffsetX = offsetX * Scale,
                            OffsetY = y0
                        };
                        ch.HasGlyph = (w > 0 && h > 0 && StbTrueType.stbtt_IsGlyphEmpty(font.fontInfo, ch.Glyph) == 0);
                        Charset[unicode] = ch;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Kerning Value between two Unicode characters at the Font Size, or 0 if there is no Kerning
        /// </summary>
        public float GetKerning(char unicode0, char unicode1)
        {
            if (Charset.TryGetValue(unicode0, out var char0) && Charset.TryGetValue(unicode1, out var char1))
            {
                if (Font.Disposed)
                    throw new Exception("Cannot get Font data as it is disposed");

                return StbTrueType.stbtt_GetGlyphKernAdvance(Font.fontInfo, char0.Glyph, char1.Glyph) * Scale;
            }

            return 0f;
        }

        /// <summary>
        /// Renders the Unicode character to a Bitmap at the Font Size, or null if the character doesn't exist
        /// </summary>
        public Bitmap? Render(char unicode)
        {
            if (Charset.TryGetValue(unicode, out var ch) && ch.HasGlyph)
            {
                var bitmap = new Bitmap(ch.Width, ch.Height);

                Render(unicode, new Span<Color>(bitmap.Pixels), out _, out _);

                return bitmap;
            }

            return null;
        }

        /// <summary>
        /// Renders the Unicode character to a buffer at the Font Size, and returns true on success
        /// </summary>
        public unsafe bool Render(char unicode, Span<Color> buffer, out int width, out int height)
        {
            if (Charset.TryGetValue(unicode, out var ch) && ch.HasGlyph)
            {
                if (buffer.Length < ch.Width * ch.Height)
                    throw new Exception("Buffer provided isn't large enough to store rendered data");

                if (Font.Disposed)
                    throw new Exception("Cannot get Font data as it is disposed");

                fixed (Color* ptr = buffer)
                {
                    // we actually use the bitmap buffer as our temporary buffer, and fill the pixels out backwards after
                    // kinda weird but it works & saves creating more memory

                    var input = (byte*)ptr;
                    StbTrueType.stbtt_MakeGlyphBitmap(Font.fontInfo, input, ch.Width, ch.Height, ch.Width, Scale, Scale, ch.Glyph);

                    for (int i = ch.Width * ch.Height - 1; i >= 0; i--)
                        ptr[i] = new Color(input[i], input[i], input[i], input[i]);
                }

                width = ch.Width;
                height = ch.Height;
                return true;
            }

            width = height = 0;
            return false;
        }

    }
}
