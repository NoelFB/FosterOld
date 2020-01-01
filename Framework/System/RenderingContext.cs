using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Graphics Context to use for drawing, usually associated with a Window
    /// </summary>
    public abstract class RenderingContext : IDisposable
    {

        /// <summary>
        /// The Thread this Context is active on
        /// </summary>
        public int ActiveThreadId { get; internal set; } = 0;

        /// <summary>
        /// The System this Context belongs to
        /// </summary>
        public readonly RenderingState RenderingState;

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

        protected RenderingContext(RenderingState state)
        {
            RenderingState = state;
        }

        /// <summary>
        /// Sets this as the current Rendering Context on the current Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public void MakeCurrent()
        {
            RenderingState.SetCurrentContext(this);
        }

        /// <summary>
        /// Unsets this Context from the current Thread, if it is currently Current
        /// </summary>
        public void MakeNotCurrent()
        {
            if (RenderingState.GetCurrentContext() == this)
                RenderingState.SetCurrentContext(null);
        }

        /// <summary>
        /// Didposes the Context
        /// </summary>
        public void Dispose()
        {
            if (RenderingState.GetCurrentContext() == this)
                RenderingState.SetCurrentContext(null);

            DisposeContextResources();
        }

        protected abstract void DisposeContextResources();
    }
}
