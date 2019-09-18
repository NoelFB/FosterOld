using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StbTrueType
{
    internal unsafe class StbTrueType
    {
        private static unsafe class CRuntime
        {
            public static void* malloc(ulong size)
            {
                return malloc((long)size);
            }

            public static void* malloc(long size)
            {
                return Marshal.AllocHGlobal((int)size).ToPointer();
            }

            public static void memcpy(void* a, void* b, long size)
            {
                Buffer.MemoryCopy(a, b, size, size);
            }

            public static void memcpy(void* a, void* b, ulong size)
            {
                memcpy(a, b, (long)size);
            }

            public static void free(void* a)
            {
                Marshal.FreeHGlobal(new IntPtr(a));
            }

            public static void memset(void* ptr, int value, long size)
            {
                Span<byte> buffer = new Span<byte>((byte*)ptr, (int)size);
                buffer.Fill((byte)value);
            }

            public static void memset(void* ptr, int value, ulong size)
            {
                memset(ptr, value, (long)size);
            }

            public static double pow(double a, double b)
            {
                return Math.Pow(a, b);
            }

            public static float fabs(double a)
            {
                return (float)Math.Abs(a);
            }

            public static double ceil(double a)
            {
                return Math.Ceiling(a);
            }

            public static double floor(double a)
            {
                return Math.Floor(a);
            }

            public static double cos(double value)
            {
                return Math.Cos(value);
            }

            public static double acos(double value)
            {
                return Math.Acos(value);
            }

            public static double sin(double value)
            {
                return Math.Sin(value);
            }

            public static double sqrt(double val)
            {
                return Math.Sqrt(val);
            }

            public static double fmod(double x, double y)
            {
                return x % y;
            }

            public static ulong strlen(sbyte* str)
            {
                sbyte* ptr = str;

                while (*ptr != '\0')
                {
                    ptr++;
                }

                return (ulong)ptr - (ulong)str - 1;
            }
        }

        public const int vmove = 1;
        public const int vline = 2;
        public const int vcurve = 3;
        public const int vcubic = 4;

        public const int PLATFORM_ID_UNICODE = 0;
        public const int PLATFORM_ID_MAC = 1;
        public const int PLATFORM_ID_ISO = 2;
        public const int PLATFORM_ID_MICROSOFT = 3;

        public const int UNICODE_EID_UNICODE_1_0 = 0;
        public const int UNICODE_EID_UNICODE_1_1 = 1;
        public const int UNICODE_EID_ISO_10646 = 2;
        public const int UNICODE_EID_UNICODE_2_0_BMP = 3;
        public const int UNICODE_EID_UNICODE_2_0_FULL = 4;

        public const int MS_EID_SYMBOL = 0;
        public const int MS_EID_UNICODE_BMP = 1;
        public const int MS_EID_SHIFTJIS = 2;
        public const int MS_EID_UNICODE_FULL = 10;

        public const int MAC_EID_ROMAN = 0;
        public const int MAC_EID_ARABIC = 4;
        public const int MAC_EID_JAPANESE = 1;
        public const int MAC_EID_HEBREW = 5;
        public const int MAC_EID_CHINESE_TRAD = 2;
        public const int MAC_EID_GREEK = 6;
        public const int MAC_EID_KOREAN = 3;
        public const int MAC_EID_RUSSIAN = 7;

        public const int MS_LANG_ENGLISH = 0x0409;
        public const int MS_LANG_ITALIAN = 0x0410;
        public const int MS_LANG_CHINESE = 0x0804;
        public const int MS_LANG_JAPANESE = 0x0411;
        public const int MS_LANG_DUTCH = 0x0413;
        public const int MS_LANG_KOREAN = 0x0412;
        public const int MS_LANG_FRENCH = 0x040c;
        public const int MS_LANG_RUSSIAN = 0x0419;
        public const int MS_LANG_GERMAN = 0x0407;
        public const int MS_LANG_SPANISH = 0x0409;
        public const int MS_LANG_HEBREW = 0x040d;
        public const int MS_LANG_SWEDISH = 0x041D;

        public const int MAC_LANG_ENGLISH = 0;
        public const int MAC_LANG_JAPANESE = 11;
        public const int MAC_LANG_ARABIC = 12;
        public const int MAC_LANG_KOREAN = 23;
        public const int MAC_LANG_DUTCH = 4;
        public const int MAC_LANG_RUSSIAN = 32;
        public const int MAC_LANG_FRENCH = 1;
        public const int MAC_LANG_SPANISH = 6;
        public const int MAC_LANG_GERMAN = 2;
        public const int MAC_LANG_SWEDISH = 5;
        public const int MAC_LANG_HEBREW = 10;
        public const int MAC_LANG_CHINESE_SIMPLIFIED = 33;
        public const int MAC_LANG_ITALIAN = 3;
        public const int MAC_LANG_CHINESE_TRAD = 19;

        public static byte buf_get8(buf* b)
        {
            if (b->cursor >= b->size)
                return 0;

            return b->data[b->cursor++];
        }

        public static byte buf_peek8(buf* b)
        {
            if (b->cursor >= b->size)
                return 0;

            return b->data[b->cursor];
        }

        public static void buf_seek(buf* b, int o)
        {
            b->cursor = ((o > b->size) || (o < 0)) ? b->size : o;
        }

        public static void buf_skip(buf* b, int o)
        {
            buf_seek(b, b->cursor + o);
        }

        public static uint buf_get(buf* b, int n)
        {
            uint v = 0;

            for (int i = 0; i < n; i++)
                v = (v << 8) | buf_get8(b);

            return v;
        }

        public static buf new_buf(void* p, ulong size)
        {
            return new buf
            {
                data = (byte*)p,
                size = (int)size,
                cursor = 0
            };
        }

        public static buf buf_range(buf* b, int o, int s)
        {
            buf r = new_buf(null, 0);
            if (((o < 0 || s < 0) || (o > b->size)) || s > (b->size - o))
                return r;

            r.data = b->data + o;
            r.size = s;
            return r;
        }

        public static buf cff_get_index(buf* b)
        {
            int start = b->cursor;
            int count = (int)(buf_get(b, 2));

            if (count != 0)
            {
                int offsize = buf_get8(b);
                buf_skip(b, offsize * count);
                buf_skip(b, (int)(buf_get(b, offsize) - 1));
            }

            return buf_range(b, start, b->cursor - start);
        }

        public static uint cff_int(buf* b)
        {
            int b0 = buf_get8(b);

            if ((b0 >= 32) && (b0 <= 246))
                return (uint)(b0 - 139);
            else if ((b0 >= 247) && (b0 <= 250))
                return (uint)((b0 - 247) * 256 + buf_get8(b) + 108);
            else if ((b0 >= 251) && (b0 <= 254))
                return (uint)(-(b0 - 251) * 256 - buf_get8(b) - 108);
            else if (b0 == 28)
                return buf_get(b, 2);
            else if (b0 == 29)
                return buf_get(b, 4);

            return 0;
        }

        public static void cff_skip_operand(buf* b)
        {
            int v = 0;
            int b0 = buf_peek8(b);
            if (b0 == 30)
            {
                buf_skip(b, 1);
                while ((b->cursor) < (b->size))
                {
                    v = buf_get8(b);
                    if (((v & 0xF) == 0xF) || ((v >> 4) == 0xF))
                        break;
                }
            }
            else
            {
                cff_int(b);
            }
        }

        public static buf dict_get(buf* b, int key)
        {
            buf_seek(b, 0);

            while ((b->cursor) < (b->size))
            {
                int start = b->cursor;
                while ((buf_peek8(b)) >= 28)
                    cff_skip_operand(b);

                int end = b->cursor;
                int op = buf_get8(b);
                if (op == 12)
                    op = buf_get8(b) | 0x100;

                if (op == key)
                    return buf_range(b, start, end - start);
            }

            return buf_range(b, 0, 0);
        }

        public static void dict_get_ints(buf* b, int key, int outcount, uint* _out_)
        {
            int i = 0;
            var operands = dict_get(b, key);
            for (i = 0; (i < outcount) && ((operands.cursor) < (operands.size)); i++)
                _out_[i] = cff_int(&operands);
        }

        public static int cff_index_count(buf* b)
        {
            buf_seek(b, 0);
            return (int)(buf_get(b, 2));
        }

        public static buf cff_index_get(buf b, int i)
        {
            buf_seek(&b, 0);
            int count = (int)buf_get(&b, 2);
            int offsize = buf_get8(&b);
            buf_skip(&b, i * offsize);
            int start = (int)buf_get(&b, offsize);
            int end = (int)buf_get(&b, offsize);
            return buf_range(&b, 2 + (count + 1) * offsize + start, end - start);
        }

        public static ushort ttUSHORT(byte* p)
        {
            return (ushort)(p[0] * 256 + p[1]);
        }

        public static short ttSHORT(byte* p)
        {
            return (short)(p[0] * 256 + p[1]);
        }

        public static uint ttULONG(byte* p)
        {
            return (uint)((p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3]);
        }

        public static int ttLONG(byte* p)
        {
            return (p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3];
        }

        public static int isfont(byte* font)
        {
            if (font[0] == '1' && font[1] == 0 && font[2] == 0 && font[3] == 0)
                return 1;

            if (font[0] == 't' && font[1] == 'y' && font[2] == 'p' && font[3] == '1')
                return 1;

            if (font[0] == 'O' && font[1] == 'T' && font[2] == 'T' && font[3] == 'O')
                return 1;

            if (font[0] == 0 && font[1] == 0 && font[2] == 0 && font[3] == 0)
                return 1;

            if (font[0] == 't' && font[0] == 'r' && font[0] == 'u' && font[0] == 'e')
                return 1;

            return 0;
        }

        public static int GetFontOffsetForIndex_internal(byte* font_collection, int index)
        {
            if ((isfont(font_collection)) != 0)
                return index == 0 ? 0 : -1;

            if (font_collection[0] == 't' && font_collection[0] == 't' && font_collection[0] == 'c' && font_collection[0] == 'f')
            {
                if ((ttULONG(font_collection + 4) == 0x00010000) || (ttULONG(font_collection + 4) == 0x00020000))
                {
                    int n = ttLONG(font_collection + 8);
                    if (index >= n)
                        return -1;

                    return (int)(ttULONG(font_collection + 12 + index * 4));
                }
            }

            return -1;
        }

        public static int GetNumberOfFonts_internal(byte* font_collection)
        {
            if ((isfont(font_collection)) != 0)
                return 1;

            if (font_collection[0] == 't' && font_collection[0] == 't' && font_collection[0] == 'c' && font_collection[0] == 'f')
            {
                if ((ttULONG(font_collection + 4) == 0x00010000) || (ttULONG(font_collection + 4) == 0x00020000))
                    return ttLONG(font_collection + 8);
            }

            return 0;
        }

        public static buf get_subrs(buf cff, buf fontdict)
        {
            uint subrsoff = 0;
            uint* private_loc = stackalloc uint[2];
            private_loc[0] = 0;
            private_loc[1] = 0;

            buf pdict = new buf();
            dict_get_ints(&fontdict, 18, 2, private_loc);
            if ((private_loc[1] == 0) || (private_loc[0] == 0))
                return new_buf(null, 0);

            pdict = buf_range(&cff, (int)(private_loc[1]), (int)(private_loc[0]));
            dict_get_ints(&pdict, 19, 1, &subrsoff);
            if (subrsoff == 0)
                return new_buf(null, 0);

            buf_seek(&cff, (int)(private_loc[1] + subrsoff));
            return cff_get_index(&cff);
        }

        public static int InitFont_internal(fontinfo info, byte* data, int fontstart)
        {
            uint cmap = 0;
            uint t = 0;
            int i = 0;
            int numTables = 0;
            info.data = data;
            info.fontstart = fontstart;
            info.cff = new_buf(null, 0);
            cmap = find_table(data, (uint)fontstart, "cmap");
            info.loca = (int)(find_table(data, (uint)fontstart, "loca"));
            info.head = (int)(find_table(data, (uint)fontstart, "head"));
            info.glyf = (int)(find_table(data, (uint)fontstart, "glyf"));
            info.hhea = (int)(find_table(data, (uint)fontstart, "hhea"));
            info.hmtx = (int)(find_table(data, (uint)fontstart, "hmtx"));
            info.kern = (int)(find_table(data, (uint)fontstart, "kern"));
            info.gpos = (int)(find_table(data, (uint)fontstart, "GPOS"));

            if (cmap == 0 || info.head == 0 || info.hhea == 0 || info.hmtx == 0)
                return 0;

            if (info.glyf != 0)
            {
                if (info.loca == 0)
                    return 0;
            }
            else
            {
                buf b = new buf();
                buf topdict = new buf();
                buf topdictidx = new buf();
                uint cstype = 2;
                uint charstrings = 0;
                uint fdarrayoff = 0;
                uint fdselectoff = 0;

                uint cff = find_table(data, (uint)fontstart, "CFF ");
                if (cff == 0)
                    return 0;

                info.fontdicts = new_buf(null, 0);
                info.fdselect = new_buf(null, 0);
                info.cff = new_buf(data + cff, 512 * 1024 * 1024);
                b = info.cff;
                buf_skip(&b, 2);
                buf_seek(&b, buf_get8(&b));
                cff_get_index(&b);
                topdictidx = cff_get_index(&b);
                topdict = cff_index_get(topdictidx, 0);
                cff_get_index(&b);
                info.gsubrs = cff_get_index(&b);
                dict_get_ints(&topdict, 17, 1, &charstrings);
                dict_get_ints(&topdict, 0x100 | 6, 1, &cstype);
                dict_get_ints(&topdict, 0x100 | 36, 1, &fdarrayoff);
                dict_get_ints(&topdict, 0x100 | 37, 1, &fdselectoff);
                info.subrs = get_subrs(b, topdict);

                if (cstype != 2)
                    return 0;

                if (charstrings == 0)
                    return 0;

                if (fdarrayoff != 0)
                {
                    if (fdselectoff == 0)
                        return 0;

                    buf_seek(&b, (int)fdarrayoff);
                    info.fontdicts = cff_get_index(&b);
                    info.fdselect =
                        buf_range(&b, (int)fdselectoff, (int)(b.size - fdselectoff));
                }

                buf_seek(&b, (int)charstrings);
                info.charstrings = cff_get_index(&b);
            }

            t = find_table(data, (uint)fontstart, "maxp");
            if (t != 0)
                info.numGlyphs = ttUSHORT(data + t + 4);
            else
                info.numGlyphs = 0xffff;

            numTables = ttUSHORT(data + cmap + 2);
            info.index_map = 0;
            for (i = 0; i < numTables; ++i)
            {
                uint encoding_record = (uint)(cmap + 4 + 8 * i);
                switch (ttUSHORT(data + encoding_record))
                {
                    case PLATFORM_ID_MICROSOFT:
                        switch (ttUSHORT(data + encoding_record + 2))
                        {
                            case MS_EID_UNICODE_BMP:
                            case MS_EID_UNICODE_FULL:
                                info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
                                break;
                        }

                        break;
                    case PLATFORM_ID_UNICODE:
                        info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
                        break;
                }
            }

            if ((info.index_map) == 0)
                return 0;

            info.indexToLocFormat = ttUSHORT(data + info.head + 50);
            return 1;
        }

        public static int FindGlyphIndex(fontinfo info, int unicode_codepoint)
        {
            byte* data = info.data;
            uint index_map = (uint)(info.index_map);
            ushort format = ttUSHORT(data + index_map + 0);
            if (format == 0)
            {
                int bytes = ttUSHORT(data + index_map + 2);
                if (unicode_codepoint < (bytes - 6))
                {
                    return *(data + index_map + 6 + unicode_codepoint);
                }

                return 0;
            }
            else if (format == 6)
            {
                uint first = ttUSHORT(data + index_map + 6);
                uint count = ttUSHORT(data + index_map + 8);
                if ((((uint)unicode_codepoint) >= first) && (((uint)unicode_codepoint) < (first + count)))
                {
                    return ttUSHORT(data + index_map + 10 + (unicode_codepoint - first) * 2);
                }

                return 0;
            }
            else if (format == 2)
            {
                return 0;
            }
            else if (format == 4)
            {
                ushort segcount = (ushort)(ttUSHORT(data + index_map + 6) >> 1);
                ushort searchRange = (ushort)(ttUSHORT(data + index_map + 8) >> 1);
                ushort entrySelector = ttUSHORT(data + index_map + 10);
                ushort rangeShift = (ushort)(ttUSHORT(data + index_map + 12) >> 1);
                uint endCount = index_map + 14;
                uint search = endCount;
                if (unicode_codepoint > 0xffff)
                {
                    return 0;
                }

                if (unicode_codepoint >= (ttUSHORT(data + search + rangeShift * 2)))
                {
                    search += (uint)(rangeShift * 2);
                }

                search -= 2;
                while (entrySelector != 0)
                {
                    ushort end = 0;
                    searchRange >>= 1;
                    end = ttUSHORT(data + search + searchRange * 2);
                    if (unicode_codepoint > end)
                    {
                        search += (uint)(searchRange * 2);
                    }

                    --entrySelector;
                }

                search += 2;
                {
                    ushort offset = 0;
                    ushort start = 0;
                    ushort item = (ushort)((search - endCount) >> 1);
                    start = ttUSHORT(data + index_map + 14 + segcount * 2 + 2 + 2 * item);
                    if (unicode_codepoint < start)
                    {
                        return 0;
                    }

                    offset = ttUSHORT(data + index_map + 14 + segcount * 6 + 2 + 2 * item);
                    if (offset == 0)
                    {
                        return (ushort)(unicode_codepoint +
                                                ttSHORT(data + index_map + 14 + segcount * 4 + 2 + 2 * item));
                    }

                    return ttUSHORT(data + offset + (unicode_codepoint - start) * 2 + index_map + 14 +
                                           segcount * 6 + 2 + 2 * item);
                }
            }
            else if ((format == 12) || (format == 13))
            {
                uint ngroups = ttULONG(data + index_map + 12);
                int low = 0;
                int high = 0;
                low = 0;
                high = ((int)ngroups);
                while (low < high)
                {
                    int mid = low + ((high - low) >> 1);
                    uint start_char = ttULONG(data + index_map + 16 + mid * 12);
                    uint end_char = ttULONG(data + index_map + 16 + mid * 12 + 4);
                    if (((uint)unicode_codepoint) < start_char)
                    {
                        high = mid;
                    }
                    else if (((uint)unicode_codepoint) > end_char)
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        uint start_glyph = ttULONG(data + index_map + 16 + mid * 12 + 8);
                        if (format == 12)
                        {
                            return (int)(start_glyph + unicode_codepoint - start_char);
                        }
                        else
                        {
                            return (int)start_glyph;
                        }
                    }
                }

                return 0;
            }

            return 0;
        }

        public static int GetCodepointShape(fontinfo info, int unicode_codepoint, vertex** vertices)
        {
            return GetGlyphShape(info, FindGlyphIndex(info, unicode_codepoint),
                vertices);
        }

        public static void setvertex(vertex* v, byte type, int x, int y, int cx, int cy)
        {
            v->type = type;
            v->x = ((short)x);
            v->y = ((short)y);
            v->cx = ((short)cx);
            v->cy = ((short)cy);
        }

        public static int GetGlyfOffset(fontinfo info, int glyph_index)
        {
            int g1 = 0;
            int g2 = 0;
            if (glyph_index >= (info.numGlyphs))
            {
                return -1;
            }

            if ((info.indexToLocFormat) >= 2)
            {
                return -1;
            }

            if ((info.indexToLocFormat) == 0)
            {
                g1 = info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2) * 2;
                g2 = info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2 + 2) * 2;
            }
            else
            {
                g1 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4));
                g2 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4 + 4));
            }

            return g1 == g2 ? -1 : g1;
        }

        public static int GetGlyphBox(fontinfo info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
        {
            if ((info.cff.size) != 0)
            {
                GetGlyphInfoT2(info, glyph_index, x0, y0, x1, y1);
            }
            else
            {
                int g = GetGlyfOffset(info, glyph_index);
                if (g < 0)
                {
                    return 0;
                }

                if (x0 != null)
                {
                    *x0 = ttSHORT(info.data + g + 2);
                }

                if (y0 != null)
                {
                    *y0 = ttSHORT(info.data + g + 4);
                }

                if (x1 != null)
                {
                    *x1 = ttSHORT(info.data + g + 6);
                }

                if (y1 != null)
                {
                    *y1 = ttSHORT(info.data + g + 8);
                }
            }

            return 1;
        }

        public static int GetCodepointBox(fontinfo info, int codepoint, int* x0, int* y0, int* x1, int* y1)
        {
            return GetGlyphBox(info, FindGlyphIndex(info, codepoint), x0, y0, x1,
                y1);
        }

        public static int IsGlyphEmpty(fontinfo info, int glyph_index)
        {
            short numberOfContours = 0;
            int g = 0;
            if ((info.cff.size) != 0)
            {
                return (GetGlyphInfoT2(info, glyph_index, null, null, null, null)) == 0
                    ? 1
                    : 0;
            }

            g = GetGlyfOffset(info, glyph_index);
            if (g < 0)
            {
                return 1;
            }

            numberOfContours = ttSHORT(info.data + g);
            return numberOfContours == 0 ? 1 : 0;
        }

        public static int close_shape(vertex* vertices, int num_vertices, int was_off, int start_off,
            int sx, int sy, int scx, int scy, int cx, int cy)
        {
            if (start_off != 0)
            {
                if (was_off != 0)
                {
                    setvertex(&vertices[num_vertices++], vcurve, (cx + scx) >> 1,
                        (cy + scy) >> 1, cx, cy);
                }

                setvertex(&vertices[num_vertices++], vcurve, sx, sy, scx,
                    scy);
            }
            else
            {
                if (was_off != 0)
                {
                    setvertex(&vertices[num_vertices++], vcurve, sx, sy,
                        cx, cy);
                }
                else
                {
                    setvertex(&vertices[num_vertices++], vline, sx, sy, 0,
                        0);
                }
            }

            return num_vertices;
        }

        public static int GetGlyphShapeTT(fontinfo info, int glyph_index, vertex** pvertices)
        {
            short numberOfContours = 0;
            byte* endPtsOfContours;
            byte* data = info.data;
            vertex* vertices = null;
            int num_vertices = 0;
            int g = GetGlyfOffset(info, glyph_index);
            *pvertices = null;
            if (g < 0)
            {
                return 0;
            }

            numberOfContours = ttSHORT(data + g);
            if (numberOfContours > 0)
            {
                byte flags = 0;
                byte flagcount = 0;
                int ins = 0;
                int i = 0;
                int j = 0;
                int m = 0;
                int n = 0;
                int next_move = 0;
                int was_off = 0;
                int off = 0;
                int start_off = 0;
                int x = 0;
                int y = 0;
                int cx = 0;
                int cy = 0;
                int sx = 0;
                int sy = 0;
                int scx = 0;
                int scy = 0;
                byte* points;
                endPtsOfContours = (data + g + 10);
                ins = ttUSHORT(data + g + 10 + numberOfContours * 2);
                points = data + g + 10 + numberOfContours * 2 + 2 + ins;
                n = 1 + ttUSHORT(endPtsOfContours + numberOfContours * 2 - 2);
                m = n + 2 * numberOfContours;
                vertices = (vertex*)(CRuntime.malloc((ulong)(m * sizeof(vertex))));
                if (vertices == null)
                {
                    return 0;
                }

                next_move = 0;
                flagcount = 0;
                off = m - n;
                for (i = 0; i < n; ++i)
                {
                    if (flagcount == 0)
                    {
                        flags = *points++;
                        if ((flags & 8) != 0)
                        {
                            flagcount = *points++;
                        }
                    }
                    else
                    {
                        --flagcount;
                    }

                    vertices[off + i].type = flags;
                }

                x = 0;
                for (i = 0; i < n; ++i)
                {
                    flags = vertices[off + i].type;
                    if ((flags & 2) != 0)
                    {
                        short dx = *points++;
                        x += (flags & 16) != 0 ? dx : -dx;
                    }
                    else
                    {
                        if ((flags & 16) == 0)
                        {
                            x = x + (short)(points[0] * 256 + points[1]);
                            points += 2;
                        }
                    }

                    vertices[off + i].x = ((short)x);
                }

                y = 0;
                for (i = 0; i < n; ++i)
                {
                    flags = vertices[off + i].type;
                    if ((flags & 4) != 0)
                    {
                        short dy = *points++;
                        y += (flags & 32) != 0 ? dy : -dy;
                    }
                    else
                    {
                        if ((flags & 32) == 0)
                        {
                            y = y + (short)(points[0] * 256 + points[1]);
                            points += 2;
                        }
                    }

                    vertices[off + i].y = ((short)y);
                }

                num_vertices = 0;
                sx = sy = cx = cy = scx = scy = 0;
                for (i = 0; i < n; ++i)
                {
                    flags = vertices[off + i].type;
                    x = vertices[off + i].x;
                    y = vertices[off + i].y;
                    if (next_move == i)
                    {
                        if (i != 0)
                        {
                            num_vertices = close_shape(vertices, num_vertices, was_off,
                                start_off, sx, sy, scx, scy, cx,
                                cy);
                        }

                        start_off = ((flags & 1) != 0 ? 0 : 1);
                        if (start_off != 0)
                        {
                            scx = x;
                            scy = y;
                            if ((vertices[off + i + 1].type & 1) == 0)
                            {
                                sx = (x + vertices[off + i + 1].x) >> 1;
                                sy = (y + vertices[off + i + 1].y) >> 1;
                            }
                            else
                            {
                                sx = vertices[off + i + 1].x;
                                sy = vertices[off + i + 1].y;
                                ++i;
                            }
                        }
                        else
                        {
                            sx = x;
                            sy = y;
                        }

                        setvertex(&vertices[num_vertices++], vmove, sx, sy,
                            0, 0);
                        was_off = 0;
                        next_move = 1 + ttUSHORT(endPtsOfContours + j * 2);
                        ++j;
                    }
                    else
                    {
                        if ((flags & 1) == 0)
                        {
                            if (was_off != 0)
                            {
                                setvertex(&vertices[num_vertices++], vcurve, (cx + x) >> 1,
                                    (cy + y) >> 1, cx, cy);
                            }

                            cx = x;
                            cy = y;
                            was_off = 1;
                        }
                        else
                        {
                            if (was_off != 0)
                            {
                                setvertex(&vertices[num_vertices++], vcurve, x, y,
                                    cx, cy);
                            }
                            else
                            {
                                setvertex(&vertices[num_vertices++], vline, x, y,
                                    0, 0);
                            }

                            was_off = 0;
                        }
                    }
                }

                num_vertices = close_shape(vertices, num_vertices, was_off,
                    start_off, sx, sy, scx, scy, cx, cy);
            }
            else if (numberOfContours == (-1))
            {
                int more = 1;
                byte* comp = data + g + 10;
                num_vertices = 0;
                vertices = null;
                while (more != 0)
                {
                    ushort flags = 0;
                    ushort gidx = 0;
                    int comp_num_verts = 0;
                    int i = 0;
                    vertex* comp_verts = null;
                    vertex* tmp = null;
                    float* mtx = stackalloc float[6];
                    mtx[0] = 1;
                    mtx[1] = 0;
                    mtx[2] = 0;
                    mtx[3] = 1;
                    mtx[4] = 0;
                    mtx[5] = 0;
                    float m = 0;
                    float n = 0;
                    flags = (ushort)(ttSHORT(comp));
                    comp += 2;
                    gidx = (ushort)(ttSHORT(comp));
                    comp += 2;
                    if ((flags & 2) != 0)
                    {
                        if ((flags & 1) != 0)
                        {
                            mtx[4] = ttSHORT(comp);
                            comp += 2;
                            mtx[5] = ttSHORT(comp);
                            comp += 2;
                        }
                        else
                        {
                            mtx[4] = *(sbyte*)comp;
                            comp += 1;
                            mtx[5] = *(sbyte*)comp;
                            comp += 1;
                        }
                    }
                    else
                    {
                    }

                    if ((flags & (1 << 3)) != 0)
                    {
                        mtx[0] = mtx[3] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                        mtx[1] = mtx[2] = 0;
                    }
                    else if ((flags & (1 << 6)) != 0)
                    {
                        mtx[0] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                        mtx[1] = mtx[2] = 0;
                        mtx[3] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                    }
                    else if ((flags & (1 << 7)) != 0)
                    {
                        mtx[0] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                        mtx[1] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                        mtx[2] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                        mtx[3] = ttSHORT(comp) / 16384.0f;
                        comp += 2;
                    }

                    m = ((float)(CRuntime.sqrt(mtx[0] * mtx[0] + mtx[1] * mtx[1])));
                    n = ((float)(CRuntime.sqrt(mtx[2] * mtx[2] + mtx[3] * mtx[3])));
                    comp_num_verts = GetGlyphShape(info, gidx, &comp_verts);
                    if (comp_num_verts > 0)
                    {
                        for (i = 0; i < comp_num_verts; ++i)
                        {
                            vertex* v = &comp_verts[i];
                            short x = 0;
                            short y = 0;
                            x = v->x;
                            y = v->y;
                            v->x = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
                            v->y = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
                            x = v->cx;
                            y = v->cy;
                            v->cx = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
                            v->cy = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
                        }

                        tmp = (vertex*)(CRuntime.malloc(
                            (ulong)((num_vertices + comp_num_verts) * sizeof(vertex))));
                        if (tmp == null)
                        {
                            if (vertices != null)
                            {
                                CRuntime.free(vertices);
                            }

                            if (comp_verts != null)
                            {
                                CRuntime.free(comp_verts);
                            }

                            return 0;
                        }

                        if (num_vertices > 0)
                        {
                            CRuntime.memcpy(tmp, vertices, (ulong)(num_vertices * sizeof(vertex)));
                        }

                        CRuntime.memcpy(tmp + num_vertices, comp_verts,
                            (ulong)(comp_num_verts * sizeof(vertex)));
                        if (vertices != null)
                        {
                            CRuntime.free(vertices);
                        }

                        vertices = tmp;
                        CRuntime.free(comp_verts);
                        num_vertices += comp_num_verts;
                    }

                    more = flags & (1 << 5);
                }
            }
            else if (numberOfContours < 0)
            {
            }
            else
            {
            }

            *pvertices = vertices;
            return num_vertices;
        }

        public static void track_vertex(csctx* c, int x, int y)
        {
            if ((x > (c->max_x)) || (c->started == 0))
                c->max_x = x;

            if ((y > (c->max_y)) || (c->started == 0))
                c->max_y = y;

            if ((x < (c->min_x)) || (c->started == 0))
                c->min_x = x;

            if ((y < (c->min_y)) || (c->started == 0))
                c->min_y = y;

            c->started = 1;
        }

        public static void csctx_v(csctx* c, byte type, int x, int y, int cx, int cy, int cx1, int cy1)
        {
            if ((c->bounds) != 0)
            {
                track_vertex(c, x, y);
                if (type == vcubic)
                {
                    track_vertex(c, cx, cy);
                    track_vertex(c, cx1, cy1);
                }
            }
            else
            {
                setvertex(&c->pvertices[c->num_vertices], type, x, y, cx,
                    cy);
                c->pvertices[c->num_vertices].cx1 = ((short)cx1);
                c->pvertices[c->num_vertices].cy1 = ((short)cy1);
            }

            c->num_vertices++;
        }

        public static void csctx_close_shape(csctx* ctx)
        {
            if ((ctx->first_x != ctx->x) || (ctx->first_y != ctx->y))
            {
                csctx_v(ctx, vline, (int)(ctx->first_x), (int)(ctx->first_y), 0,
                    0, 0, 0);
            }
        }

        public static void csctx_rmove_to(csctx* ctx, float dx, float dy)
        {
            csctx_close_shape(ctx);
            ctx->first_x = ctx->x = ctx->x + dx;
            ctx->first_y = ctx->y = ctx->y + dy;
            csctx_v(ctx, vmove, (int)(ctx->x), (int)(ctx->y), 0, 0, 0,
                0);
        }

        public static void csctx_rline_to(csctx* ctx, float dx, float dy)
        {
            ctx->x += dx;
            ctx->y += dy;
            csctx_v(ctx, vline, (int)(ctx->x), (int)(ctx->y), 0, 0, 0,
                0);
        }

        public static void csctx_rccurve_to(csctx* ctx, float dx1, float dy1, float dx2, float dy2,
            float dx3, float dy3)
        {
            float cx1 = ctx->x + dx1;
            float cy1 = ctx->y + dy1;
            float cx2 = cx1 + dx2;
            float cy2 = cy1 + dy2;
            ctx->x = cx2 + dx3;
            ctx->y = cy2 + dy3;
            csctx_v(ctx, vcubic, (int)(ctx->x), (int)(ctx->y), (int)cx1, (int)cy1,
                (int)cx2, (int)cy2);
        }

        public static buf get_subr(buf idx, int n)
        {
            int count = cff_index_count(&idx);
            int bias = 107;
            if (count >= 33900)
            {
                bias = 32768;
            }
            else if (count >= 1240)
            {
                bias = 1131;
            }

            n += bias;
            if ((n < 0) || (n >= count))
            {
                return new_buf(null, 0);
            }

            return cff_index_get(idx, n);
        }

        public static buf cid_get_glyph_subrs(fontinfo info, int glyph_index)
        {
            buf fdselect = info.fdselect;
            int nranges = 0;
            int start = 0;
            int end = 0;
            int v = 0;
            int fmt = 0;
            int fdselector = -1;
            int i = 0;
            buf_seek(&fdselect, 0);
            fmt = buf_get8(&fdselect);
            if (fmt == 0)
            {
                buf_skip(&fdselect, glyph_index);
                fdselector = buf_get8(&fdselect);
            }
            else if (fmt == 3)
            {
                nranges = (int)(buf_get((&fdselect), 2));
                start = (int)(buf_get((&fdselect), 2));
                for (i = 0; i < nranges; i++)
                {
                    v = buf_get8(&fdselect);
                    end = (int)(buf_get((&fdselect), 2));
                    if ((glyph_index >= start) && (glyph_index < end))
                    {
                        fdselector = v;
                        break;
                    }

                    start = end;
                }
            }

            if (fdselector == (-1))
            {
                new_buf(null, 0);
            }

            return get_subrs(info.cff,
                cff_index_get(info.fontdicts, fdselector));
        }

        public static int run_charstring(fontinfo info, int glyph_index, csctx* c)
        {
            int in_header = 1;
            int maskbits = 0;
            int subr_stack_height = 0;
            int sp = 0;
            int v = 0;
            int i = 0;
            int b0 = 0;
            int has_subrs = 0;
            int clear_stack = 0;
            float* s = stackalloc float[48];
            buf* subr_stack = stackalloc buf[10];
            buf subrs = info.subrs;
            buf b = new buf();
            float f = 0;
            b = cff_index_get(info.charstrings, glyph_index);
            while ((b.cursor) < (b.size))
            {
                i = 0;
                clear_stack = 1;
                b0 = buf_get8(&b);
                switch (b0)
                {
                    case 0x13:
                    case 0x14:
                        if (in_header != 0)
                        {
                            maskbits += sp / 2;
                        }

                        in_header = 0;
                        buf_skip(&b, (maskbits + 7) / 8);
                        break;
                    case 0x01:
                    case 0x03:
                    case 0x12:
                    case 0x17:
                        maskbits += sp / 2;
                        break;
                    case 0x15:
                        in_header = 0;
                        if (sp < 2)
                        {
                            return 0;
                        }

                        csctx_rmove_to(c, s[sp - 2], s[sp - 1]);
                        break;
                    case 0x04:
                        in_header = 0;
                        if (sp < 1)
                        {
                            return 0;
                        }

                        csctx_rmove_to(c, 0, s[sp - 1]);
                        break;
                    case 0x16:
                        in_header = 0;
                        if (sp < 1)
                        {
                            return 0;
                        }

                        csctx_rmove_to(c, s[sp - 1], 0);
                        break;
                    case 0x05:
                        if (sp < 2)
                        {
                            return 0;
                        }

                        for (; (i + 1) < sp; i += 2)
                        {
                            csctx_rline_to(c, s[i], s[i + 1]);
                        }

                        break;
                    case 0x07:
                    case 0x06:
                        if (sp < 1)
                        {
                            return 0;
                        }

                        int goto_vlineto = b0 == 0x07 ? 1 : 0;
                        for (; ; )
                        {
                            if (goto_vlineto == 0)
                            {
                                if (i >= sp)
                                {
                                    break;
                                }

                                csctx_rline_to(c, s[i], 0);
                                i++;
                            }

                            goto_vlineto = 0;
                            if (i >= sp)
                            {
                                break;
                            }

                            csctx_rline_to(c, 0, s[i]);
                            i++;
                        }

                        break;
                    case 0x1F:
                    case 0x1E:
                        if (sp < 4)
                        {
                            return 0;
                        }

                        int goto_hvcurveto = b0 == 0x1F ? 1 : 0;
                        for (; ; )
                        {
                            if (goto_hvcurveto == 0)
                            {
                                if ((i + 3) >= sp)
                                {
                                    break;
                                }

                                csctx_rccurve_to(c, 0, s[i], s[i + 1],
                                    s[i + 2], s[i + 3],
                                    ((sp - i) == 5) ? s[i + 4] : 0.0f);
                                i += 4;
                            }

                            goto_hvcurveto = 0;
                            if ((i + 3) >= sp)
                            {
                                break;
                            }

                            csctx_rccurve_to(c, s[i], 0, s[i + 1],
                                s[i + 2], ((sp - i) == 5) ? s[i + 4] : 0.0f, s[i + 3]);
                            i += 4;
                        }

                        break;
                    case 0x08:
                        if (sp < 6)
                        {
                            return 0;
                        }

                        for (; (i + 5) < sp; i += 6)
                        {
                            csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2],
                                s[i + 3], s[i + 4], s[i + 5]);
                        }

                        break;
                    case 0x18:
                        if (sp < 8)
                        {
                            return 0;
                        }

                        for (; (i + 5) < (sp - 2); i += 6)
                        {
                            csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2],
                                s[i + 3], s[i + 4], s[i + 5]);
                        }

                        if ((i + 1) >= sp)
                        {
                            return 0;
                        }

                        csctx_rline_to(c, s[i], s[i + 1]);
                        break;
                    case 0x19:
                        if (sp < 8)
                        {
                            return 0;
                        }

                        for (; (i + 1) < (sp - 6); i += 2)
                        {
                            csctx_rline_to(c, s[i], s[i + 1]);
                        }

                        if ((i + 5) >= sp)
                        {
                            return 0;
                        }

                        csctx_rccurve_to(c, s[i], s[i + 1], s[i + 2],
                            s[i + 3], s[i + 4], s[i + 5]);
                        break;
                    case 0x1A:
                    case 0x1B:
                        if (sp < 4)
                        {
                            return 0;
                        }

                        f = (float)(0.0);
                        if ((sp & 1) != 0)
                        {
                            f = s[i];
                            i++;
                        }

                        for (; (i + 3) < sp; i += 4)
                        {
                            if (b0 == 0x1B)
                            {
                                csctx_rccurve_to(c, s[i], f, s[i + 1],
                                    s[i + 2], s[i + 3], (float)(0.0));
                            }
                            else
                            {
                                csctx_rccurve_to(c, f, s[i], s[i + 1],
                                    s[i + 2], (float)(0.0), s[i + 3]);
                            }

                            f = (float)(0.0);
                        }

                        break;
                    case 0x0A:
                    case 0x1D:
                        if (b0 == 0x0A)
                        {
                            if (has_subrs == 0)
                            {
                                if ((info.fdselect.size) != 0)
                                {
                                    subrs = cid_get_glyph_subrs(info, glyph_index);
                                }

                                has_subrs = 1;
                            }
                        }

                        if (sp < 1)
                        {
                            return 0;
                        }

                        v = ((int)(s[--sp]));
                        if (subr_stack_height >= 10)
                        {
                            return 0;
                        }

                        subr_stack[subr_stack_height++] = b;
                        b = get_subr(b0 == 0x0A ? subrs : info.gsubrs,
                            v);
                        if ((b.size) == 0)
                        {
                            return 0;
                        }

                        b.cursor = 0;
                        clear_stack = 0;
                        break;
                    case 0x0B:
                        if (subr_stack_height <= 0)
                        {
                            return 0;
                        }

                        b = subr_stack[--subr_stack_height];
                        clear_stack = 0;
                        break;
                    case 0x0E:
                        csctx_close_shape(c);
                        return 1;
                    case 0x0C:
                        {
                            float dx1 = 0;
                            float dx2 = 0;
                            float dx3 = 0;
                            float dx4 = 0;
                            float dx5 = 0;
                            float dx6 = 0;
                            float dy1 = 0;
                            float dy2 = 0;
                            float dy3 = 0;
                            float dy4 = 0;
                            float dy5 = 0;
                            float dy6 = 0;
                            float dx = 0;
                            float dy = 0;
                            int b1 = buf_get8(&b);
                            switch (b1)
                            {
                                case 0x22:
                                    if (sp < 7)
                                    {
                                        return 0;
                                    }

                                    dx1 = s[0];
                                    dx2 = s[1];
                                    dy2 = s[2];
                                    dx3 = s[3];
                                    dx4 = s[4];
                                    dx5 = s[5];
                                    dx6 = s[6];
                                    csctx_rccurve_to(c, dx1, 0, dx2, dy2,
                                        dx3, 0);
                                    csctx_rccurve_to(c, dx4, 0, dx5, -dy2,
                                        dx6, 0);
                                    break;
                                case 0x23:
                                    if (sp < 13)
                                    {
                                        return 0;
                                    }

                                    dx1 = s[0];
                                    dy1 = s[1];
                                    dx2 = s[2];
                                    dy2 = s[3];
                                    dx3 = s[4];
                                    dy3 = s[5];
                                    dx4 = s[6];
                                    dy4 = s[7];
                                    dx5 = s[8];
                                    dy5 = s[9];
                                    dx6 = s[10];
                                    dy6 = s[11];
                                    csctx_rccurve_to(c, dx1, dy1, dx2, dy2,
                                        dx3, dy3);
                                    csctx_rccurve_to(c, dx4, dy4, dx5, dy5,
                                        dx6, dy6);
                                    break;
                                case 0x24:
                                    if (sp < 9)
                                    {
                                        return 0;
                                    }

                                    dx1 = s[0];
                                    dy1 = s[1];
                                    dx2 = s[2];
                                    dy2 = s[3];
                                    dx3 = s[4];
                                    dx4 = s[5];
                                    dx5 = s[6];
                                    dy5 = s[7];
                                    dx6 = s[8];
                                    csctx_rccurve_to(c, dx1, dy1, dx2, dy2,
                                        dx3, 0);
                                    csctx_rccurve_to(c, dx4, 0, dx5, dy5,
                                        dx6, -(dy1 + dy2 + dy5));
                                    break;
                                case 0x25:
                                    if (sp < 11)
                                    {
                                        return 0;
                                    }

                                    dx1 = s[0];
                                    dy1 = s[1];
                                    dx2 = s[2];
                                    dy2 = s[3];
                                    dx3 = s[4];
                                    dy3 = s[5];
                                    dx4 = s[6];
                                    dy4 = s[7];
                                    dx5 = s[8];
                                    dy5 = s[9];
                                    dx6 = dy6 = s[10];
                                    dx = dx1 + dx2 + dx3 + dx4 + dx5;
                                    dy = dy1 + dy2 + dy3 + dy4 + dy5;
                                    if ((CRuntime.fabs(dx)) > (CRuntime.fabs(dy)))
                                    {
                                        dy6 = -dy;
                                    }
                                    else
                                    {
                                        dx6 = -dx;
                                    }

                                    csctx_rccurve_to(c, dx1, dy1, dx2, dy2,
                                        dx3, dy3);
                                    csctx_rccurve_to(c, dx4, dy4, dx5, dy5,
                                        dx6, dy6);
                                    break;
                                default:
                                    return 0;
                            }
                        }
                        break;
                    default:
                        if (((b0 != 255) && (b0 != 28)) && ((b0 < 32) || (b0 > 254)))
                        {
                            return 0;
                        }

                        if (b0 == 255)
                        {
                            f = (float)((int)(buf_get((&b), 4))) / 0x10000;
                        }
                        else
                        {
                            buf_skip(&b, -1);
                            f = (short)(cff_int(&b));
                        }

                        if (sp >= 48)
                        {
                            return 0;
                        }

                        s[sp++] = f;
                        clear_stack = 0;
                        break;
                }

                if (clear_stack != 0)
                {
                    sp = 0;
                }
            }

            return 0;
        }

        public static int GetGlyphShapeT2(fontinfo info, int glyph_index, vertex** pvertices)
        {
            csctx count_ctx = new csctx
            {
                bounds = 1
            };
            csctx output_ctx = new csctx();
            if ((run_charstring(info, glyph_index, &count_ctx)) != 0)
            {
                *pvertices = (vertex*)(CRuntime.malloc((ulong)(count_ctx.num_vertices * sizeof(vertex))));
                output_ctx.pvertices = *pvertices;
                if ((run_charstring(info, glyph_index, &output_ctx)) != 0)
                {
                    return output_ctx.num_vertices;
                }
            }

            *pvertices = null;
            return 0;
        }

        public static int GetGlyphInfoT2(fontinfo info, int glyph_index, int* x0, int* y0, int* x1,
            int* y1)
        {
            csctx c = new csctx
            {
                bounds = 1
            };
            int r = run_charstring(info, glyph_index, &c);
            if (x0 != null)
            {
                *x0 = r != 0 ? c.min_x : 0;
            }

            if (y0 != null)
            {
                *y0 = r != 0 ? c.min_y : 0;
            }

            if (x1 != null)
            {
                *x1 = r != 0 ? c.max_x : 0;
            }

            if (y1 != null)
            {
                *y1 = r != 0 ? c.max_y : 0;
            }

            return r != 0 ? c.num_vertices : 0;
        }

        public static int GetGlyphShape(fontinfo info, int glyph_index, vertex** pvertices)
        {
            if (info.cff.size == 0)
            {
                return GetGlyphShapeTT(info, glyph_index, pvertices);
            }
            else
            {
                return GetGlyphShapeT2(info, glyph_index, pvertices);
            }
        }

        public static void GetGlyphHMetrics(fontinfo info, int glyph_index, int* advanceWidth,
            int* leftSideBearing)
        {
            ushort numOfLongHorMetrics = ttUSHORT(info.data + info.hhea + 34);
            if (glyph_index < numOfLongHorMetrics)
            {
                if (advanceWidth != null)
                {
                    *advanceWidth = ttSHORT(info.data + info.hmtx + 4 * glyph_index);
                }

                if (leftSideBearing != null)
                {
                    *leftSideBearing = ttSHORT(info.data + info.hmtx + 4 * glyph_index + 2);
                }
            }
            else
            {
                if (advanceWidth != null)
                {
                    *advanceWidth = ttSHORT(info.data + info.hmtx + 4 * (numOfLongHorMetrics - 1));
                }

                if (leftSideBearing != null)
                {
                    *leftSideBearing = ttSHORT(info.data + info.hmtx + 4 * numOfLongHorMetrics +
                                                      2 * (glyph_index - numOfLongHorMetrics));
                }
            }
        }

        public static int GetGlyphKernInfoAdvance(fontinfo info, int glyph1, int glyph2)
        {
            byte* data = info.data + info.kern;
            uint needle = 0;
            uint straw = 0;
            int l = 0;
            int r = 0;
            int m = 0;
            if (info.kern == 0)
            {
                return 0;
            }

            if ((ttUSHORT(data + 2)) < 1)
            {
                return 0;
            }

            if (ttUSHORT(data + 8) != 1)
            {
                return 0;
            }

            l = 0;
            r = ttUSHORT(data + 10) - 1;
            needle = (uint)(glyph1 << 16 | glyph2);
            while (l <= r)
            {
                m = (l + r) >> 1;
                straw = ttULONG(data + 18 + (m * 6));
                if (needle < straw)
                {
                    r = m - 1;
                }
                else if (needle > straw)
                {
                    l = m + 1;
                }
                else
                {
                    return ttSHORT(data + 22 + (m * 6));
                }
            }

            return 0;
        }

        public static int GetCoverageIndex(byte* coverageTable, int glyph)
        {
            ushort coverageFormat = ttUSHORT(coverageTable);
            switch (coverageFormat)
            {
                case 1:
                    {
                        ushort glyphCount = ttUSHORT(coverageTable + 2);
                        int l = 0;
                        int r = glyphCount - 1;
                        int m = 0;
                        int straw = 0;
                        int needle = glyph;
                        while (l <= r)
                        {
                            byte* glyphArray = coverageTable + 4;
                            ushort glyphID = 0;
                            m = (l + r) >> 1;
                            glyphID = ttUSHORT(glyphArray + 2 * m);
                            straw = glyphID;
                            if (needle < straw)
                            {
                                r = m - 1;
                            }
                            else if (needle > straw)
                            {
                                l = m + 1;
                            }
                            else
                            {
                                return m;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        ushort rangeCount = ttUSHORT(coverageTable + 2);
                        byte* rangeArray = coverageTable + 4;
                        int l = 0;
                        int r = rangeCount - 1;
                        int m = 0;
                        int strawStart = 0;
                        int strawEnd = 0;
                        int needle = glyph;
                        while (l <= r)
                        {
                            byte* rangeRecord;
                            m = (l + r) >> 1;
                            rangeRecord = rangeArray + 6 * m;
                            strawStart = ttUSHORT(rangeRecord);
                            strawEnd = ttUSHORT(rangeRecord + 2);
                            if (needle < strawStart)
                            {
                                r = m - 1;
                            }
                            else if (needle > strawEnd)
                            {
                                l = m + 1;
                            }
                            else
                            {
                                ushort startCoverageIndex = ttUSHORT(rangeRecord + 4);
                                return startCoverageIndex + glyph - strawStart;
                            }
                        }
                    }
                    break;
                default:
                    {
                    }
                    break;
            }

            return -1;
        }

        public static int GetGlyphClass(byte* classDefTable, int glyph)
        {
            ushort classDefFormat = ttUSHORT(classDefTable);
            switch (classDefFormat)
            {
                case 1:
                    {
                        ushort startGlyphID = ttUSHORT(classDefTable + 2);
                        ushort glyphCount = ttUSHORT(classDefTable + 4);
                        byte* classDef1ValueArray = classDefTable + 6;
                        if ((glyph >= startGlyphID) && (glyph < (startGlyphID + glyphCount)))
                        {
                            return ttUSHORT(classDef1ValueArray + 2 * (glyph - startGlyphID));
                        }

                        classDefTable = classDef1ValueArray + 2 * glyphCount;
                    }
                    break;
                case 2:
                    {
                        ushort classRangeCount = ttUSHORT(classDefTable + 2);
                        byte* classRangeRecords = classDefTable + 4;
                        int l = 0;
                        int r = classRangeCount - 1;
                        int m = 0;
                        int strawStart = 0;
                        int strawEnd = 0;
                        int needle = glyph;
                        while (l <= r)
                        {
                            byte* classRangeRecord;
                            m = (l + r) >> 1;
                            classRangeRecord = classRangeRecords + 6 * m;
                            strawStart = ttUSHORT(classRangeRecord);
                            strawEnd = ttUSHORT(classRangeRecord + 2);
                            if (needle < strawStart)
                            {
                                r = m - 1;
                            }
                            else if (needle > strawEnd)
                            {
                                l = m + 1;
                            }
                            else
                            {
                                return ttUSHORT(classRangeRecord + 4);
                            }
                        }

                        classDefTable = classRangeRecords + 6 * classRangeCount;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }

            return -1;
        }

        public static int GetGlyphGPOSInfoAdvance(fontinfo info, int glyph1, int glyph2)
        {
            ushort lookupListOffset = 0;
            byte* lookupList;
            ushort lookupCount = 0;
            byte* data;
            int i = 0;
            if (info.gpos == 0)
            {
                return 0;
            }

            data = info.data + info.gpos;
            if (ttUSHORT(data + 0) != 1)
            {
                return 0;
            }

            if (ttUSHORT(data + 2) != 0)
            {
                return 0;
            }

            lookupListOffset = ttUSHORT(data + 8);
            lookupList = data + lookupListOffset;
            lookupCount = ttUSHORT(lookupList);
            for (i = 0; i < lookupCount; ++i)
            {
                ushort lookupOffset = ttUSHORT(lookupList + 2 + 2 * i);
                byte* lookupTable = lookupList + lookupOffset;
                ushort lookupType = ttUSHORT(lookupTable);
                ushort subTableCount = ttUSHORT(lookupTable + 4);
                byte* subTableOffsets = lookupTable + 6;
                switch (lookupType)
                {
                    case 2:
                        {
                            int sti = 0;
                            for (sti = 0; sti < subTableCount; sti++)
                            {
                                ushort subtableOffset = ttUSHORT(subTableOffsets + 2 * sti);
                                byte* table = lookupTable + subtableOffset;
                                ushort posFormat = ttUSHORT(table);
                                ushort coverageOffset = ttUSHORT(table + 2);
                                int coverageIndex = GetCoverageIndex(table + coverageOffset, glyph1);
                                if (coverageIndex == (-1))
                                {
                                    continue;
                                }

                                switch (posFormat)
                                {
                                    case 1:
                                        {
                                            int l = 0;
                                            int r = 0;
                                            int m = 0;
                                            int straw = 0;
                                            int needle = 0;
                                            ushort valueFormat1 = ttUSHORT(table + 4);
                                            ushort valueFormat2 = ttUSHORT(table + 6);
                                            int valueRecordPairSizeInBytes = 2;
                                            ushort pairSetCount = ttUSHORT(table + 8);
                                            ushort pairPosOffset = ttUSHORT(table + 10 + 2 * coverageIndex);
                                            byte* pairValueTable = table + pairPosOffset;
                                            ushort pairValueCount = ttUSHORT(pairValueTable);
                                            byte* pairValueArray = pairValueTable + 2;
                                            if (valueFormat1 != 4)
                                            {
                                                return 0;
                                            }

                                            if (valueFormat2 != 0)
                                            {
                                                return 0;
                                            }

                                            needle = glyph2;
                                            r = pairValueCount - 1;
                                            l = 0;
                                            while (l <= r)
                                            {
                                                ushort secondGlyph = 0;
                                                byte* pairValue;
                                                m = (l + r) >> 1;
                                                pairValue = pairValueArray + (2 + valueRecordPairSizeInBytes) * m;
                                                secondGlyph = ttUSHORT(pairValue);
                                                straw = secondGlyph;
                                                if (needle < straw)
                                                {
                                                    r = m - 1;
                                                }
                                                else if (needle > straw)
                                                {
                                                    l = m + 1;
                                                }
                                                else
                                                {
                                                    short xAdvance = ttSHORT(pairValue + 2);
                                                    return xAdvance;
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            ushort valueFormat1 = ttUSHORT(table + 4);
                                            ushort valueFormat2 = ttUSHORT(table + 6);
                                            ushort classDef1Offset = ttUSHORT(table + 8);
                                            ushort classDef2Offset = ttUSHORT(table + 10);
                                            int glyph1class =
                                                GetGlyphClass(table + classDef1Offset, glyph1);
                                            int glyph2class =
                                                GetGlyphClass(table + classDef2Offset, glyph2);
                                            ushort class1Count = ttUSHORT(table + 12);
                                            ushort class2Count = ttUSHORT(table + 14);
                                            if (valueFormat1 != 4)
                                            {
                                                return 0;
                                            }

                                            if (valueFormat2 != 0)
                                            {
                                                return 0;
                                            }

                                            if ((((glyph1class >= 0) && (glyph1class < class1Count)) &&
                                                 (glyph2class >= 0)) && (glyph2class < class2Count))
                                            {
                                                byte* class1Records = table + 16;
                                                byte* class2Records = class1Records + 2 * (glyph1class * class2Count);
                                                short xAdvance = ttSHORT(class2Records + 2 * glyph2class);
                                                return xAdvance;
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            break;
                                        }
                                }
                            }

                            break;
                        }
                    default:
                        break;
                }
            }

            return 0;
        }

        public static int GetGlyphKernAdvance(fontinfo info, int g1, int g2)
        {
            int xAdvance = 0;
            if ((info.gpos) != 0)
            {
                xAdvance += GetGlyphGPOSInfoAdvance(info, g1, g2);
            }

            if ((info.kern) != 0)
            {
                xAdvance += GetGlyphKernInfoAdvance(info, g1, g2);
            }

            return xAdvance;
        }

        public static int GetCodepointKernAdvance(fontinfo info, int ch1, int ch2)
        {
            if ((info.kern == 0) && (info.gpos == 0))
            {
                return 0;
            }

            return GetGlyphKernAdvance(info, FindGlyphIndex(info, ch1),
                FindGlyphIndex(info, ch2));
        }

        public static void GetCodepointHMetrics(fontinfo info, int codepoint, int* advanceWidth,
            int* leftSideBearing)
        {
            GetGlyphHMetrics(info, FindGlyphIndex(info, codepoint), advanceWidth,
                leftSideBearing);
        }

        public static void GetFontVMetrics(fontinfo info, int* ascent, int* descent, int* lineGap)
        {
            if (ascent != null)
            {
                *ascent = ttSHORT(info.data + info.hhea + 4);
            }

            if (descent != null)
            {
                *descent = ttSHORT(info.data + info.hhea + 6);
            }

            if (lineGap != null)
            {
                *lineGap = ttSHORT(info.data + info.hhea + 8);
            }
        }

        public static int GetFontVMetricsOS2(fontinfo info, int* typoAscent, int* typoDescent,
            int* typoLineGap)
        {
            int tab = (int)(find_table(info.data, (uint)(info.fontstart), "OS/2"));
            if (tab == 0)
            {
                return 0;
            }

            if (typoAscent != null)
            {
                *typoAscent = ttSHORT(info.data + tab + 68);
            }

            if (typoDescent != null)
            {
                *typoDescent = ttSHORT(info.data + tab + 70);
            }

            if (typoLineGap != null)
            {
                *typoLineGap = ttSHORT(info.data + tab + 72);
            }

            return 1;
        }

        public static void GetFontBoundingBox(fontinfo info, int* x0, int* y0, int* x1, int* y1)
        {
            *x0 = ttSHORT(info.data + info.head + 36);
            *y0 = ttSHORT(info.data + info.head + 38);
            *x1 = ttSHORT(info.data + info.head + 40);
            *y1 = ttSHORT(info.data + info.head + 42);
        }

        public static float ScaleForPixelHeight(fontinfo info, float height)
        {
            int fheight = ttSHORT(info.data + info.hhea + 4) - ttSHORT(info.data + info.hhea + 6);
            return height / fheight;
        }

        public static float ScaleForMappingEmToPixels(fontinfo info, float pixels)
        {
            int unitsPerEm = ttUSHORT(info.data + info.head + 18);
            return pixels / unitsPerEm;
        }

        public static void FreeShape(fontinfo info, vertex* v)
        {
            CRuntime.free(v);
        }

        public static void GetGlyphBitmapBoxSubpixel(fontinfo font, int glyph, float scale_x, float scale_y,
            float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
        {
            int x0 = 0;
            int y0 = 0;
            int x1 = 0;
            int y1 = 0;
            if (GetGlyphBox(font, glyph, &x0, &y0, &x1, &y1) == 0)
            {
                if (ix0 != null)
                {
                    *ix0 = 0;
                }

                if (iy0 != null)
                {
                    *iy0 = 0;
                }

                if (ix1 != null)
                {
                    *ix1 = 0;
                }

                if (iy1 != null)
                {
                    *iy1 = 0;
                }
            }
            else
            {
                if (ix0 != null)
                {
                    *ix0 = ((int)(CRuntime.floor(x0 * scale_x + shift_x)));
                }

                if (iy0 != null)
                {
                    *iy0 = ((int)(CRuntime.floor(-y1 * scale_y + shift_y)));
                }

                if (ix1 != null)
                {
                    *ix1 = ((int)(CRuntime.ceil(x1 * scale_x + shift_x)));
                }

                if (iy1 != null)
                {
                    *iy1 = ((int)(CRuntime.ceil(-y0 * scale_y + shift_y)));
                }
            }
        }

        public static void GetGlyphBitmapBox(fontinfo font, int glyph, float scale_x, float scale_y,
            int* ix0, int* iy0, int* ix1, int* iy1)
        {
            GetGlyphBitmapBoxSubpixel(font, glyph, scale_x, scale_y, 0.0f,
                0.0f, ix0, iy0, ix1, iy1);
        }

        public static void GetCodepointBitmapBoxSubpixel(fontinfo font, int codepoint, float scale_x,
            float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
        {
            GetGlyphBitmapBoxSubpixel(font, FindGlyphIndex(font, codepoint),
                scale_x, scale_y, shift_x, shift_y, ix0, iy0, ix1, iy1);
        }

        public static void GetCodepointBitmapBox(fontinfo font, int codepoint, float scale_x, float scale_y,
            int* ix0, int* iy0, int* ix1, int* iy1)
        {
            GetCodepointBitmapBoxSubpixel(font, codepoint, scale_x, scale_y,
                0.0f, 0.0f, ix0, iy0, ix1, iy1);
        }

        public static void* hheap_alloc(hheap* hh, ulong size)
        {
            if ((hh->first_free) != null)
            {
                void* p = hh->first_free;
                hh->first_free = *(void**)p;
                return p;
            }
            else
            {
                if ((hh->num_remaining_in_head_chunk) == 0)
                {
                    int count = size < 32 ? 2000 : size < 128 ? 800 : 100;
                    hheap_chunk* c =
                        (hheap_chunk*)(CRuntime.malloc(
                            (ulong)sizeof(hheap_chunk) + size * (ulong)count));
                    if (c == null)
                    {
                        return null;
                    }

                    c->next = hh->head;
                    hh->head = c;
                    hh->num_remaining_in_head_chunk = count;
                }

                --hh->num_remaining_in_head_chunk;
                return (sbyte*)(hh->head) + sizeof(hheap_chunk) +
                       size * (ulong)hh->num_remaining_in_head_chunk;
            }
        }

        public static void hheap_free(hheap* hh, void* p)
        {
            *(void**)p = hh->first_free;
            hh->first_free = p;
        }

        public static void hheap_cleanup(hheap* hh)
        {
            hheap_chunk* c = hh->head;
            while (c != null)
            {
                hheap_chunk* n = c->next;
                CRuntime.free(c);
                c = n;
            }
        }

        public static active_edge* new_active(hheap* hh, edge* e, int off_x,
            float start_point)
        {
            active_edge* z =
                (active_edge*)(hheap_alloc(hh, (ulong)(sizeof(active_edge))));
            float dxdy = (e->x1 - e->x0) / (e->y1 - e->y0);
            if (z == null)
            {
                return z;
            }

            z->fdx = dxdy;
            z->fdy = dxdy != 0.0f ? (1.0f / dxdy) : 0.0f;
            z->fx = e->x0 + dxdy * (start_point - e->y0);
            z->fx -= off_x;
            z->direction = (e->invert) != 0 ? 1.0f : -1.0f;
            z->sy = e->y0;
            z->ey = e->y1;
            z->next = null;
            return z;
        }

        public static void handle_clipped_edge(float* scanline, int x, active_edge* e, float x0, float y0,
            float x1, float y1)
        {
            if (y0 == y1)
            {
                return;
            }

            if (y0 > (e->ey))
            {
                return;
            }

            if (y1 < (e->sy))
            {
                return;
            }

            if (y0 < (e->sy))
            {
                x0 += (x1 - x0) * (e->sy - y0) / (y1 - y0);
                y0 = e->sy;
            }

            if (y1 > (e->ey))
            {
                x1 += (x1 - x0) * (e->ey - y1) / (y1 - y0);
                y1 = e->ey;
            }

            if (x0 == x)
            {
            }
            else if (x0 == (x + 1))
            {
            }
            else if (x0 <= x)
            {
            }
            else if (x0 >= (x + 1))
            {
            }
            else
            {
            }

            if ((x0 <= x) && (x1 <= x))
            {
                scanline[x] += e->direction * (y1 - y0);
            }
            else if ((x0 >= (x + 1)) && (x1 >= (x + 1)))
            {
            }
            else
            {
                scanline[x] += e->direction * (y1 - y0) * (1 - ((x0 - x) + (x1 - x)) / 2);
            }
        }

        public static void fill_active_edges_new(float* scanline, float* scanline_fill, int len,
            active_edge* e, float y_top)
        {
            float y_bottom = y_top + 1;
            while (e != null)
            {
                if ((e->fdx) == 0)
                {
                    float x0 = e->fx;
                    if (x0 < len)
                    {
                        if (x0 >= 0)
                        {
                            handle_clipped_edge(scanline, (int)x0, e, x0, y_top,
                                x0, y_bottom);
                            handle_clipped_edge(scanline_fill - 1, (int)x0 + 1, e, x0,
                                y_top, x0, y_bottom);
                        }
                        else
                        {
                            handle_clipped_edge(scanline_fill - 1, 0, e, x0, y_top,
                                x0, y_bottom);
                        }
                    }
                }
                else
                {
                    float x0 = e->fx;
                    float dx = e->fdx;
                    float xb = x0 + dx;
                    float x_top = 0;
                    float x_bottom = 0;
                    float sy0 = 0;
                    float sy1 = 0;
                    float dy = e->fdy;
                    if ((e->sy) > y_top)
                    {
                        x_top = x0 + dx * (e->sy - y_top);
                        sy0 = e->sy;
                    }
                    else
                    {
                        x_top = x0;
                        sy0 = y_top;
                    }

                    if ((e->ey) < y_bottom)
                    {
                        x_bottom = x0 + dx * (e->ey - y_top);
                        sy1 = e->ey;
                    }
                    else
                    {
                        x_bottom = xb;
                        sy1 = y_bottom;
                    }

                    if ((((x_top >= 0) && (x_bottom >= 0)) && (x_top < len)) && (x_bottom < len))
                    {
                        if (((int)x_top) == ((int)x_bottom))
                        {
                            float height = 0;
                            int x = (int)x_top;
                            height = sy1 - sy0;
                            scanline[x] += e->direction * (1 - ((x_top - x) + (x_bottom - x)) / 2) * height;
                            scanline_fill[x] += e->direction * height;
                        }
                        else
                        {
                            int x = 0;
                            int x1 = 0;
                            int x2 = 0;
                            float y_crossing = 0;
                            float step = 0;
                            float sign = 0;
                            float area = 0;
                            if (x_top > x_bottom)
                            {
                                float t = 0;
                                sy0 = y_bottom - (sy0 - y_top);
                                sy1 = y_bottom - (sy1 - y_top);
                                t = sy0;
                                sy0 = sy1;
                                sy1 = t;
                                t = x_bottom;
                                x_bottom = x_top;
                                x_top = t;
                                dx = -dx;
                                dy = -dy;
                                t = x0;
                                x0 = xb;
                                xb = t;
                            }

                            x1 = ((int)x_top);
                            x2 = ((int)x_bottom);
                            y_crossing = (x1 + 1 - x0) * dy + y_top;
                            sign = e->direction;
                            area = sign * (y_crossing - sy0);
                            scanline[x1] += area * (1 - ((x_top - x1) + (x1 + 1 - x1)) / 2);
                            step = sign * dy;
                            for (x = x1 + 1; x < x2; ++x)
                            {
                                scanline[x] += area + step / 2;
                                area += step;
                            }

                            y_crossing += dy * (x2 - (x1 + 1));
                            scanline[x2] +=
                                area + sign * (1 - ((x2 - x2) + (x_bottom - x2)) / 2) * (sy1 - y_crossing);
                            scanline_fill[x2] += sign * (sy1 - sy0);
                        }
                    }
                    else
                    {
                        int x = 0;
                        for (x = 0; x < len; ++x)
                        {
                            float y0 = y_top;
                            float x1 = x;
                            float x2 = x + 1;
                            float x3 = xb;
                            float y3 = y_bottom;
                            float y1 = (x - x0) / dx + y_top;
                            float y2 = (x + 1 - x0) / dx + y_top;
                            if ((x0 < x1) && (x3 > x2))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x1, y1);
                                handle_clipped_edge(scanline, x, e, x1, y1,
                                    x2, y2);
                                handle_clipped_edge(scanline, x, e, x2, y2,
                                    x3, y3);
                            }
                            else if ((x3 < x1) && (x0 > x2))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x2, y2);
                                handle_clipped_edge(scanline, x, e, x2, y2,
                                    x1, y1);
                                handle_clipped_edge(scanline, x, e, x1, y1,
                                    x3, y3);
                            }
                            else if ((x0 < x1) && (x3 > x1))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x1, y1);
                                handle_clipped_edge(scanline, x, e, x1, y1,
                                    x3, y3);
                            }
                            else if ((x3 < x1) && (x0 > x1))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x1, y1);
                                handle_clipped_edge(scanline, x, e, x1, y1,
                                    x3, y3);
                            }
                            else if ((x0 < x2) && (x3 > x2))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x2, y2);
                                handle_clipped_edge(scanline, x, e, x2, y2,
                                    x3, y3);
                            }
                            else if ((x3 < x2) && (x0 > x2))
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x2, y2);
                                handle_clipped_edge(scanline, x, e, x2, y2,
                                    x3, y3);
                            }
                            else
                            {
                                handle_clipped_edge(scanline, x, e, x0, y0,
                                    x3, y3);
                            }
                        }
                    }
                }

                e = e->next;
            }
        }

        public static void rasterize_sorted_edges(bitmap* result, edge* e, int n, int vsubsample,
            int off_x, int off_y)
        {
            hheap hh = new hheap();
            active_edge* active = null;
            int y = 0;
            int j = 0;
            int i = 0;
            float* scanline_data = stackalloc float[129];
            float* scanline;
            float* scanline2;
            if ((result->w) > 64)
            {
                scanline = (float*)(CRuntime.malloc((ulong)((result->w * 2 + 1) * sizeof(float))));
            }
            else
            {
                scanline = scanline_data;
            }

            scanline2 = scanline + result->w;
            y = off_y;
            e[n].y0 = (float)(off_y + result->h) + 1;
            while (j < (result->h))
            {
                float scan_y_top = y + 0.0f;
                float scan_y_bottom = y + 1.0f;
                active_edge** step = &active;
                CRuntime.memset(scanline, 0, (ulong)(result->w * sizeof(float)));
                CRuntime.memset(scanline2, 0, (ulong)((result->w + 1) * sizeof(float)));
                while ((*step) != null)
                {
                    active_edge* z = *step;
                    if (z->ey <= scan_y_top)
                    {
                        *step = z->next;
                        z->direction = 0;
                        hheap_free(&hh, z);
                    }
                    else
                    {
                        step = &((*step)->next);
                    }
                }

                while (e->y0 <= scan_y_bottom)
                {
                    if (e->y0 != e->y1)
                    {
                        active_edge* z = new_active(&hh, e, off_x, scan_y_top);
                        if (z != null)
                        {
                            if ((j == 0) && (off_y != 0))
                            {
                                if ((z->ey) < scan_y_top)
                                {
                                    z->ey = scan_y_top;
                                }
                            }

                            z->next = active;
                            active = z;
                        }
                    }

                    ++e;
                }

                if (active != null)
                {
                    fill_active_edges_new(scanline, scanline2 + 1, result->w, active,
                        scan_y_top);
                }

                {
                    float sum = 0;
                    for (i = 0; i < (result->w); ++i)
                    {
                        float k = 0;
                        int m = 0;
                        sum += scanline2[i];
                        k = scanline[i] + sum;
                        k = CRuntime.fabs(k) * 255 + 0.5f;
                        m = ((int)k);
                        if (m > 255)
                        {
                            m = 255;
                        }

                        result->pixels[j * result->stride + i] = ((byte)m);
                    }
                }
                step = &active;
                while ((*step) != null)
                {
                    active_edge* z = *step;
                    z->fx += z->fdx;
                    step = &((*step)->next);
                }

                ++y;
                ++j;
            }

            hheap_cleanup(&hh);
            if (scanline != scanline_data)
            {
                CRuntime.free(scanline);
            }
        }

        public static void sort_edges_ins_sort(edge* p, int n)
        {
            int i = 0;
            int j = 0;
            for (i = 1; i < n; ++i)
            {
                edge t = p[i];
                edge* a = &t;
                j = i;
                while (j > 0)
                {
                    edge* b = &p[j - 1];
                    int c = a->y0 < b->y0 ? 1 : 0;
                    if (c == 0)
                    {
                        break;
                    }

                    p[j] = p[j - 1];
                    --j;
                }

                if (i != j)
                {
                    p[j] = t;
                }
            }
        }

        public static void sort_edges_quicksort(edge* p, int n)
        {
            while (n > 12)
            {
                edge t = new edge();
                int c01 = 0;
                int c12 = 0;
                int c = 0;
                int m = 0;
                int i = 0;
                int j = 0;
                m = n >> 1;
                c01 = ((&p[0])->y0) < ((&p[m])->y0) ? 1 : 0;
                c12 = ((&p[m])->y0) < ((&p[n - 1])->y0) ? 1 : 0;
                if (c01 != c12)
                {
                    int z = 0;
                    c = ((&p[0])->y0) < ((&p[n - 1])->y0) ? 1 : 0;
                    z = (c == c12) ? 0 : n - 1;
                    t = p[z];
                    p[z] = p[m];
                    p[m] = t;
                }

                t = p[0];
                p[0] = p[m];
                p[m] = t;
                i = 1;
                j = n - 1;
                for (; ; )
                {
                    for (; ; ++i)
                    {
                        if (!(((&p[i])->y0) < ((&p[0])->y0)))
                        {
                            break;
                        }
                    }

                    for (; ; --j)
                    {
                        if (!(((&p[0])->y0) < ((&p[j])->y0)))
                        {
                            break;
                        }
                    }

                    if (i >= j)
                    {
                        break;
                    }

                    t = p[i];
                    p[i] = p[j];
                    p[j] = t;
                    ++i;
                    --j;
                }

                if (j < (n - i))
                {
                    sort_edges_quicksort(p, j);
                    p = p + i;
                    n = n - i;
                }
                else
                {
                    sort_edges_quicksort(p + i, n - i);
                    n = j;
                }
            }
        }

        public static void sort_edges(edge* p, int n)
        {
            sort_edges_quicksort(p, n);
            sort_edges_ins_sort(p, n);
        }

        public static void rasterize(bitmap* result, point* pts, int* wcount, int windings,
            float scale_x, float scale_y, float shift_x, float shift_y, int off_x, int off_y, int invert)
        {
            float y_scale_inv = invert != 0 ? -scale_y : scale_y;
            edge* e;
            int n = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int vsubsample = 1;
            n = 0;
            for (i = 0; i < windings; ++i)
            {
                n += wcount[i];
            }

            e = (edge*)(CRuntime.malloc((ulong)(sizeof(edge) * (n + 1))));
            if (e == null)
            {
                return;
            }

            n = 0;
            m = 0;
            for (i = 0; i < windings; ++i)
            {
                point* p = pts + m;
                m += wcount[i];
                j = wcount[i] - 1;
                for (k = 0; k < (wcount[i]); j = k++)
                {
                    int a = k;
                    int b = j;
                    if ((p[j].y) == (p[k].y))
                    {
                        continue;
                    }

                    e[n].invert = 0;
                    if (((invert != 0) && ((p[j].y) > (p[k].y))) || ((invert == 0) && ((p[j].y) < (p[k].y))))
                    {
                        e[n].invert = 1;
                        a = j;
                        b = k;
                    }

                    e[n].x0 = p[a].x * scale_x + shift_x;
                    e[n].y0 = (p[a].y * y_scale_inv + shift_y) * vsubsample;
                    e[n].x1 = p[b].x * scale_x + shift_x;
                    e[n].y1 = (p[b].y * y_scale_inv + shift_y) * vsubsample;
                    ++n;
                }
            }

            sort_edges(e, n);
            rasterize_sorted_edges(result, e, n, vsubsample, off_x, off_y);
            CRuntime.free(e);
        }

        public static void add_point(point* points, int n, float x, float y)
        {
            if (points == null)
            {
                return;
            }

            points[n].x = x;
            points[n].y = y;
        }

        public static int tesselate_curve(point* points, int* num_points, float x0, float y0, float x1,
            float y1, float x2, float y2, float objspace_flatness_squared, int n)
        {
            float mx = (x0 + 2 * x1 + x2) / 4;
            float my = (y0 + 2 * y1 + y2) / 4;
            float dx = (x0 + x2) / 2 - mx;
            float dy = (y0 + y2) / 2 - my;
            if (n > 16)
            {
                return 1;
            }

            if ((dx * dx + dy * dy) > objspace_flatness_squared)
            {
                tesselate_curve(points, num_points, x0, y0, (x0 + x1) / 2.0f,
                    (y0 + y1) / 2.0f, mx, my, objspace_flatness_squared,
                    n + 1);
                tesselate_curve(points, num_points, mx, my, (x1 + x2) / 2.0f,
                    (y1 + y2) / 2.0f, x2, y2, objspace_flatness_squared,
                    n + 1);
            }
            else
            {
                add_point(points, *num_points, x2, y2);
                *num_points = *num_points + 1;
            }

            return 1;
        }

        public static void tesselate_cubic(point* points, int* num_points, float x0, float y0, float x1,
            float y1, float x2, float y2, float x3, float y3, float objspace_flatness_squared, int n)
        {
            float dx0 = x1 - x0;
            float dy0 = y1 - y0;
            float dx1 = x2 - x1;
            float dy1 = y2 - y1;
            float dx2 = x3 - x2;
            float dy2 = y3 - y2;
            float dx = x3 - x0;
            float dy = y3 - y0;
            float longlen = (float)(CRuntime.sqrt(dx0 * dx0 + dy0 * dy0) +
                                     CRuntime.sqrt(dx1 * dx1 + dy1 * dy1) +
                                     CRuntime.sqrt(dx2 * dx2 + dy2 * dy2));
            float shortlen = (float)(CRuntime.sqrt(dx * dx + dy * dy));
            float flatness_squared = longlen * longlen - shortlen * shortlen;
            if (n > 16)
            {
                return;
            }

            if (flatness_squared > objspace_flatness_squared)
            {
                float x01 = (x0 + x1) / 2;
                float y01 = (y0 + y1) / 2;
                float x12 = (x1 + x2) / 2;
                float y12 = (y1 + y2) / 2;
                float x23 = (x2 + x3) / 2;
                float y23 = (y2 + y3) / 2;
                float xa = (x01 + x12) / 2;
                float ya = (y01 + y12) / 2;
                float xb = (x12 + x23) / 2;
                float yb = (y12 + y23) / 2;
                float mx = (xa + xb) / 2;
                float my = (ya + yb) / 2;
                tesselate_cubic(points, num_points, x0, y0, x01, y01,
                    xa, ya, mx, my, objspace_flatness_squared,
                    n + 1);
                tesselate_cubic(points, num_points, mx, my, xb, yb,
                    x23, y23, x3, y3, objspace_flatness_squared,
                    n + 1);
            }
            else
            {
                add_point(points, *num_points, x3, y3);
                *num_points = *num_points + 1;
            }
        }

        public static point* FlattenCurves(vertex* vertices, int num_verts, float objspace_flatness,
            int** contour_lengths, int* num_contours)
        {
            point* points = null;
            int num_points = 0;
            float objspace_flatness_squared = objspace_flatness * objspace_flatness;
            int i = 0;
            int n = 0;
            int start = 0;
            int pass = 0;
            for (i = 0; i < num_verts; ++i)
            {
                if ((vertices[i].type) == vmove)
                {
                    ++n;
                }
            }

            *num_contours = n;
            if (n == 0)
            {
                return null;
            }

            *contour_lengths = (int*)(CRuntime.malloc((ulong)(sizeof(int) * n)));
            if ((*contour_lengths) == null)
            {
                *num_contours = 0;
                return null;
            }

            for (pass = 0; pass < 2; ++pass)
            {
                float x = 0;
                float y = 0;
                if (pass == 1)
                {
                    points = (point*)(CRuntime.malloc((ulong)(num_points * sizeof(point))));
                    if (points == null)
                    {
                        goto error;
                    }
                }

                num_points = 0;
                n = -1;
                for (i = 0; i < num_verts; ++i)
                {
                    switch (vertices[i].type)
                    {
                        case vmove:
                            if (n >= 0)
                            {
                                (*contour_lengths)[n] = num_points - start;
                            }

                            ++n;
                            start = num_points;
                            x = vertices[i].x;
                            y = vertices[i].y;
                            add_point(points, num_points++, x, y);
                            break;
                        case vline:
                            x = vertices[i].x;
                            y = vertices[i].y;
                            add_point(points, num_points++, x, y);
                            break;
                        case vcurve:
                            tesselate_curve(points, &num_points, x, y,
                                vertices[i].cx, vertices[i].cy, vertices[i].x,
                                vertices[i].y, objspace_flatness_squared, 0);
                            x = vertices[i].x;
                            y = vertices[i].y;
                            break;
                        case vcubic:
                            tesselate_cubic(points, &num_points, x, y,
                                vertices[i].cx, vertices[i].cy, vertices[i].cx1,
                                vertices[i].cy1, vertices[i].x, vertices[i].y,
                                objspace_flatness_squared, 0);
                            x = vertices[i].x;
                            y = vertices[i].y;
                            break;
                    }
                }

                (*contour_lengths)[n] = num_points - start;
            }

            return points;
        error:;
            CRuntime.free(points);
            CRuntime.free(*contour_lengths);
            *contour_lengths = null;
            *num_contours = 0;
            return null;
        }

        public static void Rasterize(bitmap* result, float flatness_in_pixels, vertex* vertices,
            int num_verts, float scale_x, float scale_y, float shift_x, float shift_y, int x_off, int y_off, int invert)
        {
            float scale = scale_x > scale_y ? scale_y : scale_x;
            int winding_count = 0;
            int* winding_lengths = null;
            point* windings = FlattenCurves(vertices, num_verts,
                flatness_in_pixels / scale, &winding_lengths, &winding_count);
            if (windings != null)
            {
                rasterize(result, windings, winding_lengths, winding_count, scale_x,
                    scale_y, shift_x, shift_y, x_off, y_off,
                    invert);
                CRuntime.free(winding_lengths);
                CRuntime.free(windings);
            }
        }

        public static void FreeBitmap(byte* bitmap)
        {
            CRuntime.free(bitmap);
        }

        public static byte* GetGlyphBitmapSubpixel(fontinfo info, float scale_x, float scale_y,
            float shift_x, float shift_y, int glyph, int* width, int* height, int* xoff, int* yoff)
        {
            int ix0 = 0;
            int iy0 = 0;
            int ix1 = 0;
            int iy1 = 0;
            bitmap gbm = new bitmap();
            vertex* vertices;
            int num_verts = GetGlyphShape(info, glyph, &vertices);
            if (scale_x == 0)
            {
                scale_x = scale_y;
            }

            if (scale_y == 0)
            {
                if (scale_x == 0)
                {
                    CRuntime.free(vertices);
                    return null;
                }

                scale_y = scale_x;
            }

            GetGlyphBitmapBoxSubpixel(info, glyph, scale_x, scale_y,
                shift_x, shift_y, &ix0, &iy0, &ix1, &iy1);
            gbm.w = ix1 - ix0;
            gbm.h = iy1 - iy0;
            gbm.pixels = null;
            if (width != null)
            {
                *width = gbm.w;
            }

            if (height != null)
            {
                *height = gbm.h;
            }

            if (xoff != null)
            {
                *xoff = ix0;
            }

            if (yoff != null)
            {
                *yoff = iy0;
            }

            if (((gbm.w) != 0) && ((gbm.h) != 0))
            {
                gbm.pixels = (byte*)(CRuntime.malloc((ulong)(gbm.w * gbm.h)));
                if ((gbm.pixels) != null)
                {
                    gbm.stride = gbm.w;
                    Rasterize(&gbm, 0.35f, vertices, num_verts, scale_x,
                        scale_y, shift_x, shift_y, ix0, iy0, 1);
                }
            }

            CRuntime.free(vertices);
            return gbm.pixels;
        }

        public static byte* GetGlyphBitmap(fontinfo info, float scale_x, float scale_y, int glyph,
            int* width, int* height, int* xoff, int* yoff)
        {
            return GetGlyphBitmapSubpixel(info, scale_x, scale_y, 0.0f,
                0.0f, glyph, width, height, xoff, yoff);
        }

        public static void MakeGlyphBitmapSubpixel(fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph)
        {
            int ix0 = 0;
            int iy0 = 0;
            vertex* vertices;
            int num_verts = GetGlyphShape(info, glyph, &vertices);
            bitmap gbm = new bitmap();
            GetGlyphBitmapBoxSubpixel(info, glyph, scale_x, scale_y,
                shift_x, shift_y, &ix0, &iy0, null, null);
            gbm.pixels = output;
            gbm.w = out_w;
            gbm.h = out_h;
            gbm.stride = out_stride;
            if (((gbm.w) != 0) && ((gbm.h) != 0))
            {
                Rasterize(&gbm, 0.35f, vertices, num_verts, scale_x,
                    scale_y, shift_x, shift_y, ix0, iy0, 1);
            }

            CRuntime.free(vertices);
        }

        public static void MakeGlyphBitmap(fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, int glyph)
        {
            MakeGlyphBitmapSubpixel(info, output, out_w, out_h, out_stride,
                scale_x, scale_y, 0.0f, 0.0f, glyph);
        }

        public static byte* GetCodepointBitmapSubpixel(fontinfo info, float scale_x, float scale_y,
            float shift_x, float shift_y, int codepoint, int* width, int* height, int* xoff, int* yoff)
        {
            return GetGlyphBitmapSubpixel(info, scale_x, scale_y, shift_x,
                shift_y, FindGlyphIndex(info, codepoint), width, height, xoff, yoff);
        }

        public static void MakeCodepointBitmapSubpixelPrefilter(fontinfo info, byte* output, int out_w,
            int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int oversample_x,
            int oversample_y, float* sub_x, float* sub_y, int codepoint)
        {
            MakeGlyphBitmapSubpixelPrefilter(info, output, out_w, out_h, out_stride,
                scale_x, scale_y, shift_x, shift_y, oversample_x,
                oversample_y, sub_x, sub_y, FindGlyphIndex(info, codepoint));
        }

        public static void MakeCodepointBitmapSubpixel(fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint)
        {
            MakeGlyphBitmapSubpixel(info, output, out_w, out_h, out_stride,
                scale_x, scale_y, shift_x, shift_y,
                FindGlyphIndex(info, codepoint));
        }

        public static byte* GetCodepointBitmap(fontinfo info, float scale_x, float scale_y, int codepoint,
            int* width, int* height, int* xoff, int* yoff)
        {
            return GetCodepointBitmapSubpixel(info, scale_x, scale_y, 0.0f,
                0.0f, codepoint, width, height, xoff, yoff);
        }

        public static void MakeCodepointBitmap(fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, int codepoint)
        {
            MakeCodepointBitmapSubpixel(info, output, out_w, out_h, out_stride,
                scale_x, scale_y, 0.0f, 0.0f, codepoint);
        }

        public static int BakeFontBitmap_internal(byte* data, int offset, float pixel_height, byte* pixels,
            int pw, int ph, int first_char, int num_chars, bakedchar* chardata)
        {
            float scale = 0;
            int x = 0;
            int y = 0;
            int bottom_y = 0;
            int i = 0;
            fontinfo f = new fontinfo();
            if (InitFont(f, data, offset) == 0)
            {
                return -1;
            }

            CRuntime.memset(pixels, 0, (ulong)(pw * ph));
            x = y = 1;
            bottom_y = 1;
            scale = ScaleForPixelHeight(f, pixel_height);
            for (i = 0; i < num_chars; ++i)
            {
                int advance = 0;
                int lsb = 0;
                int x0 = 0;
                int y0 = 0;
                int x1 = 0;
                int y1 = 0;
                int gw = 0;
                int gh = 0;
                int g = FindGlyphIndex(f, first_char + i);
                GetGlyphHMetrics(f, g, &advance, &lsb);
                GetGlyphBitmapBox(f, g, scale, scale, &x0, &y0, &x1, &y1);
                gw = x1 - x0;
                gh = y1 - y0;
                if ((x + gw + 1) >= pw)
                {
                    y = bottom_y;
                    x = 1;
                }

                if ((y + gh + 1) >= ph)
                {
                    return -i;
                }

                MakeGlyphBitmap(f, pixels + x + y * pw, gw, gh, pw, scale,
                    scale, g);
                chardata[i].x0 = (ushort)((short)x);
                chardata[i].y0 = (ushort)((short)y);
                chardata[i].x1 = (ushort)((short)(x + gw));
                chardata[i].y1 = (ushort)((short)(y + gh));
                chardata[i].xadvance = scale * advance;
                chardata[i].xoff = x0;
                chardata[i].yoff = y0;
                x = x + gw + 1;
                if ((y + gh + 1) > bottom_y)
                {
                    bottom_y = y + gh + 1;
                }
            }

            return bottom_y;
        }

        public static void GetBakedQuad(bakedchar* chardata, int pw, int ph, int char_index, float* xpos,
            float* ypos, aligned_quad* q, int opengl_fillrule)
        {
            float d3d_bias = opengl_fillrule != 0 ? 0 : -0.5f;
            float ipw = 1.0f / pw;
            float iph = 1.0f / ph;
            bakedchar* b = chardata + char_index;
            int round_x = ((int)(CRuntime.floor((*xpos + b->xoff) + 0.5f)));
            int round_y = ((int)(CRuntime.floor((*ypos + b->yoff) + 0.5f)));
            q->x0 = round_x + d3d_bias;
            q->y0 = round_y + d3d_bias;
            q->x1 = round_x + b->x1 - b->x0 + d3d_bias;
            q->y1 = round_y + b->y1 - b->y0 + d3d_bias;
            q->s0 = b->x0 * ipw;
            q->t0 = b->y0 * iph;
            q->s1 = b->x1 * ipw;
            q->t1 = b->y1 * iph;
            *xpos += b->xadvance;
        }

        public static void stbrp_init_target(stbrp_context* con, int pw, int ph, stbrp_node* nodes, int num_nodes)
        {
            con->width = pw;
            con->height = ph;
            con->x = 0;
            con->y = 0;
            con->bottom_y = 0;
        }

        public static void stbrp_pack_rects(stbrp_context* con, stbrp_rect* rects, int num_rects)
        {
            int i = 0;
            for (i = 0; i < num_rects; ++i)
            {
                if ((con->x + rects[i].w) > (con->width))
                {
                    con->x = 0;
                    con->y = con->bottom_y;
                }

                if ((con->y + rects[i].h) > (con->height))
                {
                    break;
                }

                rects[i].x = con->x;
                rects[i].y = con->y;
                rects[i].was_packed = 1;
                con->x += rects[i].w;
                if ((con->y + rects[i].h) > (con->bottom_y))
                {
                    con->bottom_y = con->y + rects[i].h;
                }
            }

            for (; i < num_rects; ++i)
            {
                rects[i].was_packed = 0;
            }
        }

        public static int PackBegin(pack_context spc, byte* pixels, int pw, int ph, int stride_in_bytes,
            int padding, void* alloc_context)
        {
            stbrp_context* context = (stbrp_context*)(CRuntime.malloc((ulong)(sizeof(stbrp_context))));
            int num_nodes = pw - padding;
            stbrp_node* nodes = (stbrp_node*)(CRuntime.malloc((ulong)(sizeof(stbrp_node) * num_nodes)));
            if ((context == null) || (nodes == null))
            {
                if (context != null)
                {
                    CRuntime.free(context);
                }

                if (nodes != null)
                {
                    CRuntime.free(nodes);
                }

                return 0;
            }

            spc.user_allocator_context = alloc_context;
            spc.width = pw;
            spc.height = ph;
            spc.pixels = pixels;
            spc.pack_info = context;
            spc.nodes = nodes;
            spc.padding = padding;
            spc.stride_in_bytes = stride_in_bytes != 0 ? stride_in_bytes : pw;
            spc.h_oversample = 1;
            spc.v_oversample = 1;
            spc.skip_missing = 0;
            stbrp_init_target(context, pw - padding, ph - padding, nodes, num_nodes);
            if (pixels != null)
            {
                CRuntime.memset(pixels, 0, (ulong)(pw * ph));
            }

            return 1;
        }

        public static void PackEnd(pack_context spc)
        {
            CRuntime.free(spc.nodes);
            CRuntime.free(spc.pack_info);
        }

        public static void PackSetOversampling(pack_context spc, uint h_oversample, uint v_oversample)
        {
            if (h_oversample <= 8)
            {
                spc.h_oversample = h_oversample;
            }

            if (v_oversample <= 8)
            {
                spc.v_oversample = v_oversample;
            }
        }

        public static void PackSetSkipMissingCodepoints(pack_context spc, int skip)
        {
            spc.skip_missing = skip;
        }

        public static void h_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
        {
            byte* buffer = stackalloc byte[8];
            int safe_w = (int)(w - kernel_width);
            int j = 0;
            CRuntime.memset(buffer, 0, (ulong)8);
            for (j = 0; j < h; ++j)
            {
                int i = 0;
                uint total = 0;
                CRuntime.memset(buffer, 0, (ulong)kernel_width);
                total = 0;
                switch (kernel_width)
                {
                    case 2:
                        for (i = 0; i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
                            pixels[i] = ((byte)(total / 2));
                        }

                        break;
                    case 3:
                        for (i = 0; i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
                            pixels[i] = ((byte)(total / 3));
                        }

                        break;
                    case 4:
                        for (i = 0; i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
                            pixels[i] = ((byte)(total / 4));
                        }

                        break;
                    case 5:
                        for (i = 0; i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
                            pixels[i] = ((byte)(total / 5));
                        }

                        break;
                    default:
                        for (i = 0; i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
                            pixels[i] = ((byte)(total / kernel_width));
                        }

                        break;
                }

                for (; i < w; ++i)
                {
                    total -= buffer[i & (8 - 1)];
                    pixels[i] = ((byte)(total / kernel_width));
                }

                pixels += stride_in_bytes;
            }
        }

        public static void v_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
        {
            byte* buffer = stackalloc byte[8];
            int safe_h = (int)(h - kernel_width);
            int j = 0;
            CRuntime.memset(buffer, 0, (ulong)8);
            for (j = 0; j < w; ++j)
            {
                int i = 0;
                uint total = 0;
                CRuntime.memset(buffer, 0, (ulong)kernel_width);
                total = 0;
                switch (kernel_width)
                {
                    case 2:
                        for (i = 0; i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
                            pixels[i * stride_in_bytes] = ((byte)(total / 2));
                        }

                        break;
                    case 3:
                        for (i = 0; i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
                            pixels[i * stride_in_bytes] = ((byte)(total / 3));
                        }

                        break;
                    case 4:
                        for (i = 0; i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
                            pixels[i * stride_in_bytes] = ((byte)(total / 4));
                        }

                        break;
                    case 5:
                        for (i = 0; i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
                            pixels[i * stride_in_bytes] = ((byte)(total / 5));
                        }

                        break;
                    default:
                        for (i = 0; i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
                            pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
                        }

                        break;
                }

                for (; i < h; ++i)
                {
                    total -= buffer[i & (8 - 1)];
                    pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
                }

                pixels += 1;
            }
        }

        public static float oversample_shift(int oversample)
        {
            if (oversample == 0)
            {
                return 0.0f;
            }

            return -(oversample - 1) / (2.0f * oversample);
        }

        public static int PackFontRangesGatherRects(pack_context spc, fontinfo info,
            pack_range* ranges, int num_ranges, stbrp_rect* rects)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            k = 0;
            for (i = 0; i < num_ranges; ++i)
            {
                float fh = ranges[i].font_size;
                float scale = fh > 0
                    ? ScaleForPixelHeight(info, fh)
                    : ScaleForMappingEmToPixels(info, -fh);
                ranges[i].h_oversample = ((byte)(spc.h_oversample));
                ranges[i].v_oversample = ((byte)(spc.v_oversample));
                for (j = 0; j < (ranges[i].num_chars); ++j)
                {
                    int x0 = 0;
                    int y0 = 0;
                    int x1 = 0;
                    int y1 = 0;
                    int codepoint = (ranges[i].array_of_unicode_codepoints) == null
                        ? ranges[i].first_unicode_codepoint_in_range + j
                        : ranges[i].array_of_unicode_codepoints[j];
                    int glyph = FindGlyphIndex(info, codepoint);
                    if ((glyph == 0) && ((spc.skip_missing) != 0))
                    {
                        rects[k].w = rects[k].h = 0;
                    }
                    else
                    {
                        GetGlyphBitmapBoxSubpixel(info, glyph, scale * spc.h_oversample,
                            scale * spc.v_oversample, 0, 0, &x0, &y0, &x1, &y1);
                        rects[k].w = ((int)(x1 - x0 + spc.padding + spc.h_oversample - 1));
                        rects[k].h = ((int)(y1 - y0 + spc.padding + spc.v_oversample - 1));
                    }

                    ++k;
                }
            }

            return k;
        }

        public static void MakeGlyphBitmapSubpixelPrefilter(fontinfo info, byte* output, int out_w,
            int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int prefilter_x,
            int prefilter_y, float* sub_x, float* sub_y, int glyph)
        {
            MakeGlyphBitmapSubpixel(info, output, out_w - (prefilter_x - 1),
                out_h - (prefilter_y - 1), out_stride, scale_x, scale_y,
                shift_x, shift_y, glyph);
            if (prefilter_x > 1)
            {
                h_prefilter(output, out_w, out_h, out_stride, (uint)prefilter_x);
            }

            if (prefilter_y > 1)
            {
                v_prefilter(output, out_w, out_h, out_stride, (uint)prefilter_y);
            }

            *sub_x = oversample_shift(prefilter_x);
            *sub_y = oversample_shift(prefilter_y);
        }

        public static int PackFontRangesRenderIntoRects(pack_context spc, fontinfo info,
            pack_range* ranges, int num_ranges, stbrp_rect* rects)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int return_value = 1;
            int old_h_over = (int)(spc.h_oversample);
            int old_v_over = (int)(spc.v_oversample);
            k = 0;
            for (i = 0; i < num_ranges; ++i)
            {
                float fh = ranges[i].font_size;
                float scale = fh > 0
                    ? ScaleForPixelHeight(info, fh)
                    : ScaleForMappingEmToPixels(info, -fh);
                float recip_h = 0;
                float recip_v = 0;
                float sub_x = 0;
                float sub_y = 0;
                spc.h_oversample = ranges[i].h_oversample;
                spc.v_oversample = ranges[i].v_oversample;
                recip_h = 1.0f / spc.h_oversample;
                recip_v = 1.0f / spc.v_oversample;
                sub_x = oversample_shift((int)(spc.h_oversample));
                sub_y = oversample_shift((int)(spc.v_oversample));
                for (j = 0; j < (ranges[i].num_chars); ++j)
                {
                    stbrp_rect* r = &rects[k];
                    if ((((r->was_packed) != 0) && (r->w != 0)) && (r->h != 0))
                    {
                        packedchar* bc = &ranges[i].chardata_for_range[j];
                        int advance = 0;
                        int lsb = 0;
                        int x0 = 0;
                        int y0 = 0;
                        int x1 = 0;
                        int y1 = 0;
                        int codepoint = (ranges[i].array_of_unicode_codepoints) == null
                            ? ranges[i].first_unicode_codepoint_in_range + j
                            : ranges[i].array_of_unicode_codepoints[j];
                        int glyph = FindGlyphIndex(info, codepoint);
                        int pad = spc.padding;
                        r->x += pad;
                        r->y += pad;
                        r->w -= pad;
                        r->h -= pad;
                        GetGlyphHMetrics(info, glyph, &advance, &lsb);
                        GetGlyphBitmapBox(info, glyph, scale * spc.h_oversample,
                            scale * spc.v_oversample, &x0, &y0, &x1, &y1);
                        MakeGlyphBitmapSubpixel(info, spc.pixels + r->x + r->y * spc.stride_in_bytes,
                            (int)(r->w - spc.h_oversample + 1), (int)(r->h - spc.v_oversample + 1),
                            spc.stride_in_bytes, scale * spc.h_oversample,
                            scale * spc.v_oversample, 0, 0, glyph);
                        if ((spc.h_oversample) > 1)
                        {
                            h_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, r->w,
                                r->h, spc.stride_in_bytes, spc.h_oversample);
                        }

                        if ((spc.v_oversample) > 1)
                        {
                            v_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, r->w,
                                r->h, spc.stride_in_bytes, spc.v_oversample);
                        }

                        bc->x0 = (ushort)((short)(r->x));
                        bc->y0 = (ushort)((short)(r->y));
                        bc->x1 = (ushort)((short)(r->x + r->w));
                        bc->y1 = (ushort)((short)(r->y + r->h));
                        bc->xadvance = scale * advance;
                        bc->xoff = x0 * recip_h + sub_x;
                        bc->yoff = y0 * recip_v + sub_y;
                        bc->xoff2 = (x0 + r->w) * recip_h + sub_x;
                        bc->yoff2 = (y0 + r->h) * recip_v + sub_y;
                    }
                    else
                    {
                        return_value = 0;
                    }

                    ++k;
                }
            }

            spc.h_oversample = (uint)old_h_over;
            spc.v_oversample = (uint)old_v_over;
            return return_value;
        }

        public static void PackFontRangesPackRects(pack_context spc, stbrp_rect* rects, int num_rects)
        {
            stbrp_pack_rects((stbrp_context*)(spc.pack_info), rects, num_rects);
        }

        public static int PackFontRanges(pack_context spc, byte* fontdata, int font_index,
            pack_range* ranges, int num_ranges)
        {
            fontinfo info = new fontinfo();
            int i = 0;
            int j = 0;
            int n = 0;
            int return_value = 1;
            stbrp_rect* rects;
            for (i = 0; i < num_ranges; ++i)
            {
                for (j = 0; j < (ranges[i].num_chars); ++j)
                {
                    ranges[i].chardata_for_range[j].x0 = ranges[i].chardata_for_range[j].y0 =
                        ranges[i].chardata_for_range[j].x1 =
                            ranges[i].chardata_for_range[j].y1 = 0;
                }
            }

            n = 0;
            for (i = 0; i < num_ranges; ++i)
            {
                n += ranges[i].num_chars;
            }

            rects = (stbrp_rect*)(CRuntime.malloc((ulong)(sizeof(stbrp_rect) * n)));
            if (rects == null)
            {
                return 0;
            }

            InitFont(info, fontdata, GetFontOffsetForIndex(fontdata, font_index));
            n = PackFontRangesGatherRects(spc, info, ranges, num_ranges, rects);
            PackFontRangesPackRects(spc, rects, n);
            return_value = PackFontRangesRenderIntoRects(spc, info, ranges, num_ranges, rects);
            CRuntime.free(rects);
            return return_value;
        }

        public static int PackFontRange(pack_context spc, byte* fontdata, int font_index, float font_size,
            int first_unicode_codepoint_in_range, int num_chars_in_range, packedchar* chardata_for_range)
        {
            pack_range range = new pack_range
            {
                first_unicode_codepoint_in_range = first_unicode_codepoint_in_range,
                array_of_unicode_codepoints = null,
                num_chars = num_chars_in_range,
                chardata_for_range = chardata_for_range,
                font_size = font_size
            };
            return PackFontRanges(spc, fontdata, font_index, &range, 1);
        }

        public static void GetScaledFontVMetrics(byte* fontdata, int index, float size, float* ascent,
            float* descent, float* lineGap)
        {
            int i_ascent = 0;
            int i_descent = 0;
            int i_lineGap = 0;
            float scale = 0;
            fontinfo info = new fontinfo();
            InitFont(info, fontdata, GetFontOffsetForIndex(fontdata, index));
            scale = size > 0
                ? ScaleForPixelHeight(info, size)
                : ScaleForMappingEmToPixels(info, -size);
            GetFontVMetrics(info, &i_ascent, &i_descent, &i_lineGap);
            *ascent = i_ascent * scale;
            *descent = i_descent * scale;
            *lineGap = i_lineGap * scale;
        }

        public static void GetPackedQuad(packedchar* chardata, int pw, int ph, int char_index, float* xpos,
            float* ypos, aligned_quad* q, int align_to_integer)
        {
            float ipw = 1.0f / pw;
            float iph = 1.0f / ph;
            packedchar* b = chardata + char_index;
            if (align_to_integer != 0)
            {
                float x = (int)(CRuntime.floor((*xpos + b->xoff) + 0.5f));
                float y = (int)(CRuntime.floor((*ypos + b->yoff) + 0.5f));
                q->x0 = x;
                q->y0 = y;
                q->x1 = x + b->xoff2 - b->xoff;
                q->y1 = y + b->yoff2 - b->yoff;
            }
            else
            {
                q->x0 = *xpos + b->xoff;
                q->y0 = *ypos + b->yoff;
                q->x1 = *xpos + b->xoff2;
                q->y1 = *ypos + b->yoff2;
            }

            q->s0 = b->x0 * ipw;
            q->t0 = b->y0 * iph;
            q->s1 = b->x1 * ipw;
            q->t1 = b->y1 * iph;
            *xpos += b->xadvance;
        }

        public static int ray_intersect_bezier(float* orig, float* ray, float* q0, float* q1, float* q2,
            float* hits)
        {
            float q0perp = q0[1] * ray[0] - q0[0] * ray[1];
            float q1perp = q1[1] * ray[0] - q1[0] * ray[1];
            float q2perp = q2[1] * ray[0] - q2[0] * ray[1];
            float roperp = orig[1] * ray[0] - orig[0] * ray[1];
            float a = q0perp - 2 * q1perp + q2perp;
            float b = q1perp - q0perp;
            float c = q0perp - roperp;
            float s0 = 0;
            float s1 = 0;
            int num_s = 0;
            if (a != 0.0)
            {
                float discr = b * b - a * c;
                if (discr > (0.0))
                {
                    float rcpna = -1 / a;
                    float d = (float)(CRuntime.sqrt(discr));
                    s0 = (b + d) * rcpna;
                    s1 = (b - d) * rcpna;
                    if ((s0 >= (0.0)) && (s0 <= 1.0))
                    {
                        num_s = 1;
                    }

                    if (((d > (0.0)) && (s1 >= (0.0))) && (s1 <= 1.0))
                    {
                        if (num_s == 0)
                        {
                            s0 = s1;
                        }

                        ++num_s;
                    }
                }
            }
            else
            {
                s0 = c / (-2 * b);
                if ((s0 >= (0.0)) && (s0 <= 1.0))
                {
                    num_s = 1;
                }
            }

            if (num_s == 0)
            {
                return 0;
            }
            else
            {
                float rcp_len2 = 1 / (ray[0] * ray[0] + ray[1] * ray[1]);
                float rayn_x = ray[0] * rcp_len2;
                float rayn_y = ray[1] * rcp_len2;
                float q0d = q0[0] * rayn_x + q0[1] * rayn_y;
                float q1d = q1[0] * rayn_x + q1[1] * rayn_y;
                float q2d = q2[0] * rayn_x + q2[1] * rayn_y;
                float rod = orig[0] * rayn_x + orig[1] * rayn_y;
                float q10d = q1d - q0d;
                float q20d = q2d - q0d;
                float q0rd = q0d - rod;
                hits[0] = q0rd + s0 * (2.0f - 2.0f * s0) * q10d + s0 * s0 * q20d;
                hits[1] = a * s0 + b;
                if (num_s > 1)
                {
                    hits[2] = q0rd + s1 * (2.0f - 2.0f * s1) * q10d + s1 * s1 * q20d;
                    hits[3] = a * s1 + b;
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

        public static int equal(float* a, float* b)
        {
            return ((a[0] == b[0]) && (a[1] == b[1])) ? 1 : 0;
        }

        public static int compute_crossings_x(float x, float y, int nverts, vertex* verts)
        {
            int i = 0;
            float* orig = stackalloc float[2];
            float* ray = stackalloc float[2];
            ray[0] = 1;
            ray[1] = 0;

            float y_frac = 0;
            int winding = 0;
            orig[0] = x;
            orig[1] = y;
            y_frac = ((float)(CRuntime.fmod(y, 1.0f)));
            if (y_frac < (0.01f))
            {
                y += 0.01f;
            }
            else if (y_frac > (0.99f))
            {
                y -= 0.01f;
            }

            orig[1] = y;
            for (i = 0; i < nverts; ++i)
            {
                if ((verts[i].type) == vline)
                {
                    int x0 = verts[i - 1].x;
                    int y0 = verts[i - 1].y;
                    int x1 = verts[i].x;
                    int y1 = verts[i].y;
                    if (((y > (y0 < y1 ? y0 : y1)) && (y < (y0 < y1 ? y1 : y0))) &&
                        (x > (x0 < x1 ? x0 : x1)))
                    {
                        float x_inter = (y - y0) / (y1 - y0) * (x1 - x0) + x0;
                        if (x_inter < x)
                        {
                            winding += (y0 < y1) ? 1 : -1;
                        }
                    }
                }

                if ((verts[i].type) == vcurve)
                {
                    int x0 = verts[i - 1].x;
                    int y0 = verts[i - 1].y;
                    int x1 = verts[i].cx;
                    int y1 = verts[i].cy;
                    int x2 = verts[i].x;
                    int y2 = verts[i].y;
                    int ax = x0 < (x1 < x2 ? x1 : x2) ? x0 : (x1 < x2 ? x1 : x2);
                    int ay = y0 < (y1 < y2 ? y1 : y2) ? y0 : (y1 < y2 ? y1 : y2);
                    int by = y0 < (y1 < y2 ? y2 : y1) ? (y1 < y2 ? y2 : y1) : y0;
                    if (((y > ay) && (y < by)) && (x > ax))
                    {
                        float* q0 = stackalloc float[2];
                        float* q1 = stackalloc float[2];
                        float* q2 = stackalloc float[2];
                        float* hits = stackalloc float[4];
                        q0[0] = x0;
                        q0[1] = y0;
                        q1[0] = x1;
                        q1[1] = y1;
                        q2[0] = x2;
                        q2[1] = y2;
                        if (((equal(q0, q1)) != 0) || ((equal(q1, q2)) != 0))
                        {
                            x0 = verts[i - 1].x;
                            y0 = verts[i - 1].y;
                            x1 = verts[i].x;
                            y1 = verts[i].y;
                            if (((y > (y0 < y1 ? y0 : y1)) && (y < (y0 < y1 ? y1 : y0))) &&
                                (x > (x0 < x1 ? x0 : x1)))
                            {
                                float x_inter = (y - y0) / (y1 - y0) * (x1 - x0) + x0;
                                if (x_inter < x)
                                {
                                    winding += (y0 < y1) ? 1 : -1;
                                }
                            }
                        }
                        else
                        {
                            int num_hits = ray_intersect_bezier(orig, ray, q0, q1, q2, hits);
                            if (num_hits >= 1)
                            {
                                if ((hits[0]) < 0)
                                {
                                    winding += (hits[1]) < 0 ? -1 : 1;
                                }
                            }

                            if (num_hits >= 2)
                            {
                                if ((hits[2]) < 0)
                                {
                                    winding += (hits[3]) < 0 ? -1 : 1;
                                }
                            }
                        }
                    }
                }
            }

            return winding;
        }

        public static float cuberoot(float x)
        {
            if (x < 0)
            {
                return -(float)(CRuntime.pow(-x, 1.0f / 3.0f));
            }
            else
            {
                return (float)(CRuntime.pow(x, 1.0f / 3.0f));
            }
        }

        public static int solve_cubic(float a, float b, float c, float* r)
        {
            float s = -a / 3;
            float p = b - a * a / 3;
            float q = a * (2 * a * a - 9 * b) / 27 + c;
            float p3 = p * p * p;
            float d = q * q + 4 * p3 / 27;
            if (d >= 0)
            {
                float z = (float)(CRuntime.sqrt(d));
                float u = (-q + z) / 2;
                float v = (-q - z) / 2;
                u = cuberoot(u);
                v = cuberoot(v);
                r[0] = s + u + v;
                return 1;
            }
            else
            {
                float u = (float)(CRuntime.sqrt(-p / 3));
                float v = (float)(CRuntime.acos(-CRuntime.sqrt(-27 / p3) * q / 2)) / 3;
                float m = (float)(CRuntime.cos(v));
                float n = (float)(CRuntime.cos(v - 3.141592 / 2)) * 1.732050808f;
                r[0] = s + u * 2 * m;
                r[1] = s - u * (m + n);
                r[2] = s - u * (m - n);
                return 3;
            }
        }

        public static byte* GetGlyphSDF(fontinfo info, float scale, int glyph, int padding,
            byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
        {
            float scale_x = scale;
            float scale_y = scale;
            int ix0 = 0;
            int iy0 = 0;
            int ix1 = 0;
            int iy1 = 0;
            int w = 0;
            int h = 0;
            byte* data;
            if (scale_x == 0)
            {
                scale_x = scale_y;
            }

            if (scale_y == 0)
            {
                if (scale_x == 0)
                {
                    return null;
                }

                scale_y = scale_x;
            }

            GetGlyphBitmapBoxSubpixel(info, glyph, scale, scale, 0.0f,
                0.0f, &ix0, &iy0, &ix1, &iy1);
            if ((ix0 == ix1) || (iy0 == iy1))
            {
                return null;
            }

            ix0 -= padding;
            iy0 -= padding;
            ix1 += padding;
            iy1 += padding;
            w = ix1 - ix0;
            h = iy1 - iy0;
            if (width != null)
            {
                *width = w;
            }

            if (height != null)
            {
                *height = h;
            }

            if (xoff != null)
            {
                *xoff = ix0;
            }

            if (yoff != null)
            {
                *yoff = iy0;
            }

            scale_y = -scale_y;
            {
                int x = 0;
                int y = 0;
                int i = 0;
                int j = 0;
                float* precompute;
                vertex* verts;
                int num_verts = GetGlyphShape(info, glyph, &verts);
                data = (byte*)(CRuntime.malloc((ulong)(w * h)));
                precompute = (float*)(CRuntime.malloc((ulong)(num_verts * sizeof(float))));
                for (i = 0, j = num_verts - 1; i < num_verts; j = i++)
                {
                    if ((verts[i].type) == vline)
                    {
                        float x0 = verts[i].x * scale_x;
                        float y0 = verts[i].y * scale_y;
                        float x1 = verts[j].x * scale_x;
                        float y1 = verts[j].y * scale_y;
                        float dist = (float)(CRuntime.sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
                        precompute[i] = (dist == 0) ? 0.0f : 1.0f / dist;
                    }
                    else if ((verts[i].type) == vcurve)
                    {
                        float x2 = verts[j].x * scale_x;
                        float y2 = verts[j].y * scale_y;
                        float x1 = verts[i].cx * scale_x;
                        float y1 = verts[i].cy * scale_y;
                        float x0 = verts[i].x * scale_x;
                        float y0 = verts[i].y * scale_y;
                        float bx = x0 - 2 * x1 + x2;
                        float by = y0 - 2 * y1 + y2;
                        float len2 = bx * bx + by * by;
                        if (len2 != 0.0f)
                        {
                            precompute[i] = 1.0f / (bx * bx + by * by);
                        }
                        else
                        {
                            precompute[i] = 0.0f;
                        }
                    }
                    else
                    {
                        precompute[i] = 0.0f;
                    }
                }

                for (y = iy0; y < iy1; ++y)
                {
                    for (x = ix0; x < ix1; ++x)
                    {
                        float val = 0;
                        float min_dist = 999999.0f;
                        float sx = x + 0.5f;
                        float sy = y + 0.5f;
                        float x_gspace = sx / scale_x;
                        float y_gspace = sy / scale_y;
                        int winding = compute_crossings_x(x_gspace, y_gspace,
                            num_verts, verts);
                        for (i = 0; i < num_verts; ++i)
                        {
                            float x0 = verts[i].x * scale_x;
                            float y0 = verts[i].y * scale_y;
                            float dist2 = (x0 - sx) * (x0 - sx) + (y0 - sy) * (y0 - sy);
                            if (dist2 < (min_dist * min_dist))
                            {
                                min_dist = ((float)(CRuntime.sqrt(dist2)));
                            }

                            if ((verts[i].type) == vline)
                            {
                                float x1 = verts[i - 1].x * scale_x;
                                float y1 = verts[i - 1].y * scale_y;
                                float dist =
                                    CRuntime.fabs(
                                                 (x1 - x0) * (y0 - sy) - (y1 - y0) * (x0 - sx)) *
                                             precompute[i];
                                if (dist < min_dist)
                                {
                                    float dx = x1 - x0;
                                    float dy = y1 - y0;
                                    float px = x0 - sx;
                                    float py = y0 - sy;
                                    float t = -(px * dx + py * dy) / (dx * dx + dy * dy);
                                    if ((t >= (0.0f)) && (t <= 1.0f))
                                    {
                                        min_dist = dist;
                                    }
                                }
                            }
                            else if ((verts[i].type) == vcurve)
                            {
                                float x2 = verts[i - 1].x * scale_x;
                                float y2 = verts[i - 1].y * scale_y;
                                float x1 = verts[i].cx * scale_x;
                                float y1 = verts[i].cy * scale_y;
                                float box_x0 = (x0 < x1 ? x0 : x1) < x2
                                    ? (x0 < x1 ? x0 : x1)
                                    : x2;
                                float box_y0 = (y0 < y1 ? y0 : y1) < y2
                                    ? (y0 < y1 ? y0 : y1)
                                    : y2;
                                float box_x1 = (x0 < x1 ? x1 : x0) < x2
                                    ? x2
                                    : (x0 < x1 ? x1 : x0);
                                float box_y1 = (y0 < y1 ? y1 : y0) < y2
                                    ? y2
                                    : (y0 < y1 ? y1 : y0);
                                if ((((sx > (box_x0 - min_dist)) && (sx < (box_x1 + min_dist))) &&
                                     (sy > (box_y0 - min_dist))) && (sy < (box_y1 + min_dist)))
                                {
                                    int num = 0;
                                    float ax = x1 - x0;
                                    float ay = y1 - y0;
                                    float bx = x0 - 2 * x1 + x2;
                                    float by = y0 - 2 * y1 + y2;
                                    float mx = x0 - sx;
                                    float my = y0 - sy;
                                    float* res = stackalloc float[3];
                                    float px = 0;
                                    float py = 0;
                                    float t = 0;
                                    float it = 0;
                                    float a_inv = precompute[i];
                                    if (a_inv == (0.0))
                                    {
                                        float a = 3 * (ax * bx + ay * by);
                                        float b = 2 * (ax * ax + ay * ay) + (mx * bx + my * by);
                                        float c = mx * ax + my * ay;
                                        if (a == (0.0))
                                        {
                                            if (b != 0.0)
                                            {
                                                res[num++] = -c / b;
                                            }
                                        }
                                        else
                                        {
                                            float discriminant = b * b - 4 * a * c;
                                            if (discriminant < 0)
                                            {
                                                num = 0;
                                            }
                                            else
                                            {
                                                float root = (float)(CRuntime.sqrt(discriminant));
                                                res[0] = (-b - root) / (2 * a);
                                                res[1] = (-b + root) / (2 * a);
                                                num = 2;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        float b = 3 * (ax * bx + ay * by) * a_inv;
                                        float c = (2 * (ax * ax + ay * ay) + (mx * bx + my * by)) * a_inv;
                                        float d = (mx * ax + my * ay) * a_inv;
                                        num = solve_cubic(b, c, d, res);
                                    }

                                    if (((num >= 1) && ((res[0]) >= (0.0f))) && (res[0] <= 1.0f))
                                    {
                                        t = res[0];
                                        it = 1.0f - t;
                                        px = it * it * x0 + 2 * t * it * x1 + t * t * x2;
                                        py = it * it * y0 + 2 * t * it * y1 + t * t * y2;
                                        dist2 = (px - sx) * (px - sx) + (py - sy) * (py - sy);
                                        if (dist2 < (min_dist * min_dist))
                                        {
                                            min_dist = ((float)(CRuntime.sqrt(dist2)));
                                        }
                                    }

                                    if (((num >= 2) && ((res[1]) >= (0.0f))) && (res[1] <= 1.0f))
                                    {
                                        t = res[1];
                                        it = 1.0f - t;
                                        px = it * it * x0 + 2 * t * it * x1 + t * t * x2;
                                        py = it * it * y0 + 2 * t * it * y1 + t * t * y2;
                                        dist2 = (px - sx) * (px - sx) + (py - sy) * (py - sy);
                                        if (dist2 < (min_dist * min_dist))
                                        {
                                            min_dist = ((float)(CRuntime.sqrt(dist2)));
                                        }
                                    }

                                    if (((num >= 3) && ((res[2]) >= (0.0f))) && (res[2] <= 1.0f))
                                    {
                                        t = res[2];
                                        it = 1.0f - t;
                                        px = it * it * x0 + 2 * t * it * x1 + t * t * x2;
                                        py = it * it * y0 + 2 * t * it * y1 + t * t * y2;
                                        dist2 = (px - sx) * (px - sx) + (py - sy) * (py - sy);
                                        if (dist2 < (min_dist * min_dist))
                                        {
                                            min_dist = ((float)(CRuntime.sqrt(dist2)));
                                        }
                                    }
                                }
                            }
                        }

                        if (winding == 0)
                        {
                            min_dist = -min_dist;
                        }

                        val = onedge_value + pixel_dist_scale * min_dist;
                        if (val < 0)
                        {
                            val = 0;
                        }
                        else if (val > 255)
                        {
                            val = 255;
                        }

                        data[(y - iy0) * w + (x - ix0)] = ((byte)val);
                    }
                }

                CRuntime.free(precompute);
                CRuntime.free(verts);
            }

            return data;
        }

        public static byte* GetCodepointSDF(fontinfo info, float scale, int codepoint, int padding,
            byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
        {
            return GetGlyphSDF(info, scale, FindGlyphIndex(info, codepoint),
                padding, onedge_value, pixel_dist_scale, width, height, xoff, yoff);
        }

        public static void FreeSDF(byte* bitmap)
        {
            CRuntime.free(bitmap);
        }

        public static int CompareUTF8toUTF16_bigendian_prefix(byte* s1, int len1, byte* s2, int len2)
        {
            int i = 0;
            while (len2 != 0)
            {
                ushort ch = (ushort)(s2[0] * 256 + s2[1]);
                if (ch < 0x80)
                {
                    if (i >= len1)
                    {
                        return -1;
                    }

                    if (s1[i++] != ch)
                    {
                        return -1;
                    }
                }
                else if (ch < 0x800)
                {
                    if ((i + 1) >= len1)
                    {
                        return -1;
                    }

                    if (s1[i++] != 0xc0 + (ch >> 6))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + (ch & 0x3f))
                    {
                        return -1;
                    }
                }
                else if ((ch >= 0xd800) && (ch < 0xdc00))
                {
                    uint c = 0;
                    ushort ch2 = (ushort)(s2[2] * 256 + s2[3]);
                    if ((i + 3) >= len1)
                    {
                        return -1;
                    }

                    c = (uint)(((ch - 0xd800) << 10) + (ch2 - 0xdc00) + 0x10000);
                    if (s1[i++] != 0xf0 + (c >> 18))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + ((c >> 12) & 0x3f))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + ((c >> 6) & 0x3f))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + (c & 0x3f))
                    {
                        return -1;
                    }

                    s2 += 2;
                    len2 -= 2;
                }
                else if ((ch >= 0xdc00) && (ch < 0xe000))
                {
                    return -1;
                }
                else
                {
                    if ((i + 2) >= len1)
                    {
                        return -1;
                    }

                    if (s1[i++] != 0xe0 + (ch >> 12))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + ((ch >> 6) & 0x3f))
                    {
                        return -1;
                    }

                    if (s1[i++] != 0x80 + (ch & 0x3f))
                    {
                        return -1;
                    }
                }

                s2 += 2;
                len2 -= 2;
            }

            return i;
        }

        public static int CompareUTF8toUTF16_bigendian_internal(sbyte* s1, int len1, sbyte* s2, int len2)
        {
            return len1 == (CompareUTF8toUTF16_bigendian_prefix((byte*)s1, len1,
                              (byte*)s2, len2))
                ? 1
                : 0;
        }

        public static sbyte* GetFontNameString(fontinfo font, int* length, int platformID, int encodingID,
            int languageID, int nameID)
        {
            int i = 0;
            int count = 0;
            int stringOffset = 0;
            byte* fc = font.data;
            uint offset = (uint)(font.fontstart);
            uint nm = find_table(fc, offset, "name");
            if (nm == 0)
            {
                return null;
            }

            count = ttUSHORT(fc + nm + 2);
            stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
            for (i = 0; i < count; ++i)
            {
                uint loc = (uint)(nm + 6 + 12 * i);
                if ((((platformID == (ttUSHORT(fc + loc + 0))) && (encodingID == (ttUSHORT(fc + loc + 2)))) &&
                     (languageID == (ttUSHORT(fc + loc + 4)))) && (nameID == (ttUSHORT(fc + loc + 6))))
                {
                    *length = ttUSHORT(fc + loc + 8);
                    return (sbyte*)(fc + stringOffset + ttUSHORT(fc + loc + 10));
                }
            }

            return null;
        }

        public static int matchpair(byte* fc, uint nm, byte* name, int nlen, int target_id, int next_id)
        {
            int i = 0;
            int count = ttUSHORT(fc + nm + 2);
            int stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
            for (i = 0; i < count; ++i)
            {
                uint loc = (uint)(nm + 6 + 12 * i);
                int id = ttUSHORT(fc + loc + 6);
                if (id == target_id)
                {
                    int platform = ttUSHORT(fc + loc + 0);
                    int encoding = ttUSHORT(fc + loc + 2);
                    int language = ttUSHORT(fc + loc + 4);
                    if (((platform == 0) || ((platform == 3) && (encoding == 1))) ||
                        ((platform == 3) && (encoding == 10)))
                    {
                        int slen = ttUSHORT(fc + loc + 8);
                        int off = ttUSHORT(fc + loc + 10);
                        int matchlen = CompareUTF8toUTF16_bigendian_prefix(name, nlen,
                            fc + stringOffset + off, slen);
                        if (matchlen >= 0)
                        {
                            if ((((((i + 1) < count) && ((ttUSHORT(fc + loc + 12 + 6)) == next_id)) &&
                                  ((ttUSHORT(fc + loc + 12)) == platform)) &&
                                 ((ttUSHORT(fc + loc + 12 + 2)) == encoding)) &&
                                ((ttUSHORT(fc + loc + 12 + 4)) == language))
                            {
                                slen = ttUSHORT(fc + loc + 12 + 8);
                                off = ttUSHORT(fc + loc + 12 + 10);
                                if (slen == 0)
                                {
                                    if (matchlen == nlen)
                                    {
                                        return 1;
                                    }
                                }
                                else if ((matchlen < nlen) && ((name[matchlen]) == (' ')))
                                {
                                    ++matchlen;
                                    if ((CompareUTF8toUTF16_bigendian_internal((sbyte*)(name + matchlen),
                                            nlen - matchlen, (sbyte*)(fc + stringOffset + off),
                                            slen)) != 0)
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                if (matchlen == nlen)
                                {
                                    return 1;
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public static int matches(byte* fc, uint offset, byte* name, int flags)
        {
            int nlen = (int)(CRuntime.strlen((sbyte*)name));
            uint nm = 0;
            uint hd = 0;
            if (isfont(fc + offset) == 0)
            {
                return 0;
            }

            if (flags != 0)
            {
                hd = find_table(fc, offset, "head");
                if ((ttUSHORT(fc + hd + 44) & 7) != (flags & 7))
                {
                    return 0;
                }
            }

            nm = find_table(fc, offset, "name");
            if (nm == 0)
            {
                return 0;
            }

            if (flags != 0)
            {
                if ((matchpair(fc, nm, name, nlen, 16, -1)) != 0)
                {
                    return 1;
                }

                if ((matchpair(fc, nm, name, nlen, 1, -1)) != 0)
                {
                    return 1;
                }

                if ((matchpair(fc, nm, name, nlen, 3, -1)) != 0)
                {
                    return 1;
                }
            }
            else
            {
                if ((matchpair(fc, nm, name, nlen, 16, 17)) != 0)
                {
                    return 1;
                }

                if ((matchpair(fc, nm, name, nlen, 1, 2)) != 0)
                {
                    return 1;
                }

                if ((matchpair(fc, nm, name, nlen, 3, -1)) != 0)
                {
                    return 1;
                }
            }

            return 0;
        }

        public static int FindMatchingFont_internal(byte* font_collection, sbyte* name_utf8, int flags)
        {
            int i = 0;
            for (i = 0; ; ++i)
            {
                int off = GetFontOffsetForIndex(font_collection, i);
                if (off < 0)
                {
                    return off;
                }

                if ((matches(font_collection, (uint)off, (byte*)name_utf8, flags)) != 0)
                {
                    return off;
                }
            }
        }

        public static int BakeFontBitmap(byte* data, int offset, float pixel_height, byte* pixels, int pw, int ph,
            int first_char, int num_chars, bakedchar* chardata)
        {
            return BakeFontBitmap_internal(data, offset, pixel_height, pixels,
                pw, ph, first_char, num_chars, chardata);
        }

        public static int GetFontOffsetForIndex(byte* data, int index)
        {
            return GetFontOffsetForIndex_internal(data, index);
        }

        public static int GetNumberOfFonts(byte* data)
        {
            return GetNumberOfFonts_internal(data);
        }

        public static int InitFont(fontinfo info, byte* data, int offset)
        {
            return InitFont_internal(info, data, offset);
        }

        public static int FindMatchingFont(byte* fontdata, sbyte* name, int flags)
        {
            return FindMatchingFont_internal(fontdata, name, flags);
        }

        public static int CompareUTF8toUTF16_bigendian(sbyte* s1, int len1, sbyte* s2, int len2)
        {
            return CompareUTF8toUTF16_bigendian_internal(s1, len1, s2, len2);
        }

        public static uint find_table(byte* data, uint fontstart, string tag)
        {
            int num_tables = ttUSHORT(data + fontstart + 4);
            uint tabledir = fontstart + 12;
            int i;
            for (i = 0; i < num_tables; ++i)
            {
                uint loc = (uint)(tabledir + 16 * i);
                if ((data + loc + 0)[0] == tag[0] && (data + loc + 0)[1] == tag[1] &&
                    (data + loc + 0)[2] == tag[2] && (data + loc + 0)[3] == tag[3])
                {
                    return ttULONG(data + loc + 8);
                }
            }

            return 0;
        }

        public static bool BakeFontBitmap(byte[] ttf, int offset, float pixel_height, byte[] pixels, int pw,
            int ph,
            int first_char, int num_chars, bakedchar[] chardata)
        {
            fixed (byte* ttfPtr = ttf)
            {
                fixed (byte* pixelsPtr = pixels)
                {
                    fixed (bakedchar* chardataPtr = chardata)
                    {
                        int result = BakeFontBitmap(ttfPtr, offset, pixel_height, pixelsPtr, pw, ph, first_char,
                            num_chars,
                            chardataPtr);

                        return result != 0;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct buf
        {
            public byte* data;
            public int cursor;
            public int size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct bakedchar
        {
            public ushort x0;
            public ushort y0;
            public ushort x1;
            public ushort y1;
            public float xoff;
            public float yoff;
            public float xadvance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct aligned_quad
        {
            public float x0;
            public float y0;
            public float s0;
            public float t0;
            public float x1;
            public float y1;
            public float s1;
            public float t1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct packedchar
        {
            public ushort x0;
            public ushort y0;
            public ushort x1;
            public ushort y1;
            public float xoff;
            public float yoff;
            public float xadvance;
            public float xoff2;
            public float yoff2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct pack_range
        {
            public float font_size;
            public int first_unicode_codepoint_in_range;
            public int* array_of_unicode_codepoints;
            public int num_chars;
            public packedchar* chardata_for_range;
            public byte h_oversample;
            public byte v_oversample;
        }

        public class pack_context
        {
            public uint h_oversample;
            public int height;
            public void* nodes;
            public void* pack_info;
            public int padding;
            public byte* pixels;
            public int skip_missing;
            public int stride_in_bytes;
            public void* user_allocator_context;
            public uint v_oversample;
            public int width;
        }

        public class fontinfo
        {
            public buf cff = new buf();
            public buf charstrings = new buf();
            public byte* data;
            public buf fdselect = new buf();
            public buf fontdicts = new buf();
            public int fontstart;
            public int glyf;
            public int gpos;
            public buf gsubrs = new buf();
            public int head;
            public int hhea;
            public int hmtx;
            public int index_map;
            public int indexToLocFormat;
            public int kern;
            public int loca;
            public int numGlyphs;
            public buf subrs = new buf();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct vertex
        {
            public short x;
            public short y;
            public short cx;
            public short cy;
            public short cx1;
            public short cy1;
            public byte type;
            public byte padding;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct bitmap
        {
            public int w;
            public int h;
            public int stride;
            public byte* pixels;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct csctx
        {
            public int bounds;
            public int started;
            public float first_x;
            public float first_y;
            public float x;
            public float y;
            public int min_x;
            public int max_x;
            public int min_y;
            public int max_y;
            public vertex* pvertices;
            public int num_vertices;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct hheap_chunk
        {
            public hheap_chunk* next;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct hheap
        {
            public hheap_chunk* head;
            public void* first_free;
            public int num_remaining_in_head_chunk;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct edge
        {
            public float x0;
            public float y0;
            public float x1;
            public float y1;
            public int invert;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct active_edge
        {
            public active_edge* next;
            public float fx;
            public float fdx;
            public float fdy;
            public float direction;
            public float sy;
            public float ey;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct point
        {
            public float x;
            public float y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbrp_context
        {
            public int width;
            public int height;
            public int x;
            public int y;
            public int bottom_y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbrp_node
        {
            public byte x;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbrp_rect
        {
            public int x;
            public int y;
            public int id;
            public int w;
            public int h;
            public int was_packed;
        }
    }
}