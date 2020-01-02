using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class GraphicsResource
    {

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                DisposeResources();
                IsDisposed = true;
            }
        }

        protected abstract void DisposeResources();
    }
}
