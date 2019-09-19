using System.Runtime.InteropServices;

namespace StbTrueTypeSharp
{
    internal unsafe partial class StbTrueType
    {
        public const int STBTT_vmove = 1;
        public const int STBTT_vline = 2;
        public const int STBTT_vcurve = 3;
        public const int STBTT_vcubic = 4;

        public const int STBTT_PLATFORM_ID_UNICODE = 0;
        public const int STBTT_PLATFORM_ID_MAC = 1;
        public const int STBTT_PLATFORM_ID_ISO = 2;
        public const int STBTT_PLATFORM_ID_MICROSOFT = 3;

        public const int STBTT_UNICODE_EID_UNICODE_1_0 = 0;
        public const int STBTT_UNICODE_EID_UNICODE_1_1 = 1;
        public const int STBTT_UNICODE_EID_ISO_10646 = 2;
        public const int STBTT_UNICODE_EID_UNICODE_2_0_BMP = 3;
        public const int STBTT_UNICODE_EID_UNICODE_2_0_FULL = 4;

        public const int STBTT_MS_EID_SYMBOL = 0;
        public const int STBTT_MS_EID_UNICODE_BMP = 1;
        public const int STBTT_MS_EID_SHIFTJIS = 2;
        public const int STBTT_MS_EID_UNICODE_FULL = 10;

        public const int STBTT_MAC_EID_ROMAN = 0;
        public const int STBTT_MAC_EID_ARABIC = 4;
        public const int STBTT_MAC_EID_JAPANESE = 1;
        public const int STBTT_MAC_EID_HEBREW = 5;
        public const int STBTT_MAC_EID_CHINESE_TRAD = 2;
        public const int STBTT_MAC_EID_GREEK = 6;
        public const int STBTT_MAC_EID_KOREAN = 3;
        public const int STBTT_MAC_EID_RUSSIAN = 7;

        public const int STBTT_MS_LANG_ENGLISH = 0x0409;
        public const int STBTT_MS_LANG_ITALIAN = 0x0410;
        public const int STBTT_MS_LANG_CHINESE = 0x0804;
        public const int STBTT_MS_LANG_JAPANESE = 0x0411;
        public const int STBTT_MS_LANG_DUTCH = 0x0413;
        public const int STBTT_MS_LANG_KOREAN = 0x0412;
        public const int STBTT_MS_LANG_FRENCH = 0x040c;
        public const int STBTT_MS_LANG_RUSSIAN = 0x0419;
        public const int STBTT_MS_LANG_GERMAN = 0x0407;
        public const int STBTT_MS_LANG_SPANISH = 0x0409;
        public const int STBTT_MS_LANG_HEBREW = 0x040d;
        public const int STBTT_MS_LANG_SWEDISH = 0x041D;

        public const int STBTT_MAC_LANG_ENGLISH = 0;
        public const int STBTT_MAC_LANG_JAPANESE = 11;
        public const int STBTT_MAC_LANG_ARABIC = 12;
        public const int STBTT_MAC_LANG_KOREAN = 23;
        public const int STBTT_MAC_LANG_DUTCH = 4;
        public const int STBTT_MAC_LANG_RUSSIAN = 32;
        public const int STBTT_MAC_LANG_FRENCH = 1;
        public const int STBTT_MAC_LANG_SPANISH = 6;
        public const int STBTT_MAC_LANG_GERMAN = 2;
        public const int STBTT_MAC_LANG_SWEDISH = 5;
        public const int STBTT_MAC_LANG_HEBREW = 10;
        public const int STBTT_MAC_LANG_CHINESE_SIMPLIFIED = 33;
        public const int STBTT_MAC_LANG_ITALIAN = 3;
        public const int STBTT_MAC_LANG_CHINESE_TRAD = 19;

        public static byte stbtt__buf_get8(stbtt__buf* b)
        {
            if ((b->cursor) >= (b->size))
                return (byte)(0);
            return (byte)(b->data[b->cursor++]);
        }

        public static byte stbtt__buf_peek8(stbtt__buf* b)
        {
            if ((b->cursor) >= (b->size))
                return (byte)(0);
            return (byte)(b->data[b->cursor]);
        }

        public static void stbtt__buf_seek(stbtt__buf* b, int o)
        {
            b->cursor = (int)((((o) > (b->size)) || ((o) < (0))) ? b->size : o);
        }

        public static void stbtt__buf_skip(stbtt__buf* b, int o)
        {
            stbtt__buf_seek(b, (int)(b->cursor + o));
        }

        public static uint stbtt__buf_get(stbtt__buf* b, int n)
        {
            uint v = (uint)(0);
            int i = 0;
            for (i = (int)(0); (i) < (n); i++)
            {
                v = (uint)((v << 8) | stbtt__buf_get8(b));
            }

            return (uint)(v);
        }

        public static stbtt__buf stbtt__new_buf(void* p, ulong size)
        {
            stbtt__buf r = new stbtt__buf();
            r.data = (byte*)(p);
            r.size = ((int)(size));
            r.cursor = (int)(0);
            return (stbtt__buf)(r);
        }

        public static stbtt__buf stbtt__buf_range(stbtt__buf* b, int o, int s)
        {
            stbtt__buf r = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
            if (((((o) < (0)) || ((s) < (0))) || ((o) > (b->size))) || ((s) > (b->size - o)))
                return (stbtt__buf)(r);
            r.data = b->data + o;
            r.size = (int)(s);
            return (stbtt__buf)(r);
        }

        public static stbtt__buf stbtt__cff_get_index(stbtt__buf* b)
        {
            int count = 0;
            int start = 0;
            int offsize = 0;
            start = (int)(b->cursor);
            count = (int)(stbtt__buf_get((b), (int)(2)));
            if ((count) != 0)
            {
                offsize = (int)(stbtt__buf_get8(b));
                stbtt__buf_skip(b, (int)(offsize * count));
                stbtt__buf_skip(b, (int)(stbtt__buf_get(b, (int)(offsize)) - 1));
            }

            return (stbtt__buf)(stbtt__buf_range(b, (int)(start), (int)(b->cursor - start)));
        }

        public static uint stbtt__cff_int(stbtt__buf* b)
        {
            int b0 = (int)(stbtt__buf_get8(b));
            if (((b0) >= (32)) && (b0 <= 246))
                return (uint)(b0 - 139);
            else if (((b0) >= (247)) && (b0 <= 250))
                return (uint)((b0 - 247) * 256 + stbtt__buf_get8(b) + 108);
            else if (((b0) >= (251)) && (b0 <= 254))
                return (uint)(-(b0 - 251) * 256 - stbtt__buf_get8(b) - 108);
            else if ((b0) == (28))
                return (uint)(stbtt__buf_get((b), (int)(2)));
            else if ((b0) == (29))
                return (uint)(stbtt__buf_get((b), (int)(4)));
            return (uint)(0);
        }

        public static void stbtt__cff_skip_operand(stbtt__buf* b)
        {
            int v = 0;
            int b0 = (int)(stbtt__buf_peek8(b));
            if ((b0) == (30))
            {
                stbtt__buf_skip(b, (int)(1));
                while ((b->cursor) < (b->size))
                {
                    v = (int)(stbtt__buf_get8(b));
                    if (((v & 0xF) == (0xF)) || ((v >> 4) == (0xF)))
                        break;
                }
            }
            else
            {
                stbtt__cff_int(b);
            }
        }

        public static stbtt__buf stbtt__dict_get(stbtt__buf* b, int key)
        {
            stbtt__buf_seek(b, (int)(0));
            while ((b->cursor) < (b->size))
            {
                int start = (int)(b->cursor);
                int end = 0;
                int op = 0;
                while ((stbtt__buf_peek8(b)) >= (28))
                {
                    stbtt__cff_skip_operand(b);
                }

                end = (int)(b->cursor);
                op = (int)(stbtt__buf_get8(b));
                if ((op) == (12))
                    op = (int)(stbtt__buf_get8(b) | 0x100);
                if ((op) == (key))
                    return (stbtt__buf)(stbtt__buf_range(b, (int)(start), (int)(end - start)));
            }

            return (stbtt__buf)(stbtt__buf_range(b, (int)(0), (int)(0)));
        }

        public static void stbtt__dict_get_ints(stbtt__buf* b, int key, int outcount, uint* _out_)
        {
            int i = 0;
            stbtt__buf operands = (stbtt__buf)(stbtt__dict_get(b, (int)(key)));
            for (i = (int)(0); ((i) < (outcount)) && ((operands.cursor) < (operands.size)); i++)
            {
                _out_[i] = (uint)(stbtt__cff_int(&operands));
            }
        }

        public static int stbtt__cff_index_count(stbtt__buf* b)
        {
            stbtt__buf_seek(b, (int)(0));
            return (int)(stbtt__buf_get((b), (int)(2)));
        }

        public static stbtt__buf stbtt__cff_index_get(stbtt__buf b, int i)
        {
            int count = 0;
            int offsize = 0;
            int start = 0;
            int end = 0;
            stbtt__buf_seek(&b, (int)(0));
            count = (int)(stbtt__buf_get((&b), (int)(2)));
            offsize = (int)(stbtt__buf_get8(&b));
            stbtt__buf_skip(&b, (int)(i * offsize));
            start = (int)(stbtt__buf_get(&b, (int)(offsize)));
            end = (int)(stbtt__buf_get(&b, (int)(offsize)));
            return (stbtt__buf)(stbtt__buf_range(&b, (int)(2 + (count + 1) * offsize + start), (int)(end - start)));
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
            return (int)((p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3]);
        }

        public static int stbtt__isfont(byte* font)
        {
            if (((((((font)[0]) == ('1')) && (((font)[1]) == (0))) && (((font)[2]) == (0))) && (((font)[3]) == (0))))
                return (int)(1);
            if (((((((font)[0]) == ("typ1"[0])) && (((font)[1]) == ("typ1"[1]))) && (((font)[2]) == ("typ1"[2]))) &&
                 (((font)[3]) == ("typ1"[3]))))
                return (int)(1);
            if (((((((font)[0]) == ("OTTO"[0])) && (((font)[1]) == ("OTTO"[1]))) && (((font)[2]) == ("OTTO"[2]))) &&
                 (((font)[3]) == ("OTTO"[3]))))
                return (int)(1);
            if (((((((font)[0]) == (0)) && (((font)[1]) == (1))) && (((font)[2]) == (0))) && (((font)[3]) == (0))))
                return (int)(1);
            if (((((((font)[0]) == ("true"[0])) && (((font)[1]) == ("true"[1]))) && (((font)[2]) == ("true"[2]))) &&
                 (((font)[3]) == ("true"[3]))))
                return (int)(1);
            return (int)(0);
        }

        public static int stbtt_GetFontOffsetForIndex_internal(byte* font_collection, int index)
        {
            if ((stbtt__isfont(font_collection)) != 0)
                return (int)((index) == (0) ? 0 : -1);
            if (((((((font_collection)[0]) == ("ttcf"[0])) && (((font_collection)[1]) == ("ttcf"[1]))) &&
                  (((font_collection)[2]) == ("ttcf"[2]))) && (((font_collection)[3]) == ("ttcf"[3]))))
            {
                if (((ttULONG(font_collection + 4)) == (0x00010000)) ||
                    ((ttULONG(font_collection + 4)) == (0x00020000)))
                {
                    int n = (int)(ttLONG(font_collection + 8));
                    if ((index) >= (n))
                        return (int)(-1);
                    return (int)(ttULONG(font_collection + 12 + index * 4));
                }
            }

            return (int)(-1);
        }

        public static int stbtt_GetNumberOfFonts_internal(byte* font_collection)
        {
            if ((stbtt__isfont(font_collection)) != 0)
                return (int)(1);
            if (((((((font_collection)[0]) == ("ttcf"[0])) && (((font_collection)[1]) == ("ttcf"[1]))) &&
                  (((font_collection)[2]) == ("ttcf"[2]))) && (((font_collection)[3]) == ("ttcf"[3]))))
            {
                if (((ttULONG(font_collection + 4)) == (0x00010000)) ||
                    ((ttULONG(font_collection + 4)) == (0x00020000)))
                {
                    return (int)(ttLONG(font_collection + 8));
                }
            }

            return (int)(0);
        }

