using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        public override bool SupportsMultipleWindows => true;
        public event Action<GLFW_Window>? OnWindowCreated;
        public event Action<GLFW_Window>? OnWindowClosed;

        internal readonly List<Window> windows = new List<Window>();
        internal readonly List<Framework.Monitor> monitors = new List<Framework.Monitor>();
        internal readonly List<Context> contexts = new List<Context>();

        public override ReadOnlyCollection<Window> Windows { get; }
        public override ReadOnlyCollection<Framework.Monitor> Monitors { get; }
        protected override ReadOnlyCollection<Context> Contexts { get; }

        public GLFW_System()
        {
            Windows = new ReadOnlyCollection<Window>(windows);
            Monitors = new ReadOnlyCollection<Framework.Monitor>(monitors);
            Contexts = new ReadOnlyCollection<Context>(contexts);
        }

        protected override void Initialized()
        {
            // get API info
            {
                GLFW.GetVersion(out int major, out int minor, out int rev);
                ApiName = "GLFW";
                ApiVersion = new Version(major, minor, rev);
            }

            base.Initialized();
        }

        protected override void Startup()
        {
            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out string error);
                throw new Exception($"GLFW Error: {error}");
            }

            // GLFW only supports OpenGL and Vulkan
            if (App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
                throw new Exception("GLFW Only supports OpenGL and Vulkan Graphics APIs");

            // macOS requires versions to be set to 3.2
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (App.Graphics.Api == GraphicsApi.OpenGL)
                {
                    GLFW.WindowHint(GLFW.WindowHints.OpenGLVersionMajor, 3);
                    GLFW.WindowHint(GLFW.WindowHints.OpenGLVersionMinor, 2);
                    GLFW.WindowHint(GLFW.WindowHints.OpenGLProfile, 0x00032001);
                    GLFW.WindowHint(GLFW.WindowHints.OpenGLForwardCompat, true);
                }
            }

            // Various Window Hints
            GLFW.WindowHint(GLFW.WindowHints.DoubleBuffer, true);

            // Monitors
            unsafe
            {
                var monitorPtrs = GLFW.GetMonitors(out int count);
                for (int i = 0; i < count; i++)
                    monitors.Add(new GLFW_Monitor(monitorPtrs[i]));
            }

            // Our default shared context
            CreateContext();
            SetCurrentContext(Contexts[0]);

            base.Startup();
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            GLFW.Terminate();
        }

        protected override void AfterUpdate()
        {
            GLFW.PollEvents();

            // check for closing contexts
            for (int i = contexts.Count - 1; i >= 0; i--)
            {
                if (contexts[i] is GLFW_Context context && GLFW.WindowShouldClose(context.Handle))
                {
                    // see if we have a displayed window associated with this context
                    for (int j = 0; j < windows.Count; j++)
                    {
                        if (windows[j].Context == context)
                        {
                            OnWindowClosed?.Invoke((GLFW_Window)windows[j]);
                            windows[j].OnClose?.Invoke();
                            windows[j].Close();
                            windows.RemoveAt(j);
                            break;
                        }
                    }

                    contexts.RemoveAt(i);
                    GLFW.DestroyWindow(context.Handle);
                }
            }

            // Update Monitors
            foreach (var monitor in monitors)
                ((GLFW_Monitor)monitor).Update();
        }

        public override Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");

            var context = CreateContextInternal(title, width, height, flags);
            var window = new GLFW_Window(this, context, title, !flags.HasFlag(WindowFlags.Hidden));
            windows.Add(window);

            OnWindowCreated?.Invoke(window);

            return window;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        public override Context CreateContext()
        {
            return CreateContextInternal("hidden-context", 128, 128, WindowFlags.Hidden);
        }

        private GLFW_Context CreateContextInternal(string title, int width, int height, WindowFlags flags)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Context must be called from the Main Thread");

            GLFW.WindowHint(GLFW.WindowHints.Visible, !flags.HasFlag(WindowFlags.Hidden));
            GLFW.WindowHint(GLFW.WindowHints.FocusOnshow, false);
            GLFW.WindowHint(GLFW.WindowHints.TransparentFramebuffer, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW.WindowHints.ScaleToMonitor, flags.HasFlag(WindowFlags.ScaleToMonitor));

            GLFW_Context? shared = null;
            if (Contexts.Count > 0)
                shared = Contexts[0] as GLFW_Context;

            // GLFW has no way to create a context without a window
            // so any background contexts also just create a hidden window

            var window = GLFW.CreateWindow(width, height, title, IntPtr.Zero, shared ?? IntPtr.Zero);
            var context = new GLFW_Context(this, window);
            contexts.Add(context);

            return context;
        }

        protected override void SetCurrentContextInternal(Context? context)
        {
            if (context is GLFW_Context ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.Handle);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }
    }

}
