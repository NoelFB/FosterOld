using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    public static class App
    {
        public static readonly Version Version = new Version(0, 1, 0);

        private static readonly List<Module> modules = new List<Module>();
        private static readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();

        public static bool Started { get; private set; } = false;
        public static bool Running { get; private set; } = false;
        public static bool Exiting { get; private set; } = false;

        public static System? System { get; private set; }
        public static Graphics? Graphics { get; private set; }
        public static Audio? Audio { get; private set; }
        public static Input? Input { get; private set; }
        public static Window? Window { get; private set; }

        public static event Action? OnUpdate;
        public static event Action? OnExit;
        public static event Action? OnShutdown;

        public static void Startup(string title, int width, int height, Action? onReady)
        {
            if (Running)
                throw new Exception("App is already running");

            if (Exiting)
                throw new Exception("App is still exiting");

            Started = true;

            // Find Core Modules
            System = Module<System>();
            Graphics = Module<Graphics>();
            Audio = Module<Audio>();
            Input = Module<Input>();

            if (System == null)
                throw new Exception("A System Module is required");

            Console.WriteLine($"FOSTER {Version}");

            // Startup
            foreach (var module in modules)
                module.Startup();

            // Create our first Window
            // If this window is Closed, the entire App ends
            Window = System.CreateWindow(title, width, height);

            // Tell Module's we have a Window to Display to
            foreach (var module in modules)
                module.Displayed();

            // Start Running
            onReady?.Invoke();
            Run();
        }

        private static void Run()
        {
            Running = true;

            while (Running && Window != null && Window.Opened)
            {
                foreach (var module in modules)
                    module.PreUpdate();

                OnUpdate?.Invoke();

                if (Window != null && Window.Opened)
                {
                    Window.MakeCurrent();
                    Window.Present();
                }

                foreach (var module in modules)
                    module.PostUpdate();
            }

            // Close the Window
            Window?.Close();

            // exit Modules
            foreach (var module in modules)
                module.Shutdown();
            modules.Clear();
            modulesByType.Clear();

            // dereference
            Window = null;
            Input = null;
            Audio = null;
            Graphics = null;
            System = null;

            // finalize
            Started = false;
            Exiting = false;
            OnShutdown?.Invoke();
        }

        public static void Exit()
        {
            if (Running && !Exiting)
            {
                Running = false;
                Exiting = true;
                OnExit?.Invoke();
            }
        }

        public static void Register<T>() where T : Module
        {
            if (Started)
                throw new Exception("App has already started; Registering Module's must be called before App.Startup");

            var module = Activator.CreateInstance<T>();

            // add Module to lookup
            Type? type = typeof(T);
            while (type != typeof(Module) && type != null)
            {
                modulesByType.Add(type, module);
                type = type.BaseType;
            }

            modules.Add(module);
            module.Created();
        }

        internal static T? Module<T>() where T : Module
        {
            if (modulesByType.TryGetValue(typeof(T), out var module))
                return (T)module;

            return null;
        }

    }
}
