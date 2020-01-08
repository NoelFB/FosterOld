using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A Core Application Module that is created earlier and has specific Application callbacks
    /// </summary>
    public abstract class AppModule : Module
    {

        protected AppModule(int priority = 10000) : base(priority)
        {

        }

        /// <summary>
        /// Called when Application is starting, before the Primary Window is created.
        /// </summary>
        protected internal virtual void ApplicationStarted() { }

        /// <summary>
        /// Called when the Module is created, before Startup but after the first Window is created
        /// </summary>
        protected internal virtual void FirstWindowCreated() { }

    }
}
