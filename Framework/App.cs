using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.Framework
{
    /// <summary>
    /// Foster Application
    /// </summary>
    public static class App
    {

        /// <summary>
        /// The Application Name
        /// </summary>
        public static string Name = "Application";

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
        /// Gets the System Input
        /// </summary>
        public static Input Input => System.Input;

        /// <summary>
        /// Gets the Primary Window
        /// </summary>
        public static Window Window => primaryWindow ?? throw new Exception("Application has not yet created a Primary Window");

        /// <summary>
        /// Reference to the Primary Window
        /// </summary>
        private static Window? primaryWindow;

        /// <summary>
        /// Starts running the Application
        /// You must register the System Module before calling this
        /// </summary>
        public static void Start(string title, int width, int height, WindowFlags flags = WindowFlags.None, Action? callback = null)
        {
            if (Running)
                throw new Exception("App is already running");

            if (Exiting)
                throw new Exception("App is still exiting");

            Name = title;

            Log.Message(Name, $"Version: {Version}");
            Log.Message(Name, $"Platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
            Log.Message(Name, $"Framework: {RuntimeInformation.FrameworkDescription}");

#if DEBUG
            Launch();
#else
            try
            {
                Launch();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.WriteTo("ErrorLog.txt");
                throw e;
            }
#endif
            void Launch()
            {
                // init modules
                Modules.ApplicationStarted();

                if (!Modules.Has<System>())
                    throw new Exception("App requires a System Module to be registered before it can Start");

                // our primary Window
                primaryWindow = new Window(System, title, width, height, flags);
                Modules.FirstWindowCreated();

                // startup application
                Running = true;
                Modules.Startup();
                callback?.Invoke();
                Run();
            }
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

                // Check if the Primary Window has been closed
                if (primaryWindow == null || !primaryWindow.Opened)
                    Exit();

                // render
                if (!Exiting)
                {
                    Modules.BeforeRender();

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

                    Modules.AfterRender();
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
            primaryWindow = null;
            Exiting = false;

            Log.Message(Name, "Exited");
        }

        private static void Update()
        {
            System.Input.BeforeUpdate();

            Modules.BeforeUpdate();
            Modules.Update();
            Modules.AfterUpdate();
        }

        /// <summary>
        /// Begins Exiting the Application. This will not exit the application immediately and will finish the current Update.
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
