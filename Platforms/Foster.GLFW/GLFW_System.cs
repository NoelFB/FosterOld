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

        internal readonly List<Window> windows = new List<Window>();

        public override ReadOnlyCollection<Window> Windows { get; }

        public GLFW_System()
        {
            Windows = windows.AsReadOnly();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetProcessDPIAware();
            }
        }

        protected override void OnCreated()
        {
            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out string error);
                throw new Exception($"GLFW Error: {error}");
            }

            GLFW.GetVersion(out int major, out int minor, out int rev);

            ApiName = "GLFW";
            ApiVersion = new Version(major, minor, rev);

            base.OnCreated();
        }

        protected override void OnStartup()
        {
            base.OnStartup();

            if (App.Graphics != null && App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
            {
                throw new Exception("GLFW Only supports OpenGL and Vulkan Graphics APIs");
            }
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            GLFW.Terminate();
        }

        protected override void OnPostUpdate()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                var window = (windows[i] as GLFW_Window);
                if (window == null)
                    continue;

                if (GLFW.WindowShouldClose(window.handle))
                {
                    window.Close();
                    windows.RemoveAt(i);

                    GLFW.DestroyWindow(window.handle);
                }
            }

            GLFW.PollEvents();
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
