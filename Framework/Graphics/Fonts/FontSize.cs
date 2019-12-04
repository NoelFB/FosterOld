using StbTrueTypeSharp;
using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    public class FontSize
    {

        public class Character
        {
            public char Unicode;
            public int Glyph;
            public int Width;
            public int Height;
            public float Advance;
            public float OffsetX;
            public float OffsetY;
            public bool HasGlyph;
        };

        public readonly Font Font;
        public readonly int Size;
        public readonly float Ascent;
        public readonly float Descent;
        public readonly float LineGap;
        public readonly float Height;
        public readonly float LineHeight;
        public readonly float Scale;

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
