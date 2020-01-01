using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

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
        /// A list of active Monitors
        /// </summary>
        public readonly ReadOnlyCollection<Monitor> Monitors;

        /// <summary>
        /// The System Render State
        /// This is internal because it should really only be used by the Graphics class.
        /// Ex. OpenGL needs to handle Contexts, but nothing else should really touch this.
        /// </summary>
        protected internal readonly RenderingState RenderingState;

        /// <summary>
        /// Creates a new Window. This must be called from the Main Thread.
        /// Note that on High DPI displays the given width and height may not match
        /// the resulting Window size.
        /// </summary>
        public Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (MainThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new Exception("Creating a Window can only be done from the Main Thread");

            var window = CreateWindowInternal(title, width, height, flags);
            window.Context.MakeCurrent();

            return window;
        }

        protected abstract Window CreateWindowInternal(string title, int width, int height, WindowFlags flags = WindowFlags.None);

        /// <summary>
        /// The application directory
        /// </summary>
        public virtual string Directory => AppDomain.CurrentDomain?.BaseDirectory ?? "";

        /// <summary>
        /// Gets a Pointer to a Platform rendering method of the given name
        /// This is used internally by the Graphics
        /// </summary>
        public abstract IntPtr GetProcAddress(string name);

        /// <summary>
        /// Internal list of all Windows owned by the System. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<Window> windows = new List<Window>();

        /// <summary>
        /// Internal list of all Monitors owned by the System. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<Monitor> monitors = new List<Monitor>();

        /// <summary>
        /// Creates the Rendering State
        /// </summary>
        protected abstract RenderingState CreateRenderingState();

        protected System() : base(100)
        {
            Windows = new ReadOnlyCollection<Window>(windows);
            Monitors = new ReadOnlyCollection<Monitor>(monitors);
            RenderingState = CreateRenderingState();
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

        protected internal override void Shutdown()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion} : Shutdown");
        }

    }
}
