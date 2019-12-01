using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class GraphicsResource : IDisposable
    {
        public readonly Graphics Graphics;
        public bool Disposed { get; private set; }

        public GraphicsResource(Graphics graphics)
        {
            Graphics = graphics;
        }

        ~GraphicsResource()
        {
            if (!Disposed)
                Dispose();
        }

        public virtual void Dispose()
        {
            Disposed = true;
        }

    }
}
