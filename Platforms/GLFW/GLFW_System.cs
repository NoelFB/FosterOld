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

        private readonly List<IntPtr> windowPointers = new List<IntPtr>();
        private readonly Dictionary<IntPtr, GLFW_Window> glfwWindows = new Dictionary<IntPtr, GLFW_Window>();
        private readonly Dictionary<IntPtr, GLFW_GLContext> glContexts = new Dictionary<IntPtr, GLFW_GLContext>();
        private readonly Dictionary<IntPtr, IntPtr> vkSurfaces = new Dictionary<IntPtr, IntPtr>();

        public override bool SupportsMultipleWindows => true;
        public override Input Input => input;

        public GLFW_System()
        {
            input = new GLFW_Input();

            GLFW.GetVersion(out int major, out int minor, out int rev);

            ApiName = "GLFW";
            ApiVersion = new Version(major, minor, rev);
        }

        protected override void ApplicationStarted()
        {
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
            }
            else
            {
                GLFW.WindowHint(GLFW_Enum.CLIENT_API, (int)GLFW_Enum.NO_API);
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
            input.Dispose();

            foreach (var window in windowPointers)
                GLFW.SetWindowShouldClose(window, true);

            Poll(); // this will actually close the Windows
        }

        protected override void Disposed()
        {
            GLFW.Terminate();
        }

        protected override void FrameStart()
        {
            input.BeforeUpdate();
        }

        protected override void FrameEnd()
        {
            Poll();

            // Update Monitors
            foreach (GLFW_Monitor monitor in monitors)
                monitor.FetchProperties();

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
                    if (glfwWindows.TryGetValue(windowPointers[i], out var window))
                    {
                        input.StopListening(window.pointer);
                        glfwWindows.Remove(window.pointer);
                        window.InvokeCloseWindowCallback();
                    }

                    // remove OpenGL context
                    if (App.Graphics is IGraphicsOpenGL)
                    {
                        glContexts.Remove(windowPointers[i]);
                    }
                    // remove Vulkan Surface
                    else if (App.Graphics is IGraphicsVulkan vkGraphics)
                    {
                        var vkInstance = vkGraphics.GetVulkanInstancePointer();

                        if (vkDestroySurfaceKHR == null)
                        {
                            var ptr = GetVKProcAddress(vkInstance, "vkDestroySurfaceKHR");
                            if (ptr != null)
                                vkDestroySurfaceKHR = (VkDestroySurfaceKHR)Marshal.GetDelegateForFunctionPointer(ptr, typeof(VkDestroySurfaceKHR));
                        }

                        vkDestroySurfaceKHR?.Invoke(vkInstance, vkSurfaces[windowPointers[i]], IntPtr.Zero);
                        vkSurfaces.Remove(windowPointers[i]);
                    }

                    GLFW.DestroyWindow(windowPointers[i]);
                    windowPointers.RemoveAt(i);
                }
            }
        }

        protected override Window.Platform CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");

            // create GLFW Window
            var ptr = CreateGlfwWindow(title, width, height, flags);

            // start listening for input on this Window
            input.StartListening(ptr);

            // Add the GL Context
            if (App.Graphics is IGraphicsOpenGL)
            {
                glContexts.Add(ptr, new GLFW_GLContext(ptr));
            }

            // create the actual Window object
            var window = new GLFW_Window(this, ptr, title, !flags.HasFlag(WindowFlags.Hidden));
            glfwWindows.Add(ptr, window);
            return window;
        }

        internal IntPtr CreateGlfwWindow(string title, int width, int height, WindowFlags flags)
        {
            GLFW.WindowHint(GLFW_Enum.VISIBLE, false);
            GLFW.WindowHint(GLFW_Enum.FOCUS_ON_SHOW, true);
            GLFW.WindowHint(GLFW_Enum.FOCUSED, true);
            GLFW.WindowHint(GLFW_Enum.TRANSPARENT_FRAMEBUFFER, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW_Enum.SCALE_TO_MONITOR, flags.HasFlag(WindowFlags.ScaleToMonitor));
            GLFW.WindowHint(GLFW_Enum.SAMPLES, flags.HasFlag(WindowFlags.MultiSampling) ? 4 : 0);

            IntPtr shared = IntPtr.Zero;
            if (App.Graphics is IGraphicsOpenGL && windowPointers.Count > 0)
                shared = windowPointers[0];

            var monitor = IntPtr.Zero;
            if (flags.HasFlag(WindowFlags.Fullscreen))
                monitor = GLFW.GetPrimaryMonitor();

            // create the GLFW Window and return thr pointer
            var ptr = GLFW.CreateWindow(width, height, title, monitor, shared);
            if (ptr == IntPtr.Zero)
            {
                GLFW.GetError(out string error);
                throw new Exception($"Unable to create a new Window: {error}");
            }
            windowPointers.Add(ptr);

            // create the Vulkan surface
            if (App.Graphics is IGraphicsVulkan vkGraphics)
            {
                var vkInstance = vkGraphics.GetVulkanInstancePointer();
                var result = GLFW.CreateWindowSurface(vkInstance, ptr, IntPtr.Zero, out var surface);

                if (result != GLFW_VkResult.Success)
                    throw new Exception($"Unable to create a Vulkan Surface, {result}");

                vkSurfaces.Add(ptr, surface);
            }

            // show window
            if (!flags.HasFlag(WindowFlags.Hidden))
                GLFW.ShowWindow(ptr);

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
            glContexts.Add(ptr, context);
            return context;
        }

        public ISystemOpenGL.Context GetWindowGLContext(Window window)
        {
            return glContexts[((GLFW_Window)window.Implementation).pointer];
        }

        public ISystemOpenGL.Context? GetCurrentGLContext()
        {
            var ptr = GLFW.GetCurrentContext();

            if (ptr != IntPtr.Zero)
                return glContexts[ptr];

            return null;
        }

        public void SetCurrentGLContext(ISystemOpenGL.Context? context)
        {
            if (context is GLFW_GLContext ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.window);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }

        internal void SetCurrentGLContext(IntPtr window)
        {
            GLFW.MakeContextCurrent(window);
        }

        #endregion

        #region ISystemVulkan Method Calls

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private unsafe delegate int VkDestroySurfaceKHR(IntPtr instance, IntPtr surface, IntPtr allocator);
        private VkDestroySurfaceKHR? vkDestroySurfaceKHR;

        public IntPtr GetVKProcAddress(IntPtr instance, string name)
        {
            return GLFW.GetInstanceProcAddress(instance, name);
        }

        public IntPtr GetVKSurface(Window window)
        {
            return vkSurfaces[((GLFW_Window)window.Implementation).pointer];
        }

        public List<string> GetVKExtensions()
        {
            unsafe
            {
                var ptr = (byte**)GLFW.GetRequiredInstanceExtensions(out uint count);
                var list = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    var str = Marshal.PtrToStringAnsi(new IntPtr(ptr[i]));
                    if (str != null)
                        list.Add(str);
                }

                return new List<string>(list);
            }
        }

        #endregion
    }

}
