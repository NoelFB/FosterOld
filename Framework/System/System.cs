using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class System : Module
    {

        /// <summary>
        /// Underlying System implementation API Name
        /// </summary>
        public string? ApiName;

        /// <summary>
        /// Underlying System implementation API Version
        /// </summary>
        public Version? ApiVersion;

        /// <summary>
        /// The Active Window
        /// </summary>
        public abstract Window? Window { get; }

        /// <summary>
        /// The Active Rendering Context
        /// </summary>
        public abstract Context? Context { get; }

        /// <summary>
        /// Sets the Active Window on the current Thread
        /// </summary>
        public abstract void SetActiveWindow(Window? window);

        /// <summary>
        /// Sets the Actve Rendering Context on the current Thread
        /// </summary>
        public abstract void SetActiveContext(Context? context);

        /// <summary>
        /// Creates a new Window
        /// </summary>
        public abstract Window CreateWindow(string title, int width, int height, bool visible = true);

        /// <summary>
        /// A list of all opened Windows
        /// </summary>
        public readonly ReadOnlyCollection<Window> Windows;

        /// <summary>
        /// Gets a Pointer to a Platform rendering method of the given name
        /// This is used internally by the Graphics
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IntPtr GetProcAddress(string name);

        protected List<Window> windows = new List<Window>();

        protected System()
        {
            Windows = windows.AsReadOnly();
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

    }
}
