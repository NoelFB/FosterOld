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
                App.Graphics.Clear(0x113355);

                var p = 140f;

                batch.Clear();
                batch.PushMatrix(new Vector2(p, p), Vector2.One * 0.3f, Vector2.Zero, 0f);
                batch.Text(font, "Welcome to the world wide web\n\nI'm happy to be here :)", Color.White * 0.9f);
                batch.PopMatrix();

                batch.PushMatrix(new Vector2(p, App.Graphics.Viewport.Height - p - font.LineHeight * 0.3f), Vector2.One * 0.3f, Vector2.Zero, 0f);
                batch.Text(font, $"> FPS: {Time.FPS}", 0x44eeaa);
                batch.PopMatrix();
                batch.Render();
            }
        }


    }
}
