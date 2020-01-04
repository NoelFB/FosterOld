using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// An Implementation of the System Module that supports the OpenGL Graphics API
    /// </summary>
    public interface ISystemOpenGL
    {
        /// <summary>
        /// An OpenGL Graphics Context
        /// </summary>
        public abstract class Context : IDisposable
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

        /// <summary>
        /// Gets a pointer to a OpenGL function
        /// </summary>
        IntPtr GetGLProcAddress(string name);

        /// <summary>
        /// Creates a new Graphics Context. It does not make it Current.
        /// </summary>
        Context CreateGLContext();

        /// <summary>
        /// Gets the Context associated with a Window
        /// </summary>
        Context GetWindowGLContext(Window window);

        /// <summary>
        /// Gets the current Graphics Context on the Active Thread
        /// </summary>
        Context? GetCurrentGLContext();

        /// <summary>
        /// Sets the current Graphics Context on the Active Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        void SetCurrentGLContext(Context? context);
    }
}
