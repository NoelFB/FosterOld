using System;
using System.Collections.Generic;
using System.Linq;

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
        public static event Action<Window>? OnRender;
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
                module.OnStartup();

            // Create our first Window
            Window = System.CreateWindow(title, width, height, false);

            // We now have a Context
            foreach (var module in modules)
                module.OnContext();

            Window.Visible = true;

            // Tell Module's we have a Window to Display to
            foreach (var module in modules)
                module.OnDisplayed();

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
                    module.OnPreUpdate();

                OnUpdate?.Invoke();

                if (System != null)
                {
                    foreach (var window in System.Windows)
                    {
                        window.MakeCurrent();
                        Graphics?.Target(null);
                        OnRender?.Invoke(window);
                        window.Present();
                    }
                }

                foreach (var module in modules)
                    module.OnPostUpdate();
            }

            // Close the Window
            Window?.Close();

            // exit Modules
            foreach (var module in modules)
                module.OnShutdown();
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

        public static void RegisterModule<T>() where T : Module
        {
            RegisterModule(typeof(T));
        }

        public static void RegisterModule(Type type)
        {
            if (Started)
                throw new Exception("App has already started; Registering Module's must be called before App.Startup");

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
            module.OnCreated();
        }

        internal static T? Module<T>() where T : Module
        {
            if (modulesByType.TryGetValue(typeof(T), out var module))
                return (T)module;

            return null;
        }

    }
}
