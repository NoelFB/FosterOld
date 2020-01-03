using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Graphics Context which stores the graphical state for rendering.
    /// Not all Graphics Module implementations will need to use this, but OpenGL does.
    /// Note that a Context can only be Current on one thread at a time.
    /// </summary>
    public abstract class GraphicsContext : IDisposable
    {
        /// <summary>
        /// The Thread this Context is active on
        /// </summary>
        public int ActiveThreadId { get; internal set; } = 0;

        /// <summary>
        /// The Graphics Device this Context belongs to
        /// </summary>
        protected internal readonly GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Whether the Context has been disposed
        /// </summary>
        public abstract bool Disposed { get; }

        protected GraphicsContext(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Didposes the Context
        /// </summary>
        public abstract void Dispose();
    }
}
