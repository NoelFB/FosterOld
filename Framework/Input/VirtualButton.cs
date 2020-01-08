using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    /// <summary>
    /// A Virtual Input Button that can be mapped to different keyboards and gamepads
    /// </summary>
    public class VirtualButton
    {
        private interface INode
        {
            bool Pressed(float buffer, long lastBufferConsumedTime);
            bool Down { get; }
            bool Released { get; }
            bool Repeated(float delay, float interval);
            void Update();
        }

        private class KeyNode : INode
        {
            public Input Input;
            public Keys Key;

            public bool Pressed(float buffer, long lastBufferConsumedTime)
            {
                if (Input.Keyboard.Pressed(Key))
                    return true;

                var timestamp = Input.Keyboard.Timestamp(Key);
                var time = Time.Duration.Ticks;
                
                if (time - timestamp <= TimeSpan.FromSeconds(buffer).Ticks && timestamp > lastBufferConsumedTime)
                    return true;

                return false;
            }

            public bool Down => Input.Keyboard.Down(Key);
            public bool Released => Input.Keyboard.Released(Key);
            public bool Repeated(float delay, float interval) => Input.Keyboard.Repeated(Key, delay, interval);
            public void Update() { }

            public KeyNode(Input input, Keys key)
            {
                Input = input;
                Key = key;
            }
        }

        private class ButtonNode : INode
        {
            public Input Input;
            public int Index;
            public Buttons Button;

            public bool Pressed(float buffer, long lastBufferConsumedTime)
            {
                if (Input.Controllers[Index].Pressed(Button))
                    return true;

                var timestamp = Input.Controllers[Index].Timestamp(Button);
                var time = Time.Duration.Ticks;

                if (time - timestamp <= TimeSpan.FromSeconds(buffer).Ticks && timestamp > lastBufferConsumedTime)
                    return true;

                return false;
            }

            public bool Down => Input.Controllers[Index].Down(Button);
            public bool Released => Input.Controllers[Index].Released(Button);
            public bool Repeated(float delay, float interval) => Input.Controllers[Index].Repeated(Button, delay, interval);
            public void Update() { }

            public ButtonNode(Input input, int controller, Buttons button)
            {
                Input = input;
                Index = controller;
                Button = button;
            }
        }

        private class AxisNode : INode
        {
            public Input Input;
            public int Index;
            public Axes Axis;
            public float Threshold;

            private float pressedTimestamp;

            private const float AXIS_EPSILON = 0.00001f;

            public bool Pressed(float buffer, long lastBufferConsumedTime)
            {
                if (Pressed())
                    return true;

                var time = Time.Duration.Ticks;

                if (time - pressedTimestamp <= TimeSpan.FromSeconds(buffer).Ticks && pressedTimestamp > lastBufferConsumedTime)
                    return true;

                return false;
            }

            public bool Down
            {
                get
                {
                    if (Math.Abs(Threshold) <= AXIS_EPSILON)
                        return Math.Abs(Input.Controllers[Index].Axis(Axis)) > AXIS_EPSILON;

                    if (Threshold < 0)
                        return Input.Controllers[Index].Axis(Axis) <= Threshold;
                    
                    return Input.Controllers[Index].Axis(Axis) >= Threshold;
                }
            }

            public bool Released
            {
                get
                {
                    if (Math.Abs(Threshold) <= AXIS_EPSILON)
                        return Math.Abs(Input.LastState.Controllers[Index].Axis(Axis)) > AXIS_EPSILON && Math.Abs(Input.Controllers[Index].Axis(Axis)) < AXIS_EPSILON;

                    if (Threshold < 0)
                        return Input.LastState.Controllers[Index].Axis(Axis) <= Threshold && Input.Controllers[Index].Axis(Axis) > Threshold;

                    return Input.LastState.Controllers[Index].Axis(Axis) >= Threshold && Input.Controllers[Index].Axis(Axis) < Threshold;
                }
            }

            public bool Repeated(float delay, float interval)
            {
                throw new NotImplementedException();
            }

            private bool Pressed()
            {
                if (Math.Abs(Threshold) <= AXIS_EPSILON)
                    return (Math.Abs(Input.LastState.Controllers[Index].Axis(Axis)) < AXIS_EPSILON && Math.Abs(Input.Controllers[Index].Axis(Axis)) > AXIS_EPSILON);

                if (Threshold < 0)
                    return (Input.LastState.Controllers[Index].Axis(Axis) > Threshold && Input.Controllers[Index].Axis(Axis) <= Threshold);
                
                return (Input.LastState.Controllers[Index].Axis(Axis) < Threshold && Input.Controllers[Index].Axis(Axis) >= Threshold);
            }

            public void Update()
            {
                if (Pressed())
                    pressedTimestamp = Input.Controllers[Index].Timestamp(Axis);
            }

            public AxisNode(Input input, int controller, Axes axis, float threshold)
            {
                Input = input;
                Index = controller;
                Axis = axis;
                Threshold = threshold;
            }
        }

        private readonly List<INode> nodes = new List<INode>();
        private long lastBufferConsumeTime;

        public readonly Input Input;
        public float RepeatDelay;
        public float RepeatInterval;
        public float Buffer;

        public bool Pressed
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].Pressed(Buffer, lastBufferConsumeTime))
                        return true;

                return false;
            }
        }

        public bool Down
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].Down)
                        return true;

                return false;
            }
        }

        public bool Released
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].Released)
                        return true;

                return false;
            }
        }

        public bool Repeated
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].Pressed(Buffer, lastBufferConsumeTime) || nodes[i].Repeated(RepeatDelay, RepeatInterval))
                        return true;

                return false;
            }
        }

        public VirtualButton(Input input, float buffer = 0f)
        {
            Input = input;

            // Using a Weak Reference to subscribe this object to Updates
            // This way it's automatically collected if the user is no longer
            // using it, and we don't require the user to call a Dispose or 
            // Unsubscribe callback
            Input.virtualButtons.Add(new WeakReference<VirtualButton>(this));

            RepeatDelay = Input.RepeatDelay;
            RepeatInterval = Input.RepeatInterval;
            Buffer = buffer;
        }

        public void ConsumeBuffer()
        {
            lastBufferConsumeTime = Time.Duration.Ticks;
        }

        public VirtualButton Add(params Keys[] keys)
        {
            foreach (var key in keys)
                nodes.Add(new KeyNode(Input, key));
            return this;
        }

        public VirtualButton Add(int controller, params Buttons[] buttons)
        {
            foreach (var button in buttons)
                nodes.Add(new ButtonNode(Input, controller, button));
            return this;
        }

        public VirtualButton Add(int controller, Axes axis, float threshold)
        {
            nodes.Add(new AxisNode(Input, controller, axis, threshold));
            return this;
        }

        internal void Update()
        {
            foreach (var node in nodes)
                node.Update();
        }

    }
}
