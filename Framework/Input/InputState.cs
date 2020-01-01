using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public class InputState
    {
        /// <summary>
        /// The Maximum number of Controllers
        /// </summary>
        public const int MaxControllers = 32;

        /// <summary>
        /// Our Input Module
        /// </summary>
        public readonly Input Input;

        /// <summary>
        /// The Keyboard State
        /// </summary>
        public readonly Keyboard Keyboard;

        /// <summary>
        /// The Mouse State
        /// </summary>
        public readonly Mouse Mouse;

        /// <summary>
        /// A list of all the Controllers
        /// </summary>
        private readonly Controller[] controllers;

        /// <summary>
        /// A Read-Only Collection of the Controllers
        /// Note that they aren't necessarily connected
        /// </summary>
        public readonly ReadOnlyCollection<Controller> Controllers;

        public InputState(Input input)
        {
            Input = input;

            controllers = new Controller[MaxControllers];
            for (int i = 0; i < controllers.Length; i++)
                controllers[i] = new Controller(input);

            Controllers = new ReadOnlyCollection<Controller>(controllers);
            Keyboard = new Keyboard(input);
            Mouse = new Mouse();
        }

        internal void Step()
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                if (Controllers[i].Connected)
                    Controllers[i].Step();
            }
            Keyboard.Step();
            Mouse.Step();
        }

        internal void Copy(InputState other)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (other.controllers[i].Connected || (controllers[i].Connected != other.controllers[i].Connected))
                    controllers[i].Copy(other.controllers[i]);
            }

            Keyboard.Copy(other.Keyboard);
            Mouse.Copy(other.Mouse);
        }
    }
}
