using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Context : IDisposable
    {

        /// <summary>
        /// The System this Context belongs to
        /// </summary>
        public abstract System System { get; }

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

        protected Context()
        {

        }

        /// <summary>
        /// Sets this as the current Rendering Context on the current Thread
        /// Note that this will fail if the context is current on another thread
        /// </summary>
        public void MakeCurrent()
        {
            System.SetCurrentContext(this);
        }

        /// <summary>
        /// Didposes the Context
        /// </summary>
        public abstract void Dispose();
    }
}
