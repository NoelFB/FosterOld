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
        /// System Input
        public abstract Input Input { get; }

        /// <summary>
        /// The application directory
        /// </summary>
        public virtual string Directory => AppDomain.CurrentDomain?.BaseDirectory ?? "";

        /// <summary>
        /// Internal list of all Windows owned by the System. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<Window> windows = new List<Window>();

        /// <summary>
        /// Internal list of all Monitors owned by the System. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<Monitor> monitors = new List<Monitor>();

        protected System() : base(100)
        {
            Windows = new ReadOnlyCollection<Window>(windows);
            Monitors = new ReadOnlyCollection<Monitor>(monitors);
        }

        /// <summary>
        /// Creates a new Window. This must be called from the Main Thread.
        /// 
        /// Not every Platform supports multiple Windows, in which case
        /// creating more than one will throw an exception. You can check whether multiple
        /// Windows is supported with System.SupportsMultipleWindows.
        /// </summary>
        public abstract Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None);

        /// <summary>
        /// Whether the System can support the given Graphics API
        /// </summary>
        public abstract bool SupportsGraphicsApi(GraphicsApi api);

        /// <summary>
        /// Gets an OpenGL Graphics Device and returns null if the System does not support it.
        /// This is internal as it should only be used by the Graphics Module
        /// </summary>
        protected internal virtual GLDevice? GetOpenGLGraphicsDevice() => null;

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");

            if (!SupportsGraphicsApi(App.Graphics.Api))
                throw new Exception($"System Module doesn't support Graphics API {App.Graphics.Api}");
        }

        protected internal override void BeforeUpdate()
        {
            Input.BeforeUpdate();
        }

        protected internal override void Shutdown()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion} : Shutdown");
        }

    }
}
