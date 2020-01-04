using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System, ISystemOpenGL, ISystemVulkan
    {

        private readonly GLFW_Input input;
        internal List<GLFW.Window> windowPointers = new List<GLFW.Window>();
        private readonly List<GLFW_GLContext> glContexts = new List<GLFW_GLContext>();

        public GLFW_System()
        {
            input = new GLFW_Input();
        }

        protected override void Initialized()
        {
            GLFW.GetVersion(out int major, out int minor, out int rev);

            ApiName = "GLFW";
            ApiVersion = new Version(major, minor, rev);
        }

        public override bool SupportsMultipleWindows => true;

        public override Input Input => input;

        protected override void Startup()
        {
            base.Startup();

            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out string error);
                throw new Exception($"GLFW Error: {error}");
            }

            // OpenGL Setup
            if (App.Graphics is IGraphicsOpenGL)
            {
                // macOS requires versions to be set to 3.2
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MAJOR, 3);
                    GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MINOR, 2);
                    GLFW.WindowHint(GLFW_Enum.OPENGL_PROFILE, 0x00032001);
                    GLFW.WindowHint(GLFW_Enum.OPENGL_FORWARD_COMPAT, true);
                }

                // create the GL Device
                CreateGLContext();
                SetCurrentGLContext(windowPointers[0]);
            }
            // Vulkan Setup
            else if (App.Graphics is IGraphicsVulkan)
            {
                GLFW.WindowHint(GLFW_Enum.OPENGL_API, (int)GLFW_Enum.NO_API);

                if (GLFW.VulkanSupported() != 0)
                    throw new Exception("Vulkan is not supported on this platform");
            }
            else
            {
                GLFW.WindowHint(GLFW_Enum.OPENGL_API, (int)GLFW_Enum.NO_API);
            }

            // Various constant Window Hints
            GLFW.WindowHint(GLFW_Enum.DOUBLEBUFFER, true);
            GLFW.WindowHint(GLFW_Enum.DEPTH_BITS, 24);
            GLFW.WindowHint(GLFW_Enum.STENCIL_BITS, 8);

            // Monitors
            unsafe
            {
                var monitorPtrs = GLFW.GetMonitors(out int count);
                for (int i = 0; i < count; i++)
                    monitors.Add(new GLFW_Monitor(monitorPtrs[i]));
            }

            // Init Input
            input.Init();
        }

        protected override void Shutdown()
        {
            base.Shutdown();

            // destroy all windows
            foreach (var window in windowPointers)
                GLFW.SetWindowShouldClose(window.Ptr, true);
            Poll();

            // terminate GLFW
            GLFW.Terminate();
        }

        protected override void AfterUpdate()
        {
            Poll();

            // Update Monitors
            foreach (var monitor in monitors)
                ((GLFW_Monitor)monitor).FetchProperties();

            // update input
            input.AfterUpdate();
        }

        private void Poll()
        {
            GLFW.PollEvents();

            // check for closing windows
            for (int i = windowPointers.Count - 1; i >= 0; i--)
            {
                if (GLFW.WindowShouldClose(windowPointers[i]))
                {
                    // see if we have a GLFW_Window associated
                    for (int j = 0; j < windows.Count; j++)
                    {
                        if (windows[j] is GLFW_Window window && window.window.Ptr == windowPointers[i].Ptr)
                        {
                            input.StopListening(window.window);

                            windows[j].OnClose?.Invoke(windows[j]);
                            windows[j].Close();
                            windows.RemoveAt(j);

                            break;
                        }
                    }

                    if (App.Graphics is IGraphicsOpenGL)
                    {
                        for (int j = 0; j < glContexts.Count; j ++)
                            if (glContexts[j].window.Ptr == windowPointers[i].Ptr)
                            {
                                glContexts.RemoveAt(j);
                                break;
                            }
                    }

                    GLFW.DestroyWindow(windowPointers[i]);
                    windowPointers.RemoveAt(i);
                }
            }
        }

        public override Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");

            // create GLFW Window
            var ptr = CreateGlfwWindow(title, width, height, flags);

            // start listening for input on this Window
            input.StartListening(ptr);

            // Add the GL Context
            if (App.Graphics is IGraphicsOpenGL)
                glContexts.Add(new GLFW_GLContext(ptr));

            // create the actual Window object
            var window = new GLFW_Window(this, ptr, title, !flags.HasFlag(WindowFlags.Hidden));
            windows.Add(window);
            return window;
        }

        internal GLFW.Window CreateGlfwWindow(string title, int width, int height, WindowFlags flags)
        {
            GLFW.WindowHint(GLFW_Enum.VISIBLE, !flags.HasFlag(WindowFlags.Hidden));
            GLFW.WindowHint(GLFW_Enum.FOCUS_ON_SHOW, false);
            GLFW.WindowHint(GLFW_Enum.TRANSPARENT_FRAMEBUFFER, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW_Enum.SCALE_TO_MONITOR, flags.HasFlag(WindowFlags.ScaleToMonitor));
            GLFW.WindowHint(GLFW_Enum.SAMPLES, flags.HasFlag(WindowFlags.MultiSampling) ? 4 : 0);

            IntPtr shared = IntPtr.Zero;
            if (windowPointers.Count > 0)
                shared = windowPointers[0];

            // create the GLFW Window and return thr pointer
            var ptr = GLFW.CreateWindow(width, height, title, IntPtr.Zero, shared);
            windowPointers.Add(ptr);

            return ptr;
        }

        #region ISystemOpenGL Method Calls

        public IntPtr GetGLProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        public ISystemOpenGL.Context CreateGLContext()
        {
            // GLFW has no way to create a context without a window ...
            // so we create a Window and just hide it

            var ptr = CreateGlfwWindow("hidden-context", 128, 128, WindowFlags.Hidden);
            var context = new GLFW_GLContext(ptr);
            glContexts.Add(context);
            return context;
        }

        public ISystemOpenGL.Context GetWindowGLContext(Window window)
        {
            if (window is GLFW_Window glfwWindow)
            {
                for (int i = 0; i < glContexts.Count; i++)
                    if (glContexts[i].window.Ptr == glfwWindow.window.Ptr)
                        return glContexts[i];
            }

            throw new Exception("Window does not have a valid Context");
        }

        public ISystemOpenGL.Context? GetCurrentGLContext()
        {
            var ptr = GLFW.GetCurrentContext();
            if (ptr != IntPtr.Zero)
            {
                for (int i = 0; i < glContexts.Count; i++)
                    if (glContexts[i].window.Ptr == ptr)
                        return glContexts[i];
            }

            return null;
        }

        public void SetCurrentGLContext(ISystemOpenGL.Context? context)
        {
            if (context is GLFW_GLContext ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.window);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }

        internal void SetCurrentGLContext(GLFW.Window window)
        {
            GLFW.MakeContextCurrent(window.Ptr);
        }

        #endregion
    }

}
