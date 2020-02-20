using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// Represents a Gamepad or Joystick
    /// </summary>
    public class Controller
    {
        public const int MaxButtons = 64;
        public const int MaxAxis = 64;

        public readonly Input Input;

        public string Name { get; private set; } = "Unknown";
        public bool Connected { get; private set; } = false;
        public bool IsGamepad { get; private set; } = false;
        public int Buttons { get; private set; } = 0;
        public int Axes { get; private set; } = 0;

        internal readonly bool[] pressed = new bool[MaxButtons];
        internal readonly bool[] down = new bool[MaxButtons];
        internal readonly bool[] released = new bool[MaxButtons];
        internal readonly long[] timestamp = new long[MaxButtons];
        internal readonly float[] axis = new float[MaxAxis];
        internal readonly long[] axisTimestamp = new long[MaxAxis];

        internal void Connect(string name, uint buttonCount, uint axisCount, bool isGamepad)
        {
            Name = name;
            Buttons = (int)Math.Min(buttonCount, MaxButtons);
            Axes = (int)Math.Min(axisCount, MaxAxis);
            IsGamepad = isGamepad;
            Connected = true;
        }

        internal void Disconnect()
        {
            Name = "Unknown";
            Connected = false;
            IsGamepad = false;
            Buttons = 0;
            Axes = 0;

            Array.Fill(pressed, false);
            Array.Fill(down, false);
            Array.Fill(released, false);
            Array.Fill(timestamp, 0L);
            Array.Fill(axis, 0);
            Array.Fill(axisTimestamp, 0L);
        }

        internal void Step()
        {
            Array.Fill(pressed, false);
            Array.Fill(released, false);
        }

        internal void Copy(Controller other)
        {
            Name = other.Name;
            Connected = other.Connected;
            IsGamepad = other.IsGamepad;
            Buttons = other.Buttons;
            Axes = other.Axes;

            Array.Copy(other.pressed, 0, pressed, 0, pressed.Length);
            Array.Copy(other.down, 0, down, 0, pressed.Length);
            Array.Copy(other.released, 0, released, 0, pressed.Length);
            Array.Copy(other.timestamp, 0, timestamp, 0, pressed.Length);
            Array.Copy(other.axis, 0, axis, 0, axis.Length);
            Array.Copy(other.axisTimestamp, 0, axisTimestamp, 0, axis.Length);
        }

        public bool Pressed(int buttonIndex) => buttonIndex >= 0 && buttonIndex < MaxButtons && pressed[buttonIndex];
        public bool Pressed(Buttons button) => Pressed((int)button);

        public long Timestamp(int buttonIndex) => buttonIndex >= 0 && buttonIndex < MaxButtons ? timestamp[buttonIndex] : 0;
        public long Timestamp(Buttons button) => Timestamp((int)button);
        public long Timestamp(Axes axis) => axisTimestamp[(int)axis];

        public bool Down(int buttonIndex) => buttonIndex >= 0 && buttonIndex < MaxButtons && down[buttonIndex];
        public bool Down(Buttons button) => Down((int)button);

        public bool Released(int buttonIndex) => buttonIndex >= 0 && buttonIndex < MaxButtons && released[buttonIndex];
        public bool Released(Buttons button) => Released((int)button);

        public float Axis(int axisIndex) => (axisIndex >= 0 && axisIndex < MaxAxis) ? axis[axisIndex] : 0f;
        public float Axis(Axes axis) => Axis((int)axis);

        public Vector2 Axis(int axisX, int axisY) => new Vector2(Axis(axisX), Axis(axisY));
        public Vector2 Axis(Axes axisX, Axes axisY) => new Vector2(Axis(axisX), Axis(axisY));

        public Vector2 LeftStick => Axis(Framework.Axes.LeftX, Framework.Axes.LeftY);
        public Vector2 RightStick => Axis(Framework.Axes.RightX, Framework.Axes.RightY);

        public bool Repeated(Buttons button)
        {
            return Repeated(button, Input.RepeatDelay, Input.RepeatInterval);
        }

        public bool Repeated(Buttons button, float delay, float interval)
        {
            if (Pressed(button))
                return true;

            if (Down(button))
            {
                var time = Timestamp(button) / 1000.0;
                return (Time.Duration.TotalSeconds - time) > delay && Time.OnInterval(interval, time);
            }

            return false;
        }

        public Controller(Input input)
        {
            Input = input;
        }

    }
}
