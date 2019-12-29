using System;
using System.Text;

namespace Foster.Framework
{
    public class Keyboard
    {
        
        public const int MaxKeys = 400;

        internal readonly bool[] pressed = new bool[MaxKeys];
        internal readonly bool[] down = new bool[MaxKeys];
        internal readonly bool[] released = new bool[MaxKeys];
        internal readonly ulong[] timestamp = new ulong[MaxKeys];

        public readonly Input Input;
        public readonly StringBuilder Text = new StringBuilder();

        public bool Pressed(Keys key) => pressed[(int)key];
        public bool Down(Keys key) => down[(int)key];
        public bool Released(Keys key) => released[(int)key];

        public bool Pressed(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (pressed[(int)keys[i]])
                    return true;

            return false;
        }

        public bool Down(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (down[(int)keys[i]])
                    return true;

            return false;
        }

        public bool Released(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (released[(int)keys[i]])
                    return true;

            return false;
        }

        public bool Repeated(Keys key)
        {
            return Repeated(key, Input.RepeatDelay, Input.RepeatInterval);
        }

        public bool Repeated(Keys key, float delay, float interval)
        {
            if (Pressed(key))
                return true;

            var time = timestamp[(int)key] / 1000.0;

            return Down(key) && (Time.Duration.TotalSeconds - time) > delay && Time.OnInterval(interval, time);
        }

        public ulong Timestamp(Keys key)
        {
            return timestamp[(int)key];
        }

        internal Keyboard(Input input)
        {
            Input = input;
        }

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

            Text.Clear();
        }

    }
}
