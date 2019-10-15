using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Controller
    {
        public const int MaxButtons = 64;
        public const int MaxAxis = 64;

        public string Name { get; private set; } = "Unknown";
        public bool Connected { get; private set; } = false;
        public bool IsGamepad { get; private set; } = false;
        public int ButtonCount { get; private set; } = 0;
        public int AxisCount { get; private set; } = 0;

        internal readonly bool[] pressed = new bool[MaxButtons];
        internal readonly bool[] down = new bool[MaxButtons];
        internal readonly bool[] released = new bool[MaxButtons];
        internal readonly ulong[] timestamp = new ulong[MaxButtons];
        internal readonly float[] axis = new float[MaxAxis];
        internal readonly ulong[] axisTimestamp = new ulong[MaxAxis];


        public Controller()
        {

        }

        internal void Connect(string name, uint buttonCount, uint axisCount, bool isGamepad)
        {
            Name = name;
            ButtonCount = (int)Math.Min(buttonCount, MaxButtons);
            AxisCount = (int)Math.Min(axisCount, MaxAxis);
            IsGamepad = isGamepad;
        }

        internal void Disconnect()
        {
            Name = "Unknown";
            Connected = false;
            IsGamepad = false;
            ButtonCount = 0;
            AxisCount = 0;

            Array.Fill(pressed, false);
            Array.Fill(down, false);
            Array.Fill(released, false);
            Array.Fill(timestamp, 0UL);
            Array.Fill(axis, 0);
            Array.Fill(axisTimestamp, 0UL);
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
            ButtonCount = other.ButtonCount;
            AxisCount = other.AxisCount;

            Array.Copy(other.pressed, 0, pressed, 0, ButtonCount);
            Array.Copy(other.down, 0, down, 0, ButtonCount);
            Array.Copy(other.released, 0, released, 0, ButtonCount);
            Array.Copy(other.timestamp, 0, timestamp, 0, ButtonCount);
            Array.Copy(other.axis, 0, axis, 0, AxisCount);
            Array.Copy(other.axisTimestamp, 0, axisTimestamp, 0, AxisCount);
        }

        public bool Pressed(int buttonIndex) => buttonIndex >= 0 && buttonIndex < ButtonCount && pressed[buttonIndex];
        public bool Pressed(Buttons button) => Pressed((int)button);

        public bool Down(int buttonIndex) => buttonIndex >= 0 && buttonIndex < ButtonCount && down[buttonIndex];
        public bool Down(Buttons button) => Down((int)button);

        public bool Released(int buttonIndex) => buttonIndex >= 0 && buttonIndex < ButtonCount && released[buttonIndex];
        public bool Released(Buttons button) => Released((int)button);

        public float Axis(int axisIndex) => (axisIndex >= 0 && axisIndex < AxisCount) ? axis[axisIndex] : 0f;
        public float Axis(ControllerAxis axis) => Axis((int)axis);

        public Vector2 Axis(int axisX, int axisY) => new Vector2(Axis(axisX), Axis(axisY));
        public Vector2 Axis(ControllerAxis axisX, ControllerAxis axisY) => new Vector2(Axis(axisX), Axis(axisY));

        public Vector2 LeftStick => Axis(ControllerAxis.LeftX, ControllerAxis.LeftY);
        public Vector2 RightStick => Axis(ControllerAxis.RightX, ControllerAxis.RightY);

    }
}
