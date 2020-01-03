using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Foster.Framework
{

    /// <summary>
    /// Represents en OpenGL Graphics Device, used for handling OpenGL Contexts
    /// </summary>
    public abstract class GLDevice
    {
        /// <summary>
        /// Gets a Pointer to a Platform rendering method of the given name
        /// This is used internally by the Graphics Module
        /// </summary>
        public abstract IntPtr GetProcAddress(string name);

        /// <summary>
        /// Creates a new Graphics Context. It does not make it Current
        /// </summary>
        public abstract GLContext CreateContext();

        /// <summary>
        /// Gets the Context associated with a Window
        /// </summary>
        public abstract GLContext GetWindowContext(Window window);

        /// <summary>
        /// Gets the current Graphics Context on the Active Thread
        /// </summary>
        public abstract GLContext? GetCurrentContext();

        /// <summary>
        /// Sets the current Graphics Context on the Active Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public abstract void SetCurrentContext(GLContext? context);

    }
}
