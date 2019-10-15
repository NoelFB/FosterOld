using System;
using System.Collections.ObjectModel;

namespace Foster.Framework
{

    public abstract class Input : Module
    {
        public string? ApiName { get; protected set; }
        public Version? ApiVersion { get; protected set; }

        public InputState State { get; } = new InputState();
        public InputState LastState { get; } = new InputState();
        private readonly InputState nextState = new InputState();

        protected Input()
        {
            Priority = 300;
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Input {ApiName} {ApiVersion}");
        }

        protected internal override void BeforeUpdate()
        {
            LastState.Copy(State);
            State.Copy(nextState);
            nextState.Step();
        }

        protected void OnText(char value)
        {
            nextState.Keyboard.Text.Append(value);
        }

        protected void OnKeyDown(uint key, ulong timestamp)
        {
            nextState.Keyboard.down[key] = true;
            nextState.Keyboard.pressed[key] = true;
            nextState.Keyboard.timestamp[key] = timestamp;
        }

        protected void OnKeyUp(uint key, ulong timestamp)
        {
            nextState.Keyboard.down[key] = false;
            nextState.Keyboard.released[key] = true;
        }
        
        protected void OnMouseDown(MouseButtons button, ulong timestamp)
        {
            nextState.Mouse.down[(int)button] = true;
            nextState.Mouse.pressed[(int)button] = true;
            nextState.Mouse.timestamp[(int)button] = timestamp;
        }

        protected void OnMouseUp(MouseButtons button, ulong timestamp)
        {
            nextState.Mouse.down[(int)button] = false;
            nextState.Mouse.released[(int)button] = true;
        }

        protected void OnJoystickConnect(uint index, string name, uint buttonCount, uint axisCount, bool isGamepad)
        {
            if (index < InputState.MaxControllers)
                nextState.Controllers[(int)index].Connect(name, buttonCount, axisCount, isGamepad);
        }

        protected void OnJoystickDisconnect(uint index)
        {
            if (index < InputState.MaxControllers)
                nextState.Controllers[(int)index].Disconnect();
        }

        protected void OnJoystickButtonDown(uint index, uint button, ulong timestamp)
        {
            if (index < InputState.MaxControllers)
            {
                nextState.Controllers[(int)index].down[button] = true;
                nextState.Controllers[(int)index].pressed[button] = true;
                nextState.Controllers[(int)index].timestamp[button] = timestamp;
            }
        }

        protected void OnJoystickButtonUp(uint index, uint button, ulong timestamp)
        {
            if (index < InputState.MaxControllers)
            {
                nextState.Controllers[(int)index].down[button] = false;
                nextState.Controllers[(int)index].released[button] = true;
            }
        }

        protected void OnJoystickAxis(uint index, uint axis, float value, ulong timestamp)
        {
            if (index < InputState.MaxControllers)
            {
                nextState.Controllers[(int)index].axis[axis] = value;
                nextState.Controllers[(int)index].axisTimestamp[axis] = timestamp;
            }
        }

    }
}
