using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDPIAware();

        public List<GLFW_Context> Contexts = new List<GLFW_Context>();

        public override bool SupportsMultipleWindows => true;

        public GLFW_System()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetProcessDPIAware();
        }

        protected override void Created()
        {
            // get API info
            {
                GLFW.GetVersion(out int major, out int minor, out int rev);
                ApiName = "GLFW";
                ApiVersion = new Version(major, minor, rev);
            }

            base.Created();
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
            GLFW.WindowHint(GLFW.WindowHints.ScaleToMonitor, true);
            GLFW.WindowHint(GLFW.WindowHints.DoubleBuffer, true);

            // Our default shared context
            CreateContext();

            base.Startup();
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            GLFW.Terminate();
        }

        protected override void BeforeUpdate()
        {
            GLFW.PollEvents();
        }

        protected override void AfterUpdate()
        {
            // check for closing contexts
            for (int i = Contexts.Count - 1; i >= 0; i--)
            {
                var context = Contexts[i];

                if (GLFW.WindowShouldClose(context.Handle))
                {
                    // see if we have a displayed window associated with this context
                    for (int j = 0; j < windows.Count; j++)
                    {
                        if (windows[j].Context == context)
                        {
                            windows[j].Close();
                            windows.RemoveAt(j);
                            break;
                        }
                    }

                    Contexts.RemoveAt(i);
                    GLFW.DestroyWindow(context.Handle);
                }
            }
        }

        public override Window CreateWindow(string title, int width, int height, bool visible = true)
        {
            GLFW_Window window = new GLFW_Window(this, title, width, height, visible);
            windows.Add(window);
            return window;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        public override Context CreateContext()
        {
            // GLFW has no way to create a context without an associated window ...
            // So we create a hidden Window

            GLFW.WindowHint(GLFW.WindowHints.Visible, false);

            GLFW_Context? shared = null;
            if (Contexts.Count > 0)
                shared = Contexts[0];

            var context = new GLFW_Context(this, GLFW.CreateWindow(128, 128, "", IntPtr.Zero, shared ?? IntPtr.Zero));
            Contexts.Add(context);
            SetCurrentContext(context);

            GLFW.WindowHint(GLFW.WindowHints.Visible, true);

            return context;
        }

        public override Context? GetCurrentContext()
        {
            var ptr = GLFW.GetCurrentContext();

            foreach (var context in Contexts)
                if (context.Handle.Ptr == ptr)
                    return context;

            return null;
        }

        public override void SetCurrentContext(Context? context)
        {
            // set current context
            if (context is GLFW_Context ctx)
            {
                if (ctx.Disposed)
                    throw new Exception("The Context is Disposed");

                // already on the current thread
                if (ctx.ActiveThreadId == Thread.CurrentThread.ManagedThreadId)
                    return;

                // currently assigned to a different thread
                if (ctx.ActiveThreadId != 0)
                    throw new Exception("The Context is active on another Thread. A Context can only be current for a single Thread at a time. You must make it non-current on the old Thread before making setting it on another.");

                // unset existing
                if (GetCurrentContext() is GLFW_Context current)
                    current.ActiveThreadId = 0;

                ctx.ActiveThreadId = Thread.CurrentThread.ManagedThreadId;
                GLFW.MakeContextCurrent(ctx.Handle);
            }
            // unset existing context if we're setting to null
            else 
            {
                if (GetCurrentContext() is GLFW_Context current)
                    current.ActiveThreadId = 0;

                GLFW.MakeContextCurrent(IntPtr.Zero);
            }
        }
    }

}
