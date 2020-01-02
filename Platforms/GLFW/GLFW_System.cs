using Foster.Framework;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        public override bool SupportsMultipleWindows => true;
        public event Action<GLFW_Window>? OnWindowCreated;
        public event Action<GLFW_Window>? OnWindowClosed;

        private readonly GLFW_Input input;

        public GLFW_System() : base(new GLFW_Input())
        {
            input = (GLFW_Input)Input;
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
                    GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MAJOR, 3);
                    GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MINOR, 2);
                    GLFW.WindowHint(GLFW_Enum.OPENGL_PROFILE, 0x00032001);
                    GLFW.WindowHint(GLFW_Enum.OPENGL_FORWARD_COMPAT, true);
                }
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

            // Our default shared context
            CreateContext();
            SetCurrentContext(Contexts[0]);

            // setup input
            input.Init(this);

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
                if (contexts[i] is GLFW_Context context && GLFW.WindowShouldClose(context.GlfwWindowPointer))
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
                    GLFW.DestroyWindow(context.GlfwWindowPointer);
                }
            }

            // Update Monitors
            foreach (var monitor in monitors)
                ((GLFW_Monitor)monitor).FetchProperties();

            // update input
            input.AfterUpdate();
        }

        public override Window CreateWindow(Graphics graphics, string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");

            var context = CreateContextInternal(title, width, height, flags);
            var window = new GLFW_Window(this, graphics, context, title, !flags.HasFlag(WindowFlags.Hidden));
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

            GLFW.WindowHint(GLFW_Enum.VISIBLE, !flags.HasFlag(WindowFlags.Hidden));
            GLFW.WindowHint(GLFW_Enum.FOCUS_ON_SHOW, false);
            GLFW.WindowHint(GLFW_Enum.TRANSPARENT_FRAMEBUFFER, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW_Enum.SCALE_TO_MONITOR, flags.HasFlag(WindowFlags.ScaleToMonitor));
            GLFW.WindowHint(GLFW_Enum.SAMPLES, flags.HasFlag(WindowFlags.MultiSampling) ? 4 : 0);

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
                GLFW.MakeContextCurrent(ctx.GlfwWindowPointer);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }
    }

}
