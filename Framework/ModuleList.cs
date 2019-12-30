using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Foster.Framework
{
    public class ModuleList : IEnumerable<Module>
    {

        private readonly List<Module?> modules = new List<Module?>();
        private readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();
        private bool isRendering;

        /// <summary>
        /// Registers a Module
        /// </summary>
        public T Register<T>() where T : Module
        {
            return Register(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Registers a Module
        /// </summary>
        public Module Register(Type type)
        {
            if (!(Activator.CreateInstance(type) is Module module))
                throw new Exception("Type must inheirt from Module");

            return Register(module);
        }

        /// <summary>
        /// Registers a Module
        /// </summary>
        public T Register<T>(T module) where T : Module
        {
            if (isRendering)
                throw new Exception("Cannot Add or Remove Modules during Rendering");

            if (module.Registered)
                throw new Exception("Module is already registered");

            // add Module to lookup
            var type = module.GetType();
            while (type != typeof(Module))
            {
                if (!modulesByType.ContainsKey(type))
                    modulesByType[type] = module;

                if (type.BaseType == null)
                    break;

                type = type.BaseType;
            }

            // insert in order
            var insert = 0;
            while (insert < modules.Count && (modules[insert]?.Priority ?? int.MinValue) <= module.Priority)
                insert++;
            modules.Insert(insert, module);

            // registered
            module.Registered = true;
            module.MainThreadId = Thread.CurrentThread.ManagedThreadId;
            module.Initialized();

            if (App.Running)
                module.Startup();

            return module;
        }

        /// <summary>
        /// Removes a Module
        /// Note: Removing core modules (such as System) will make everything break
        /// </summary>
        public void Remove(Module module)
        {
            if (isRendering)
                throw new Exception("Cannot Add or Remove Modules during Rendering");

            if (!module.Registered)
                throw new Exception("Module is not already registered");

            module.Shutdown();

            var index = modules.IndexOf(module);
            modules[index] = null;

            var type = module.GetType();
            while (type != typeof(Module))
            {
                if (modulesByType[type] == module)
                    modulesByType.Remove(type);

                if (type.BaseType == null)
                    break;

                type = type.BaseType;
            }

            module.Registered = false;
        }

        /// <summary>
        /// Tries to get the First Module of the given type
        /// </summary>
        public bool TryGet<T>(out T? module) where T : Module
        {
            if (modulesByType.TryGetValue(typeof(T), out var m))
            {
                module = (T)m;
                return true;
            }

            module = null;
            return false;
        }

        /// <summary>
        /// Gets the First Module of the given type, if it exists, or throws an Exception
        /// </summary>
        public T Get<T>() where T : Module
        {
            if (!modulesByType.TryGetValue(typeof(T), out var module))
                throw new Exception($"App is does not have a {typeof(T).Name} Module registered");

            return (T)module;
        }

        /// <summary>
        /// Checks if a Module of the given type exists
        /// </summary>
        public bool Has<T>() where T : Module
        {
            return modulesByType.ContainsKey(typeof(T));
        }

        internal void Startup()
        {
            Console.WriteLine("Begin Modules Startup");

            for (int i = 0; i < modules.Count; i++)
                modules[i]?.Startup();
        }

        internal void Shutdown()
        {
            Console.WriteLine("Begin Modules Shutdown");

            for (int i = modules.Count - 1; i >= 0; i--)
                modules[i]?.Shutdown();

            modules.Clear();
            modulesByType.Clear();
        }

        internal void BeforeUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.BeforeUpdate();
        }

        internal void Update()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.Update();
        }

        internal void AfterUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.AfterUpdate();
        }

        internal void ContextChanged(Context context)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.ContextChanged(context);
        }

        internal void BeforeRender(Window window)
        {
            isRendering = true;

            for (int i = 0; i < modules.Count; i++)
                modules[i]?.BeforeRender(window);

            isRendering = false;
        }

        internal void AfterRender(Window window)
        {
            isRendering = true;

            for (int i = 0; i < modules.Count; i++)
                modules[i]?.AfterRender(window);

            isRendering = false;
        }

        internal void Tick()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.Tick();

            // remove null module entries
            int toRemove;
            while ((toRemove = modules.IndexOf(null)) >= 0)
                modules.RemoveAt(toRemove);
        }

        public IEnumerator<Module> GetEnumerator()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module != null)
                    yield return module;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module != null)
                    yield return module;
            }
        }
    }
}
