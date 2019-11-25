using Foster.Framework;
using Foster.GuiSystem;
using Foster.GLFW;
using Foster.SDL2;
using Foster.OpenGL;
using System;
using System.IO;

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
            var font = new SpriteFont(Path.Combine(App.System.AppDirectory, "Roboto-Medium.ttf"), 64, Charsets.ASCII);
            var gui = App.Modules.Register(new Gui(font, "Gui", 1280, 720));

            var scene = gui.CreatePanel("Assets", new Rect(32, 32, 200, 200));
            scene = gui.CreatePanel("Inspector", new Rect(32, 32, 200, 200));
            scene = gui.CreatePanel("Log", new Rect(32, 32, 200, 200));
            scene = gui.CreatePanel("Scene", new Rect(32, 32, 200, 200));
            var game = gui.CreatePanel("Game", new Rect(200, 32, 200, 200));

            game.OnRefresh = (imgui) =>
            {
                if (imgui.Header("WHAT"))
                {
                    imgui.Row(3);
                    imgui.Label("Position X");
                    if (imgui.Button("Snap Left"))
                        game.DockLeftOf(scene);
                    if (imgui.Button("Snap Right"))
                        game.DockRightOf(scene);

                    imgui.Row(3);
                    imgui.Label("Position Y");
                    if (imgui.Button("Snap Top"))
                        game.DockTopOf(scene);
                    if (imgui.Button("Snap Bottom"))
                        game.DockBottomOf(scene);

                    if (imgui.Button("Popout"))
                        game.Popout();
                    if (imgui.Button("Close"))
                        game.Close();

                    imgui.EndHeader();
                }

                imgui.Button(new Icon(font.Charset['o'].Image));
            };
        }

    }
}
