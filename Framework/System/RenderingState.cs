using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Foster.Framework
{
    public abstract class RenderingState
    {
        /// <summary>
        /// The System this RenderingState belongs to
        /// </summary>
        public readonly System System;

        /// <summary>
        /// A list of all the Rendering Contexts
        /// </summary>
        public readonly ReadOnlyCollection<RenderingContext> Contexts;

        /// <summary>
        /// Internal list of all Contexts owned by the Rendering State. The Platform implementation should maintain this.
        /// </summary>
        protected readonly List<RenderingContext> contexts = new List<RenderingContext>();

        protected RenderingState(System system)
        {
            System = system;
            Contexts = new ReadOnlyCollection<RenderingContext>(contexts);
        }

        /// <summary>
        /// Creates a new RenderingContext. This does not make the new RenderingContext Current (you must call MakeCurrent())
        /// </summary>
        public abstract RenderingContext CreateContext();

        /// <summary>
        /// Gets the current Rendering Context on the current Thread
        /// </summary>
        public RenderingContext? GetCurrentContext()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            for (int i = 0; i < Contexts.Count; i++)
                if (Contexts[i].ActiveThreadId == threadId)
                    return Contexts[i];

            return null;
        }

        /// <summary>
        /// Sets the current Rendering Context on the current Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public void SetCurrentContext(RenderingContext? context)
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
                    throw new Exception("The Context is active on another Thread. A Context can only be current for a single Thread at a time. You must make it non-current on the old Thread before making it current on another.");

                context.ActiveThreadId = Thread.CurrentThread.ManagedThreadId;
                SetCurrentContextInternal(context);
            }
            else
            {
                SetCurrentContextInternal(null);
            }
        }

        /// <summary>
        /// Sets the current Rendering Context on the current Thread
        /// </summary>
        protected abstract void SetCurrentContextInternal(RenderingContext? context);


    }
}
