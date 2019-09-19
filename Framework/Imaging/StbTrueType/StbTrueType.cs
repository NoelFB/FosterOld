namespace StbTrueTypeSharp
{
    internal static unsafe partial class StbTrueType
    {
        public static uint stbtt__find_table(byte* data, uint fontstart, string tag)
        {
            int num_tables = ttUSHORT(data + fontstart + 4);
            var tabledir = fontstart + 12;
            int i;
            for (i = 0; i < num_tables; ++i)
            {
                var loc = (uint)(tabledir + 16 * i);
                if ((data + loc + 0)[0] == tag[0] && (data + loc + 0)[1] == tag[1] &&
                    (data + loc + 0)[2] == tag[2] && (data + loc + 0)[3] == tag[3])
                    return ttULONG(data + loc + 8);
            }

            return 0;
        }

        public static bool stbtt_BakeFontBitmap(byte[] ttf, int offset, float pixel_height, byte[] pixels, int pw,
            int ph,
            int first_char, int num_chars, stbtt_bakedchar[] chardata)
        {
            fixed (byte* ttfPtr = ttf)
            {
                fixed (byte* pixelsPtr = pixels)
                {
                    fixed (stbtt_bakedchar* chardataPtr = chardata)
                    {
                        var result = stbtt_BakeFontBitmap(ttfPtr, offset, pixel_height, pixelsPtr, pw, ph, first_char,
                            num_chars,
                            chardataPtr);

                        return result != 0;
                    }
                }
            }
        }
    }
}