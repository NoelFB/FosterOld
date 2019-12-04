using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Foster.Framework
{
    public class ModuleList : IEnumerable<Module>
    {

        private readonly List<Module> modules = new List<Module>();
        private readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();

        public T Register<T>() where T : Module
        {
            return Register(Activator.CreateInstance<T>());
        }

        public Module Register(Type type)
        {
            if (!(Activator.CreateInstance(type) is Module module))
                throw new Exception("Type must inheirt from Module");

            return Register(module);
        }

        public T Register<T>(T module) where T : Module
        {
            if (module.Registered)
                throw new Exception("Module is already registered");

            // add Module to lookup
            var type = module.GetType();
            while (type != typeof(Module))
            {
                modulesByType[type] = module;

                if (type.BaseType == null)
                    break;

                type = type.BaseType;
            }

            modules.Add(module);
            module.Registered = true;
            module.MainThreadId = Thread.CurrentThread.ManagedThreadId;
            module.Initialized();

            if (App.Running)
                module.Startup();

            return module;
        }

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

        public T Get<T>() where T : Module
        {
            if (!modulesByType.TryGetValue(typeof(T), out var module))
                throw new Exception($"App is does not have a {typeof(T).Name} Module registered");

            return (T)module;
        }

        public bool Has<T>() where T : Module
        {
            return modulesByType.ContainsKey(typeof(T));
        }

        internal void Startup()
        {
            modules.Sort((a, b) => a.Priority - b.Priority);

            for (int i = 0; i < modules.Count; i++)
                modules[i].Startup();
        }

        internal void Shutdown()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].Shutdown();
        }

        internal void BeforeUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].BeforeUpdate();
        }

        internal void Update()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].Update();
        }

        internal void AfterUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].AfterUpdate();
        }

        internal void ContextChanged(Context context)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].ContextChanged(context);
        }

        internal void BeforeRender(Window window)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].BeforeRender(window);
        }

        internal void AfterRender(Window window)
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].AfterRender(window);
        }

        internal void Tick()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].Tick();
        }

        public void Clear()
        {
            modules.Clear();
            modulesByType.Clear();
        }

        public IEnumerator<Module> GetEnumerator()
        {
            return modules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return modules.GetEnumerator();
        }
    }
}
