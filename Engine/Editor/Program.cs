using Foster.Framework;
using Foster.GLFW;
using Foster.GuiSystem;
using Foster.OpenGL;
using System;
using System.IO;

namespace Foster.Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            App.Modules.Register<GLFW_System>();
            App.Modules.Register<GLFW_Input>();
            App.Modules.Register<GL_Graphics>();

            App.Start(Ready);
        }

        private static void Ready()
        {
            var font = new SpriteFont(Path.Combine(App.System.AppDirectory, "Content", "Roboto-Medium.ttf"), 64, Charsets.ASCII);
            var gui = App.Modules.Register(new Gui(font, "Gui", 1280, 720));

            var scene = gui.CreatePanel("Scene", new Rect(32, 32, 200, 200));
            scene.DockWith(null);
            var game = gui.CreatePanel("Game", scene);

            var assets = gui.CreatePanel("Assets", new Rect(32, 32, 200, 200));
            assets.DockLeftOf(scene);
            var inspector = gui.CreatePanel("Inspector", new Rect(32, 32, 200, 200));
            inspector.DockRightOf(scene);
            var log = gui.CreatePanel("Log", new Rect(32, 32, 200, 200));
            log.DockBottomOf(scene);

            game.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    imgui.PushId(i);

                    if (imgui.Header("WHAT"))
                    {
                        imgui.Title("Something");
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

                        imgui.Title("Dangerous");
                        if (imgui.Button("Popout"))
                            game.Popout();
                        if (imgui.Button("Close"))
                            game.Close();

                        imgui.EndHeader();
                    }

                    imgui.PopId();
                }
            };
        }
    }
}
