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

        public static void Start()
        {
            if (Running)
                throw new Exception("App is already running");

            if (Exiting)
                throw new Exception("App is still exiting");

            Started = true;

            Console.WriteLine($"FOSTER {Version}");

            // Startup
            foreach (var module in modules)
                module.Startup();

            // Start Running
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

            while (Running && System.Windows.Count > 0)
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
                        module.BeforeRender();

                    foreach (var window in System.Windows)
                    {
                        if (!window.Opened)
                            continue;

                        window.SetActive();

                        foreach (var module in modules)
                            module.Render(window);
                    }

                    foreach (var module in modules)
                        module.AfterRender();

                    foreach (var window in System.Windows)
                        window.Present();
                }

                // determine fps
                framecount++;
                if (TimeSpan.FromTicks(timer.Elapsed.Ticks - frameticks).TotalSeconds >= 1)
                {
                    Time.FPS = framecount;
                    frameticks = timer.Elapsed.Ticks;
                    framecount = 0;
                }

                foreach (var module in modules)
                    module.Tick();
            }

            // exit Modules
            foreach (var module in modules)
                module.Shutdown();
            modules.Clear();
            modulesByType.Clear();

            // finalize
            Started = false;
            Exiting = false;
        }

        private static void Update()
        {
            foreach (var module in modules)
                module.BeforeUpdate();

            foreach (var module in modules)
                module.Update();

            foreach (var module in modules)
                module.AfterUpdate();
        }

        public static void Exit()
        {
            if (Running && !Exiting)
            {
                Running = false;
                Exiting = true;
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
            module.Created();
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
