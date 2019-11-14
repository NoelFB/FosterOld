using Foster.Framework;
using Foster.GLFW;
using Foster.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Test1
{
    internal class Program
    {
        
        private static void Main()
        {
            App.Modules.Register<GLFW_System>();
            App.Modules.Register<GLFW_Input>();
            App.Modules.Register<GL_Graphics>();

            App.Start(Ready);
        }

        private static void Ready()
        {
            var font = new SpriteFont(Path.Combine(App.System.AppDirectory, "SourceSansPro-SemiBold.ttf"), 64, Charsets.ASCII);
            var gui = App.Modules.Register(new Gui(font, "Gui", 1280, 720));
            

            for (int i = 0; i < 5; i++)
            {
                var panel = gui.CreatePanel($"Hello World {i}", new Rect(32, 32, 400, 400));
                panel.OnRefresh = (imgui) =>
                {
                    for (int i = 0; i < 4; i++)
                        if (imgui.Button("What " + i))
                            Console.WriteLine("PRESSED " + i);
                };
            }
        }

    }
}
