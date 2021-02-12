using System;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// Stores a Keyboard State
    /// </summary>
    public class Keyboard
    {
        
        public const int MaxKeys = 400;

        internal readonly bool[] pressed = new bool[MaxKeys];
        internal readonly bool[] down = new bool[MaxKeys];
        internal readonly bool[] released = new bool[MaxKeys];
        internal readonly long[] timestamp = new long[MaxKeys];

        /// <summary>
        /// The Input Module this Keyboard belong to
        /// </summary>
        public readonly Input Input;

        /// <summary>
        /// Any Text that was typed over the last frame
        /// </summary>
        public readonly StringBuilder Text = new StringBuilder();

        internal Keyboard(Input input)
        {
            Input = input;
        }

        /// <summary>
        /// Checks if the given key was pressed
        /// </summary>
        public bool Pressed(Keys key) => pressed[(int)key];

        /// <summary>
        /// Checks if any of the given keys were pressed
        /// </summary>
        public bool Pressed(Keys key1, Keys key2) => pressed[(int)key1] || pressed[(int)key2];

        /// <summary>
        /// Checks if any of the given keys were pressed
        /// </summary>
        public bool Pressed(Keys key1, Keys key2, Keys key3) => pressed[(int)key1] || pressed[(int)key2] || pressed[(int)key3];

        /// <summary>
        /// Checks if the given key is held
        /// </summary>
        public bool Down(Keys key) => down[(int)key];

        /// <summary>
        /// Checks if any of the given keys were down
        /// </summary>
        public bool Down(Keys key1, Keys key2) => down[(int)key1] || down[(int)key2];

        /// <summary>
        /// Checks if any of the given keys were down
        /// </summary>
        public bool Down(Keys key1, Keys key2, Keys key3) => down[(int)key1] || down[(int)key2] || down[(int)key3];

        /// <summary>
        /// Checks if the given key was released
        /// </summary>
        public bool Released(Keys key) => released[(int)key];

        /// <summary>
        /// Checks if any of the given keys were released
        /// </summary>
        public bool Released(Keys key1, Keys key2) => released[(int)key1] || released[(int)key2];

        /// <summary>
        /// Checks if any of the given keys were released
        /// </summary>
        public bool Released(Keys key1, Keys key2, Keys key3) => released[(int)key1] || released[(int)key2] || released[(int)key3];

        /// <summary>
        /// Checks if any of the given keys were pressed
        /// </summary>
        public bool Pressed(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (pressed[(int)keys[i]])
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if any of the given keys are held
        /// </summary>
        public bool Down(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (down[(int)keys[i]])
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if any of the given keys were released
        /// </summary>
        public bool Released(ReadOnlySpan<Keys> keys)
        {
            for (int i = 0; i < keys.Length; i++)
                if (released[(int)keys[i]])
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if the given key was Repeated
        /// </summary>
        public bool Repeated(Keys key)
        {
            return Repeated(key, Input.RepeatDelay, Input.RepeatInterval);
        }

        /// <summary>
        /// Checks if the given key was Repeated, given the delay and interval
        /// </summary>
        public bool Repeated(Keys key, float delay, float interval)
        {
            if (Pressed(key))
                return true;

            var time = timestamp[(int)key] / (float) TimeSpan.TicksPerSecond;

            return Down(key) && (Time.Duration.TotalSeconds - time) > delay && Time.OnInterval(interval, time);
        }

        /// <summary>
        /// Gets the Timestamp of when the given key was last pressed, in Ticks
        /// </summary>
        public long Timestamp(Keys key)
        {
            return timestamp[(int)key];
        }

        /// <summary>
        /// Returns True if the Left or Right Control keys are held
        /// </summary>
        public bool Ctrl => Down(Keys.LeftControl, Keys.RightControl);

        /// <summary>
        /// Returns True if the Left or Right Control keys are held (or Command on MacOS)
        /// </summary>
        public bool CtrlOrCommand
        {
            get
            {
                if (OperatingSystem.IsMacOS())
                    return Down(Keys.LeftSuper, Keys.RightSuper);
                else
                    return Down(Keys.LeftControl, Keys.RightControl);
            }
        }

        /// <summary>
        /// Returns True if the Left or Right Alt keys are held
        /// </summary>
        public bool Alt => Down(Keys.LeftAlt, Keys.RightAlt);

        /// <summary>
        /// Returns True of the Left or Right Shift keys are held
        /// </summary>
        public bool Shift => Down(Keys.LeftShift, Keys.RightShift);

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
