using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework.Internal
{
    public abstract class InternalResource
    {

        public bool IsDisposed { get; private set; }

        protected internal void Dispose()
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
