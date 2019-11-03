using Foster.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Test1
{
    internal class Program
    {
        private static void Main()
        {
            App.Modules.Register<Foster.GLFW.GLFW_System>();
            App.Modules.Register<Foster.GLFW.GLFW_Input>();
            App.Modules.Register<Foster.OpenGL.GL_Graphics>();
            App.Modules.Register<Game>();

            App.Start();
        }

        class Game : Module
        {
            private SpriteFont font;
            private ImguiContext imgui2;

            protected override void Startup()
            {
                var win = App.System.CreateWindow("Hello!", 1280, 720);
                win.OnClose += App.Exit;

                font = new SpriteFont("RobotoMono-Medium.ttf", 64, Charsets.ASCII);

                var enabled = true;

                imgui2 = new ImguiContext(font);
                imgui2.Bounds = new Rect(32, 32, 400, 800);
                imgui2.Refresh = (imgui) =>
                {
                    imgui.Title("Some nice Content");
                    imgui.Label("Lot's to talk about here ...");

                    imgui.Row(3);
                    imgui.Button("Hello", ImguiContext.PreferredSize);
                    imgui.Button("What's up?");
                    imgui.Button("OK");
                    imgui.Separator();
                    imgui.Label("Other stuff");

                    if (imgui.Button("Toggle?"))
                        enabled = !enabled;

                    if (enabled)
                    {
                        imgui.PushIndent(32f);
                        imgui.Button("Hello!!");
                        imgui.Button("What's up?!");
                        imgui.Button("OK interesting ...");
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

                    imgui.BeginGroup("Hello", 200, true);
                    imgui.Button("something");
                    imgui.Button("another");
                    imgui.EndGroup();
                };

            }

            protected override void Update()
            {
                imgui2.Update(App.Window!.Mouse);
            }

            protected override void Render(Window window)
            {
                App.Graphics.Clear(0x113355);
                imgui2.Render();
            }
        }


    }
}
