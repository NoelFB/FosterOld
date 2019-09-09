using Foster.Framework;
using System;

namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            App.Register<Foster.GLFW.GLFW_System>();
            App.Register<Foster.OpenGL.OpenGL_Graphics>();

            App.Startup("hello", 1280, 720, null);
        }
    }
}
