using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Mouse
    {
        public const int MaxButtons = 5;

        internal readonly bool[] pressed = new bool[MaxButtons];
        internal readonly bool[] down = new bool[MaxButtons];
        internal readonly bool[] released = new bool[MaxButtons];
        internal readonly ulong[] timestamp = new ulong[MaxButtons];

        public bool Pressed(MouseButtons button) => pressed[(int)button];
        public bool Down(MouseButtons button) => down[(int)button];
        public bool Released(MouseButtons button) => released[(int)button];

        public bool LeftPressed => Pressed(MouseButtons.Left);
        public bool LeftDown => Down(MouseButtons.Left);
        public bool LeftReleased => Released(MouseButtons.Left);

        public bool RightPressed => Pressed(MouseButtons.Right);
        public bool RightDown => Down(MouseButtons.Right);
        public bool RightReleased => Released(MouseButtons.Right);

        internal void Copy(Mouse other)
        {
            Array.Copy(other.pressed, 0, pressed, 0, MaxButtons);
            Array.Copy(other.down, 0, down, 0, MaxButtons);
            Array.Copy(other.released, 0, released, 0, MaxButtons);
            Array.Copy(other.timestamp, 0, timestamp, 0, MaxButtons);
        }

        internal void Step()
        {
            Array.Fill(pressed, false);
            Array.Fill(released, false);
        }

    }
}
