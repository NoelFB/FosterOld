using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foster.Framework;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {
        internal readonly List<Window> windows = new List<Window>();

        public override ReadOnlyCollection<Window> Windows { get; }

        public GLFW_System()
        {
            Windows = windows.AsReadOnly();
        }

        protected override void OnCreated()
        {
            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out var error);
                throw new Exception($"GLFW Error: {error}");
            }

            GLFW.GetVersion(out var major, out var minor, out var rev);

            ApiName = "GLFW";
            ApiVersion = new Version(major, minor, rev);

            base.OnCreated();
        }

        protected override void OnStartup()
        {
            base.OnStartup();

            if (App.Graphics != null && App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
                throw new Exception("GLFW Only supports OpenGL and Vulkan Graphics APIs");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            GLFW.Terminate();
        }

        protected override void OnPostUpdate()
        {
            GLFW.PollEvents();
        }

        public override Window CreateWindow(string title, int width, int height, bool visible = true)
        {
            var window = new GLFW_Window(this, title, width, height, visible);
            windows.Add(window);
            return window;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }
    }

}
