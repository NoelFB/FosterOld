using Foster.Framework;
using System;
using System.IO;

namespace Test1
{
    internal class Program
    {
        private static void Main()
        {
            App.RegisterModule<Foster.GLFW.GLFW_System>();
            App.RegisterModule<Foster.OpenGL.GL_Graphics>();

            App.Startup("hello", 1280, 720, () =>
            {
                var game = new Game();
                App.OnRender += game.Render;

            });
        }

        class Game
        {
            private Batch2D batch;
            private SpriteFont font;

            public Game()
            {
                batch = new Batch2D(App.Graphics);

                font = new SpriteFont("RobotoMono-Medium.ttf", 128, Charsets.ASCII);
            }

            public void Render(Window window)
            {
                batch.Clear();
                batch.PushMatrix(new Vector2(32, 32), Vector2.One, Vector2.Zero, 0f);
                batch.Text(font, "Welcome to the world wide web\n\nI'm happy to be here :)", Color.White);
                batch.PopMatrix();
                batch.Render();
            }
        }


    }
}
