using Foster.Editor;
using Foster.Framework;
using Foster.GLFW;
using Foster.GuiSystem;
using Foster.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Foster.Engine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App.Modules.Register<GLFW_System>();
            App.Modules.Register<GLFW_Input>();
            App.Modules.Register<GL_Graphics>();

            App.Start(() =>
            {
                App.Modules.Register(new Startup(args));
            });
        }
    }
}
