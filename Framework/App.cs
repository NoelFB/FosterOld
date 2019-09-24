using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Foster.Framework
{
    public static class App
    {
        public static readonly Version Version = new Version(0, 1, 0);

        private static readonly List<Module> modules = new List<Module>();
        private static readonly Dictionary<Type, Module> modulesByType = new Dictionary<Type, Module>();
        private static readonly TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500);

        public static bool Started { get; private set; } = false;
        public static bool Running { get; private set; } = false;
        public static bool Exiting { get; private set; } = false;

        public static System System => GetModule<System>();
        public static Graphics Graphics => GetModule<Graphics>();
        public static Audio Audio => GetModule<Audio>();
        public static Input Input => GetModule<Input>();

        public static Window? MainWindow => System?.MainWindow;
        public static Window? CurrentWindow => System?.CurrentWindow;

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

            Console.WriteLine($"FOSTER {Version}");

            // Startup
            foreach (var module in modules)
                module.OnStartup();

            // Create our first Window
            System.MainWindow = System.CreateWindow(title, width, height, false);
            System.MainWindow.MakeCurrent();

            // We now have a Context
            foreach (var module in modules)
                module.OnContext();

            System.MainWindow.Visible = true;

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

            // timer
            var framecount = 0;
            var frameticks = 0L;
            var lastTime = new TimeSpan();
            var timer = Stopwatch.StartNew();
            var accumulator = TimeSpan.Zero;

            while (Running && MainWindow != null && MainWindow.Opened)
            {
                // update
                {
                    // fixed timestep update
                    if (Time.FixedStepEnabled)
                    {
                        // delta time is always the same
                        Time.Delta = 1f / Time.FixedStepTarget;

                        var target = TimeSpan.FromSeconds(1f / Time.FixedStepTarget);
                        var current = TimeSpan.FromTicks(timer.Elapsed.Ticks);
                        accumulator += (current - lastTime);
                        lastTime = current;

                        // while we're too fast, wait
                        while (accumulator < target)
                        {
                            Thread.Sleep((int)((target - accumulator).TotalMilliseconds));

                            current = TimeSpan.FromTicks(timer.Elapsed.Ticks);
                            accumulator += (current - lastTime);
                            lastTime = current;
                        }

                        // Do not allow any update to take longer than our maximum.
                        if (accumulator > maxElapsedTime)
                            accumulator = maxElapsedTime;

                        // do as many updates as we can
                        while (accumulator >= target)
                        {
                            accumulator -= target;
                            Time.Duration += target;
                            Update();
                        }
                    }
                    // non-fixed timestep update
                    else
                    {
                        Time.Duration = TimeSpan.FromTicks(timer.Elapsed.Ticks);
                        Time.Delta = (float)(Time.Duration - lastTime).TotalSeconds;
                        lastTime = Time.Duration;
                        accumulator = TimeSpan.Zero;

                        Update();
                    }
                }

                // render
                {
                    foreach (var module in modules)
                        module.OnPreRender();

                    foreach (var window in System.Windows)
                    {
                        if (!window.Opened)
                            continue;

                        window.MakeCurrent();
                        Graphics.Target(null);
                        Graphics.Clear(Color.Black);
                        OnRender?.Invoke(window);
                        window.Present();
                    }

                    foreach (var module in modules)
                        module.OnPostRender();
                }

                // determine fps
                framecount++;
                if (TimeSpan.FromTicks(timer.Elapsed.Ticks - frameticks).TotalSeconds >= 1)
                {
                    Time.FPS = framecount;
                    frameticks = timer.Elapsed.Ticks;
                    framecount = 0;
                }
            }

            // Close the Window
            MainWindow?.Close();

            // exit Modules
            foreach (var module in modules)
                module.OnShutdown();
            modules.Clear();
            modulesByType.Clear();

            // finalize
            Started = false;
            Exiting = false;
            OnShutdown?.Invoke();
        }

        private static void Update()
        {
            foreach (var module in modules)
                module.OnPreUpdate();

            OnUpdate?.Invoke();

            foreach (var module in modules)
                module.OnPostUpdate();
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

        internal static bool TryGetModule<T>(out T? module) where T : Module
        {
            if (modulesByType.TryGetValue(typeof(T), out var m))
            {
                module = (T)m;
                return true;
            }

            module = null;
            return false;
        }

        internal static T GetModule<T>() where T : Module
        {
            if (!modulesByType.TryGetValue(typeof(T), out var module))
                throw new Exception($"App is missing a Module of type {typeof(T).Name}");

            return (T)module;
        }

    }
}
