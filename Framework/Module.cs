
using System.Collections;
using System.Collections.Generic;

namespace Foster.Framework
{
    /// <summary>
    /// Foster Module, used to get callbacks during Application Events
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// The Name of the Module
        /// </summary>
        public string Name;

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
        public bool IsRegistered { get; internal set; }

        /// <summary>
        /// Whether the Module has been Started by the Application
        /// </summary>
        public bool IsStarted { get; internal set; }

        /// <summary>
        /// Creates the Module with the given Priority
        /// </summary>
        protected Module(int priority = 10000)
        {
            Name = GetType().Name;
            Priority = priority;
        }

        /// <summary>
        /// Called when the Application begins, after the Primary Window is created.
        /// If the Application has already started when the module is Registered, this will be called immediately.
        /// </summary>
        protected internal virtual void Startup() { }

        /// <summary>
        /// Called when the Application shuts down, or the Module is Removed
        /// </summary>
        protected internal virtual void Shutdown() { }

        /// <summary>
        /// Called after the Shutdown method when the Module should be fully Disposed
        /// </summary>
        protected internal virtual void Disposed() { }

        /// <summary>
        /// Called at the start of the frame, before Update or Fixed Update.
        /// </summary>
        protected internal virtual void FrameStart() { }

        /// <summary>
        /// Called every fixed step
        /// </summary>
        protected internal virtual void FixedUpdate() { }

        /// <summary>
        /// Called every variable step
        /// </summary>
        protected internal virtual void Update() { }

        /// <summary>
        /// Called at the end of the frame, after Update and Fixed Update.
        /// </summary>
        protected internal virtual void FrameEnd() { }

        /// <summary>
        /// Called before any rendering
        /// </summary>
        protected internal virtual void BeforeRender() { }

        /// <summary>
        /// Called when a Window is being rendered to, before the Window.OnRender callback
        /// </summary>
        protected internal virtual void BeforeRenderWindow(Window window) { }

        /// <summary>
        /// Called when a Window is being rendered to, after the Window.OnRender callback
        /// </summary>
        protected internal virtual void AfterRenderWindow(Window window) { }

        /// <summary>
        /// Called after all rendering
        /// </summary>
        protected internal virtual void AfterRender() { }

    }
}
