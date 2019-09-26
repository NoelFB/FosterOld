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
        
        public static bool Started { get; private set; } = false;
        public static bool Running { get; private set; } = false;
        public static bool Exiting { get; private set; } = false;

        public readonly static ModuleList Modules = new ModuleList();
        public static System System => Modules.Get<System>();
        public static Graphics Graphics => Modules.Get<Graphics>();
        public static Audio Audio => Modules.Get<Audio>();
        public static Input Input => Modules.Get<Input>();

        private static readonly TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500);

        public static void Start()
        {
            if (Running)
                throw new Exception("App is already running");

            if (Exiting)
                throw new Exception("App is still exiting");

            Console.WriteLine($"FOSTER {Version}");

            Started = true;
            Modules.Startup();
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

                            Modules.BeforeUpdate();
                            Modules.Update();
                            Modules.AfterUpdate();
                        }
                    }
                    // non-fixed timestep update
                    else
                    {
                        Time.Duration = TimeSpan.FromTicks(timer.Elapsed.Ticks);
                        Time.Delta = (float)(Time.Duration - lastTime).TotalSeconds;
                        lastTime = Time.Duration;
                        accumulator = TimeSpan.Zero;

                        Modules.BeforeUpdate();
                        Modules.Update();
                        Modules.AfterUpdate();
                    }
                }

                // render
                {
                    Modules.BeforeRender();

                    foreach (var window in System.Windows)
                    {
                        if (!window.Opened)
                            continue;

                        System.SetCurrentContext(window.Context);
                        Modules.Render(window);
                    }

                    Modules.AfterRender();

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

                Modules.Tick();
            }

            Modules.Shutdown();
            Modules.Clear();

            // finalize
            Started = false;
            Exiting = false;
        }

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
