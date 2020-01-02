using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Graphics Context to which Windows and Off-screen buffers use for drawing.
    /// Note that a Context can only be active on one thread at a time.
    /// </summary>
    public abstract class Context : IDisposable
    {

        /// <summary>
        /// The Thread this Context is active on
        /// </summary>
        public int ActiveThreadId { get; internal set; } = 0;

        /// <summary>
        /// The System this Context belongs to
        /// </summary>
        public readonly System System;

        /// <summary>
        /// Whether the Context has been disposed
        /// </summary>
        public abstract bool Disposed { get; }

        /// <summary>
        /// The Width of the Context, in Pixels
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// The Height of the Context, in Pixels
        /// </summary>
        public abstract int Height { get; }

        protected Context(System system)
        {
            System = system;
        }

        public bool IsCurrent()
        {
            return System.GetCurrentContext() == this;
        }

        /// <summary>
        /// Sets this as the current Rendering Context on the current Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public void MakeCurrent()
        {
            System.SetCurrentContext(this);
        }

        public void MakeNonCurrent()
        {
            if (System.GetCurrentContext() == this)
                System.SetCurrentContext(null);
        }

        /// <summary>
        /// Didposes the Context
        /// </summary>
        public abstract void Dispose();
    }
}
