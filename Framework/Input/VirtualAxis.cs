using System;
using System.Collections.Generic;

namespace Foster.Framework
{

    public class VirtualAxis
    {
        public enum Overlaps
        {
            CancelOut,
            TakeOlder,
            TakeNewer
        };

        private interface INode
        {
            float Value(bool deadzone);
            double Timestamp { get; }
        }

        private class KeyNode : INode
        {
            public Input Input;
            public Keys Key;
            public float Direction;

            public float Value(bool deadzone) { return (Input.Keyboard.Down(Key) ? Direction : 0); }
            public double Timestamp { get { return Input.Keyboard.Timestamp(Key); } }

            public KeyNode(Input input, Keys key, bool positive)
            {
                Input = input;
                Key = key;
                Direction = (positive ? 1 : -1);
            }
        }

        private class ButtonNode : INode
        {
            public Input Input;
            public int Index;
            public Buttons Button;
            public float Direction;

            public float Value(bool deadzone) { return (Input.Controllers[Index].Down(Button) ? Direction : 0); }
            public double Timestamp { get { return Input.Controllers[Index].Timestamp(Button); } }

            public ButtonNode(Input input, int controller, Buttons button, bool positive)
            {
                Input = input;
                Index = controller;
                Button = button;
                Direction = (positive ? 1 : -1);
            }
        }

        private class AxisNode : INode
        {
            public Input Input;
            public int Index;
            public Axes Axis;
            public float Direction;
            public float Deadzone;

            public float Value(bool deadzone)
            {
                if (!deadzone || Math.Abs(Input.Controllers[Index].Axis(Axis)) >= Deadzone)
                    return Input.Controllers[Index].Axis(Axis) * Direction;
                return 0f;
            }

            public double Timestamp
            {
                get
                {
                    if (Math.Abs(Input.Controllers[Index].Axis(Axis)) < Deadzone)
                        return 0;
                    return Input.Controllers[Index].Timestamp(Axis);
                }
            }

            public AxisNode(Input input, int controller, Axes axis, float deadzone, bool positive)
            {
                Input = input;
                Index = controller;
                Axis = axis;
                Deadzone = deadzone;
                Direction = (positive ? 1 : -1);
            }
        }

        public float Value => GetValue(true);
        public float ValueNoDeadzone => GetValue(false);

        public int IntValue => Math.Sign(Value);
        public int IntValueNoDeadzone => Math.Sign(ValueNoDeadzone);

        public readonly Input Input;
        public Overlaps OverlapBehaviour = Overlaps.CancelOut;

        private readonly List<INode> nodes = new List<INode>();

        private const float EPSILON = 0.00001f;

        public VirtualAxis(Input input)
        {
            Input = input;
        }

        public VirtualAxis(Input input, Overlaps overlapBehaviour)
        {
            Input = input;
            OverlapBehaviour = overlapBehaviour;
        }

        private float GetValue(bool deadzone)
        {
            var value = 0f;

            if (OverlapBehaviour == Overlaps.CancelOut)
            {
                foreach (var input in nodes)
                    value += input.Value(deadzone);
                value = Calc.Clamp(value, -1, 1);
            }
            else if (OverlapBehaviour == Overlaps.TakeNewer)
            {
                var timestamp = 0d;
                for (int i = 0; i < nodes.Count; i++)
                {
                    var time = nodes[i].Timestamp;
                    var val = nodes[i].Value(deadzone);

                    if (time > 0 && Math.Abs(val) > EPSILON && time > timestamp)
                    {
                        value = val;
                        timestamp = time;
                    }
                }
            }
            else if (OverlapBehaviour == Overlaps.TakeOlder)
            {
                var timestamp = double.MaxValue;
                for (int i = 0; i < nodes.Count; i++)
                {
                    var time = nodes[i].Timestamp;
                    var val = nodes[i].Value(deadzone);

                    if (time > 0 && Math.Abs(val) > EPSILON && time < timestamp)
                    {
                        value = val;
                        timestamp = time;
                    }
                }
            }

            return value;
        }

        public VirtualAxis Add(Keys negative, Keys positive)
        {
            nodes.Add(new KeyNode(Input, negative, false));
            nodes.Add(new KeyNode(Input, positive, true));
            return this;
        }

        public VirtualAxis Add(int controller, Buttons negative, Buttons positive)
        {
            nodes.Add(new ButtonNode(Input, controller, negative, false));
            nodes.Add(new ButtonNode(Input, controller, positive, true));
            return this;
        }

        public VirtualAxis Add(int controller, Axes axis, float deadzone = 0f)
        {
            nodes.Add(new AxisNode(Input, controller, axis, deadzone, true));
            return this;
        }

        public VirtualAxis Add(int controller, Axes axis, bool inverse, float deadzone = 0f)
        {
            nodes.Add(new AxisNode(Input, controller, axis, deadzone, !inverse));
            return this;
        }

    }
}