        public static stbtt__buf stbtt__get_subrs(stbtt__buf cff, stbtt__buf fontdict)
        {
            uint subrsoff = (uint)(0);
            uint* private_loc = stackalloc uint[2];
            private_loc[0] = (uint)(0);
            private_loc[1] = (uint)(0);

            stbtt__buf pdict = new stbtt__buf();
            stbtt__dict_get_ints(&fontdict, (int)(18), (int)(2), private_loc);
            if ((private_loc[1] == 0) || (private_loc[0] == 0))
                return (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
            pdict = (stbtt__buf)(stbtt__buf_range(&cff, (int)(private_loc[1]), (int)(private_loc[0])));
            stbtt__dict_get_ints(&pdict, (int)(19), (int)(1), &subrsoff);
            if (subrsoff == 0)
                return (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
            stbtt__buf_seek(&cff, (int)(private_loc[1] + subrsoff));
            return (stbtt__buf)(stbtt__cff_get_index(&cff));
        }

        public static int stbtt_InitFont_internal(stbtt_fontinfo info, byte* data, int fontstart)
        {
            uint cmap = 0;
            uint t = 0;
            int i = 0;
            int numTables = 0;
            info.data = data;
            info.fontstart = (int)(fontstart);
            info.cff = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
            cmap = (uint)(stbtt__find_table(data, (uint)(fontstart), "cmap"));
            info.loca = (int)(stbtt__find_table(data, (uint)(fontstart), "loca"));
            info.head = (int)(stbtt__find_table(data, (uint)(fontstart), "head"));
            info.glyf = (int)(stbtt__find_table(data, (uint)(fontstart), "glyf"));
            info.hhea = (int)(stbtt__find_table(data, (uint)(fontstart), "hhea"));
            info.hmtx = (int)(stbtt__find_table(data, (uint)(fontstart), "hmtx"));
            info.kern = (int)(stbtt__find_table(data, (uint)(fontstart), "kern"));
            info.gpos = (int)(stbtt__find_table(data, (uint)(fontstart), "GPOS"));
            if ((((cmap == 0) || (info.head == 0)) || (info.hhea == 0)) || (info.hmtx == 0))
                return (int)(0);
            if ((info.glyf) != 0)
            {
                if (info.loca == 0)
                    return (int)(0);
            }
            else
            {
                stbtt__buf b = new stbtt__buf();
                stbtt__buf topdict = new stbtt__buf();
                stbtt__buf topdictidx = new stbtt__buf();
                uint cstype = (uint)(2);
                uint charstrings = (uint)(0);
                uint fdarrayoff = (uint)(0);
                uint fdselectoff = (uint)(0);
                uint cff = 0;
                cff = (uint)(stbtt__find_table(data, (uint)(fontstart), "CFF "));
                if (cff == 0)
                    return (int)(0);
                info.fontdicts = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
                info.fdselect = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
                info.cff = (stbtt__buf)(stbtt__new_buf(data + cff, (ulong)(512 * 1024 * 1024)));
                b = (stbtt__buf)(info.cff);
                stbtt__buf_skip(&b, (int)(2));
                stbtt__buf_seek(&b, (int)(stbtt__buf_get8(&b)));
                stbtt__cff_get_index(&b);
                topdictidx = (stbtt__buf)(stbtt__cff_get_index(&b));
                topdict = (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(topdictidx), (int)(0)));
                stbtt__cff_get_index(&b);
                info.gsubrs = (stbtt__buf)(stbtt__cff_get_index(&b));
                stbtt__dict_get_ints(&topdict, (int)(17), (int)(1), &charstrings);
                stbtt__dict_get_ints(&topdict, (int)(0x100 | 6), (int)(1), &cstype);
                stbtt__dict_get_ints(&topdict, (int)(0x100 | 36), (int)(1), &fdarrayoff);
                stbtt__dict_get_ints(&topdict, (int)(0x100 | 37), (int)(1), &fdselectoff);
                info.subrs = (stbtt__buf)(stbtt__get_subrs((stbtt__buf)(b), (stbtt__buf)(topdict)));
                if (cstype != 2)
                    return (int)(0);
                if ((charstrings) == (0))
                    return (int)(0);
                if ((fdarrayoff) != 0)
                {
                    if (fdselectoff == 0)
                        return (int)(0);
                    stbtt__buf_seek(&b, (int)(fdarrayoff));
                    info.fontdicts = (stbtt__buf)(stbtt__cff_get_index(&b));
                    info.fdselect =
                        (stbtt__buf)(stbtt__buf_range(&b, (int)(fdselectoff), (int)(b.size - fdselectoff)));
                }

                stbtt__buf_seek(&b, (int)(charstrings));
                info.charstrings = (stbtt__buf)(stbtt__cff_get_index(&b));
            }

            t = (uint)(stbtt__find_table(data, (uint)(fontstart), "maxp"));
            if ((t) != 0)
                info.numGlyphs = (int)(ttUSHORT(data + t + 4));
            else
                info.numGlyphs = (int)(0xffff);
            numTables = (int)(ttUSHORT(data + cmap + 2));
            info.index_map = (int)(0);
            for (i = (int)(0); (i) < (numTables); ++i)
            {
                uint encoding_record = (uint)(cmap + 4 + 8 * i);
                switch (ttUSHORT(data + encoding_record))
                {
                    case STBTT_PLATFORM_ID_MICROSOFT:
                        switch (ttUSHORT(data + encoding_record + 2))
                        {
                            case STBTT_MS_EID_UNICODE_BMP:
                            case STBTT_MS_EID_UNICODE_FULL:
                                info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
                                break;
                        }

                        break;
                    case STBTT_PLATFORM_ID_UNICODE:
                        info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
                        break;
                }
            }

            if ((info.index_map) == (0))
                return (int)(0);
            info.indexToLocFormat = (int)(ttUSHORT(data + info.head + 50));
            return (int)(1);
        }

        public static int stbtt_FindGlyphIndex(stbtt_fontinfo info, int unicode_codepoint)
        {
            byte* data = info.data;
            uint index_map = (uint)(info.index_map);
            ushort format = (ushort)(ttUSHORT(data + index_map + 0));
            if ((format) == (0))
            {
                int bytes = (int)(ttUSHORT(data + index_map + 2));
                if ((unicode_codepoint) < (bytes - 6))
                    return (int)(*(data + index_map + 6 + unicode_codepoint));
                return (int)(0);
            }
            else if ((format) == (6))
            {
                uint first = (uint)(ttUSHORT(data + index_map + 6));
                uint count = (uint)(ttUSHORT(data + index_map + 8));
                if ((((uint)(unicode_codepoint)) >= (first)) && (((uint)(unicode_codepoint)) < (first + count)))
                    return (int)(ttUSHORT(data + index_map + 10 + (unicode_codepoint - first) * 2));
                return (int)(0);
            }
            else if ((format) == (2))
            {
                return (int)(0);
            }
            else if ((format) == (4))
            {
                ushort segcount = (ushort)(ttUSHORT(data + index_map + 6) >> 1);
                ushort searchRange = (ushort)(ttUSHORT(data + index_map + 8) >> 1);
                ushort entrySelector = (ushort)(ttUSHORT(data + index_map + 10));
                ushort rangeShift = (ushort)(ttUSHORT(data + index_map + 12) >> 1);
                uint endCount = (uint)(index_map + 14);
                uint search = (uint)(endCount);
                if ((unicode_codepoint) > (0xffff))
                    return (int)(0);
                if ((unicode_codepoint) >= (ttUSHORT(data + search + rangeShift * 2)))
                    search += (uint)(rangeShift * 2);
                search -= (uint)(2);
                while ((entrySelector) != 0)
                {
                    ushort end = 0;
                    searchRange >>= 1;
                    end = (ushort)(ttUSHORT(data + search + searchRange * 2));
                    if ((unicode_codepoint) > (end))
                        search += (uint)(searchRange * 2);
                    --entrySelector;
                }

                search += (uint)(2);
                {
                    ushort offset = 0;
                    ushort start = 0;
                    ushort item = (ushort)((search - endCount) >> 1);
                    start = (ushort)(ttUSHORT(data + index_map + 14 + segcount * 2 + 2 + 2 * item));
                    if ((unicode_codepoint) < (start))
                        return (int)(0);
                    offset = (ushort)(ttUSHORT(data + index_map + 14 + segcount * 6 + 2 + 2 * item));
                    if ((offset) == (0))
                        return (int)((ushort)(unicode_codepoint +
                                                ttSHORT(data + index_map + 14 + segcount * 4 + 2 + 2 * item)));
                    return (int)(ttUSHORT(data + offset + (unicode_codepoint - start) * 2 + index_map + 14 +
                                           segcount * 6 + 2 + 2 * item));
                }
            }
            else if (((format) == (12)) || ((format) == (13)))
            {
                uint ngroups = (uint)(ttULONG(data + index_map + 12));
                int low = 0;
                int high = 0;
                low = (int)(0);
                high = ((int)(ngroups));
                while ((low) < (high))
                {
                    int mid = (int)(low + ((high - low) >> 1));
                    uint start_char = (uint)(ttULONG(data + index_map + 16 + mid * 12));
                    uint end_char = (uint)(ttULONG(data + index_map + 16 + mid * 12 + 4));
                    if (((uint)(unicode_codepoint)) < (start_char))
                        high = (int)(mid);
                    else if (((uint)(unicode_codepoint)) > (end_char))
                        low = (int)(mid + 1);
                    else
                    {
                        uint start_glyph = (uint)(ttULONG(data + index_map + 16 + mid * 12 + 8));
                        if ((format) == (12))
                            return (int)(start_glyph + unicode_codepoint - start_char);
                        else
                            return (int)(start_glyph);
                    }
                }

                return (int)(0);
            }

            return (int)(0);
        }

        public static int stbtt_GetCodepointShape(stbtt_fontinfo info, int unicode_codepoint, stbtt_vertex** vertices)
        {
            return (int)(stbtt_GetGlyphShape(info, (int)(stbtt_FindGlyphIndex(info, (int)(unicode_codepoint))),
                vertices));
        }

        public static void stbtt_setvertex(stbtt_vertex* v, byte type, int x, int y, int cx, int cy)
        {
            v->type = (byte)(type);
            v->x = ((short)(x));
            v->y = ((short)(y));
            v->cx = ((short)(cx));
            v->cy = ((short)(cy));
        }

        public static int stbtt__GetGlyfOffset(stbtt_fontinfo info, int glyph_index)
        {
            int g1 = 0;
            int g2 = 0;
            if ((glyph_index) >= (info.numGlyphs))
                return (int)(-1);
            if ((info.indexToLocFormat) >= (2))
                return (int)(-1);
            if ((info.indexToLocFormat) == (0))
            {
                g1 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2) * 2);
                g2 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2 + 2) * 2);
            }
            else
            {
                g1 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4));
                g2 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4 + 4));
            }

            return (int)((g1) == (g2) ? -1 : g1);
        }

        public static int stbtt_GetGlyphBox(stbtt_fontinfo info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
        {
            if ((info.cff.size) != 0)
            {
                stbtt__GetGlyphInfoT2(info, (int)(glyph_index), x0, y0, x1, y1);
            }
            else
            {
                int g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
                if ((g) < (0))
                    return (int)(0);
                if ((x0) != null)
                    *x0 = (int)(ttSHORT(info.data + g + 2));
                if ((y0) != null)
                    *y0 = (int)(ttSHORT(info.data + g + 4));
                if ((x1) != null)
                    *x1 = (int)(ttSHORT(info.data + g + 6));
                if ((y1) != null)
                    *y1 = (int)(ttSHORT(info.data + g + 8));
            }

            return (int)(1);
        }

        public static int stbtt_GetCodepointBox(stbtt_fontinfo info, int codepoint, int* x0, int* y0, int* x1, int* y1)
        {
            return (int)(stbtt_GetGlyphBox(info, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), x0, y0, x1,
                y1));
        }

        public static int stbtt_IsGlyphEmpty(stbtt_fontinfo info, int glyph_index)
        {
            short numberOfContours = 0;
            int g = 0;
            if ((info.cff.size) != 0)
                return (int)((stbtt__GetGlyphInfoT2(info, (int)(glyph_index), (null), (null), (null), (null))) == (0)
                    ? 1
                    : 0);
            g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
            if ((g) < (0))
                return (int)(1);
            numberOfContours = (short)(ttSHORT(info.data + g));
            return (int)((numberOfContours) == (0) ? 1 : 0);
        }

        public static int stbtt__close_shape(stbtt_vertex* vertices, int num_vertices, int was_off, int start_off,
            int sx, int sy, int scx, int scy, int cx, int cy)
        {
            if ((start_off) != 0)
            {
                if ((was_off) != 0)
                    stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)((cx + scx) >> 1),
                        (int)((cy + scy) >> 1), (int)(cx), (int)(cy));
                stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(sx), (int)(sy), (int)(scx),
                    (int)(scy));
            }
            else
            {
                if ((was_off) != 0)
                    stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(sx), (int)(sy),
                        (int)(cx), (int)(cy));
                else
                    stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vline), (int)(sx), (int)(sy), (int)(0),
                        (int)(0));
            }

            return (int)(num_vertices);
        }

        public static int stbtt__GetGlyphShapeTT(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
        {
            short numberOfContours = 0;
            byte* endPtsOfContours;
            byte* data = info.data;
            stbtt_vertex* vertices = null;
            int num_vertices = (int)(0);
            int g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
            *pvertices = (null);
            if ((g) < (0))
                return (int)(0);
            numberOfContours = (short)(ttSHORT(data + g));
            if ((numberOfContours) > (0))
            {
                byte flags = (byte)(0);
                byte flagcount = 0;
                int ins = 0;
                int i = 0;
                int j = (int)(0);
                int m = 0;
                int n = 0;
                int next_move = 0;
                int was_off = (int)(0);
                int off = 0;
                int start_off = (int)(0);
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
                ins = (int)(ttUSHORT(data + g + 10 + numberOfContours * 2));
                points = data + g + 10 + numberOfContours * 2 + 2 + ins;
                n = (int)(1 + ttUSHORT(endPtsOfContours + numberOfContours * 2 - 2));
                m = (int)(n + 2 * numberOfContours);
                vertices = (stbtt_vertex*)(CRuntime.malloc((ulong)(m * sizeof(stbtt_vertex))));
                if ((vertices) == (null))
                    return (int)(0);
                next_move = (int)(0);
                flagcount = (byte)(0);
                off = (int)(m - n);
                for (i = (int)(0); (i) < (n); ++i)
                {
                    if ((flagcount) == (0))
                    {
                        flags = (byte)(*points++);
                        if ((flags & 8) != 0)
                            flagcount = (byte)(*points++);
                    }
                    else
                        --flagcount;

                    vertices[off + i].type = (byte)(flags);
                }

                x = (int)(0);
                for (i = (int)(0); (i) < (n); ++i)
                {
                    flags = (byte)(vertices[off + i].type);
                    if ((flags & 2) != 0)
                    {
                        short dx = (short)(*points++);
                        x += (int)((flags & 16) != 0 ? dx : -dx);
                    }
                    else
                    {
                        if ((flags & 16) == 0)
                        {
                            x = (int)(x + (short)(points[0] * 256 + points[1]));
                            points += 2;
                        }
                    }

                    vertices[off + i].x = ((short)(x));
                }

                y = (int)(0);
                for (i = (int)(0); (i) < (n); ++i)
                {
                    flags = (byte)(vertices[off + i].type);
                    if ((flags & 4) != 0)
                    {
                        short dy = (short)(*points++);
                        y += (int)((flags & 32) != 0 ? dy : -dy);
                    }
                    else
                    {
                        if ((flags & 32) == 0)
                        {
                            y = (int)(y + (short)(points[0] * 256 + points[1]));
                            points += 2;
                        }
                    }

                    vertices[off + i].y = ((short)(y));
                }

                num_vertices = (int)(0);
                sx = (int)(sy = (int)(cx = (int)(cy = (int)(scx = (int)(scy = (int)(0))))));
                for (i = (int)(0); (i) < (n); ++i)
                {
                    flags = (byte)(vertices[off + i].type);
                    x = (int)(vertices[off + i].x);
                    y = (int)(vertices[off + i].y);
                    if ((next_move) == (i))
                    {
                        if (i != 0)
                            num_vertices = (int)(stbtt__close_shape(vertices, (int)(num_vertices), (int)(was_off),
                                (int)(start_off), (int)(sx), (int)(sy), (int)(scx), (int)(scy), (int)(cx),
                                (int)(cy)));
                        start_off = ((flags & 1) != 0 ? 0 : 1);
                        if ((start_off) != 0)
                        {
                            scx = (int)(x);
                            scy = (int)(y);
                            if ((vertices[off + i + 1].type & 1) == 0)
                            {
                                sx = (int)((x + (int)(vertices[off + i + 1].x)) >> 1);
                                sy = (int)((y + (int)(vertices[off + i + 1].y)) >> 1);
                            }
                            else
                            {
                                sx = ((int)(vertices[off + i + 1].x));
                                sy = ((int)(vertices[off + i + 1].y));
                                ++i;
                            }
                        }
                        else
                        {
                            sx = (int)(x);
                            sy = (int)(y);
                        }

                        stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vmove), (int)(sx), (int)(sy),
                            (int)(0), (int)(0));
                        was_off = (int)(0);
                        next_move = (int)(1 + ttUSHORT(endPtsOfContours + j * 2));
                        ++j;
                    }
                    else
                    {
                        if ((flags & 1) == 0)
                        {
                            if ((was_off) != 0)
                                stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)((cx + x) >> 1),
                                    (int)((cy + y) >> 1), (int)(cx), (int)(cy));
                            cx = (int)(x);
                            cy = (int)(y);
                            was_off = (int)(1);
                        }
                        else
                        {
                            if ((was_off) != 0)
                                stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(x), (int)(y),
                                    (int)(cx), (int)(cy));
                            else
                                stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vline), (int)(x), (int)(y),
                                    (int)(0), (int)(0));
                            was_off = (int)(0);
                        }
                    }
                }

                num_vertices = (int)(stbtt__close_shape(vertices, (int)(num_vertices), (int)(was_off),
                    (int)(start_off), (int)(sx), (int)(sy), (int)(scx), (int)(scy), (int)(cx), (int)(cy)));
            }
            else if ((numberOfContours) == (-1))
            {
                int more = (int)(1);
                byte* comp = data + g + 10;
                num_vertices = (int)(0);
                vertices = null;
                while ((more) != 0)
                {
                    ushort flags = 0;
                    ushort gidx = 0;
                    int comp_num_verts = (int)(0);
                    int i = 0;
                    stbtt_vertex* comp_verts = null;
                    stbtt_vertex* tmp = null;
                    float* mtx = stackalloc float[6];
                    mtx[0] = (float)(1);
                    mtx[1] = (float)(0);
                    mtx[2] = (float)(0);
                    mtx[3] = (float)(1);
                    mtx[4] = (float)(0);
                    mtx[5] = (float)(0);
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
                            mtx[4] = (float)(ttSHORT(comp));
                            comp += 2;
                            mtx[5] = (float)(ttSHORT(comp));
                            comp += 2;
                        }
                        else
                        {
                            mtx[4] = (float)(*(sbyte*)(comp));
                            comp += 1;
                            mtx[5] = (float)(*(sbyte*)(comp));
                            comp += 1;
                        }
                    }
                    else
                    {
                    }

                    if ((flags & (1 << 3)) != 0)
                    {
                        mtx[0] = (float)(mtx[3] = (float)(ttSHORT(comp) / 16384.0f));
                        comp += 2;
                        mtx[1] = (float)(mtx[2] = (float)(0));
                    }
                    else if ((flags & (1 << 6)) != 0)
                    {
                        mtx[0] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                        mtx[1] = (float)(mtx[2] = (float)(0));
                        mtx[3] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                    }
                    else if ((flags & (1 << 7)) != 0)
                    {
                        mtx[0] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                        mtx[1] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                        mtx[2] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                        mtx[3] = (float)(ttSHORT(comp) / 16384.0f);
                        comp += 2;
                    }

                    m = ((float)(CRuntime.sqrt((double)(mtx[0] * mtx[0] + mtx[1] * mtx[1]))));
                    n = ((float)(CRuntime.sqrt((double)(mtx[2] * mtx[2] + mtx[3] * mtx[3]))));
                    comp_num_verts = (int)(stbtt_GetGlyphShape(info, (int)(gidx), &comp_verts));
                    if ((comp_num_verts) > (0))
                    {
                        for (i = (int)(0); (i) < (comp_num_verts); ++i)
                        {
                            stbtt_vertex* v = &comp_verts[i];
                            short x = 0;
                            short y = 0;
                            x = (short)(v->x);
                            y = (short)(v->y);
                            v->x = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
                            v->y = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
                            x = (short)(v->cx);
                            y = (short)(v->cy);
                            v->cx = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
                            v->cy = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
                        }

                        tmp = (stbtt_vertex*)(CRuntime.malloc(
                            (ulong)((num_vertices + comp_num_verts) * sizeof(stbtt_vertex))));
                        if (tmp == null)
                        {
                            if ((vertices) != null)
                                CRuntime.free(vertices);
                            if ((comp_verts) != null)
                                CRuntime.free(comp_verts);
                            return (int)(0);
                        }

                        if ((num_vertices) > (0))
                            CRuntime.memcpy(tmp, vertices, (ulong)(num_vertices * sizeof(stbtt_vertex)));
                        CRuntime.memcpy(tmp + num_vertices, comp_verts,
                            (ulong)(comp_num_verts * sizeof(stbtt_vertex)));
                        if ((vertices) != null)
                            CRuntime.free(vertices);
                        vertices = tmp;
                        CRuntime.free(comp_verts);
                        num_vertices += (int)(comp_num_verts);
                    }

                    more = (int)(flags & (1 << 5));
                }
            }
            else if ((numberOfContours) < (0))
            {
            }
            else
            {
            }

            *pvertices = vertices;
            return (int)(num_vertices);
        }

        public static void stbtt__track_vertex(stbtt__csctx* c, int x, int y)
        {
            if (((x) > (c->max_x)) || (c->started == 0))
                c->max_x = (int)(x);
            if (((y) > (c->max_y)) || (c->started == 0))
                c->max_y = (int)(y);
            if (((x) < (c->min_x)) || (c->started == 0))
                c->min_x = (int)(x);
            if (((y) < (c->min_y)) || (c->started == 0))
                c->min_y = (int)(y);
            c->started = (int)(1);
        }

        public static void stbtt__csctx_v(stbtt__csctx* c, byte type, int x, int y, int cx, int cy, int cx1, int cy1)
        {
            if ((c->bounds) != 0)
            {
                stbtt__track_vertex(c, (int)(x), (int)(y));
                if ((type) == (STBTT_vcubic))
                {
                    stbtt__track_vertex(c, (int)(cx), (int)(cy));
                    stbtt__track_vertex(c, (int)(cx1), (int)(cy1));
                }
            }
            else
            {
                stbtt_setvertex(&c->pvertices[c->num_vertices], (byte)(type), (int)(x), (int)(y), (int)(cx),
                    (int)(cy));
                c->pvertices[c->num_vertices].cx1 = ((short)(cx1));
                c->pvertices[c->num_vertices].cy1 = ((short)(cy1));
            }

            c->num_vertices++;
        }

        public static void stbtt__csctx_close_shape(stbtt__csctx* ctx)
        {
            if ((ctx->first_x != ctx->x) || (ctx->first_y != ctx->y))
                stbtt__csctx_v(ctx, (byte)(STBTT_vline), (int)(ctx->first_x), (int)(ctx->first_y), (int)(0),
                    (int)(0), (int)(0), (int)(0));
        }

        public static void stbtt__csctx_rmove_to(stbtt__csctx* ctx, float dx, float dy)
        {
            stbtt__csctx_close_shape(ctx);
            ctx->first_x = (float)(ctx->x = (float)(ctx->x + dx));
            ctx->first_y = (float)(ctx->y = (float)(ctx->y + dy));
            stbtt__csctx_v(ctx, (byte)(STBTT_vmove), (int)(ctx->x), (int)(ctx->y), (int)(0), (int)(0), (int)(0),
                (int)(0));
        }

        public static void stbtt__csctx_rline_to(stbtt__csctx* ctx, float dx, float dy)
        {
            ctx->x += (float)(dx);
            ctx->y += (float)(dy);
            stbtt__csctx_v(ctx, (byte)(STBTT_vline), (int)(ctx->x), (int)(ctx->y), (int)(0), (int)(0), (int)(0),
                (int)(0));
        }

        public static void stbtt__csctx_rccurve_to(stbtt__csctx* ctx, float dx1, float dy1, float dx2, float dy2,
            float dx3, float dy3)
        {
            float cx1 = (float)(ctx->x + dx1);
            float cy1 = (float)(ctx->y + dy1);
            float cx2 = (float)(cx1 + dx2);
            float cy2 = (float)(cy1 + dy2);
            ctx->x = (float)(cx2 + dx3);
            ctx->y = (float)(cy2 + dy3);
            stbtt__csctx_v(ctx, (byte)(STBTT_vcubic), (int)(ctx->x), (int)(ctx->y), (int)(cx1), (int)(cy1),
                (int)(cx2), (int)(cy2));
        }

        public static stbtt__buf stbtt__get_subr(stbtt__buf idx, int n)
        {
            int count = (int)(stbtt__cff_index_count(&idx));
            int bias = (int)(107);
            if ((count) >= (33900))
                bias = (int)(32768);
            else if ((count) >= (1240))
                bias = (int)(1131);
            n += (int)(bias);
            if (((n) < (0)) || ((n) >= (count)))
                return (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
            return (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(idx), (int)(n)));
        }

        public static stbtt__buf stbtt__cid_get_glyph_subrs(stbtt_fontinfo info, int glyph_index)
        {
            stbtt__buf fdselect = (stbtt__buf)(info.fdselect);
            int nranges = 0;
            int start = 0;
            int end = 0;
            int v = 0;
            int fmt = 0;
            int fdselector = (int)(-1);
            int i = 0;
            stbtt__buf_seek(&fdselect, (int)(0));
            fmt = (int)(stbtt__buf_get8(&fdselect));
            if ((fmt) == (0))
            {
                stbtt__buf_skip(&fdselect, (int)(glyph_index));
                fdselector = (int)(stbtt__buf_get8(&fdselect));
            }
            else if ((fmt) == (3))
            {
                nranges = (int)(stbtt__buf_get((&fdselect), (int)(2)));
                start = (int)(stbtt__buf_get((&fdselect), (int)(2)));
                for (i = (int)(0); (i) < (nranges); i++)
                {
                    v = (int)(stbtt__buf_get8(&fdselect));
                    end = (int)(stbtt__buf_get((&fdselect), (int)(2)));
                    if (((glyph_index) >= (start)) && ((glyph_index) < (end)))
                    {
                        fdselector = (int)(v);
                        break;
                    }

                    start = (int)(end);
                }
            }

            if ((fdselector) == (-1))
                stbtt__new_buf((null), (ulong)(0));
            return (stbtt__buf)(stbtt__get_subrs((stbtt__buf)(info.cff),
                (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(info.fontdicts), (int)(fdselector)))));
        }

        public static int stbtt__run_charstring(stbtt_fontinfo info, int glyph_index, stbtt__csctx* c)
        {
            int in_header = (int)(1);
            int maskbits = (int)(0);
            int subr_stack_height = (int)(0);
            int sp = (int)(0);
            int v = 0;
            int i = 0;
            int b0 = 0;
            int has_subrs = (int)(0);
            int clear_stack = 0;
            float* s = stackalloc float[48];
            stbtt__buf* subr_stack = stackalloc stbtt__buf[10];
            stbtt__buf subrs = (stbtt__buf)(info.subrs);
            stbtt__buf b = new stbtt__buf();
            float f = 0;
            b = (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(info.charstrings), (int)(glyph_index)));
            while ((b.cursor) < (b.size))
            {
                i = (int)(0);
                clear_stack = (int)(1);
                b0 = (int)(stbtt__buf_get8(&b));
                switch (b0)
                {
                    case 0x13:
                    case 0x14:
                        if ((in_header) != 0)
                            maskbits += (int)(sp / 2);
                        in_header = (int)(0);
                        stbtt__buf_skip(&b, (int)((maskbits + 7) / 8));
                        break;
                    case 0x01:
                    case 0x03:
                    case 0x12:
                    case 0x17:
                        maskbits += (int)(sp / 2);
                        break;
                    case 0x15:
                        in_header = (int)(0);
                        if ((sp) < (2))
                            return (int)(0);
                        stbtt__csctx_rmove_to(c, (float)(s[sp - 2]), (float)(s[sp - 1]));
                        break;
                    case 0x04:
                        in_header = (int)(0);
                        if ((sp) < (1))
                            return (int)(0);
                        stbtt__csctx_rmove_to(c, (float)(0), (float)(s[sp - 1]));
                        break;
                    case 0x16:
                        in_header = (int)(0);
                        if ((sp) < (1))
                            return (int)(0);
                        stbtt__csctx_rmove_to(c, (float)(s[sp - 1]), (float)(0));
                        break;
                    case 0x05:
                        if ((sp) < (2))
                            return (int)(0);
                        for (; (i + 1) < (sp); i += (int)(2))
                        {
                            stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
                        }

                        break;
                    case 0x07:
                    case 0x06:
                        if ((sp) < (1))
                            return (int)(0);
                        int goto_vlineto = (int)((b0) == (0x07) ? 1 : 0);
                        for (; ; )
                        {
                            if ((goto_vlineto) == (0))
                            {
                                if ((i) >= (sp))
                                    break;
                                stbtt__csctx_rline_to(c, (float)(s[i]), (float)(0));
                                i++;
                            }

                            goto_vlineto = (int)(0);
                            if ((i) >= (sp))
                                break;
                            stbtt__csctx_rline_to(c, (float)(0), (float)(s[i]));
                            i++;
                        }

                        break;
                    case 0x1F:
                    case 0x1E:
                        if ((sp) < (4))
                            return (int)(0);
                        int goto_hvcurveto = (int)((b0) == (0x1F) ? 1 : 0);
                        for (; ; )
                        {
                            if ((goto_hvcurveto) == (0))
                            {
                                if ((i + 3) >= (sp))
                                    break;
                                stbtt__csctx_rccurve_to(c, (float)(0), (float)(s[i]), (float)(s[i + 1]),
                                    (float)(s[i + 2]), (float)(s[i + 3]),
                                    (float)(((sp - i) == (5)) ? s[i + 4] : 0.0f));
                                i += (int)(4);
                            }

                            goto_hvcurveto = (int)(0);
                            if ((i + 3) >= (sp))
                                break;
                            stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(0), (float)(s[i + 1]),
                                (float)(s[i + 2]), (float)(((sp - i) == (5)) ? s[i + 4] : 0.0f), (float)(s[i + 3]));
                            i += (int)(4);
                        }

                        break;
                    case 0x08:
                        if ((sp) < (6))
                            return (int)(0);
                        for (; (i + 5) < (sp); i += (int)(6))
                        {
                            stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]),
                                (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
                        }

                        break;
                    case 0x18:
                        if ((sp) < (8))
                            return (int)(0);
                        for (; (i + 5) < (sp - 2); i += (int)(6))
                        {
                            stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]),
                                (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
                        }

                        if ((i + 1) >= (sp))
                            return (int)(0);
                        stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
                        break;
                    case 0x19:
                        if ((sp) < (8))
                            return (int)(0);
                        for (; (i + 1) < (sp - 6); i += (int)(2))
                        {
                            stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
                        }

                        if ((i + 5) >= (sp))
                            return (int)(0);
                        stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]),
                            (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
                        break;
                    case 0x1A:
                    case 0x1B:
                        if ((sp) < (4))
                            return (int)(0);
                        f = (float)(0.0);
                        if ((sp & 1) != 0)
                        {
                            f = (float)(s[i]);
                            i++;
                        }

                        for (; (i + 3) < (sp); i += (int)(4))
                        {
                            if ((b0) == (0x1B))
                                stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(f), (float)(s[i + 1]),
                                    (float)(s[i + 2]), (float)(s[i + 3]), (float)(0.0));
                            else
                                stbtt__csctx_rccurve_to(c, (float)(f), (float)(s[i]), (float)(s[i + 1]),
                                    (float)(s[i + 2]), (float)(0.0), (float)(s[i + 3]));
                            f = (float)(0.0);
                        }

                        break;
                    case 0x0A:
                    case 0x1D:
                        if ((b0) == (0x0A))
                        {
                            if (has_subrs == 0)
                            {
                                if ((info.fdselect.size) != 0)
                                    subrs = (stbtt__buf)(stbtt__cid_get_glyph_subrs(info, (int)(glyph_index)));
                                has_subrs = (int)(1);
                            }
                        }

                        if ((sp) < (1))
                            return (int)(0);
                        v = ((int)(s[--sp]));
                        if ((subr_stack_height) >= (10))
                            return (int)(0);
                        subr_stack[subr_stack_height++] = (stbtt__buf)(b);
                        b = (stbtt__buf)(stbtt__get_subr((stbtt__buf)((b0) == (0x0A) ? subrs : info.gsubrs),
                            (int)(v)));
                        if ((b.size) == (0))
                            return (int)(0);
                        b.cursor = (int)(0);
                        clear_stack = (int)(0);
                        break;
                    case 0x0B:
                        if (subr_stack_height <= 0)
                            return (int)(0);
                        b = (stbtt__buf)(subr_stack[--subr_stack_height]);
                        clear_stack = (int)(0);
                        break;
                    case 0x0E:
                        stbtt__csctx_close_shape(c);
                        return (int)(1);
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
                            int b1 = (int)(stbtt__buf_get8(&b));
                            switch (b1)
                            {
                                case 0x22:
                                    if ((sp) < (7))
                                        return (int)(0);
                                    dx1 = (float)(s[0]);
                                    dx2 = (float)(s[1]);
                                    dy2 = (float)(s[2]);
                                    dx3 = (float)(s[3]);
                                    dx4 = (float)(s[4]);
                                    dx5 = (float)(s[5]);
                                    dx6 = (float)(s[6]);
                                    stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(0), (float)(dx2), (float)(dy2),
                                        (float)(dx3), (float)(0));
                                    stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(0), (float)(dx5), (float)(-dy2),
                                        (float)(dx6), (float)(0));
                                    break;
                                case 0x23:
                                    if ((sp) < (13))
                                        return (int)(0);
                                    dx1 = (float)(s[0]);
                                    dy1 = (float)(s[1]);
                                    dx2 = (float)(s[2]);
                                    dy2 = (float)(s[3]);
                                    dx3 = (float)(s[4]);
                                    dy3 = (float)(s[5]);
                                    dx4 = (float)(s[6]);
                                    dy4 = (float)(s[7]);
                                    dx5 = (float)(s[8]);
                                    dy5 = (float)(s[9]);
                                    dx6 = (float)(s[10]);
                                    dy6 = (float)(s[11]);
                                    stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2),
                                        (float)(dx3), (float)(dy3));
                                    stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(dy4), (float)(dx5), (float)(dy5),
                                        (float)(dx6), (float)(dy6));
                                    break;
                                case 0x24:
                                    if ((sp) < (9))
                                        return (int)(0);
                                    dx1 = (float)(s[0]);
                                    dy1 = (float)(s[1]);
                                    dx2 = (float)(s[2]);
                                    dy2 = (float)(s[3]);
                                    dx3 = (float)(s[4]);
                                    dx4 = (float)(s[5]);
                                    dx5 = (float)(s[6]);
                                    dy5 = (float)(s[7]);
                                    dx6 = (float)(s[8]);
                                    stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2),
                                        (float)(dx3), (float)(0));
                                    stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(0), (float)(dx5), (float)(dy5),
                                        (float)(dx6), (float)(-(dy1 + dy2 + dy5)));
                                    break;
                                case 0x25:
                                    if ((sp) < (11))
                                        return (int)(0);
                                    dx1 = (float)(s[0]);
                                    dy1 = (float)(s[1]);
                                    dx2 = (float)(s[2]);
                                    dy2 = (float)(s[3]);
                                    dx3 = (float)(s[4]);
                                    dy3 = (float)(s[5]);
                                    dx4 = (float)(s[6]);
                                    dy4 = (float)(s[7]);
                                    dx5 = (float)(s[8]);
                                    dy5 = (float)(s[9]);
                                    dx6 = (float)(dy6 = (float)(s[10]));
                                    dx = (float)(dx1 + dx2 + dx3 + dx4 + dx5);
                                    dy = (float)(dy1 + dy2 + dy3 + dy4 + dy5);
                                    if ((CRuntime.fabs((double)(dx))) > (CRuntime.fabs((double)(dy))))
                                        dy6 = (float)(-dy);
                                    else
                                        dx6 = (float)(-dx);
                                    stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2),
                                        (float)(dx3), (float)(dy3));
                                    stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(dy4), (float)(dx5), (float)(dy5),
                                        (float)(dx6), (float)(dy6));
                                    break;
                                default:
                                    return (int)(0);
                            }
                        }
                        break;
                    default:
                        if (((b0 != 255) && (b0 != 28)) && (((b0) < (32)) || ((b0) > (254))))
                            return (int)(0);
                        if ((b0) == (255))
                        {
                            f = (float)((float)((int)(stbtt__buf_get((&b), (int)(4)))) / 0x10000);
                        }
                        else
                        {
                            stbtt__buf_skip(&b, (int)(-1));
                            f = ((float)((short)(stbtt__cff_int(&b))));
                        }

                        if ((sp) >= (48))
                            return (int)(0);
                        s[sp++] = (float)(f);
                        clear_stack = (int)(0);
                        break;
                }

                if ((clear_stack) != 0)
                    sp = (int)(0);
            }

            return (int)(0);
        }

        public static int stbtt__GetGlyphShapeT2(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
        {
            stbtt__csctx count_ctx = new stbtt__csctx();
            count_ctx.bounds = (int)(1);
            stbtt__csctx output_ctx = new stbtt__csctx();
            if ((stbtt__run_charstring(info, (int)(glyph_index), &count_ctx)) != 0)
            {
                *pvertices = (stbtt_vertex*)(CRuntime.malloc((ulong)(count_ctx.num_vertices * sizeof(stbtt_vertex))));
                output_ctx.pvertices = *pvertices;
                if ((stbtt__run_charstring(info, (int)(glyph_index), &output_ctx)) != 0)
                {
                    return (int)(output_ctx.num_vertices);
                }
            }

            *pvertices = (null);
            return (int)(0);
        }

        public static int stbtt__GetGlyphInfoT2(stbtt_fontinfo info, int glyph_index, int* x0, int* y0, int* x1,
            int* y1)
        {
            stbtt__csctx c = new stbtt__csctx();
            c.bounds = (int)(1);
            int r = (int)(stbtt__run_charstring(info, (int)(glyph_index), &c));
            if ((x0) != null)
                *x0 = (int)((r) != 0 ? c.min_x : 0);
            if ((y0) != null)
                *y0 = (int)((r) != 0 ? c.min_y : 0);
            if ((x1) != null)
                *x1 = (int)((r) != 0 ? c.max_x : 0);
            if ((y1) != null)
                *y1 = (int)((r) != 0 ? c.max_y : 0);
            return (int)((r) != 0 ? c.num_vertices : 0);
        }

        public static int stbtt_GetGlyphShape(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
        {
            if (info.cff.size == 0)
                return (int)(stbtt__GetGlyphShapeTT(info, (int)(glyph_index), pvertices));
            else
                return (int)(stbtt__GetGlyphShapeT2(info, (int)(glyph_index), pvertices));
        }

        public static void stbtt_GetGlyphHMetrics(stbtt_fontinfo info, int glyph_index, int* advanceWidth,
            int* leftSideBearing)
        {
            ushort numOfLongHorMetrics = (ushort)(ttUSHORT(info.data + info.hhea + 34));
            if ((glyph_index) < (numOfLongHorMetrics))
            {
                if ((advanceWidth) != null)
                    *advanceWidth = (int)(ttSHORT(info.data + info.hmtx + 4 * glyph_index));
                if ((leftSideBearing) != null)
                    *leftSideBearing = (int)(ttSHORT(info.data + info.hmtx + 4 * glyph_index + 2));
            }
            else
            {
                if ((advanceWidth) != null)
                    *advanceWidth = (int)(ttSHORT(info.data + info.hmtx + 4 * (numOfLongHorMetrics - 1)));
                if ((leftSideBearing) != null)
                    *leftSideBearing = (int)(ttSHORT(info.data + info.hmtx + 4 * numOfLongHorMetrics +
                                                      2 * (glyph_index - numOfLongHorMetrics)));
            }
        }

        public static int stbtt__GetGlyphKernInfoAdvance(stbtt_fontinfo info, int glyph1, int glyph2)
        {
            byte* data = info.data + info.kern;
            uint needle = 0;
            uint straw = 0;
            int l = 0;
            int r = 0;
            int m = 0;
            if (info.kern == 0)
                return (int)(0);
            if ((ttUSHORT(data + 2)) < (1))
                return (int)(0);
            if (ttUSHORT(data + 8) != 1)
                return (int)(0);
            l = (int)(0);
            r = (int)(ttUSHORT(data + 10) - 1);
            needle = (uint)(glyph1 << 16 | glyph2);
            while (l <= r)
            {
                m = (int)((l + r) >> 1);
                straw = (uint)(ttULONG(data + 18 + (m * 6)));
                if ((needle) < (straw))
                    r = (int)(m - 1);
                else if ((needle) > (straw))
                    l = (int)(m + 1);
                else
                    return (int)(ttSHORT(data + 22 + (m * 6)));
            }

            return (int)(0);
        }

        public static int stbtt__GetCoverageIndex(byte* coverageTable, int glyph)
        {
            ushort coverageFormat = (ushort)(ttUSHORT(coverageTable));
            switch (coverageFormat)
            {
                case 1:
                    {
                        ushort glyphCount = (ushort)(ttUSHORT(coverageTable + 2));
                        int l = (int)(0);
                        int r = (int)(glyphCount - 1);
                        int m = 0;
                        int straw = 0;
                        int needle = (int)(glyph);
                        while (l <= r)
                        {
                            byte* glyphArray = coverageTable + 4;
                            ushort glyphID = 0;
                            m = (int)((l + r) >> 1);
                            glyphID = (ushort)(ttUSHORT(glyphArray + 2 * m));
                            straw = (int)(glyphID);
                            if ((needle) < (straw))
                                r = (int)(m - 1);
                            else if ((needle) > (straw))
                                l = (int)(m + 1);
                            else
                            {
                                return (int)(m);
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        ushort rangeCount = (ushort)(ttUSHORT(coverageTable + 2));
                        byte* rangeArray = coverageTable + 4;
                        int l = (int)(0);
                        int r = (int)(rangeCount - 1);
                        int m = 0;
                        int strawStart = 0;
                        int strawEnd = 0;
                        int needle = (int)(glyph);
                        while (l <= r)
                        {
                            byte* rangeRecord;
                            m = (int)((l + r) >> 1);
                            rangeRecord = rangeArray + 6 * m;
                            strawStart = (int)(ttUSHORT(rangeRecord));
                            strawEnd = (int)(ttUSHORT(rangeRecord + 2));
                            if ((needle) < (strawStart))
                                r = (int)(m - 1);
                            else if ((needle) > (strawEnd))
                                l = (int)(m + 1);
                            else
                            {
                                ushort startCoverageIndex = (ushort)(ttUSHORT(rangeRecord + 4));
                                return (int)(startCoverageIndex + glyph - strawStart);
                            }
                        }
                    }
                    break;
                default:
                    {
                    }
                    break;
            }

            return (int)(-1);
        }

        public static int stbtt__GetGlyphClass(byte* classDefTable, int glyph)
        {
            ushort classDefFormat = (ushort)(ttUSHORT(classDefTable));
            switch (classDefFormat)
            {
                case 1:
                    {
                        ushort startGlyphID = (ushort)(ttUSHORT(classDefTable + 2));
                        ushort glyphCount = (ushort)(ttUSHORT(classDefTable + 4));
                        byte* classDef1ValueArray = classDefTable + 6;
                        if (((glyph) >= (startGlyphID)) && ((glyph) < (startGlyphID + glyphCount)))
                            return (int)(ttUSHORT(classDef1ValueArray + 2 * (glyph - startGlyphID)));
                        classDefTable = classDef1ValueArray + 2 * glyphCount;
                    }
                    break;
                case 2:
                    {
                        ushort classRangeCount = (ushort)(ttUSHORT(classDefTable + 2));
                        byte* classRangeRecords = classDefTable + 4;
                        int l = (int)(0);
                        int r = (int)(classRangeCount - 1);
                        int m = 0;
                        int strawStart = 0;
                        int strawEnd = 0;
                        int needle = (int)(glyph);
                        while (l <= r)
                        {
                            byte* classRangeRecord;
                            m = (int)((l + r) >> 1);
                            classRangeRecord = classRangeRecords + 6 * m;
                            strawStart = (int)(ttUSHORT(classRangeRecord));
                            strawEnd = (int)(ttUSHORT(classRangeRecord + 2));
                            if ((needle) < (strawStart))
                                r = (int)(m - 1);
                            else if ((needle) > (strawEnd))
                                l = (int)(m + 1);
                            else
                                return (int)(ttUSHORT(classRangeRecord + 4));
                        }

                        classDefTable = classRangeRecords + 6 * classRangeCount;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }

            return (int)(-1);
        }

        public static int stbtt__GetGlyphGPOSInfoAdvance(stbtt_fontinfo info, int glyph1, int glyph2)
        {
            ushort lookupListOffset = 0;
            byte* lookupList;
            ushort lookupCount = 0;
            byte* data;
            int i = 0;
            if (info.gpos == 0)
                return (int)(0);
            data = info.data + info.gpos;
            if (ttUSHORT(data + 0) != 1)
                return (int)(0);
            if (ttUSHORT(data + 2) != 0)
                return (int)(0);
            lookupListOffset = (ushort)(ttUSHORT(data + 8));
            lookupList = data + lookupListOffset;
            lookupCount = (ushort)(ttUSHORT(lookupList));
            for (i = (int)(0); (i) < (lookupCount); ++i)
            {
                ushort lookupOffset = (ushort)(ttUSHORT(lookupList + 2 + 2 * i));
                byte* lookupTable = lookupList + lookupOffset;
                ushort lookupType = (ushort)(ttUSHORT(lookupTable));
                ushort subTableCount = (ushort)(ttUSHORT(lookupTable + 4));
                byte* subTableOffsets = lookupTable + 6;
                switch (lookupType)
                {
                    case 2:
                        {
                            int sti = 0;
                            for (sti = (int)(0); (sti) < (subTableCount); sti++)
                            {
                                ushort subtableOffset = (ushort)(ttUSHORT(subTableOffsets + 2 * sti));
                                byte* table = lookupTable + subtableOffset;
                                ushort posFormat = (ushort)(ttUSHORT(table));
                                ushort coverageOffset = (ushort)(ttUSHORT(table + 2));
                                int coverageIndex = (int)(stbtt__GetCoverageIndex(table + coverageOffset, (int)(glyph1)));
                                if ((coverageIndex) == (-1))
                                    continue;
                                switch (posFormat)
                                {
                                    case 1:
                                        {
                                            int l = 0;
                                            int r = 0;
                                            int m = 0;
                                            int straw = 0;
                                            int needle = 0;
                                            ushort valueFormat1 = (ushort)(ttUSHORT(table + 4));
                                            ushort valueFormat2 = (ushort)(ttUSHORT(table + 6));
                                            int valueRecordPairSizeInBytes = (int)(2);
                                            ushort pairSetCount = (ushort)(ttUSHORT(table + 8));
                                            ushort pairPosOffset = (ushort)(ttUSHORT(table + 10 + 2 * coverageIndex));
                                            byte* pairValueTable = table + pairPosOffset;
                                            ushort pairValueCount = (ushort)(ttUSHORT(pairValueTable));
                                            byte* pairValueArray = pairValueTable + 2;
                                            if (valueFormat1 != 4)
                                                return (int)(0);
                                            if (valueFormat2 != 0)
                                                return (int)(0);
                                            needle = (int)(glyph2);
                                            r = (int)(pairValueCount - 1);
                                            l = (int)(0);
                                            while (l <= r)
                                            {
                                                ushort secondGlyph = 0;
                                                byte* pairValue;
                                                m = (int)((l + r) >> 1);
                                                pairValue = pairValueArray + (2 + valueRecordPairSizeInBytes) * m;
                                                secondGlyph = (ushort)(ttUSHORT(pairValue));
                                                straw = (int)(secondGlyph);
                                                if ((needle) < (straw))
                                                    r = (int)(m - 1);
                                                else if ((needle) > (straw))
                                                    l = (int)(m + 1);
                                                else
                                                {
                                                    short xAdvance = (short)(ttSHORT(pairValue + 2));
                                                    return (int)(xAdvance);
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            ushort valueFormat1 = (ushort)(ttUSHORT(table + 4));
                                            ushort valueFormat2 = (ushort)(ttUSHORT(table + 6));
                                            ushort classDef1Offset = (ushort)(ttUSHORT(table + 8));
                                            ushort classDef2Offset = (ushort)(ttUSHORT(table + 10));
                                            int glyph1class =
                                                (int)(stbtt__GetGlyphClass(table + classDef1Offset, (int)(glyph1)));
                                            int glyph2class =
                                                (int)(stbtt__GetGlyphClass(table + classDef2Offset, (int)(glyph2)));
                                            ushort class1Count = (ushort)(ttUSHORT(table + 12));
                                            ushort class2Count = (ushort)(ttUSHORT(table + 14));
                                            if (valueFormat1 != 4)
                                                return (int)(0);
                                            if (valueFormat2 != 0)
                                                return (int)(0);
                                            if (((((glyph1class) >= (0)) && ((glyph1class) < (class1Count))) &&
                                                 ((glyph2class) >= (0))) && ((glyph2class) < (class2Count)))
                                            {
                                                byte* class1Records = table + 16;
                                                byte* class2Records = class1Records + 2 * (glyph1class * class2Count);
                                                short xAdvance = (short)(ttSHORT(class2Records + 2 * glyph2class));
                                                return (int)(xAdvance);
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

            return (int)(0);
        }

        public static int stbtt_GetGlyphKernAdvance(stbtt_fontinfo info, int g1, int g2)
        {
            int xAdvance = (int)(0);
            if ((info.gpos) != 0)
                xAdvance += (int)(stbtt__GetGlyphGPOSInfoAdvance(info, (int)(g1), (int)(g2)));
            if ((info.kern) != 0)
                xAdvance += (int)(stbtt__GetGlyphKernInfoAdvance(info, (int)(g1), (int)(g2)));
            return (int)(xAdvance);
        }

        public static int stbtt_GetCodepointKernAdvance(stbtt_fontinfo info, int ch1, int ch2)
        {
            if ((info.kern == 0) && (info.gpos == 0))
                return (int)(0);
            return (int)(stbtt_GetGlyphKernAdvance(info, (int)(stbtt_FindGlyphIndex(info, (int)(ch1))),
                (int)(stbtt_FindGlyphIndex(info, (int)(ch2)))));
        }

        public static void stbtt_GetCodepointHMetrics(stbtt_fontinfo info, int codepoint, int* advanceWidth,
            int* leftSideBearing)
        {
            stbtt_GetGlyphHMetrics(info, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), advanceWidth,
                leftSideBearing);
        }

        public static void stbtt_GetFontVMetrics(stbtt_fontinfo info, int* ascent, int* descent, int* lineGap)
        {
            if ((ascent) != null)
                *ascent = (int)(ttSHORT(info.data + info.hhea + 4));
            if ((descent) != null)
                *descent = (int)(ttSHORT(info.data + info.hhea + 6));
            if ((lineGap) != null)
                *lineGap = (int)(ttSHORT(info.data + info.hhea + 8));
        }

        public static int stbtt_GetFontVMetricsOS2(stbtt_fontinfo info, int* typoAscent, int* typoDescent,
            int* typoLineGap)
        {
            int tab = (int)(stbtt__find_table(info.data, (uint)(info.fontstart), "OS/2"));
            if (tab == 0)
                return (int)(0);
            if ((typoAscent) != null)
                *typoAscent = (int)(ttSHORT(info.data + tab + 68));
            if ((typoDescent) != null)
                *typoDescent = (int)(ttSHORT(info.data + tab + 70));
            if ((typoLineGap) != null)
                *typoLineGap = (int)(ttSHORT(info.data + tab + 72));
            return (int)(1);
        }

        public static void stbtt_GetFontBoundingBox(stbtt_fontinfo info, int* x0, int* y0, int* x1, int* y1)
        {
            *x0 = (int)(ttSHORT(info.data + info.head + 36));
            *y0 = (int)(ttSHORT(info.data + info.head + 38));
            *x1 = (int)(ttSHORT(info.data + info.head + 40));
            *y1 = (int)(ttSHORT(info.data + info.head + 42));
        }

        public static float stbtt_ScaleForPixelHeight(stbtt_fontinfo info, float height)
        {
            int fheight = (int)(ttSHORT(info.data + info.hhea + 4) - ttSHORT(info.data + info.hhea + 6));
            return (float)(height / fheight);
        }

        public static float stbtt_ScaleForMappingEmToPixels(stbtt_fontinfo info, float pixels)
        {
            int unitsPerEm = (int)(ttUSHORT(info.data + info.head + 18));
            return (float)(pixels / unitsPerEm);
        }

        public static void stbtt_FreeShape(stbtt_fontinfo info, stbtt_vertex* v)
        {
            CRuntime.free(v);
        }

        public static void stbtt_GetGlyphBitmapBoxSubpixel(stbtt_fontinfo font, int glyph, float scale_x, float scale_y,
            float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
        {
            int x0 = (int)(0);
            int y0 = (int)(0);
            int x1 = 0;
            int y1 = 0;
            if (stbtt_GetGlyphBox(font, (int)(glyph), &x0, &y0, &x1, &y1) == 0)
            {
                if ((ix0) != null)
                    *ix0 = (int)(0);
                if ((iy0) != null)
                    *iy0 = (int)(0);
                if ((ix1) != null)
                    *ix1 = (int)(0);
                if ((iy1) != null)
                    *iy1 = (int)(0);
            }
            else
            {
                if ((ix0) != null)
                    *ix0 = ((int)(CRuntime.floor((double)(x0 * scale_x + shift_x))));
                if ((iy0) != null)
                    *iy0 = ((int)(CRuntime.floor((double)(-y1 * scale_y + shift_y))));
                if ((ix1) != null)
                    *ix1 = ((int)(CRuntime.ceil((double)(x1 * scale_x + shift_x))));
                if ((iy1) != null)
                    *iy1 = ((int)(CRuntime.ceil((double)(-y0 * scale_y + shift_y))));
            }
        }

        public static void stbtt_GetGlyphBitmapBox(stbtt_fontinfo font, int glyph, float scale_x, float scale_y,
            int* ix0, int* iy0, int* ix1, int* iy1)
        {
            stbtt_GetGlyphBitmapBoxSubpixel(font, (int)(glyph), (float)(scale_x), (float)(scale_y), (float)(0.0f),
                (float)(0.0f), ix0, iy0, ix1, iy1);
        }

        public static void stbtt_GetCodepointBitmapBoxSubpixel(stbtt_fontinfo font, int codepoint, float scale_x,
            float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
        {
            stbtt_GetGlyphBitmapBoxSubpixel(font, (int)(stbtt_FindGlyphIndex(font, (int)(codepoint))),
                (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), ix0, iy0, ix1, iy1);
        }

        public static void stbtt_GetCodepointBitmapBox(stbtt_fontinfo font, int codepoint, float scale_x, float scale_y,
            int* ix0, int* iy0, int* ix1, int* iy1)
        {
            stbtt_GetCodepointBitmapBoxSubpixel(font, (int)(codepoint), (float)(scale_x), (float)(scale_y),
                (float)(0.0f), (float)(0.0f), ix0, iy0, ix1, iy1);
        }

        public static void* stbtt__hheap_alloc(stbtt__hheap* hh, ulong size)
        {
            if ((hh->first_free) != null)
            {
                void* p = hh->first_free;
                hh->first_free = *(void**)(p);
                return p;
            }
            else
            {
                if ((hh->num_remaining_in_head_chunk) == (0))
                {
                    int count = (int)((size) < (32) ? 2000 : (size) < (128) ? 800 : 100);
                    stbtt__hheap_chunk* c =
                        (stbtt__hheap_chunk*)(CRuntime.malloc(
                            (ulong)((ulong)sizeof(stbtt__hheap_chunk) + size * (ulong)(count))));
                    if ((c) == (null))
                        return (null);
                    c->next = hh->head;
                    hh->head = c;
                    hh->num_remaining_in_head_chunk = (int)(count);
                }

                --hh->num_remaining_in_head_chunk;
                return (sbyte*)(hh->head) + sizeof(stbtt__hheap_chunk) +
                       size * (ulong)hh->num_remaining_in_head_chunk;
            }
        }

        public static void stbtt__hheap_free(stbtt__hheap* hh, void* p)
        {
            *(void**)(p) = hh->first_free;
            hh->first_free = p;
        }

        public static void stbtt__hheap_cleanup(stbtt__hheap* hh)
        {
            stbtt__hheap_chunk* c = hh->head;
            while ((c) != null)
            {
                stbtt__hheap_chunk* n = c->next;
                CRuntime.free(c);
                c = n;
            }
        }

        public static stbtt__active_edge* stbtt__new_active(stbtt__hheap* hh, stbtt__edge* e, int off_x,
            float start_point)
        {
            stbtt__active_edge* z =
                (stbtt__active_edge*)(stbtt__hheap_alloc(hh, (ulong)(sizeof(stbtt__active_edge))));
            float dxdy = (float)((e->x1 - e->x0) / (e->y1 - e->y0));
            if (z == null)
                return z;
            z->fdx = (float)(dxdy);
            z->fdy = (float)(dxdy != 0.0f ? (1.0f / dxdy) : 0.0f);
            z->fx = (float)(e->x0 + dxdy * (start_point - e->y0));
            z->fx -= (float)(off_x);
            z->direction = (float)((e->invert) != 0 ? 1.0f : -1.0f);
            z->sy = (float)(e->y0);
            z->ey = (float)(e->y1);
            z->next = null;
            return z;
        }

        public static void stbtt__handle_clipped_edge(float* scanline, int x, stbtt__active_edge* e, float x0, float y0,
            float x1, float y1)
        {
            if ((y0) == (y1))
                return;
            if ((y0) > (e->ey))
                return;
            if ((y1) < (e->sy))
                return;
            if ((y0) < (e->sy))
            {
                x0 += (float)((x1 - x0) * (e->sy - y0) / (y1 - y0));
                y0 = (float)(e->sy);
            }

            if ((y1) > (e->ey))
            {
                x1 += (float)((x1 - x0) * (e->ey - y1) / (y1 - y0));
                y1 = (float)(e->ey);
            }

            if ((x0) == (x))
            {
            }
            else if ((x0) == (x + 1))
            {
            }
            else if (x0 <= x)
            {
            }
            else if ((x0) >= (x + 1))
            {
            }
            else
            {
            }

            if ((x0 <= x) && (x1 <= x))
            {
                scanline[x] += (float)(e->direction * (y1 - y0));
            }
            else if (((x0) >= (x + 1)) && ((x1) >= (x + 1)))
            {
            }
            else
            {
                scanline[x] += (float)(e->direction * (y1 - y0) * (1 - ((x0 - x) + (x1 - x)) / 2));
            }
        }

        public static void stbtt__fill_active_edges_new(float* scanline, float* scanline_fill, int len,
            stbtt__active_edge* e, float y_top)
        {
            float y_bottom = (float)(y_top + 1);
            while ((e) != null)
            {
                if ((e->fdx) == (0))
                {
                    float x0 = (float)(e->fx);
                    if ((x0) < (len))
                    {
                        if ((x0) >= (0))
                        {
                            stbtt__handle_clipped_edge(scanline, (int)(x0), e, (float)(x0), (float)(y_top),
                                (float)(x0), (float)(y_bottom));
                            stbtt__handle_clipped_edge(scanline_fill - 1, (int)((int)(x0) + 1), e, (float)(x0),
                                (float)(y_top), (float)(x0), (float)(y_bottom));
                        }
                        else
                        {
                            stbtt__handle_clipped_edge(scanline_fill - 1, (int)(0), e, (float)(x0), (float)(y_top),
                                (float)(x0), (float)(y_bottom));
                        }
                    }
                }
                else
                {
                    float x0 = (float)(e->fx);
                    float dx = (float)(e->fdx);
                    float xb = (float)(x0 + dx);
                    float x_top = 0;
                    float x_bottom = 0;
                    float sy0 = 0;
                    float sy1 = 0;
                    float dy = (float)(e->fdy);
                    if ((e->sy) > (y_top))
                    {
                        x_top = (float)(x0 + dx * (e->sy - y_top));
                        sy0 = (float)(e->sy);
                    }
                    else
                    {
                        x_top = (float)(x0);
                        sy0 = (float)(y_top);
                    }

                    if ((e->ey) < (y_bottom))
                    {
                        x_bottom = (float)(x0 + dx * (e->ey - y_top));
                        sy1 = (float)(e->ey);
                    }
                    else
                    {
                        x_bottom = (float)(xb);
                        sy1 = (float)(y_bottom);
                    }

                    if (((((x_top) >= (0)) && ((x_bottom) >= (0))) && ((x_top) < (len))) && ((x_bottom) < (len)))
                    {
                        if (((int)(x_top)) == ((int)(x_bottom)))
                        {
                            float height = 0;
                            int x = (int)(x_top);
                            height = (float)(sy1 - sy0);
                            scanline[x] += (float)(e->direction * (1 - ((x_top - x) + (x_bottom - x)) / 2) * height);
                            scanline_fill[x] += (float)(e->direction * height);
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
                            if ((x_top) > (x_bottom))
                            {
                                float t = 0;
                                sy0 = (float)(y_bottom - (sy0 - y_top));
                                sy1 = (float)(y_bottom - (sy1 - y_top));
                                t = (float)(sy0);
                                sy0 = (float)(sy1);
                                sy1 = (float)(t);
                                t = (float)(x_bottom);
                                x_bottom = (float)(x_top);
                                x_top = (float)(t);
                                dx = (float)(-dx);
                                dy = (float)(-dy);
                                t = (float)(x0);
                                x0 = (float)(xb);
                                xb = (float)(t);
                            }

                            x1 = ((int)(x_top));
                            x2 = ((int)(x_bottom));
                            y_crossing = (float)((x1 + 1 - x0) * dy + y_top);
                            sign = (float)(e->direction);
                            area = (float)(sign * (y_crossing - sy0));
                            scanline[x1] += (float)(area * (1 - ((x_top - x1) + (x1 + 1 - x1)) / 2));
                            step = (float)(sign * dy);
                            for (x = (int)(x1 + 1); (x) < (x2); ++x)
                            {
                                scanline[x] += (float)(area + step / 2);
                                area += (float)(step);
                            }

                            y_crossing += (float)(dy * (x2 - (x1 + 1)));
                            scanline[x2] +=
                                (float)(area + sign * (1 - ((x2 - x2) + (x_bottom - x2)) / 2) * (sy1 - y_crossing));
                            scanline_fill[x2] += (float)(sign * (sy1 - sy0));
                        }
                    }
                    else
                    {
                        int x = 0;
                        for (x = (int)(0); (x) < (len); ++x)
                        {
                            float y0 = (float)(y_top);
                            float x1 = (float)(x);
                            float x2 = (float)(x + 1);
                            float x3 = (float)(xb);
                            float y3 = (float)(y_bottom);
                            float y1 = (float)((x - x0) / dx + y_top);
                            float y2 = (float)((x + 1 - x0) / dx + y_top);
                            if (((x0) < (x1)) && ((x3) > (x2)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x1), (float)(y1));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x1), (float)(y1),
                                    (float)(x2), (float)(y2));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x2), (float)(y2),
                                    (float)(x3), (float)(y3));
                            }
                            else if (((x3) < (x1)) && ((x0) > (x2)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x2), (float)(y2));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x2), (float)(y2),
                                    (float)(x1), (float)(y1));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x1), (float)(y1),
                                    (float)(x3), (float)(y3));
                            }
                            else if (((x0) < (x1)) && ((x3) > (x1)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x1), (float)(y1));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x1), (float)(y1),
                                    (float)(x3), (float)(y3));
                            }
                            else if (((x3) < (x1)) && ((x0) > (x1)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x1), (float)(y1));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x1), (float)(y1),
                                    (float)(x3), (float)(y3));
                            }
                            else if (((x0) < (x2)) && ((x3) > (x2)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x2), (float)(y2));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x2), (float)(y2),
                                    (float)(x3), (float)(y3));
                            }
                            else if (((x3) < (x2)) && ((x0) > (x2)))
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x2), (float)(y2));
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x2), (float)(y2),
                                    (float)(x3), (float)(y3));
                            }
                            else
                            {
                                stbtt__handle_clipped_edge(scanline, (int)(x), e, (float)(x0), (float)(y0),
                                    (float)(x3), (float)(y3));
                            }
                        }
                    }
                }

                e = e->next;
            }
        }

        public static void stbtt__rasterize_sorted_edges(stbtt__bitmap* result, stbtt__edge* e, int n, int vsubsample,
            int off_x, int off_y)
        {
            stbtt__hheap hh = new stbtt__hheap();
            stbtt__active_edge* active = (null);
            int y = 0;
            int j = (int)(0);
            int i = 0;
            float* scanline_data = stackalloc float[129];
            float* scanline;
            float* scanline2;
            if ((result->w) > (64))
                scanline = (float*)(CRuntime.malloc((ulong)((result->w * 2 + 1) * sizeof(float))));
            else
                scanline = scanline_data;
            scanline2 = scanline + result->w;
            y = (int)(off_y);
            e[n].y0 = (float)((float)(off_y + result->h) + 1);
            while ((j) < (result->h))
            {
                float scan_y_top = (float)(y + 0.0f);
                float scan_y_bottom = (float)(y + 1.0f);
                stbtt__active_edge** step = &active;
                CRuntime.memset(scanline, (int)(0), (ulong)(result->w * sizeof(float)));
                CRuntime.memset(scanline2, (int)(0), (ulong)((result->w + 1) * sizeof(float)));
                while ((*step) != null)
                {
                    stbtt__active_edge* z = *step;
                    if (z->ey <= scan_y_top)
                    {
                        *step = z->next;
                        z->direction = (float)(0);
                        stbtt__hheap_free(&hh, z);
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
                        stbtt__active_edge* z = stbtt__new_active(&hh, e, (int)(off_x), (float)(scan_y_top));
                        if (z != (null))
                        {
                            if (((j) == (0)) && (off_y != 0))
                            {
                                if ((z->ey) < (scan_y_top))
                                {
                                    z->ey = (float)(scan_y_top);
                                }
                            }

                            z->next = active;
                            active = z;
                        }
                    }

                    ++e;
                }

                if ((active) != null)
                    stbtt__fill_active_edges_new(scanline, scanline2 + 1, (int)(result->w), active,
                        (float)(scan_y_top));
                {
                    float sum = (float)(0);
                    for (i = (int)(0); (i) < (result->w); ++i)
                    {
                        float k = 0;
                        int m = 0;
                        sum += (float)(scanline2[i]);
                        k = (float)(scanline[i] + sum);
                        k = (float)((float)(CRuntime.fabs((double)(k))) * 255 + 0.5f);
                        m = ((int)(k));
                        if ((m) > (255))
                            m = (int)(255);
                        result->pixels[j * result->stride + i] = ((byte)(m));
                    }
                }
                step = &active;
                while ((*step) != null)
                {
                    stbtt__active_edge* z = *step;
                    z->fx += (float)(z->fdx);
                    step = &((*step)->next);
                }

                ++y;
                ++j;
            }

            stbtt__hheap_cleanup(&hh);
            if (scanline != scanline_data)
                CRuntime.free(scanline);
        }

        public static void stbtt__sort_edges_ins_sort(stbtt__edge* p, int n)
        {
            int i = 0;
            int j = 0;
            for (i = (int)(1); (i) < (n); ++i)
            {
                stbtt__edge t = (stbtt__edge)(p[i]);
                stbtt__edge* a = &t;
                j = (int)(i);
                while ((j) > (0))
                {
                    stbtt__edge* b = &p[j - 1];
                    int c = (int)(a->y0 < b->y0 ? 1 : 0);
                    if (c == 0)
                        break;
                    p[j] = (stbtt__edge)(p[j - 1]);
                    --j;
                }

                if (i != j)
                    p[j] = (stbtt__edge)(t);
            }
        }

        public static void stbtt__sort_edges_quicksort(stbtt__edge* p, int n)
        {
            while ((n) > (12))
            {
                stbtt__edge t = new stbtt__edge();
                int c01 = 0;
                int c12 = 0;
                int c = 0;
                int m = 0;
                int i = 0;
                int j = 0;
                m = (int)(n >> 1);
                c01 = (int)(((&p[0])->y0) < ((&p[m])->y0) ? 1 : 0);
                c12 = (int)(((&p[m])->y0) < ((&p[n - 1])->y0) ? 1 : 0);
                if (c01 != c12)
                {
                    int z = 0;
                    c = (int)(((&p[0])->y0) < ((&p[n - 1])->y0) ? 1 : 0);
                    z = (int)(((c) == (c12)) ? 0 : n - 1);
                    t = (stbtt__edge)(p[z]);
                    p[z] = (stbtt__edge)(p[m]);
                    p[m] = (stbtt__edge)(t);
                }

                t = (stbtt__edge)(p[0]);
                p[0] = (stbtt__edge)(p[m]);
                p[m] = (stbtt__edge)(t);
                i = (int)(1);
                j = (int)(n - 1);
                for (; ; )
                {
                    for (; ; ++i)
                    {
                        if (!(((&p[i])->y0) < ((&p[0])->y0)))
                            break;
                    }

                    for (; ; --j)
                    {
                        if (!(((&p[0])->y0) < ((&p[j])->y0)))
                            break;
                    }

                    if ((i) >= (j))
                        break;
                    t = (stbtt__edge)(p[i]);
                    p[i] = (stbtt__edge)(p[j]);
                    p[j] = (stbtt__edge)(t);
                    ++i;
                    --j;
                }

                if ((j) < (n - i))
                {
                    stbtt__sort_edges_quicksort(p, (int)(j));
                    p = p + i;
                    n = (int)(n - i);
                }
                else
                {
                    stbtt__sort_edges_quicksort(p + i, (int)(n - i));
                    n = (int)(j);
                }
            }
        }

        public static void stbtt__sort_edges(stbtt__edge* p, int n)
        {
            stbtt__sort_edges_quicksort(p, (int)(n));
            stbtt__sort_edges_ins_sort(p, (int)(n));
        }

        public static void stbtt__rasterize(stbtt__bitmap* result, stbtt__point* pts, int* wcount, int windings,
            float scale_x, float scale_y, float shift_x, float shift_y, int off_x, int off_y, int invert)
        {
            float y_scale_inv = (float)((invert) != 0 ? -scale_y : scale_y);
            stbtt__edge* e;
            int n = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int vsubsample = (int)(1);
            n = (int)(0);
            for (i = (int)(0); (i) < (windings); ++i)
            {
                n += (int)(wcount[i]);
            }

            e = (stbtt__edge*)(CRuntime.malloc((ulong)(sizeof(stbtt__edge) * (n + 1))));
            if ((e) == (null))
                return;
            n = (int)(0);
            m = (int)(0);
            for (i = (int)(0); (i) < (windings); ++i)
            {
                stbtt__point* p = pts + m;
                m += (int)(wcount[i]);
                j = (int)(wcount[i] - 1);
                for (k = (int)(0); (k) < (wcount[i]); j = (int)(k++))
                {
                    int a = (int)(k);
                    int b = (int)(j);
                    if ((p[j].y) == (p[k].y))
                        continue;
                    e[n].invert = (int)(0);
                    if ((((invert) != 0) && ((p[j].y) > (p[k].y))) || ((invert == 0) && ((p[j].y) < (p[k].y))))
                    {
                        e[n].invert = (int)(1);
                        a = (int)(j);
                        b = (int)(k);
                    }

                    e[n].x0 = (float)(p[a].x * scale_x + shift_x);
                    e[n].y0 = (float)((p[a].y * y_scale_inv + shift_y) * vsubsample);
                    e[n].x1 = (float)(p[b].x * scale_x + shift_x);
                    e[n].y1 = (float)((p[b].y * y_scale_inv + shift_y) * vsubsample);
                    ++n;
                }
            }

            stbtt__sort_edges(e, (int)(n));
            stbtt__rasterize_sorted_edges(result, e, (int)(n), (int)(vsubsample), (int)(off_x), (int)(off_y));
            CRuntime.free(e);
        }

        public static void stbtt__add_point(stbtt__point* points, int n, float x, float y)
        {
            if (points == null)
                return;
            points[n].x = (float)(x);
            points[n].y = (float)(y);
        }

        public static int stbtt__tesselate_curve(stbtt__point* points, int* num_points, float x0, float y0, float x1,
            float y1, float x2, float y2, float objspace_flatness_squared, int n)
        {
            float mx = (float)((x0 + 2 * x1 + x2) / 4);
            float my = (float)((y0 + 2 * y1 + y2) / 4);
            float dx = (float)((x0 + x2) / 2 - mx);
            float dy = (float)((y0 + y2) / 2 - my);
            if ((n) > (16))
                return (int)(1);
            if ((dx * dx + dy * dy) > (objspace_flatness_squared))
            {
                stbtt__tesselate_curve(points, num_points, (float)(x0), (float)(y0), (float)((x0 + x1) / 2.0f),
                    (float)((y0 + y1) / 2.0f), (float)(mx), (float)(my), (float)(objspace_flatness_squared),
                    (int)(n + 1));
                stbtt__tesselate_curve(points, num_points, (float)(mx), (float)(my), (float)((x1 + x2) / 2.0f),
                    (float)((y1 + y2) / 2.0f), (float)(x2), (float)(y2), (float)(objspace_flatness_squared),
                    (int)(n + 1));
            }
            else
            {
                stbtt__add_point(points, (int)(*num_points), (float)(x2), (float)(y2));
                *num_points = (int)(*num_points + 1);
            }

            return (int)(1);
        }

        public static void stbtt__tesselate_cubic(stbtt__point* points, int* num_points, float x0, float y0, float x1,
            float y1, float x2, float y2, float x3, float y3, float objspace_flatness_squared, int n)
        {
            float dx0 = (float)(x1 - x0);
            float dy0 = (float)(y1 - y0);
            float dx1 = (float)(x2 - x1);
            float dy1 = (float)(y2 - y1);
            float dx2 = (float)(x3 - x2);
            float dy2 = (float)(y3 - y2);
            float dx = (float)(x3 - x0);
            float dy = (float)(y3 - y0);
            float longlen = (float)(CRuntime.sqrt((double)(dx0 * dx0 + dy0 * dy0)) +
                                     CRuntime.sqrt((double)(dx1 * dx1 + dy1 * dy1)) +
                                     CRuntime.sqrt((double)(dx2 * dx2 + dy2 * dy2)));
            float shortlen = (float)(CRuntime.sqrt((double)(dx * dx + dy * dy)));
            float flatness_squared = (float)(longlen * longlen - shortlen * shortlen);
            if ((n) > (16))
                return;
            if ((flatness_squared) > (objspace_flatness_squared))
            {
                float x01 = (float)((x0 + x1) / 2);
                float y01 = (float)((y0 + y1) / 2);
                float x12 = (float)((x1 + x2) / 2);
                float y12 = (float)((y1 + y2) / 2);
                float x23 = (float)((x2 + x3) / 2);
                float y23 = (float)((y2 + y3) / 2);
                float xa = (float)((x01 + x12) / 2);
                float ya = (float)((y01 + y12) / 2);
                float xb = (float)((x12 + x23) / 2);
                float yb = (float)((y12 + y23) / 2);
                float mx = (float)((xa + xb) / 2);
                float my = (float)((ya + yb) / 2);
                stbtt__tesselate_cubic(points, num_points, (float)(x0), (float)(y0), (float)(x01), (float)(y01),
                    (float)(xa), (float)(ya), (float)(mx), (float)(my), (float)(objspace_flatness_squared),
                    (int)(n + 1));
                stbtt__tesselate_cubic(points, num_points, (float)(mx), (float)(my), (float)(xb), (float)(yb),
                    (float)(x23), (float)(y23), (float)(x3), (float)(y3), (float)(objspace_flatness_squared),
                    (int)(n + 1));
            }
            else
            {
                stbtt__add_point(points, (int)(*num_points), (float)(x3), (float)(y3));
                *num_points = (int)(*num_points + 1);
            }
        }

        public static stbtt__point* stbtt_FlattenCurves(stbtt_vertex* vertices, int num_verts, float objspace_flatness,
            int** contour_lengths, int* num_contours)
        {
            stbtt__point* points = null;
            int num_points = (int)(0);
            float objspace_flatness_squared = (float)(objspace_flatness * objspace_flatness);
            int i = 0;
            int n = (int)(0);
            int start = (int)(0);
            int pass = 0;
            for (i = (int)(0); (i) < (num_verts); ++i)
            {
                if ((vertices[i].type) == (STBTT_vmove))
                    ++n;
            }

            *num_contours = (int)(n);
            if ((n) == (0))
                return null;
            *contour_lengths = (int*)(CRuntime.malloc((ulong)(sizeof(int) * n)));
            if ((*contour_lengths) == (null))
            {
                *num_contours = (int)(0);
                return null;
            }

            for (pass = (int)(0); (pass) < (2); ++pass)
            {
                float x = (float)(0);
                float y = (float)(0);
                if ((pass) == (1))
                {
                    points = (stbtt__point*)(CRuntime.malloc((ulong)(num_points * sizeof(stbtt__point))));
                    if ((points) == (null))
                        goto error;
                }

                num_points = (int)(0);
                n = (int)(-1);
                for (i = (int)(0); (i) < (num_verts); ++i)
                {
                    switch (vertices[i].type)
                    {
                        case STBTT_vmove:
                            if ((n) >= (0))
                                (*contour_lengths)[n] = (int)(num_points - start);
                            ++n;
                            start = (int)(num_points);
                            x = (float)(vertices[i].x);
                            y = (float)(vertices[i].y);
                            stbtt__add_point(points, (int)(num_points++), (float)(x), (float)(y));
                            break;
                        case STBTT_vline:
                            x = (float)(vertices[i].x);
                            y = (float)(vertices[i].y);
                            stbtt__add_point(points, (int)(num_points++), (float)(x), (float)(y));
                            break;
                        case STBTT_vcurve:
                            stbtt__tesselate_curve(points, &num_points, (float)(x), (float)(y),
                                (float)(vertices[i].cx), (float)(vertices[i].cy), (float)(vertices[i].x),
                                (float)(vertices[i].y), (float)(objspace_flatness_squared), (int)(0));
                            x = (float)(vertices[i].x);
                            y = (float)(vertices[i].y);
                            break;
                        case STBTT_vcubic:
                            stbtt__tesselate_cubic(points, &num_points, (float)(x), (float)(y),
                                (float)(vertices[i].cx), (float)(vertices[i].cy), (float)(vertices[i].cx1),
                                (float)(vertices[i].cy1), (float)(vertices[i].x), (float)(vertices[i].y),
                                (float)(objspace_flatness_squared), (int)(0));
                            x = (float)(vertices[i].x);
                            y = (float)(vertices[i].y);
                            break;
                    }
                }

                (*contour_lengths)[n] = (int)(num_points - start);
            }

            return points;
        error:;
            CRuntime.free(points);
            CRuntime.free(*contour_lengths);
            *contour_lengths = null;
            *num_contours = (int)(0);
            return (null);
        }

        public static void stbtt_Rasterize(stbtt__bitmap* result, float flatness_in_pixels, stbtt_vertex* vertices,
            int num_verts, float scale_x, float scale_y, float shift_x, float shift_y, int x_off, int y_off, int invert)
        {
            float scale = (float)((scale_x) > (scale_y) ? scale_y : scale_x);
            int winding_count = (int)(0);
            int* winding_lengths = (null);
            stbtt__point* windings = stbtt_FlattenCurves(vertices, (int)(num_verts),
                (float)(flatness_in_pixels / scale), &winding_lengths, &winding_count);
            if ((windings) != null)
            {
                stbtt__rasterize(result, windings, winding_lengths, (int)(winding_count), (float)(scale_x),
                    (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(x_off), (int)(y_off),
                    (int)(invert));
                CRuntime.free(winding_lengths);
                CRuntime.free(windings);
            }
        }

        public static void stbtt_FreeBitmap(byte* bitmap)
        {
            CRuntime.free(bitmap);
        }

        public static byte* stbtt_GetGlyphBitmapSubpixel(stbtt_fontinfo info, float scale_x, float scale_y,
            float shift_x, float shift_y, int glyph, int* width, int* height, int* xoff, int* yoff)
        {
            int ix0 = 0;
            int iy0 = 0;
            int ix1 = 0;
            int iy1 = 0;
            stbtt__bitmap gbm = new stbtt__bitmap();
            stbtt_vertex* vertices;
            int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &vertices));
            if ((scale_x) == (0))
                scale_x = (float)(scale_y);
            if ((scale_y) == (0))
            {
                if ((scale_x) == (0))
                {
                    CRuntime.free(vertices);
                    return (null);
                }

                scale_y = (float)(scale_x);
            }

            stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale_x), (float)(scale_y),
                (float)(shift_x), (float)(shift_y), &ix0, &iy0, &ix1, &iy1);
            gbm.w = (int)(ix1 - ix0);
            gbm.h = (int)(iy1 - iy0);
            gbm.pixels = (null);
            if ((width) != null)
                *width = (int)(gbm.w);
            if ((height) != null)
                *height = (int)(gbm.h);
            if ((xoff) != null)
                *xoff = (int)(ix0);
            if ((yoff) != null)
                *yoff = (int)(iy0);
            if (((gbm.w) != 0) && ((gbm.h) != 0))
            {
                gbm.pixels = (byte*)(CRuntime.malloc((ulong)(gbm.w * gbm.h)));
                if ((gbm.pixels) != null)
                {
                    gbm.stride = (int)(gbm.w);
                    stbtt_Rasterize(&gbm, (float)(0.35f), vertices, (int)(num_verts), (float)(scale_x),
                        (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(ix0), (int)(iy0), (int)(1));
                }
            }

            CRuntime.free(vertices);
            return gbm.pixels;
        }

        public static byte* stbtt_GetGlyphBitmap(stbtt_fontinfo info, float scale_x, float scale_y, int glyph,
            int* width, int* height, int* xoff, int* yoff)
        {
            return stbtt_GetGlyphBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(0.0f),
                (float)(0.0f), (int)(glyph), width, height, xoff, yoff);
        }

        public static void stbtt_MakeGlyphBitmapSubpixel(stbtt_fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph)
        {
            int ix0 = 0;
            int iy0 = 0;
            stbtt_vertex* vertices;
            int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &vertices));
            stbtt__bitmap gbm = new stbtt__bitmap();
            stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale_x), (float)(scale_y),
                (float)(shift_x), (float)(shift_y), &ix0, &iy0, null, null);
            gbm.pixels = output;
            gbm.w = (int)(out_w);
            gbm.h = (int)(out_h);
            gbm.stride = (int)(out_stride);
            if (((gbm.w) != 0) && ((gbm.h) != 0))
                stbtt_Rasterize(&gbm, (float)(0.35f), vertices, (int)(num_verts), (float)(scale_x),
                    (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(ix0), (int)(iy0), (int)(1));
            CRuntime.free(vertices);
        }

        public static void stbtt_MakeGlyphBitmap(stbtt_fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, int glyph)
        {
            stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride),
                (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(glyph));
        }

        public static byte* stbtt_GetCodepointBitmapSubpixel(stbtt_fontinfo info, float scale_x, float scale_y,
            float shift_x, float shift_y, int codepoint, int* width, int* height, int* xoff, int* yoff)
        {
            return stbtt_GetGlyphBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(shift_x),
                (float)(shift_y), (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), width, height, xoff, yoff);
        }

        public static void stbtt_MakeCodepointBitmapSubpixelPrefilter(stbtt_fontinfo info, byte* output, int out_w,
            int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int oversample_x,
            int oversample_y, float* sub_x, float* sub_y, int codepoint)
        {
            stbtt_MakeGlyphBitmapSubpixelPrefilter(info, output, (int)(out_w), (int)(out_h), (int)(out_stride),
                (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(oversample_x),
                (int)(oversample_y), sub_x, sub_y, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))));
        }

        public static void stbtt_MakeCodepointBitmapSubpixel(stbtt_fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint)
        {
            stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride),
                (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y),
                (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))));
        }

        public static byte* stbtt_GetCodepointBitmap(stbtt_fontinfo info, float scale_x, float scale_y, int codepoint,
            int* width, int* height, int* xoff, int* yoff)
        {
            return stbtt_GetCodepointBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(0.0f),
                (float)(0.0f), (int)(codepoint), width, height, xoff, yoff);
        }

        public static void stbtt_MakeCodepointBitmap(stbtt_fontinfo info, byte* output, int out_w, int out_h,
            int out_stride, float scale_x, float scale_y, int codepoint)
        {
            stbtt_MakeCodepointBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride),
                (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(codepoint));
        }

        public static int stbtt_BakeFontBitmap_internal(byte* data, int offset, float pixel_height, byte* pixels,
            int pw, int ph, int first_char, int num_chars, stbtt_bakedchar* chardata)
        {
            float scale = 0;
            int x = 0;
            int y = 0;
            int bottom_y = 0;
            int i = 0;
            stbtt_fontinfo f = new stbtt_fontinfo();
            if (stbtt_InitFont(f, data, (int)(offset)) == 0)
                return (int)(-1);
            CRuntime.memset(pixels, (int)(0), (ulong)(pw * ph));
            x = (int)(y = (int)(1));
            bottom_y = (int)(1);
            scale = (float)(stbtt_ScaleForPixelHeight(f, (float)(pixel_height)));
            for (i = (int)(0); (i) < (num_chars); ++i)
            {
                int advance = 0;
                int lsb = 0;
                int x0 = 0;
                int y0 = 0;
                int x1 = 0;
                int y1 = 0;
                int gw = 0;
                int gh = 0;
                int g = (int)(stbtt_FindGlyphIndex(f, (int)(first_char + i)));
                stbtt_GetGlyphHMetrics(f, (int)(g), &advance, &lsb);
                stbtt_GetGlyphBitmapBox(f, (int)(g), (float)(scale), (float)(scale), &x0, &y0, &x1, &y1);
                gw = (int)(x1 - x0);
                gh = (int)(y1 - y0);
                if ((x + gw + 1) >= (pw))
                {
                    y = (int)(bottom_y);
                    x = (int)(1);
                }

                if ((y + gh + 1) >= (ph))
                    return (int)(-i);
                stbtt_MakeGlyphBitmap(f, pixels + x + y * pw, (int)(gw), (int)(gh), (int)(pw), (float)(scale),
                    (float)(scale), (int)(g));
                chardata[i].x0 = (ushort)((short)(x));
                chardata[i].y0 = (ushort)((short)(y));
                chardata[i].x1 = (ushort)((short)(x + gw));
                chardata[i].y1 = (ushort)((short)(y + gh));
                chardata[i].xadvance = (float)(scale * advance);
                chardata[i].xoff = ((float)(x0));
                chardata[i].yoff = ((float)(y0));
                x = (int)(x + gw + 1);
                if ((y + gh + 1) > (bottom_y))
                    bottom_y = (int)(y + gh + 1);
            }

            return (int)(bottom_y);
        }

        public static void stbtt_GetBakedQuad(stbtt_bakedchar* chardata, int pw, int ph, int char_index, float* xpos,
            float* ypos, stbtt_aligned_quad* q, int opengl_fillrule)
        {
            float d3d_bias = (float)((opengl_fillrule) != 0 ? 0 : -0.5f);
            float ipw = (float)(1.0f / pw);
            float iph = (float)(1.0f / ph);
            stbtt_bakedchar* b = chardata + char_index;
            int round_x = ((int)(CRuntime.floor((double)((*xpos + b->xoff) + 0.5f))));
            int round_y = ((int)(CRuntime.floor((double)((*ypos + b->yoff) + 0.5f))));
            q->x0 = (float)(round_x + d3d_bias);
            q->y0 = (float)(round_y + d3d_bias);
            q->x1 = (float)(round_x + b->x1 - b->x0 + d3d_bias);
            q->y1 = (float)(round_y + b->y1 - b->y0 + d3d_bias);
            q->s0 = (float)(b->x0 * ipw);
            q->t0 = (float)(b->y0 * iph);
            q->s1 = (float)(b->x1 * ipw);
            q->t1 = (float)(b->y1 * iph);
            *xpos += (float)(b->xadvance);
        }

        public static void stbrp_init_target(stbrp_context* con, int pw, int ph, stbrp_node* nodes, int num_nodes)
        {
            con->width = (int)(pw);
            con->height = (int)(ph);
            con->x = (int)(0);
            con->y = (int)(0);
            con->bottom_y = (int)(0);
        }

        public static void stbrp_pack_rects(stbrp_context* con, stbrp_rect* rects, int num_rects)
        {
            int i = 0;
            for (i = (int)(0); (i) < (num_rects); ++i)
            {
                if ((con->x + rects[i].w) > (con->width))
                {
                    con->x = (int)(0);
                    con->y = (int)(con->bottom_y);
                }

                if ((con->y + rects[i].h) > (con->height))
                    break;
                rects[i].x = (int)(con->x);
                rects[i].y = (int)(con->y);
                rects[i].was_packed = (int)(1);
                con->x += (int)(rects[i].w);
                if ((con->y + rects[i].h) > (con->bottom_y))
                    con->bottom_y = (int)(con->y + rects[i].h);
            }

            for (; (i) < (num_rects); ++i)
            {
                rects[i].was_packed = (int)(0);
            }
        }

        public static int stbtt_PackBegin(stbtt_pack_context spc, byte* pixels, int pw, int ph, int stride_in_bytes,
            int padding, void* alloc_context)
        {
            stbrp_context* context = (stbrp_context*)(CRuntime.malloc((ulong)(sizeof(stbrp_context))));
            int num_nodes = (int)(pw - padding);
            stbrp_node* nodes = (stbrp_node*)(CRuntime.malloc((ulong)(sizeof(stbrp_node) * num_nodes)));
            if (((context) == (null)) || ((nodes) == (null)))
            {
                if (context != (null))
                    CRuntime.free(context);
                if (nodes != (null))
                    CRuntime.free(nodes);
                return (int)(0);
            }

            spc.user_allocator_context = alloc_context;
            spc.width = (int)(pw);
            spc.height = (int)(ph);
            spc.pixels = pixels;
            spc.pack_info = context;
            spc.nodes = nodes;
            spc.padding = (int)(padding);
            spc.stride_in_bytes = (int)(stride_in_bytes != 0 ? stride_in_bytes : pw);
            spc.h_oversample = (uint)(1);
            spc.v_oversample = (uint)(1);
            spc.skip_missing = (int)(0);
            stbrp_init_target(context, (int)(pw - padding), (int)(ph - padding), nodes, (int)(num_nodes));
            if ((pixels) != null)
                CRuntime.memset(pixels, (int)(0), (ulong)(pw * ph));
            return (int)(1);
        }

        public static void stbtt_PackEnd(stbtt_pack_context spc)
        {
            CRuntime.free(spc.nodes);
            CRuntime.free(spc.pack_info);
        }

        public static void stbtt_PackSetOversampling(stbtt_pack_context spc, uint h_oversample, uint v_oversample)
        {
            if (h_oversample <= 8)
                spc.h_oversample = (uint)(h_oversample);
            if (v_oversample <= 8)
                spc.v_oversample = (uint)(v_oversample);
        }

        public static void stbtt_PackSetSkipMissingCodepoints(stbtt_pack_context spc, int skip)
        {
            spc.skip_missing = (int)(skip);
        }

        public static void stbtt__h_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
        {
            byte* buffer = stackalloc byte[8];
            int safe_w = (int)(w - kernel_width);
            int j = 0;
            CRuntime.memset(buffer, (int)(0), (ulong)(8));
            for (j = (int)(0); (j) < (h); ++j)
            {
                int i = 0;
                uint total = 0;
                CRuntime.memset(buffer, (int)(0), (ulong)(kernel_width));
                total = (uint)(0);
                switch (kernel_width)
                {
                    case 2:
                        for (i = (int)(0); i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i]);
                            pixels[i] = ((byte)(total / 2));
                        }

                        break;
                    case 3:
                        for (i = (int)(0); i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i]);
                            pixels[i] = ((byte)(total / 3));
                        }

                        break;
                    case 4:
                        for (i = (int)(0); i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i]);
                            pixels[i] = ((byte)(total / 4));
                        }

                        break;
                    case 5:
                        for (i = (int)(0); i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i]);
                            pixels[i] = ((byte)(total / 5));
                        }

                        break;
                    default:
                        for (i = (int)(0); i <= safe_w; ++i)
                        {
                            total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i]);
                            pixels[i] = ((byte)(total / kernel_width));
                        }

                        break;
                }

                for (; (i) < (w); ++i)
                {
                    total -= (uint)(buffer[i & (8 - 1)]);
                    pixels[i] = ((byte)(total / kernel_width));
                }

                pixels += stride_in_bytes;
            }
        }

        public static void stbtt__v_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
        {
            byte* buffer = stackalloc byte[8];
            int safe_h = (int)(h - kernel_width);
            int j = 0;
            CRuntime.memset(buffer, (int)(0), (ulong)(8));
            for (j = (int)(0); (j) < (w); ++j)
            {
                int i = 0;
                uint total = 0;
                CRuntime.memset(buffer, (int)(0), (ulong)(kernel_width));
                total = (uint)(0);
                switch (kernel_width)
                {
                    case 2:
                        for (i = (int)(0); i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i * stride_in_bytes]);
                            pixels[i * stride_in_bytes] = ((byte)(total / 2));
                        }

                        break;
                    case 3:
                        for (i = (int)(0); i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i * stride_in_bytes]);
                            pixels[i * stride_in_bytes] = ((byte)(total / 3));
                        }

                        break;
                    case 4:
                        for (i = (int)(0); i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i * stride_in_bytes]);
                            pixels[i * stride_in_bytes] = ((byte)(total / 4));
                        }

                        break;
                    case 5:
                        for (i = (int)(0); i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i * stride_in_bytes]);
                            pixels[i * stride_in_bytes] = ((byte)(total / 5));
                        }

                        break;
                    default:
                        for (i = (int)(0); i <= safe_h; ++i)
                        {
                            total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
                            buffer[(i + kernel_width) & (8 - 1)] = (byte)(pixels[i * stride_in_bytes]);
                            pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
                        }

                        break;
                }

                for (; (i) < (h); ++i)
                {
                    total -= (uint)(buffer[i & (8 - 1)]);
                    pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
                }

                pixels += 1;
            }
        }

        public static float stbtt__oversample_shift(int oversample)
        {
            if (oversample == 0)
                return (float)(0.0f);
            return (float)((float)(-(oversample - 1)) / (2.0f * (float)(oversample)));
        }

        public static int stbtt_PackFontRangesGatherRects(stbtt_pack_context spc, stbtt_fontinfo info,
            stbtt_pack_range* ranges, int num_ranges, stbrp_rect* rects)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            k = (int)(0);
            for (i = (int)(0); (i) < (num_ranges); ++i)
            {
                float fh = (float)(ranges[i].font_size);
                float scale = (float)((fh) > (0)
                    ? stbtt_ScaleForPixelHeight(info, (float)(fh))
                    : stbtt_ScaleForMappingEmToPixels(info, (float)(-fh)));
                ranges[i].h_oversample = ((byte)(spc.h_oversample));
                ranges[i].v_oversample = ((byte)(spc.v_oversample));
                for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
                {
                    int x0 = 0;
                    int y0 = 0;
                    int x1 = 0;
                    int y1 = 0;
                    int codepoint = (int)((ranges[i].array_of_unicode_codepoints) == (null)
                        ? ranges[i].first_unicode_codepoint_in_range + j
                        : ranges[i].array_of_unicode_codepoints[j]);
                    int glyph = (int)(stbtt_FindGlyphIndex(info, (int)(codepoint)));
                    if (((glyph) == (0)) && ((spc.skip_missing) != 0))
                    {
                        rects[k].w = (int)(rects[k].h = (int)(0));
                    }
                    else
                    {
                        stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale * spc.h_oversample),
                            (float)(scale * spc.v_oversample), (float)(0), (float)(0), &x0, &y0, &x1, &y1);
                        rects[k].w = ((int)(x1 - x0 + spc.padding + spc.h_oversample - 1));
                        rects[k].h = ((int)(y1 - y0 + spc.padding + spc.v_oversample - 1));
                    }

                    ++k;
                }
            }

            return (int)(k);
        }

        public static void stbtt_MakeGlyphBitmapSubpixelPrefilter(stbtt_fontinfo info, byte* output, int out_w,
            int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int prefilter_x,
            int prefilter_y, float* sub_x, float* sub_y, int glyph)
        {
            stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w - (prefilter_x - 1)),
                (int)(out_h - (prefilter_y - 1)), (int)(out_stride), (float)(scale_x), (float)(scale_y),
                (float)(shift_x), (float)(shift_y), (int)(glyph));
            if ((prefilter_x) > (1))
                stbtt__h_prefilter(output, (int)(out_w), (int)(out_h), (int)(out_stride), (uint)(prefilter_x));
            if ((prefilter_y) > (1))
                stbtt__v_prefilter(output, (int)(out_w), (int)(out_h), (int)(out_stride), (uint)(prefilter_y));
            *sub_x = (float)(stbtt__oversample_shift((int)(prefilter_x)));
            *sub_y = (float)(stbtt__oversample_shift((int)(prefilter_y)));
        }

        public static int stbtt_PackFontRangesRenderIntoRects(stbtt_pack_context spc, stbtt_fontinfo info,
            stbtt_pack_range* ranges, int num_ranges, stbrp_rect* rects)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int return_value = (int)(1);
            int old_h_over = (int)(spc.h_oversample);
            int old_v_over = (int)(spc.v_oversample);
            k = (int)(0);
            for (i = (int)(0); (i) < (num_ranges); ++i)
            {
                float fh = (float)(ranges[i].font_size);
                float scale = (float)((fh) > (0)
                    ? stbtt_ScaleForPixelHeight(info, (float)(fh))
                    : stbtt_ScaleForMappingEmToPixels(info, (float)(-fh)));
                float recip_h = 0;
                float recip_v = 0;
                float sub_x = 0;
                float sub_y = 0;
                spc.h_oversample = (uint)(ranges[i].h_oversample);
                spc.v_oversample = (uint)(ranges[i].v_oversample);
                recip_h = (float)(1.0f / spc.h_oversample);
                recip_v = (float)(1.0f / spc.v_oversample);
                sub_x = (float)(stbtt__oversample_shift((int)(spc.h_oversample)));
                sub_y = (float)(stbtt__oversample_shift((int)(spc.v_oversample)));
                for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
                {
                    stbrp_rect* r = &rects[k];
                    if ((((r->was_packed) != 0) && (r->w != 0)) && (r->h != 0))
                    {
                        stbtt_packedchar* bc = &ranges[i].chardata_for_range[j];
                        int advance = 0;
                        int lsb = 0;
                        int x0 = 0;
                        int y0 = 0;
                        int x1 = 0;
                        int y1 = 0;
                        int codepoint = (int)((ranges[i].array_of_unicode_codepoints) == (null)
                            ? ranges[i].first_unicode_codepoint_in_range + j
                            : ranges[i].array_of_unicode_codepoints[j]);
                        int glyph = (int)(stbtt_FindGlyphIndex(info, (int)(codepoint)));
                        int pad = (int)(spc.padding);
                        r->x += (int)(pad);
                        r->y += (int)(pad);
                        r->w -= (int)(pad);
                        r->h -= (int)(pad);
                        stbtt_GetGlyphHMetrics(info, (int)(glyph), &advance, &lsb);
                        stbtt_GetGlyphBitmapBox(info, (int)(glyph), (float)(scale * spc.h_oversample),
                            (float)(scale * spc.v_oversample), &x0, &y0, &x1, &y1);
                        stbtt_MakeGlyphBitmapSubpixel(info, spc.pixels + r->x + r->y * spc.stride_in_bytes,
                            (int)(r->w - spc.h_oversample + 1), (int)(r->h - spc.v_oversample + 1),
                            (int)(spc.stride_in_bytes), (float)(scale * spc.h_oversample),
                            (float)(scale * spc.v_oversample), (float)(0), (float)(0), (int)(glyph));
                        if ((spc.h_oversample) > (1))
                            stbtt__h_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, (int)(r->w),
                                (int)(r->h), (int)(spc.stride_in_bytes), (uint)(spc.h_oversample));
                        if ((spc.v_oversample) > (1))
                            stbtt__v_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, (int)(r->w),
                                (int)(r->h), (int)(spc.stride_in_bytes), (uint)(spc.v_oversample));
                        bc->x0 = (ushort)((short)(r->x));
                        bc->y0 = (ushort)((short)(r->y));
                        bc->x1 = (ushort)((short)(r->x + r->w));
                        bc->y1 = (ushort)((short)(r->y + r->h));
                        bc->xadvance = (float)(scale * advance);
                        bc->xoff = (float)((float)(x0) * recip_h + sub_x);
                        bc->yoff = (float)((float)(y0) * recip_v + sub_y);
                        bc->xoff2 = (float)((x0 + r->w) * recip_h + sub_x);
                        bc->yoff2 = (float)((y0 + r->h) * recip_v + sub_y);
                    }
                    else
                    {
                        return_value = (int)(0);
                    }

                    ++k;
                }
            }

            spc.h_oversample = (uint)(old_h_over);
            spc.v_oversample = (uint)(old_v_over);
            return (int)(return_value);
        }

        public static void stbtt_PackFontRangesPackRects(stbtt_pack_context spc, stbrp_rect* rects, int num_rects)
        {
            stbrp_pack_rects((stbrp_context*)(spc.pack_info), rects, (int)(num_rects));
        }

        public static int stbtt_PackFontRanges(stbtt_pack_context spc, byte* fontdata, int font_index,
            stbtt_pack_range* ranges, int num_ranges)
        {
            stbtt_fontinfo info = new stbtt_fontinfo();
            int i = 0;
            int j = 0;
            int n = 0;
            int return_value = (int)(1);
            stbrp_rect* rects;
            for (i = (int)(0); (i) < (num_ranges); ++i)
            {
                for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
                {
                    ranges[i].chardata_for_range[j].x0 = (ushort)(ranges[i].chardata_for_range[j].y0 =
                        (ushort)(ranges[i].chardata_for_range[j].x1 =
                            (ushort)(ranges[i].chardata_for_range[j].y1 = (ushort)(0))));
                }
            }

            n = (int)(0);
            for (i = (int)(0); (i) < (num_ranges); ++i)
            {
                n += (int)(ranges[i].num_chars);
            }

            rects = (stbrp_rect*)(CRuntime.malloc((ulong)(sizeof(stbrp_rect) * n)));
            if ((rects) == (null))
                return (int)(0);
            stbtt_InitFont(info, fontdata, (int)(stbtt_GetFontOffsetForIndex(fontdata, (int)(font_index))));
            n = (int)(stbtt_PackFontRangesGatherRects(spc, info, ranges, (int)(num_ranges), rects));
            stbtt_PackFontRangesPackRects(spc, rects, (int)(n));
            return_value = (int)(stbtt_PackFontRangesRenderIntoRects(spc, info, ranges, (int)(num_ranges), rects));
            CRuntime.free(rects);
            return (int)(return_value);
        }

        public static int stbtt_PackFontRange(stbtt_pack_context spc, byte* fontdata, int font_index, float font_size,
            int first_unicode_codepoint_in_range, int num_chars_in_range, stbtt_packedchar* chardata_for_range)
        {
            stbtt_pack_range range = new stbtt_pack_range();
            range.first_unicode_codepoint_in_range = (int)(first_unicode_codepoint_in_range);
            range.array_of_unicode_codepoints = (null);
            range.num_chars = (int)(num_chars_in_range);
            range.chardata_for_range = chardata_for_range;
            range.font_size = (float)(font_size);
            return (int)(stbtt_PackFontRanges(spc, fontdata, (int)(font_index), &range, (int)(1)));
        }

        public static void stbtt_GetScaledFontVMetrics(byte* fontdata, int index, float size, float* ascent,
            float* descent, float* lineGap)
        {
            int i_ascent = 0;
            int i_descent = 0;
            int i_lineGap = 0;
            float scale = 0;
            stbtt_fontinfo info = new stbtt_fontinfo();
            stbtt_InitFont(info, fontdata, (int)(stbtt_GetFontOffsetForIndex(fontdata, (int)(index))));
            scale = (float)((size) > (0)
                ? stbtt_ScaleForPixelHeight(info, (float)(size))
                : stbtt_ScaleForMappingEmToPixels(info, (float)(-size)));
            stbtt_GetFontVMetrics(info, &i_ascent, &i_descent, &i_lineGap);
            *ascent = (float)((float)(i_ascent) * scale);
            *descent = (float)((float)(i_descent) * scale);
            *lineGap = (float)((float)(i_lineGap) * scale);
        }

        public static void stbtt_GetPackedQuad(stbtt_packedchar* chardata, int pw, int ph, int char_index, float* xpos,
            float* ypos, stbtt_aligned_quad* q, int align_to_integer)
        {
            float ipw = (float)(1.0f / pw);
            float iph = (float)(1.0f / ph);
            stbtt_packedchar* b = chardata + char_index;
            if ((align_to_integer) != 0)
            {
                float x = (float)((int)(CRuntime.floor((double)((*xpos + b->xoff) + 0.5f))));
                float y = (float)((int)(CRuntime.floor((double)((*ypos + b->yoff) + 0.5f))));
                q->x0 = (float)(x);
                q->y0 = (float)(y);
                q->x1 = (float)(x + b->xoff2 - b->xoff);
                q->y1 = (float)(y + b->yoff2 - b->yoff);
            }
            else
            {
                q->x0 = (float)(*xpos + b->xoff);
                q->y0 = (float)(*ypos + b->yoff);
                q->x1 = (float)(*xpos + b->xoff2);
                q->y1 = (float)(*ypos + b->yoff2);
            }

            q->s0 = (float)(b->x0 * ipw);
            q->t0 = (float)(b->y0 * iph);
            q->s1 = (float)(b->x1 * ipw);
            q->t1 = (float)(b->y1 * iph);
            *xpos += (float)(b->xadvance);
        }

        public static int stbtt__ray_intersect_bezier(float* orig, float* ray, float* q0, float* q1, float* q2,
            float* hits)
        {
            float q0perp = (float)(q0[1] * ray[0] - q0[0] * ray[1]);
            float q1perp = (float)(q1[1] * ray[0] - q1[0] * ray[1]);
            float q2perp = (float)(q2[1] * ray[0] - q2[0] * ray[1]);
            float roperp = (float)(orig[1] * ray[0] - orig[0] * ray[1]);
            float a = (float)(q0perp - 2 * q1perp + q2perp);
            float b = (float)(q1perp - q0perp);
            float c = (float)(q0perp - roperp);
            float s0 = (float)(0);
            float s1 = (float)(0);
            int num_s = (int)(0);
            if (a != 0.0)
            {
                float discr = (float)(b * b - a * c);
                if ((discr) > (0.0))
                {
                    float rcpna = (float)(-1 / a);
                    float d = (float)(CRuntime.sqrt((double)(discr)));
                    s0 = (float)((b + d) * rcpna);
                    s1 = (float)((b - d) * rcpna);
                    if (((s0) >= (0.0)) && (s0 <= 1.0))
                        num_s = (int)(1);
                    if ((((d) > (0.0)) && ((s1) >= (0.0))) && (s1 <= 1.0))
                    {
                        if ((num_s) == (0))
                            s0 = (float)(s1);
                        ++num_s;
                    }
                }
            }
            else
            {
                s0 = (float)(c / (-2 * b));
                if (((s0) >= (0.0)) && (s0 <= 1.0))
                    num_s = (int)(1);
            }

            if ((num_s) == (0))
                return (int)(0);
            else
            {
                float rcp_len2 = (float)(1 / (ray[0] * ray[0] + ray[1] * ray[1]));
                float rayn_x = (float)(ray[0] * rcp_len2);
                float rayn_y = (float)(ray[1] * rcp_len2);
                float q0d = (float)(q0[0] * rayn_x + q0[1] * rayn_y);
                float q1d = (float)(q1[0] * rayn_x + q1[1] * rayn_y);
                float q2d = (float)(q2[0] * rayn_x + q2[1] * rayn_y);
                float rod = (float)(orig[0] * rayn_x + orig[1] * rayn_y);
                float q10d = (float)(q1d - q0d);
                float q20d = (float)(q2d - q0d);
                float q0rd = (float)(q0d - rod);
                hits[0] = (float)(q0rd + s0 * (2.0f - 2.0f * s0) * q10d + s0 * s0 * q20d);
                hits[1] = (float)(a * s0 + b);
                if ((num_s) > (1))
                {
                    hits[2] = (float)(q0rd + s1 * (2.0f - 2.0f * s1) * q10d + s1 * s1 * q20d);
                    hits[3] = (float)(a * s1 + b);
                    return (int)(2);
                }
                else
                {
                    return (int)(1);
                }
            }
        }

        public static int equal(float* a, float* b)
        {
            return (int)(((a[0] == b[0]) && (a[1] == b[1])) ? 1 : 0);
        }

        public static int stbtt__compute_crossings_x(float x, float y, int nverts, stbtt_vertex* verts)
        {
            int i = 0;
            float* orig = stackalloc float[2];
            float* ray = stackalloc float[2];
            ray[0] = (float)(1);
            ray[1] = (float)(0);

            float y_frac = 0;
            int winding = (int)(0);
            orig[0] = (float)(x);
            orig[1] = (float)(y);
            y_frac = ((float)(CRuntime.fmod((double)(y), (double)(1.0f))));
            if ((y_frac) < (0.01f))
                y += (float)(0.01f);
            else if ((y_frac) > (0.99f))
                y -= (float)(0.01f);
            orig[1] = (float)(y);
            for (i = (int)(0); (i) < (nverts); ++i)
            {
                if ((verts[i].type) == (STBTT_vline))
                {
                    int x0 = (int)(verts[i - 1].x);
                    int y0 = (int)(verts[i - 1].y);
                    int x1 = (int)(verts[i].x);
                    int y1 = (int)(verts[i].y);
                    if ((((y) > ((y0) < (y1) ? (y0) : (y1))) && ((y) < ((y0) < (y1) ? (y1) : (y0)))) &&
                        ((x) > ((x0) < (x1) ? (x0) : (x1))))
                    {
                        float x_inter = (float)((y - y0) / (y1 - y0) * (x1 - x0) + x0);
                        if ((x_inter) < (x))
                            winding += (int)(((y0) < (y1)) ? 1 : -1);
                    }
                }

                if ((verts[i].type) == (STBTT_vcurve))
                {
                    int x0 = (int)(verts[i - 1].x);
                    int y0 = (int)(verts[i - 1].y);
                    int x1 = (int)(verts[i].cx);
                    int y1 = (int)(verts[i].cy);
                    int x2 = (int)(verts[i].x);
                    int y2 = (int)(verts[i].y);
                    int ax = (int)((x0) < ((x1) < (x2) ? (x1) : (x2)) ? (x0) : ((x1) < (x2) ? (x1) : (x2)));
                    int ay = (int)((y0) < ((y1) < (y2) ? (y1) : (y2)) ? (y0) : ((y1) < (y2) ? (y1) : (y2)));
                    int by = (int)((y0) < ((y1) < (y2) ? (y2) : (y1)) ? ((y1) < (y2) ? (y2) : (y1)) : (y0));
                    if ((((y) > (ay)) && ((y) < (by))) && ((x) > (ax)))
                    {
                        float* q0 = stackalloc float[2];
                        float* q1 = stackalloc float[2];
                        float* q2 = stackalloc float[2];
                        float* hits = stackalloc float[4];
                        q0[0] = ((float)(x0));
                        q0[1] = ((float)(y0));
                        q1[0] = ((float)(x1));
                        q1[1] = ((float)(y1));
                        q2[0] = ((float)(x2));
                        q2[1] = ((float)(y2));
                        if (((equal(q0, q1)) != 0) || ((equal(q1, q2)) != 0))
                        {
                            x0 = ((int)(verts[i - 1].x));
                            y0 = ((int)(verts[i - 1].y));
                            x1 = ((int)(verts[i].x));
                            y1 = ((int)(verts[i].y));
                            if ((((y) > ((y0) < (y1) ? (y0) : (y1))) && ((y) < ((y0) < (y1) ? (y1) : (y0)))) &&
                                ((x) > ((x0) < (x1) ? (x0) : (x1))))
                            {
                                float x_inter = (float)((y - y0) / (y1 - y0) * (x1 - x0) + x0);
                                if ((x_inter) < (x))
                                    winding += (int)(((y0) < (y1)) ? 1 : -1);
                            }
                        }
                        else
                        {
                            int num_hits = (int)(stbtt__ray_intersect_bezier(orig, ray, q0, q1, q2, hits));
                            if ((num_hits) >= (1))
                                if ((hits[0]) < (0))
                                    winding += (int)((hits[1]) < (0) ? -1 : 1);
                            if ((num_hits) >= (2))
                                if ((hits[2]) < (0))
                                    winding += (int)((hits[3]) < (0) ? -1 : 1);
                        }
                    }
                }
            }

            return (int)(winding);
        }

        public static float stbtt__cuberoot(float x)
        {
            if ((x) < (0))
                return (float)(-(float)(CRuntime.pow((double)(-x), (double)(1.0f / 3.0f))));
            else
                return (float)(CRuntime.pow((double)(x), (double)(1.0f / 3.0f)));
        }

        public static int stbtt__solve_cubic(float a, float b, float c, float* r)
        {
            float s = (float)(-a / 3);
            float p = (float)(b - a * a / 3);
            float q = (float)(a * (2 * a * a - 9 * b) / 27 + c);
            float p3 = (float)(p * p * p);
            float d = (float)(q * q + 4 * p3 / 27);
            if ((d) >= (0))
            {
                float z = (float)(CRuntime.sqrt((double)(d)));
                float u = (float)((-q + z) / 2);
                float v = (float)((-q - z) / 2);
                u = (float)(stbtt__cuberoot((float)(u)));
                v = (float)(stbtt__cuberoot((float)(v)));
                r[0] = (float)(s + u + v);
                return (int)(1);
            }
            else
            {
                float u = (float)(CRuntime.sqrt((double)(-p / 3)));
                float v = (float)((float)(CRuntime.acos((double)(-CRuntime.sqrt((double)(-27 / p3)) * q / 2))) / 3);
                float m = (float)(CRuntime.cos((double)(v)));
                float n = (float)((float)(CRuntime.cos((double)(v - 3.141592 / 2))) * 1.732050808f);
                r[0] = (float)(s + u * 2 * m);
                r[1] = (float)(s - u * (m + n));
                r[2] = (float)(s - u * (m - n));
                return (int)(3);
            }
        }

        public static byte* stbtt_GetGlyphSDF(stbtt_fontinfo info, float scale, int glyph, int padding,
            byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
        {
            float scale_x = (float)(scale);
            float scale_y = (float)(scale);
            int ix0 = 0;
            int iy0 = 0;
            int ix1 = 0;
            int iy1 = 0;
            int w = 0;
            int h = 0;
            byte* data;
            if ((scale_x) == (0))
                scale_x = (float)(scale_y);
            if ((scale_y) == (0))
            {
                if ((scale_x) == (0))
                    return (null);
                scale_y = (float)(scale_x);
            }

            stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale), (float)(scale), (float)(0.0f),
                (float)(0.0f), &ix0, &iy0, &ix1, &iy1);
            if (((ix0) == (ix1)) || ((iy0) == (iy1)))
                return (null);
            ix0 -= (int)(padding);
            iy0 -= (int)(padding);
            ix1 += (int)(padding);
            iy1 += (int)(padding);
            w = (int)(ix1 - ix0);
            h = (int)(iy1 - iy0);
            if ((width) != null)
                *width = (int)(w);
            if ((height) != null)
                *height = (int)(h);
            if ((xoff) != null)
                *xoff = (int)(ix0);
            if ((yoff) != null)
                *yoff = (int)(iy0);
            scale_y = (float)(-scale_y);
            {
                int x = 0;
                int y = 0;
                int i = 0;
                int j = 0;
                float* precompute;
                stbtt_vertex* verts;
                int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &verts));
                data = (byte*)(CRuntime.malloc((ulong)(w * h)));
                precompute = (float*)(CRuntime.malloc((ulong)(num_verts * sizeof(float))));
                for (i = (int)(0), j = (int)(num_verts - 1); (i) < (num_verts); j = (int)(i++))
                {
                    if ((verts[i].type) == (STBTT_vline))
                    {
                        float x0 = (float)(verts[i].x * scale_x);
                        float y0 = (float)(verts[i].y * scale_y);
                        float x1 = (float)(verts[j].x * scale_x);
                        float y1 = (float)(verts[j].y * scale_y);
                        float dist = (float)(CRuntime.sqrt((double)((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0))));
                        precompute[i] = (float)(((dist) == (0)) ? 0.0f : 1.0f / dist);
                    }
                    else if ((verts[i].type) == (STBTT_vcurve))
                    {
                        float x2 = (float)(verts[j].x * scale_x);
                        float y2 = (float)(verts[j].y * scale_y);
                        float x1 = (float)(verts[i].cx * scale_x);
                        float y1 = (float)(verts[i].cy * scale_y);
                        float x0 = (float)(verts[i].x * scale_x);
                        float y0 = (float)(verts[i].y * scale_y);
                        float bx = (float)(x0 - 2 * x1 + x2);
                        float by = (float)(y0 - 2 * y1 + y2);
                        float len2 = (float)(bx * bx + by * by);
                        if (len2 != 0.0f)
                            precompute[i] = (float)(1.0f / (bx * bx + by * by));
                        else
                            precompute[i] = (float)(0.0f);
                    }
                    else
                        precompute[i] = (float)(0.0f);
                }

                for (y = (int)(iy0); (y) < (iy1); ++y)
                {
                    for (x = (int)(ix0); (x) < (ix1); ++x)
                    {
                        float val = 0;
                        float min_dist = (float)(999999.0f);
                        float sx = (float)((float)(x) + 0.5f);
                        float sy = (float)((float)(y) + 0.5f);
                        float x_gspace = (float)(sx / scale_x);
                        float y_gspace = (float)(sy / scale_y);
                        int winding = (int)(stbtt__compute_crossings_x((float)(x_gspace), (float)(y_gspace),
                            (int)(num_verts), verts));
                        for (i = (int)(0); (i) < (num_verts); ++i)
                        {
                            float x0 = (float)(verts[i].x * scale_x);
                            float y0 = (float)(verts[i].y * scale_y);
                            float dist2 = (float)((x0 - sx) * (x0 - sx) + (y0 - sy) * (y0 - sy));
                            if ((dist2) < (min_dist * min_dist))
                                min_dist = ((float)(CRuntime.sqrt((double)(dist2))));
                            if ((verts[i].type) == (STBTT_vline))
                            {
                                float x1 = (float)(verts[i - 1].x * scale_x);
                                float y1 = (float)(verts[i - 1].y * scale_y);
                                float dist =
                                    (float)((float)(CRuntime.fabs(
                                                 (double)((x1 - x0) * (y0 - sy) - (y1 - y0) * (x0 - sx)))) *
                                             precompute[i]);
                                if ((dist) < (min_dist))
                                {
                                    float dx = (float)(x1 - x0);
                                    float dy = (float)(y1 - y0);
                                    float px = (float)(x0 - sx);
                                    float py = (float)(y0 - sy);
                                    float t = (float)(-(px * dx + py * dy) / (dx * dx + dy * dy));
                                    if (((t) >= (0.0f)) && (t <= 1.0f))
                                        min_dist = (float)(dist);
                                }
                            }
                            else if ((verts[i].type) == (STBTT_vcurve))
                            {
                                float x2 = (float)(verts[i - 1].x * scale_x);
                                float y2 = (float)(verts[i - 1].y * scale_y);
                                float x1 = (float)(verts[i].cx * scale_x);
                                float y1 = (float)(verts[i].cy * scale_y);
                                float box_x0 = (float)(((x0) < (x1) ? (x0) : (x1)) < (x2)
                                    ? ((x0) < (x1) ? (x0) : (x1))
                                    : (x2));
                                float box_y0 = (float)(((y0) < (y1) ? (y0) : (y1)) < (y2)
                                    ? ((y0) < (y1) ? (y0) : (y1))
                                    : (y2));
                                float box_x1 = (float)(((x0) < (x1) ? (x1) : (x0)) < (x2)
                                    ? (x2)
                                    : ((x0) < (x1) ? (x1) : (x0)));
                                float box_y1 = (float)(((y0) < (y1) ? (y1) : (y0)) < (y2)
                                    ? (y2)
                                    : ((y0) < (y1) ? (y1) : (y0)));
                                if (((((sx) > (box_x0 - min_dist)) && ((sx) < (box_x1 + min_dist))) &&
                                     ((sy) > (box_y0 - min_dist))) && ((sy) < (box_y1 + min_dist)))
                                {
                                    int num = (int)(0);
                                    float ax = (float)(x1 - x0);
                                    float ay = (float)(y1 - y0);
                                    float bx = (float)(x0 - 2 * x1 + x2);
                                    float by = (float)(y0 - 2 * y1 + y2);
                                    float mx = (float)(x0 - sx);
                                    float my = (float)(y0 - sy);
                                    float* res = stackalloc float[3];
                                    float px = 0;
                                    float py = 0;
                                    float t = 0;
                                    float it = 0;
                                    float a_inv = (float)(precompute[i]);
                                    if ((a_inv) == (0.0))
                                    {
                                        float a = (float)(3 * (ax * bx + ay * by));
                                        float b = (float)(2 * (ax * ax + ay * ay) + (mx * bx + my * by));
                                        float c = (float)(mx * ax + my * ay);
                                        if ((a) == (0.0))
                                        {
                                            if (b != 0.0)
                                            {
                                                res[num++] = (float)(-c / b);
                                            }
                                        }
                                        else
                                        {
                                            float discriminant = (float)(b * b - 4 * a * c);
                                            if ((discriminant) < (0))
                                                num = (int)(0);
                                            else
                                            {
                                                float root = (float)(CRuntime.sqrt((double)(discriminant)));
                                                res[0] = (float)((-b - root) / (2 * a));
                                                res[1] = (float)((-b + root) / (2 * a));
                                                num = (int)(2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        float b = (float)(3 * (ax * bx + ay * by) * a_inv);
                                        float c = (float)((2 * (ax * ax + ay * ay) + (mx * bx + my * by)) * a_inv);
                                        float d = (float)((mx * ax + my * ay) * a_inv);
                                        num = (int)(stbtt__solve_cubic((float)(b), (float)(c), (float)(d), res));
                                    }

                                    if ((((num) >= (1)) && ((res[0]) >= (0.0f))) && (res[0] <= 1.0f))
                                    {
                                        t = (float)(res[0]);
                                        it = (float)(1.0f - t);
                                        px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
                                        py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
                                        dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
                                        if ((dist2) < (min_dist * min_dist))
                                            min_dist = ((float)(CRuntime.sqrt((double)(dist2))));
                                    }

                                    if ((((num) >= (2)) && ((res[1]) >= (0.0f))) && (res[1] <= 1.0f))
                                    {
                                        t = (float)(res[1]);
                                        it = (float)(1.0f - t);
                                        px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
                                        py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
                                        dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
                                        if ((dist2) < (min_dist * min_dist))
                                            min_dist = ((float)(CRuntime.sqrt((double)(dist2))));
                                    }

                                    if ((((num) >= (3)) && ((res[2]) >= (0.0f))) && (res[2] <= 1.0f))
                                    {
                                        t = (float)(res[2]);
                                        it = (float)(1.0f - t);
                                        px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
                                        py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
                                        dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
                                        if ((dist2) < (min_dist * min_dist))
                                            min_dist = ((float)(CRuntime.sqrt((double)(dist2))));
                                    }
                                }
                            }
                        }

                        if ((winding) == (0))
                            min_dist = (float)(-min_dist);
                        val = (float)(onedge_value + pixel_dist_scale * min_dist);
                        if ((val) < (0))
                            val = (float)(0);
                        else if ((val) > (255))
                            val = (float)(255);
                        data[(y - iy0) * w + (x - ix0)] = ((byte)(val));
                    }
                }

                CRuntime.free(precompute);
                CRuntime.free(verts);
            }

            return data;
        }

        public static byte* stbtt_GetCodepointSDF(stbtt_fontinfo info, float scale, int codepoint, int padding,
            byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
        {
            return stbtt_GetGlyphSDF(info, (float)(scale), (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))),
                (int)(padding), (byte)(onedge_value), (float)(pixel_dist_scale), width, height, xoff, yoff);
        }

        public static void stbtt_FreeSDF(byte* bitmap)
        {
            CRuntime.free(bitmap);
        }

        public static int stbtt__CompareUTF8toUTF16_bigendian_prefix(byte* s1, int len1, byte* s2, int len2)
        {
            int i = (int)(0);
            while ((len2) != 0)
            {
                ushort ch = (ushort)(s2[0] * 256 + s2[1]);
                if ((ch) < (0x80))
                {
                    if ((i) >= (len1))
                        return (int)(-1);
                    if (s1[i++] != ch)
                        return (int)(-1);
                }
                else if ((ch) < (0x800))
                {
                    if ((i + 1) >= (len1))
                        return (int)(-1);
                    if (s1[i++] != 0xc0 + (ch >> 6))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + (ch & 0x3f))
                        return (int)(-1);
                }
                else if (((ch) >= (0xd800)) && ((ch) < (0xdc00)))
                {
                    uint c = 0;
                    ushort ch2 = (ushort)(s2[2] * 256 + s2[3]);
                    if ((i + 3) >= (len1))
                        return (int)(-1);
                    c = (uint)(((ch - 0xd800) << 10) + (ch2 - 0xdc00) + 0x10000);
                    if (s1[i++] != 0xf0 + (c >> 18))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + ((c >> 12) & 0x3f))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + ((c >> 6) & 0x3f))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + ((c) & 0x3f))
                        return (int)(-1);
                    s2 += 2;
                    len2 -= (int)(2);
                }
                else if (((ch) >= (0xdc00)) && ((ch) < (0xe000)))
                {
                    return (int)(-1);
                }
                else
                {
                    if ((i + 2) >= (len1))
                        return (int)(-1);
                    if (s1[i++] != 0xe0 + (ch >> 12))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + ((ch >> 6) & 0x3f))
                        return (int)(-1);
                    if (s1[i++] != 0x80 + ((ch) & 0x3f))
                        return (int)(-1);
                }

                s2 += 2;
                len2 -= (int)(2);
            }

            return (int)(i);
        }

        public static int stbtt_CompareUTF8toUTF16_bigendian_internal(sbyte* s1, int len1, sbyte* s2, int len2)
        {
            return (int)((len1) == (stbtt__CompareUTF8toUTF16_bigendian_prefix((byte*)(s1), (int)(len1),
                              (byte*)(s2), (int)(len2)))
                ? 1
                : 0);
        }

        public static sbyte* stbtt_GetFontNameString(stbtt_fontinfo font, int* length, int platformID, int encodingID,
            int languageID, int nameID)
        {
            int i = 0;
            int count = 0;
            int stringOffset = 0;
            byte* fc = font.data;
            uint offset = (uint)(font.fontstart);
            uint nm = (uint)(stbtt__find_table(fc, (uint)(offset), "name"));
            if (nm == 0)
                return (null);
            count = (int)(ttUSHORT(fc + nm + 2));
            stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
            for (i = (int)(0); (i) < (count); ++i)
            {
                uint loc = (uint)(nm + 6 + 12 * i);
                if (((((platformID) == (ttUSHORT(fc + loc + 0))) && ((encodingID) == (ttUSHORT(fc + loc + 2)))) &&
                     ((languageID) == (ttUSHORT(fc + loc + 4)))) && ((nameID) == (ttUSHORT(fc + loc + 6))))
                {
                    *length = (int)(ttUSHORT(fc + loc + 8));
                    return (sbyte*)(fc + stringOffset + ttUSHORT(fc + loc + 10));
                }
            }

            return (null);
        }

        public static int stbtt__matchpair(byte* fc, uint nm, byte* name, int nlen, int target_id, int next_id)
        {
            int i = 0;
            int count = (int)(ttUSHORT(fc + nm + 2));
            int stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
            for (i = (int)(0); (i) < (count); ++i)
            {
                uint loc = (uint)(nm + 6 + 12 * i);
                int id = (int)(ttUSHORT(fc + loc + 6));
                if ((id) == (target_id))
                {
                    int platform = (int)(ttUSHORT(fc + loc + 0));
                    int encoding = (int)(ttUSHORT(fc + loc + 2));
                    int language = (int)(ttUSHORT(fc + loc + 4));
                    if ((((platform) == (0)) || (((platform) == (3)) && ((encoding) == (1)))) ||
                        (((platform) == (3)) && ((encoding) == (10))))
                    {
                        int slen = (int)(ttUSHORT(fc + loc + 8));
                        int off = (int)(ttUSHORT(fc + loc + 10));
                        int matchlen = (int)(stbtt__CompareUTF8toUTF16_bigendian_prefix(name, (int)(nlen),
                            fc + stringOffset + off, (int)(slen)));
                        if ((matchlen) >= (0))
                        {
                            if ((((((i + 1) < (count)) && ((ttUSHORT(fc + loc + 12 + 6)) == (next_id))) &&
                                  ((ttUSHORT(fc + loc + 12)) == (platform))) &&
                                 ((ttUSHORT(fc + loc + 12 + 2)) == (encoding))) &&
                                ((ttUSHORT(fc + loc + 12 + 4)) == (language)))
                            {
                                slen = (int)(ttUSHORT(fc + loc + 12 + 8));
                                off = (int)(ttUSHORT(fc + loc + 12 + 10));
                                if ((slen) == (0))
                                {
                                    if ((matchlen) == (nlen))
                                        return (int)(1);
                                }
                                else if (((matchlen) < (nlen)) && ((name[matchlen]) == (' ')))
                                {
                                    ++matchlen;
                                    if ((stbtt_CompareUTF8toUTF16_bigendian_internal((sbyte*)(name + matchlen),
                                            (int)(nlen - matchlen), (sbyte*)(fc + stringOffset + off),
                                            (int)(slen))) != 0)
                                        return (int)(1);
                                }
                            }
                            else
                            {
                                if ((matchlen) == (nlen))
                                    return (int)(1);
                            }
                        }
                    }
                }
            }

            return (int)(0);
        }

        public static int stbtt__matches(byte* fc, uint offset, byte* name, int flags)
        {
            int nlen = (int)(CRuntime.strlen((sbyte*)(name)));
            uint nm = 0;
            uint hd = 0;
            if (stbtt__isfont(fc + offset) == 0)
                return (int)(0);
            if ((flags) != 0)
            {
                hd = (uint)(stbtt__find_table(fc, (uint)(offset), "head"));
                if ((ttUSHORT(fc + hd + 44) & 7) != (flags & 7))
                    return (int)(0);
            }

            nm = (uint)(stbtt__find_table(fc, (uint)(offset), "name"));
            if (nm == 0)
                return (int)(0);
            if ((flags) != 0)
            {
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(16), (int)(-1))) != 0)
                    return (int)(1);
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(1), (int)(-1))) != 0)
                    return (int)(1);
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(3), (int)(-1))) != 0)
                    return (int)(1);
            }
            else
            {
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(16), (int)(17))) != 0)
                    return (int)(1);
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(1), (int)(2))) != 0)
                    return (int)(1);
                if ((stbtt__matchpair(fc, (uint)(nm), name, (int)(nlen), (int)(3), (int)(-1))) != 0)
                    return (int)(1);
            }

            return (int)(0);
        }

        public static int stbtt_FindMatchingFont_internal(byte* font_collection, sbyte* name_utf8, int flags)
        {
            int i = 0;
            for (i = (int)(0); ; ++i)
            {
                int off = (int)(stbtt_GetFontOffsetForIndex(font_collection, (int)(i)));
                if ((off) < (0))
                    return (int)(off);
                if ((stbtt__matches(font_collection, (uint)(off), (byte*)(name_utf8), (int)(flags))) != 0)
                    return (int)(off);
            }
        }

        public static int stbtt_BakeFontBitmap(byte* data, int offset, float pixel_height, byte* pixels, int pw, int ph,
            int first_char, int num_chars, stbtt_bakedchar* chardata)
        {
            return (int)(stbtt_BakeFontBitmap_internal(data, (int)(offset), (float)(pixel_height), pixels,
                (int)(pw), (int)(ph), (int)(first_char), (int)(num_chars), chardata));
        }

        public static int stbtt_GetFontOffsetForIndex(byte* data, int index)
        {
            return (int)(stbtt_GetFontOffsetForIndex_internal(data, (int)(index)));
        }

        public static int stbtt_GetNumberOfFonts(byte* data)
        {
            return (int)(stbtt_GetNumberOfFonts_internal(data));
        }

        public static int stbtt_InitFont(stbtt_fontinfo info, byte* data, int offset)
        {
            return (int)(stbtt_InitFont_internal(info, data, (int)(offset)));
        }

        public static int stbtt_FindMatchingFont(byte* fontdata, sbyte* name, int flags)
        {
            return (int)(stbtt_FindMatchingFont_internal(fontdata, name, (int)(flags)));
        }

        public static int stbtt_CompareUTF8toUTF16_bigendian(sbyte* s1, int len1, sbyte* s2, int len2)
        {
            return (int)(stbtt_CompareUTF8toUTF16_bigendian_internal(s1, (int)(len1), s2, (int)(len2)));
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__buf
        {
            public byte* data;
            public int cursor;
            public int size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt_bakedchar
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
        public struct stbtt_aligned_quad
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
        public struct stbtt_packedchar
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
        public struct stbtt_pack_range
        {
            public float font_size;
            public int first_unicode_codepoint_in_range;
            public int* array_of_unicode_codepoints;
            public int num_chars;
            public stbtt_packedchar* chardata_for_range;
            public byte h_oversample;
            public byte v_oversample;
        }

        public class stbtt_pack_context
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

        public class stbtt_fontinfo
        {
            public stbtt__buf cff = new stbtt__buf();
            public stbtt__buf charstrings = new stbtt__buf();
            public byte* data;
            public stbtt__buf fdselect = new stbtt__buf();
            public stbtt__buf fontdicts = new stbtt__buf();
            public int fontstart;
            public int glyf;
            public int gpos;
            public stbtt__buf gsubrs = new stbtt__buf();
            public int head;
            public int hhea;
            public int hmtx;
            public int index_map;
            public int indexToLocFormat;
            public int kern;
            public int loca;
            public int numGlyphs;
            public stbtt__buf subrs = new stbtt__buf();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt_vertex
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
        public struct stbtt__bitmap
        {
            public int w;
            public int h;
            public int stride;
            public byte* pixels;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__csctx
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
            public stbtt_vertex* pvertices;
            public int num_vertices;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__hheap_chunk
        {
            public stbtt__hheap_chunk* next;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__hheap
        {
            public stbtt__hheap_chunk* head;
            public void* first_free;
            public int num_remaining_in_head_chunk;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__edge
        {
            public float x0;
            public float y0;
            public float x1;
            public float y1;
            public int invert;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__active_edge
        {
            public stbtt__active_edge* next;
            public float fx;
            public float fdx;
            public float fdy;
            public float direction;
            public float sy;
            public float ey;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stbtt__point
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