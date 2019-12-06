using System;
using Foster.Editor;
using Foster.Framework;

namespace Foster.Engine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App.Modules.Register<GLFW.GLFW_System>();
            App.Modules.Register<GLFW.GLFW_Input>();
            App.Modules.Register<OpenGL.GL_Graphics>();

            App.Start(() =>
            {
                App.Modules.Register(new Startup(args));
            });
        }
    }
}
