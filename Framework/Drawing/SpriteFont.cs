using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class SpriteFont
    {

        public class Character
        {
            public char Unicode;
            public Subtexture? Image;
            public Vector2 Offset;
            public float Advance;
            public Dictionary<char, float> Kerning = new Dictionary<char, float>();

            public Character(char unicode, Subtexture? image, Vector2 offset, float advance)
            {
                Unicode = unicode;
                Image = image;
                Offset = offset;
                Advance = advance;
            }
        }

        public readonly Dictionary<char, Character> Charset = new Dictionary<char, Character>();

        public string FamilyName;
        public string StyleName;
        public int Size;
        public float Ascent;
        public float Descent;
        public float LineGap;
        public float Height;
        public float LineHeight;

        public SpriteFont(string? familyName = null, string? styleName = null)
        {
            FamilyName = familyName ?? "Unknown";
            StyleName = styleName ?? "Unknown";
        }

        public SpriteFont(string fontFile, int size, string charset)
            : this(new FontSize(new Font(fontFile), size, charset))
        {

        }

        public SpriteFont(Font font, int size, string charset) 
            : this(new FontSize(font, size, charset))
        {

        }

        public SpriteFont(FontSize fontSize)
        {
            FamilyName = fontSize.Font.FamilyName;
            StyleName = fontSize.Font.StyleName;
            Size = fontSize.Size;
            Ascent = fontSize.Ascent;
            Descent = fontSize.Descent;
            LineGap = fontSize.LineGap;
            Height = fontSize.Height;
            LineHeight = fontSize.LineHeight;

            var packer = new Packer();
            {
                var bufferSize = (fontSize.Size * 2) * (fontSize.Size * 2);
                var buffer = (bufferSize <= 16384 ? stackalloc Color[bufferSize] : new Color[bufferSize]);

                foreach (var ch in fontSize.Charset.Values)
                {
                    var name = ch.Unicode.ToString();

                    // pack bmp
                    if (fontSize.Render(ch.Unicode, buffer, out int w, out int h))
                        packer.AddPixels(name, w, h, buffer);

                    // create character
                    var sprChar = new Character(ch.Unicode, null, new Vector2(ch.OffsetX, ch.OffsetY), ch.Advance);
                    Charset.Add(ch.Unicode, sprChar);

                    // get all kerning
                    foreach (var ch2 in fontSize.Charset.Values)
                    {
                        var kerning = fontSize.GetKerning(ch.Unicode, ch2.Unicode);
                        if (kerning != 0)
                            sprChar.Kerning[ch2.Unicode] = kerning;
                    }
                }

                packer.Pack();
            }

            // link textures
            var atlas = new Atlas(packer);
            foreach (var kv in atlas.Subtextures)
            {
                if (Charset.TryGetValue(kv.Key[0], out var ch))
                    ch.Image = kv.Value;
            }
        }

    }
}
