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
        private GLFW_RenderingState? renderingState;

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

            // Various constant Window Hints
            GLFW.WindowHint(GLFW_Enum.DOUBLEBUFFER, true);
            GLFW.WindowHint(GLFW_Enum.DEPTH_BITS, 24);
            GLFW.WindowHint(GLFW_Enum.STENCIL_BITS, 8);

            // macOS requires OpenGL versions to be set to 3.2
            if (App.Graphics.Api == GraphicsApi.OpenGL && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MAJOR, 3);
                GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MINOR, 2);
                GLFW.WindowHint(GLFW_Enum.OPENGL_PROFILE, 0x00032001);
                GLFW.WindowHint(GLFW_Enum.OPENGL_FORWARD_COMPAT, true);
            }

            // Vulkan doesn't need an OpenGL context
            if (App.Graphics.Api == GraphicsApi.Vulkan)
            {
                GLFW.WindowHint(GLFW_Enum.CLIENT_API, (int)GLFW_Enum.NO_API);
            }

            // Monitors
            unsafe
            {
                var monitorPtrs = GLFW.GetMonitors(out int count);
                for (int i = 0; i < count; i++)
                    monitors.Add(new GLFW_Monitor(monitorPtrs[i]));
            }

            // Our default shared context
            var context = RenderingState.CreateContext();
            context.MakeCurrent();

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
            if (RenderingState is GLFW_RenderingState state)
            {
                for (int i = state.Contexts.Count - 1; i >= 0; i--)
                {
                    if (state.Contexts[i] is GLFW_RenderingContext context && GLFW.WindowShouldClose(context.GlfwWindowPointer))
                    {
                        // see if we have a displayed window associated with this context
                        for (int j = 0; j < windows.Count; j++)
                        {
                            if (windows[j] is GLFW_Window window && window.GlfwContext == context)
                            {
                                OnWindowClosed?.Invoke((GLFW_Window)windows[j]);
                                windows[j].OnClose?.Invoke();
                                windows[j].Close();
                                windows.RemoveAt(j);
                                break;
                            }
                        }

                        state.RemoveContext(state.Contexts[i]);
                        GLFW.DestroyWindow(context.GlfwWindowPointer);
                    }
                }
            }

            // Update Monitors
            foreach (var monitor in monitors)
                ((GLFW_Monitor)monitor).FetchProperties();
        }

        protected override RenderingState CreateRenderingState()
        {
            return renderingState = new GLFW_RenderingState(this);
        }

        protected override Window CreateWindowInternal(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");
            if (renderingState == null)
                throw new Exception("Rendering State hasn't been created");

            var context = renderingState.CreateContextInternal(title, width, height, flags);
            var window = new GLFW_Window(this, context, title, !flags.HasFlag(WindowFlags.Hidden));
            windows.Add(window);

            OnWindowCreated?.Invoke(window);

            return window;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        
    }

}
