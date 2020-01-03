using System;

namespace Foster.Framework
{
    /// <summary>
    /// An OpenGL Graphics Context
    /// </summary>
    public abstract class GLContext : IDisposable
    {
        /// <summary>
        /// Whether the Context is Disposed
        /// </summary>
        public abstract bool IsDisposed { get; }

        /// <summary>
        /// Didposes the Context
        /// </summary>
        public abstract void Dispose();
    }
}
