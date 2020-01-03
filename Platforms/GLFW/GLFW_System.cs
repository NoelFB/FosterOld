using Foster.Framework;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        public override bool SupportsMultipleWindows => true;

        // These values are set from the Constructor fo the System class
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        internal new GLFW_Input Input;
        internal new GLFW_GraphicsDevice GraphicsDevice;
#pragma warning restore CS8618

        protected override Input CreateInput()
        {
            return Input = new GLFW_Input();
        }

        protected override GraphicsDevice CreateGraphicsDevice()
        {
            return GraphicsDevice = new GLFW_GraphicsDevice(this);
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && App.Graphics.Api == GraphicsApi.OpenGL)
            {
                GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MAJOR, 3);
                GLFW.WindowHint(GLFW_Enum.CONTEXT_VERSION_MINOR, 2);
                GLFW.WindowHint(GLFW_Enum.OPENGL_PROFILE, 0x00032001);
                GLFW.WindowHint(GLFW_Enum.OPENGL_FORWARD_COMPAT, true);
            }

            // Non-OpenGL Graphics APIs need to let GLFW know ...
            if (App.Graphics.Api != GraphicsApi.OpenGL)
                GLFW.WindowHint(GLFW_Enum.OPENGL_API, (int)GLFW_Enum.NO_API);

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
            GraphicsDevice.CreateContext();
            GraphicsDevice.SetCurrentContext(GraphicsDevice.Contexts[0]);

            // Init Input
            Input.Init();

            base.Startup();
        }

        protected override void Shutdown()
        {
            base.Shutdown();

            // destroy all contexts
            foreach (var context in GraphicsDevice.Contexts)
                context.Dispose();
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
            Input.AfterUpdate();
        }

        private void Poll()
        {
            GLFW.PollEvents();

            // check for closing contexts
            for (int i = GraphicsDevice.Contexts.Count - 1; i >= 0; i--)
            {
                if (GraphicsDevice.Contexts[i] is GLFW_GraphicsContext context && GLFW.WindowShouldClose(context.GlfwWindowPointer))
                {
                    // see if we have a displayed window associated with this context
                    for (int j = 0; j < windows.Count; j++)
                    {
                        if (windows[j].Context == context)
                        {
                            Input.StopWatchingContext(context);

                            windows[j].OnClose?.Invoke(windows[j]);
                            windows[j].Close();
                            windows.RemoveAt(j);

                            break;
                        }
                    }

                    GraphicsDevice.RemoveContext(GraphicsDevice.Contexts[i]);
                    GLFW.DestroyWindow(context.GlfwWindowPointer);
                }
            }
        }

        public override Window CreateWindow(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
                throw new Exception("Creating a Window must be called from the Main Thread");

            var context = GraphicsDevice.CreateContextInternal(title, width, height, flags);
            var window = new GLFW_Window(this, context, title, !flags.HasFlag(WindowFlags.Hidden));
            windows.Add(window);

            Input.StartWatchingContext(context);

            return window;
        }


    }

}
