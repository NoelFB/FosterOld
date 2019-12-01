using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using StbTrueTypeSharp;

namespace Foster.Framework
{

    public static class Charsets
    {
        public static readonly string ASCII = Make((char)32, (char)126);

        public static string Make(char from, char to)
        {
            Span<char> range = stackalloc char[to - from + 1];

            for (var i = 0; i < range.Length; i++)
                range[i] = (char)(from + i);

            return new string(range);
        }
    }

    public class Font : IDisposable
    {

        internal readonly StbTrueType.stbtt_fontinfo fontInfo;

        private readonly byte[] fontBuffer;
        private readonly GCHandle fontHandle;
        private readonly Dictionary<char, int> glyphs = new Dictionary<char, int>();

        public readonly string FamilyName;
        public readonly string StyleName;
        public readonly int Ascent;
        public readonly int Descent;
        public readonly int LineGap;
        public readonly int Height;
        public readonly int LineHeight;

        public bool Disposed { get; private set; } = false;

        public Font(string path) : this(File.ReadAllBytes(path))
        {
            
        }

        ~Font()
        {
            Dispose();
        }

        public unsafe Font(byte[] buffer)
        {
            fontBuffer = buffer;
            fontHandle = GCHandle.Alloc(fontBuffer, GCHandleType.Pinned);
            fontInfo = new StbTrueType.stbtt_fontinfo();

            StbTrueType.stbtt_InitFont(fontInfo, (byte*)(fontHandle.AddrOfPinnedObject().ToPointer()), 0);

            FamilyName = GetName(fontInfo, 1);
            StyleName = GetName(fontInfo, 2);

            // properties
            int ascent, descent, linegap;
            StbTrueType.stbtt_GetFontVMetrics(fontInfo, &ascent, &descent, &linegap);
            Ascent = ascent;
            Descent = descent;
            LineGap = linegap;
            Height = Ascent - Descent;
            LineHeight = Height + LineGap;

            static unsafe string GetName(StbTrueType.stbtt_fontinfo fontInfo, int nameID)
            {
                int length = 0;

                sbyte* ptr = StbTrueType.stbtt_GetFontNameString(fontInfo, &length,
                    StbTrueType.STBTT_PLATFORM_ID_MICROSOFT,
                    StbTrueType.STBTT_MS_EID_UNICODE_BMP,
                    StbTrueType.STBTT_MS_LANG_ENGLISH,
                    nameID);

                if (length > 0)
                    return new string(ptr, 0, length, Encoding.BigEndianUnicode);

                return "Unknown";
            }
        }

        public float GetScale(int height)
        {
            if (Disposed)
                throw new Exception("Cannot get Font data as it is disposed");

            return StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, height);
        }

        public int GetGlyph(char unicode)
        {
            if (!glyphs.TryGetValue(unicode, out var glyph))
            {
                if (Disposed)
                    throw new Exception("Cannot get Font data as it is disposed");

                glyph = StbTrueType.stbtt_FindGlyphIndex(fontInfo, unicode);
                glyphs[unicode] = glyph;
            }

            return glyph;
        }

        public void Dispose()
        {
            Disposed = true;

            if (fontHandle.IsAllocated)
                fontHandle.Free();
        }
    }
}
