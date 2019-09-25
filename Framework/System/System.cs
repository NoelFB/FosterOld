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
        public string? ApiName { get; protected set; }

        /// <summary>
        /// Underlying System implementation API Version
        /// </summary>
        public Version? ApiVersion { get; protected set; }

        /// <summary>
        /// The Active Window
        /// </summary>
        public abstract Window? ActiveWindow { get; set; }

        /// <summary>
        /// The Active Rendering Context
        /// </summary>
        public abstract Context? ActiveContext { get; set; }

        /// <summary>
        /// Whether the System can support Multiple Windows
        /// </summary>
        public abstract bool SupportsMultipleWindows { get; }

        /// <summary>
        /// A list of all opened Windows
        /// </summary>
        public readonly ReadOnlyCollection<Window> Windows;
        
        /// <summary>
        /// Creates a new Window
        /// </summary>
        public abstract Window CreateWindow(string title, int width, int height, bool visible = true);

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
            Priority = 100;
            Windows = windows.AsReadOnly();
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

    }
}
