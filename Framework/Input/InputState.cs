using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public class InputState
    {
        public const int MaxControllers = 32;

        public readonly Keyboard Keyboard;
        public readonly Mouse Mouse;

        private readonly Controller[] controllers;
        public readonly ReadOnlyCollection<Controller> Controllers;

        public InputState()
        {
            controllers = new Controller[MaxControllers];
            for (int i = 0; i < controllers.Length; i++)
                controllers[i] = new Controller();
            Keyboard = new Keyboard();
            Mouse = new Mouse();
            Controllers = new ReadOnlyCollection<Controller>(controllers);
        }

        internal void Step()
        {
            for (int i = 0; i < Controllers.Count; i++)
                Controllers[i].Step();
            Keyboard.Step();
            Mouse.Step();
        }

        internal void Copy(InputState other)
        {
            for (int i = 0; i < Controllers.Count; i++)
                Controllers[i].Copy(other.Controllers[i]);
            Keyboard.Copy(other.Keyboard);
            Mouse.Copy(other.Mouse);
        }
    }
}
