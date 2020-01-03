using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Foster.Framework
{

    /// <summary>
    /// Represents a System Graphics Device, used for managing Graphics State & Contexts
    /// </summary>
    public abstract class GraphicsDevice
    {

        /// <summary>
        /// Internal list of all Graphics Contexts owned by the Graphics Device. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<GraphicsContext> contexts = new List<GraphicsContext>();

        /// <summary>
        /// A list of all the Graphics Contexts
        /// </summary>
        public readonly ReadOnlyCollection<GraphicsContext> Contexts;

        /// <summary>
        /// The System this Graphics Device belongs to
        /// </summary>
        public readonly System System;

        protected GraphicsDevice(System system)
        {
            System = system;
            Contexts = new ReadOnlyCollection<GraphicsContext>(contexts);
        }

        /// <summary>
        /// Gets a Pointer to a Platform rendering method of the given name
        /// This is used internally by the Graphics Module
        /// </summary>
        public abstract IntPtr GetProcAddress(string name);

        /// <summary>
        /// Creates a new Graphics Context. It does not make it Current
        /// </summary>
        public abstract GraphicsContext CreateContext();

        /// <summary>
        /// Gets the current Graphics Context on the Active Thread
        /// </summary>
        public GraphicsContext? GetCurrentContext()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            for (int i = 0; i < Contexts.Count; i++)
                if (Contexts[i].ActiveThreadId == threadId)
                    return Contexts[i];

            return null;
        }

        /// <summary>
        /// Sets the current Graphics Context on the Active Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public void SetCurrentContext(GraphicsContext? context)
        {
            // context is already set on this thread
            if (context != null && context.ActiveThreadId == Thread.CurrentThread.ManagedThreadId)
                return;

            // unset existing context
            {
                var current = GetCurrentContext();
                if (current != null)
                    current.ActiveThreadId = 0;
            }

            if (context != null)
            {
                if (context.Disposed)
                    throw new Exception("The Context is Disposed");

                // currently assigned to a different thread
                if (context.ActiveThreadId != 0)
                    throw new Exception("The Context is active on another Thread. A Context can only be current for a single Thread at a time. You must make it non-current on the old Thread before making setting it on another.");

                context.ActiveThreadId = Thread.CurrentThread.ManagedThreadId;
                SetCurrentContextInternal(context);
            }
            else
            {
                SetCurrentContextInternal(null);
            }
        }

        /// <summary>
        /// Sets the current Graphics Context on the current Thread
        /// </summary>
        protected abstract void SetCurrentContextInternal(GraphicsContext? context);

    }
}
