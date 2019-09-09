using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Module
    {

        protected internal virtual void Created() { }
        protected internal virtual void Startup() { }
        protected internal virtual void Displayed() { }
        protected internal virtual void Shutdown() { }
        protected internal virtual void Destroyed() { }
        protected internal virtual void PreUpdate() { }
        protected internal virtual void PostUpdate() { }

    }
}
