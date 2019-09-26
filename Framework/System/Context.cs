using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Context : IDisposable
    {
        public abstract System System { get; }
        public abstract bool Disposed { get; }
        public abstract int BackbufferWidth { get; }
        public abstract int BackbufferHeight { get; }

        protected Context()
        {

        }

        public abstract void Dispose();
    }
}
