using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// Sprite Font is a Font rendered to a Texture at a given size, which is useful for drawing Text with sprite batchers
    /// </summary>
    public class SpriteFont
    {
        /// <summary>
        /// A single Sprite Font Character
        /// </summary>
        public class Character
        {
            /// <summary>
            /// The Unicode Value
            /// </summary>
            public char Unicode;

            /// <summary>
            /// The rendered Character Image
            /// </summary>            
            public Subtexture Image;

            /// <summary>
            /// The Offset to draw the Character at
            /// </summary>
            public Vector2 Offset;

            /// <summary>
            /// The Amount to Advance the rendering by, horizontally
            /// </summary>
            public float Advance;

            /// <summary>
            /// The Kerning value for following Characters
            /// </summary>
            public Dictionary<char, float> Kerning = new Dictionary<char, float>();

            public Character(char unicode, Subtexture image, Vector2 offset, float advance)
            {
                Unicode = unicode;
                Image = image;
                Offset = offset;
                Advance = advance;
            }
        }

        /// <summary>
        /// A list of all the Characters in the Sprite Font
        /// </summary>
        public readonly Dictionary<char, Character> Charset = new Dictionary<char, Character>();

        /// <summary>
        /// The Font Family Name
        /// </summary>
        public string FamilyName;

        /// <summary>
        /// The Font Style Name
        /// </summary>
        public string StyleName;

        /// <summary>
        /// The Size of the Sprite Font
        /// </summary>
        public int Size;

        /// <summary>
        /// The Font Ascent
        /// </summary>
        public float Ascent;

        /// <summary>
        /// The Font Descent
        /// </summary>
        public float Descent;

        /// <summary>
        /// The Line Gap of the Font. This is the vertical space between lines
        /// </summary>
        public float LineGap;

        /// <summary>
        /// The Height of the Font (Ascent - Descent)
        /// </summary>
        public float Height;

        /// <summary>
        /// The Line Height of the Font (Height + LineGap). This is the total height of a single line, including the line gap
        /// </summary>
        public float LineHeight;

        public SpriteFont(string? familyName = null, string? styleName = null)
        {
            FamilyName = familyName ?? "Unknown";
            StyleName = styleName ?? "Unknown";
        }

        public SpriteFont(string fontFile, int size, string charset, TextureFilter filter = TextureFilter.Linear)
            : this(new FontSize(new Font(fontFile), size, charset), filter)
        {

        }

        public SpriteFont(Stream stream, int size, string charset, TextureFilter filter = TextureFilter.Linear)
            : this(new FontSize(new Font(stream), size, charset), filter)
        {

        }

        public SpriteFont(Font font, int size, string charset, TextureFilter filter = TextureFilter.Linear)
            : this(new FontSize(font, size, charset), filter)
        {

        }

        public SpriteFont(FontSize fontSize, TextureFilter filter = TextureFilter.Linear)
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
                    var sprChar = new Character(ch.Unicode, new Subtexture(), new Vector2(ch.OffsetX, ch.OffsetY), ch.Advance);
                    Charset.Add(ch.Unicode, sprChar);

                    // get all kerning
                    foreach (var ch2 in fontSize.Charset.Values)
                    {
                        var kerning = fontSize.GetKerning(ch.Unicode, ch2.Unicode);
                        if (Math.Abs(kerning) > 0.000001f)
                            sprChar.Kerning[ch2.Unicode] = kerning;
                    }
                }

                packer.Pack();
            }

            // link textures
            var output = packer.Pack();
            if (output != null)
            {
                for (int i = 0; i < output.Pages.Count; i++)
                {
                    var texture = new Texture(output.Pages[i]);
                    texture.Filter = filter;

                    foreach (var entry in output.Entries.Values)
                    {
                        if (entry.Page != i)
                            continue;

                        if (Charset.TryGetValue(entry.Name[0], out var character))
                            character.Image.Reset(texture, entry.Source, entry.Frame);
                    }

                }
            }
        }

        /// <summary>
        /// Measures the Width of the given text
        /// </summary>
        public float WidthOf(string text)
        {
            return WidthOf(text.AsSpan());
        }

        /// <summary>
        /// Measures the Width of the given text
        /// </summary>
        public float WidthOf(ReadOnlySpan<char> text)
        {
            var width = 0f;
            var line = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    if (line > width)
                        width = line;
                    line = 0;
                    continue;
                }

                if (!Charset.TryGetValue(text[i], out var ch))
                    continue;

                line += ch.Advance;
            }

            return Math.Max(width, line);
        }

        public float HeightOf(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return HeightOf(text.AsSpan());
        }

        public float HeightOf(ReadOnlySpan<char> text)
        {
            if (text.Length <= 0)
                return 0;

            var height = Height;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                    height += LineHeight;
            }

            return height;
        }

        public Vector2 SizeOf(string text)
        {
            return new Vector2(WidthOf(text.AsSpan()), HeightOf(text.AsSpan()));
        }

        public Vector2 SizeOf(ReadOnlySpan<char> text)
        {
            return new Vector2(WidthOf(text), HeightOf(text));
        }

    }
}
