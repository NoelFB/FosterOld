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
            var enabled = false;

            var font = new SpriteFont("RobotoMono-Medium.ttf", 64, Charsets.ASCII);

            var gui = App.Modules.Register(new Gui(font, "Hello!", 1280, 720));
            gui.Workspace.OnClose = App.Exit;

            var panel = gui.CreatePanel("Something", new RectInt(32, 32, 400, 500));
            panel.Imgui.Refresh = (imgui) =>
            {
                Console.WriteLine(gui.Workspace.Bounds);

                imgui.Title("Some nice Content");
                imgui.Label("Lot's to talk about here ...");

                imgui.Row(3);
                imgui.Button("Hello", ImguiContext.PreferredSize);
                imgui.Button("What's up?");
                imgui.Separator();
                imgui.Label("Other stuff");

                if (imgui.Button("Toggle?"))
                    enabled = !enabled;

                if (enabled)
                {
                    imgui.PushIndent(32f);
                    imgui.Button("Hello!!");
                    imgui.Button("What's up?!");
                    imgui.Button("OK interesting ... 1");
                    imgui.Button("OK interesting ... 2");
                    imgui.Button("OK interesting ... 3");
                    imgui.Button("OK interesting ... 4");
                    imgui.Button("OK interesting ... 5");
                    imgui.Button("OK interesting ... 6");
                    imgui.Button("OK interesting ... 7");
                    imgui.Button("OK interesting ... 8");
                    imgui.Button("OK interesting ... 9");
                    imgui.PopIndent();
                }

                if (imgui.Header("More Info", true))
                {
                    imgui.Row(4);
                    imgui.Button("0");
                    imgui.Button("1");
                    imgui.Button("2");
                    imgui.Button("3");
                    imgui.EndHeader();
                }

                if (imgui.BeginGroup("Hello", 190))
                {
                    imgui.Button("something");
                    imgui.Button("another1");
                    imgui.Button("another2");
                    imgui.Button("another3");
                    imgui.Button("another4");
                    imgui.Button("another5");
                    imgui.Button("another6");
                    imgui.Button("another7");
                    imgui.EndGroup();
                }

                imgui.Button("another1");
                imgui.Button("another2");
                imgui.Button("another3");
            };
        }


    }
}
