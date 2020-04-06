using System;
using System.Collections.Generic;

namespace Foster.Framework
{

    /// <summary>
    /// A Virtual Input Axis that can be mapped to different keyboards and gamepads
    /// </summary>
    public class VirtualAxis
    {
        public enum Overlaps
        {
            CancelOut,
            TakeOlder,
            TakeNewer
        };

        public interface INode
        {
            float Value(bool deadzone);
            double Timestamp { get; }
        }

        public class KeyNode : INode
        {
            public Input Input;
            public Keys Key;
            public bool Positive;

            public float Value(bool deadzone) => (Input.Keyboard.Down(Key) ? (Positive ? 1 : -1) : 0);
            public double Timestamp => Input.Keyboard.Timestamp(Key);

            public KeyNode(Input input, Keys key, bool positive)
            {
                Input = input;
                Key = key;
                Positive = positive;
            }
        }

        public class ButtonNode : INode
        {
            public Input Input;
            public int Index;
            public Buttons Button;
            public bool Positive;

            public float Value(bool deadzone) => (Input.Controllers[Index].Down(Button) ? (Positive ? 1 : -1) : 0);
            public double Timestamp => Input.Controllers[Index].Timestamp(Button);

            public ButtonNode(Input input, int controller, Buttons button, bool positive)
            {
                Input = input;
                Index = controller;
                Button = button;
                Positive = positive;
            }
        }

        public class AxisNode : INode
        {
            public Input Input;
            public int Index;
            public Axes Axis;
            public bool Positive;
            public float Deadzone;

            public float Value(bool deadzone)
            {
                if (!deadzone || Math.Abs(Input.Controllers[Index].Axis(Axis)) >= Deadzone)
                    return Input.Controllers[Index].Axis(Axis) * (Positive ? 1 : -1);
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
                Positive = positive;
            }
        }

        public float Value => GetValue(true);
        public float ValueNoDeadzone => GetValue(false);

        public int IntValue => Math.Sign(Value);
        public int IntValueNoDeadzone => Math.Sign(ValueNoDeadzone);

        public readonly Input Input;
        public readonly List<INode> Nodes = new List<INode>();
        public Overlaps OverlapBehaviour = Overlaps.CancelOut;

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
                foreach (var input in Nodes)
                    value += input.Value(deadzone);
                value = Calc.Clamp(value, -1, 1);
            }
            else if (OverlapBehaviour == Overlaps.TakeNewer)
            {
                var timestamp = 0d;
                for (int i = 0; i < Nodes.Count; i++)
                {
                    var time = Nodes[i].Timestamp;
                    var val = Nodes[i].Value(deadzone);

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
                for (int i = 0; i < Nodes.Count; i++)
                {
                    var time = Nodes[i].Timestamp;
                    var val = Nodes[i].Value(deadzone);

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
            Nodes.Add(new KeyNode(Input, negative, false));
            Nodes.Add(new KeyNode(Input, positive, true));
            return this;
        }

        public VirtualAxis Add(Keys key, bool isPositive)
        {
            Nodes.Add(new KeyNode(Input, key, isPositive));
            return this;
        }

        public VirtualAxis Add(int controller, Buttons negative, Buttons positive)
        {
            Nodes.Add(new ButtonNode(Input, controller, negative, false));
            Nodes.Add(new ButtonNode(Input, controller, positive, true));
            return this;
        }

        public VirtualAxis Add(int controller, Buttons button, bool isPositive)
        {
            Nodes.Add(new ButtonNode(Input, controller, button, isPositive));
            return this;
        }

        public VirtualAxis Add(int controller, Axes axis, float deadzone = 0f)
        {
            Nodes.Add(new AxisNode(Input, controller, axis, deadzone, true));
            return this;
        }

        public VirtualAxis Add(int controller, Axes axis, bool inverse, float deadzone = 0f)
        {
            Nodes.Add(new AxisNode(Input, controller, axis, deadzone, !inverse));
            return this;
        }

        public void Clear()
        {
            Nodes.Clear();
        }

    }
}
