using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class ModuleList : IEnumerable<Module>
    {

        private readonly List<Module> modules = new List<Module>();
        private readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();

        public void Register<T>() where T : Module
        {
            Register(typeof(T));
        }

        public void Register(Type type)
        {
            if (!(Activator.CreateInstance(type) is Module module))
                throw new Exception("Type must inheirt from Module");

            // add Module to lookup
            while (type != typeof(Module))
            {
                modulesByType.Add(type, module);

                if (type.BaseType == null)
                    break;
                type = type.BaseType;
            }

            modules.Add(module);
            module.Created();
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
                throw new Exception($"App is missing a Module of type {typeof(T).Name}");

            return (T)module;
        }

        internal void Startup()
        {
            modules.Sort((a, b) => a.Priority - b.Priority);

            foreach (var module in modules)
                module.Startup();
        }

        internal void Shutdown()
        {
            foreach (var module in modules)
                module.Shutdown();
        }

        internal void BeforeUpdate() 
        {
            foreach (var module in modules)
                module.BeforeUpdate();
        }

        internal void Update() 
        {
            foreach (var module in modules)
                module.Update();
        }

        internal void AfterUpdate() 
        {
            foreach (var module in modules)
                module.AfterUpdate();
        }

        internal void BeforeRender() 
        {
            foreach (var module in modules)
                module.BeforeRender();
        }

        internal void Render(Window window)
        {
            foreach (var module in modules)
                module.Render(window);
        }

        internal void AfterRender() 
        {
            foreach (var module in modules)
                module.AfterRender();
        }

        internal void Tick()
        {
            foreach (var module in modules)
                module.Tick();
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
