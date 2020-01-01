using System;
using System.Collections.Generic;
using System.IO;
using Foster.Editor;
using Foster.Framework;
using Foster.Framework.Json;

namespace Foster.Engine
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            App.Modules.Register<GLFW.GLFW_System>();
            App.Modules.Register<OpenGL.GL_Graphics>();

            App.Start(() =>
            {
                App.Modules.Register(new StartEditor(args));
            });
        }
    }
}
