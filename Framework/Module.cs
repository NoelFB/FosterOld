
using System.Collections;
using System.Collections.Generic;

namespace Foster.Framework
{
    public abstract class Module
    {

        /// <summary>
        /// A lower priority is run first
        /// </summary>
        protected internal readonly int Priority;

        /// <summary>
        /// The Application Main Thread ID
        /// </summary>
        public int MainThreadId { get; internal set; }

        /// <summary>
        /// Whether the Module has been Registered by the Application
        /// </summary>
        public bool Registered { get; internal set; }

        /// <summary>
        /// Creates the Module with the given Priority
        /// </summary>
        protected Module(int priority = 10000)
        {
            Priority = priority;
        }

        /// <summary>
        /// Called immediately when the Module has been registered
        /// </summary>
        protected internal virtual void Initialized() { }

        /// <summary>
        /// Called when the Application begins
        /// If the Application has already started when the module is registered, this will be called immediately
        /// </summary>
        protected internal virtual void Startup() { }

        /// <summary>
        /// Called when the Application shuts down, or the Module is Removed
        /// </summary>
        protected internal virtual void Shutdown() { }

        /// <summary>
        /// Called every frame before the Update function
        /// </summary>
        protected internal virtual void BeforeUpdate() { }

        /// <summary>
        /// Called every frame
        /// </summary>
        protected internal virtual void Update() { }

        /// <summary>
        /// Called every frame after the Update function
        /// </summary>
        protected internal virtual void AfterUpdate() { }

        /// <summary>
        /// Called when a Window is being rendered to, before the Window.OnRender callback
        /// </summary>
        protected internal virtual void BeforeRender(Window window) { }

        /// <summary>
        /// Called when a Window is being rendered to, after the Window.OnRender callback
        /// </summary>
        protected internal virtual void AfterRender(Window window) { }

        /// <summary>
        /// Called every Tick of the Application
        /// Multiple update functions can be called per tick
        /// </summary>
        protected internal virtual void Tick() { }

    }
}
