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
        public readonly Input Input;

        /// <summary>
        /// System Graphics Device
        /// We keep this Internal as the Graphics Module is really the only thing that should be touching this
        /// </summary>
        protected internal readonly GraphicsDevice GraphicsDevice;

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

            Input = CreateInput();
            GraphicsDevice = CreateGraphicsDevice();
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

        protected internal override void BeforeUpdate()
        {
            Input.BeforeUpdate();
        }

        protected internal override void Shutdown()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion} : Shutdown");
        }

        /// <summary>
        /// Creates a new Window. This must be called from the Main Thread.
        /// </summary>
        public abstract Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None);

        /// <summary>
        /// Creates the Input Manager
        /// </summary>
        protected abstract Input CreateInput();

        /// <summary>
        /// Creates the Graphics Device
        /// </summary>
        protected abstract GraphicsDevice CreateGraphicsDevice();

    }
}
