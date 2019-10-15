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
        public string ApiName { get; protected set; } = "Unknown";

        /// <summary>
        /// Underlying System implementation API Version
        /// </summary>
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);

        /// <summary>
        /// Whether the System can support Multiple Windows
        /// </summary>
        public abstract bool SupportsMultipleWindows { get; }

        /// <summary>
        /// A list of all opened Windows
        /// </summary>
        public readonly ReadOnlyCollection<Window> Windows;
        
        /// <summary>
        /// Creates a new Window. This must be called from the Main Thread.
        /// </summary>
        public abstract Window CreateWindow(string title, int width, int height, bool visible = true);

        /// <summary>
        /// Creates a new Rendering Context. This must be called from the Main Thread.
        /// </summary>
        /// <returns></returns>
        public abstract Context CreateContext();

        /// <summary>
        /// Gets the current Rendering Context on the current Thread
        /// </summary>
        /// <returns></returns>
        public abstract Context? GetCurrentContext();

        /// <summary>
        /// Sets the current Rendering Context on the current Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        /// <param name="context"></param>
        public abstract void SetCurrentContext(Context? context);

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
