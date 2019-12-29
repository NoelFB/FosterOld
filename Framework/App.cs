using System;
using System.Diagnostics;
using System.Threading;

namespace Foster.Framework
{
    public static class App
    {
        /// <summary>
        /// Foster.Framework Version Number
        /// </summary>
        public static readonly Version Version = new Version(0, 1, 0);

        /// <summary>
        /// Whether the Application is running
        /// </summary>
        public static bool Running { get; private set; } = false;

        /// <summary>
        /// Whether the Application is exiting
        /// </summary>
        public static bool Exiting { get; private set; } = false;

        /// <summary>
        /// A List of all the Application Modules
        /// </summary>
        public static readonly ModuleList Modules = new ModuleList();

        /// <summary>
        /// Gets the System Module
        /// </summary>
        public static System System => Modules.Get<System>();

        /// <summary>
        /// Gets the Graphics Module
        /// </summary>
        public static Graphics Graphics => Modules.Get<Graphics>();

        /// <summary>
        /// Gets the Audio Module
        /// </summary>
        public static Audio Audio => Modules.Get<Audio>();

        /// <summary>
        /// Gets the Input Module
        /// </summary>
        public static Input Input => Modules.Get<Input>();

        /// <summary>
        /// Gets the First Window from the System, or null if there are none
        /// </summary>
        public static Window? Window => System.Windows.Count > 0 ? System.Windows[0] : null;

        /// <summary>
        /// Starts running the Application
        /// You must register the System Module before calling this
        /// </summary>
        public static void Start(Action? callback = null)
        {
            if (Running)
                throw new Exception("App is already running");

            if (Exiting)
                throw new Exception("App is still exiting");

            if (!Modules.Has<System>())
                throw new Exception("App requires a System Module to be registered before it can Start");

            Console.WriteLine($"FOSTER {Version}");

            Modules.Startup();
            Running = true;
            callback?.Invoke();
            Run();
        }

        private static void Run()
        {
            // timer
            var framecount = 0;
            var frameticks = 0L;
            var lastTime = new TimeSpan();
            var timer = Stopwatch.StartNew();
            var accumulator = TimeSpan.Zero;

            while (Running)
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
                        if (accumulator > Time.FixedMaxElapsedTime)
                            accumulator = Time.FixedMaxElapsedTime;

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
                if (!Exiting)
                {
                    foreach (var window in System.Windows)
                    {
                        if (!window.Opened)
                            continue;

                        window.Render();
                    }

                    foreach (var window in System.Windows)
                    {
                        if (!window.Opened)
                            continue;

                        window.Present();
                    }
                }

                if (!Exiting)
                {
                    // determine fps
                    framecount++;
                    if (TimeSpan.FromTicks(timer.Elapsed.Ticks - frameticks).TotalSeconds >= 1)
                    {
                        Time.FPS = framecount;
                        frameticks = timer.Elapsed.Ticks;
                        framecount = 0;
                    }

                    Modules.Tick();
                }
            }

            // finalize
            Modules.Shutdown();
            Exiting = false;
        }

        private static void Update()
        {
            Modules.BeforeUpdate();
            Modules.Update();
            Modules.AfterUpdate();
        }

        /// <summary>
        /// Begins Exiting the Application
        /// </summary>
        public static void Exit()
        {
            if (Running && !Exiting)
            {
                Running = false;
                Exiting = true;
            }
        }
    }
}
