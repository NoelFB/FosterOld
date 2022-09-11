﻿using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// Stores a Mouse State
    /// </summary>
    public class Mouse
    {
        public const int MaxButtons = 5;

        internal readonly bool[] pressed = new bool[MaxButtons];
        internal readonly bool[] down = new bool[MaxButtons];
        internal readonly bool[] released = new bool[MaxButtons];
        internal readonly long[] timestamp = new long[MaxButtons];
        internal Vector2 wheelValue;
        internal Point2 mousePosition;

        public bool Pressed(MouseButtons button) => pressed[(int)button];
        public bool Down(MouseButtons button) => down[(int)button];
        public bool Released(MouseButtons button) => released[(int)button];

        public long Timestamp(MouseButtons button)
        {
            return timestamp[(int)button];
        }

        public bool Repeated(MouseButtons button, float delay, float interval)
        {
            if (Pressed(button))
                return true;

            var time = timestamp[(int)button] / (float)TimeSpan.TicksPerSecond;

            return Down(button) && (Time.Duration.TotalSeconds - time) > delay && Time.OnInterval(interval, time);
        }

        public bool LeftPressed => pressed[(int)MouseButtons.Left];
        public bool LeftDown => down[(int)MouseButtons.Left];
        public bool LeftReleased => released[(int)MouseButtons.Left];

        public bool RightPressed => pressed[(int)MouseButtons.Right];
        public bool RightDown => down[(int)MouseButtons.Right];
        public bool RightReleased => released[(int)MouseButtons.Right];

        public bool MiddlePressed => pressed[(int)MouseButtons.Middle];
        public bool MiddleDown => down[(int)MouseButtons.Middle];
        public bool MiddleReleased => released[(int)MouseButtons.Middle];

        public Vector2 Wheel => wheelValue;
        public Point2 Position => mousePosition;

        internal void Copy(Mouse other)
        {
            Array.Copy(other.pressed, 0, pressed, 0, MaxButtons);
            Array.Copy(other.down, 0, down, 0, MaxButtons);
            Array.Copy(other.released, 0, released, 0, MaxButtons);
            Array.Copy(other.timestamp, 0, timestamp, 0, MaxButtons);

            wheelValue = other.wheelValue;
            mousePosition = other.mousePosition;
        }

        internal void Step()
        {
            Array.Fill(pressed, false);
            Array.Fill(released, false);
            wheelValue = Vector2.Zero;
        }
    }
}
