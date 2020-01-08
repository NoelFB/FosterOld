using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Foster.Framework
{
    /// <summary>
    /// A List of Modules
    /// </summary>
    public class ModuleList : IEnumerable<Module>
    {

        private readonly List<Type> registered = new List<Type>();
        private readonly List<Module?> modules = new List<Module?>();
        private readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();

        /// <summary>
        /// Registers a Module
        /// </summary>
        public void Register<T>() where T : Module
        {
            Register(typeof(T));
        }

        /// <summary>
        /// Registers a Module
        /// </summary>
        public void Register(Type type)
        {
            if (App.Running)
            {
                var module = Instantiate(type);

                if (module is AppModule appModule)
                {
                    appModule.ApplicationStarted();
                    appModule.FirstWindowCreated();
                }

                module.Startup();
            }
            else
            {
                registered.Add(type);
            }
        }

        /// <summary>
        /// Registers a Module
        /// </summary>
        private Module Instantiate(Type type)
        {
            if (!(Activator.CreateInstance(type) is Module module))
                throw new Exception("Type must inheirt from Module");

            // add Module to lookup
            while (type != typeof(Module) && type != typeof(AppModule))
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
            module.IsRegistered = true;
            module.MainThreadId = Thread.CurrentThread.ManagedThreadId;
            return module;
        }

        /// <summary>
        /// Removes a Module
        /// Note: Removing core modules (such as System) will make everything break
        /// </summary>
        public void Remove(Module module)
        {
            if (!module.IsRegistered)
                throw new Exception("Module is not already registered");

            module.Shutdown();
            module.Disposed();

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

            module.IsRegistered = false;
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

        internal void ApplicationStarted()
        {
            // create Application Modules
            for (int i = 0; i < registered.Count; i++)
            {
                if (typeof(AppModule).IsAssignableFrom(registered[i]))
                {
                    Instantiate(registered[i]);
                    registered.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] != null && modules[i] is AppModule module)
                    module.ApplicationStarted();
            }
        }

        internal void FirstWindowCreated()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] != null && modules[i] is AppModule module)
                    module.FirstWindowCreated();
            }
        }

        internal void Startup()
        {
            // create non-core Modules
            while (registered.Count > 0)
            {
                Instantiate(registered[0]);
                registered.RemoveAt(0);
            }

            for (int i = 0; i < modules.Count; i++)
                modules[i]?.Startup();
        }

        internal void Shutdown()
        {
            for (int i = modules.Count - 1; i >= 0; i--)
                modules[i]?.Shutdown();

            for (int i = modules.Count - 1; i >= 0; i--)
                modules[i]?.Disposed();

            registered.Clear();
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

        internal void BeforeRender(Window window)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.BeforeRender(window);
        }

        internal void AfterRender(Window window)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i]?.AfterRender(window);
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
