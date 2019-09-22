using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class SpriteFont
    {

        public class Character
        {
            public uint Unicode;
            public Subtexture? Image;
            public float Advance;
        }

        public readonly Dictionary<uint, Character> Characters = new Dictionary<uint, Character>();

        public SpriteFont(Font font, int size, string charset)
        {

        }
    }
}
