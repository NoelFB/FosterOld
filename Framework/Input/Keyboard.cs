using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Keyboard
    {
        public const int MaxKeys = 300;

        internal readonly bool[] pressed = new bool[MaxKeys];
        internal readonly bool[] down = new bool[MaxKeys];
        internal readonly bool[] released = new bool[MaxKeys];
        internal readonly ulong[] timestamp = new ulong[MaxKeys];

        public readonly StringBuilder Text = new StringBuilder();

        internal void Copy(Keyboard other)
        {
            Array.Copy(other.pressed, 0, pressed, 0, MaxKeys);
            Array.Copy(other.down, 0, down, 0, MaxKeys);
            Array.Copy(other.released, 0, released, 0, MaxKeys);
            Array.Copy(other.timestamp, 0, timestamp, 0, MaxKeys);

            Text.Clear();
            Text.Append(other.Text);
        }

        internal void Step()
        {
            Array.Fill(pressed, false);
            Array.Fill(released, false);

            if (Text.Length > 0)
                Console.WriteLine(Text.ToString());
            Text.Clear();
        }

    }
}
