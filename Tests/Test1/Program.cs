using Foster.Framework;
using System;

namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            App.RegisterModule<Foster.GLFW.GLFW_System>();
            App.RegisterModule<Foster.OpenGL.GL_Graphics>();

            App.Startup("hello", 1280, 720, () =>
            {
                App.System.CreateWindow("Hello 2", 1000, 1000, true);

                App.OnUpdate += Update;
                App.OnRender += Render;
            });
        }

        static void Update()
        {

        }

        static void Render(Window window)
        {
            App.Graphics.Clear(window.Title == "hello" ? Color.Red : Color.Green);
        }


    }
}
