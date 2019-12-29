
using System.Collections;
using System.Collections.Generic;

namespace Foster.Framework
{
    public abstract class Module
    {

        /// <summary>
        /// A lower priority is run first
        /// </summary>
        protected internal int Priority { get; private set; }

        /// <summary>
        /// The Application Main Thread ID
        /// </summary>
        protected internal int MainThreadId { get; internal set; }

        /// <summary>
        /// Whether the Module has been Registered by the Application
        /// </summary>
        protected internal bool Registered { get; internal set; }

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
        protected internal virtual void Update()
        {
            for (int i = 0; i < routines.Count; i ++)
            {
                routines[i].Update();
                if (routines[i].Finished)
                    routines.RemoveAt(i--);
            }
        }

        /// <summary>
        /// Called every frame after the Update function
        /// </summary>
        protected internal virtual void AfterUpdate() { }

        /// <summary>
        /// Called when the Rendering Context in the System has changed
        /// </summary>
        protected internal virtual void ContextChanged(Context context) { }

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

        private readonly List<Coroutine> routines = new List<Coroutine>();

        protected Module(int priority = 10000)
        {
            Priority = priority;
        }

        public void RunRoutine(Coroutine routine)
        {
            routines.Add(routine);
        }

        public void RunRoutine(IEnumerator routine)
        {
            routines.Add(new Coroutine(routine));
        }

    }
}
