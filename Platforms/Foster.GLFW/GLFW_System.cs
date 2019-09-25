using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDPIAware();

        public GLFW_Context SharedContext;

        private Window? activeWindow;
        public override Window? Window => activeWindow;

        private Context? activeContext;
        public override Context? Context => activeContext;

        public GLFW_System()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetProcessDPIAware();

            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out string error);
                throw new Exception($"GLFW Error: {error}");
            }

            // get API info
            {
                GLFW.GetVersion(out int major, out int minor, out int rev);
                ApiName = "GLFW";
                ApiVersion = new Version(major, minor, rev);
            }

            // create a hidden context
            // the only way to create a "windowless" context in GLFW is to create a window and hide it
            {
                GLFW.WindowHint(GLFW.WindowHints.ScaleToMonitor, true);
                GLFW.WindowHint(GLFW.WindowHints.DoubleBuffer, true);
                GLFW.WindowHint(GLFW.WindowHints.Visible, false);

                SharedContext = new GLFW_Context(this, GLFW.CreateWindow(128, 128, "SharedContext", IntPtr.Zero, IntPtr.Zero));
                SharedContext.SetActive();

                GLFW.WindowHint(GLFW.WindowHints.Visible, true);
            }
        }

        protected override void Startup()
        {
            base.Startup();

            if (App.Graphics != null && App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
            {
                throw new Exception("GLFW Only supports OpenGL and Vulkan Graphics APIs");
            }
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
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                var window = (windows[i] as GLFW_Window);
                if (window == null)
                    continue;

                if (GLFW.WindowShouldClose(window.context.Handle))
                {
                    window.Close();
                    window.context.IsDisposed = true;
                    windows.RemoveAt(i);

                    GLFW.DestroyWindow(window.context.Handle);
                }
            }
        }

        public override void SetActiveWindow(Window? window)
        {
            if (activeWindow != window && window is GLFW_Window win)
            {
                activeWindow = win;
                SetActiveContext(win.context);
            }
        }

        public override void SetActiveContext(Context? context)
        {
            if (activeContext != context && context is GLFW_Context ctx)
            {
                activeContext = ctx;
                GLFW.MakeContextCurrent(ctx.Handle);
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
    }

}
