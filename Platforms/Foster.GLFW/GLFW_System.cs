using System;
using Foster.Framework;

namespace Foster.GLFW
{
    public class GLFW_System : Framework.System
    {

        protected override void Created()
        {
            base.Created();

            if (GLFW.Init() == 0)
            {
                GLFW.GetError(out var error);
                throw new Exception($"GLFW Error: {error}");
            }

            GLFW.GetVersion(out var major, out var minor, out var rev);

            ApiName = "GLFW";
            ApiVersion = new Version(major, minor, rev);
        }

        protected override void Startup()
        {
            base.Startup();

            if (App.Graphics != null)
            {
                if (App.Graphics.Api != GraphicsApi.OpenGL && App.Graphics.Api != GraphicsApi.Vulkan)
                    throw new Exception("GLFW Only supports OpenGL and Vulkan Graphics APIs");
            }
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            GLFW.Terminate();
        }

        protected override void PostUpdate()
        {
            GLFW.PollEvents();
        }

        public override Window CreateWindow(string title, int width, int height)
        {
            return new GLFW_Window(title, width, height);
        }

        public override IntPtr ProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }
    }

}
