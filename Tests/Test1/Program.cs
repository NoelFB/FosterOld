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
            App.RegisterModule<Game>();
            App.Start();
        }

        class Game : Module
        {
            private Batch2D batch;
            private Batch2D batch2;
            private SpriteFont font;

            protected override void Startup()
            {

                batch = new Batch2D(App.Graphics);
                font = new SpriteFont("RobotoMono-Medium.ttf", 128, Charsets.ASCII);
                batch2 = new Batch2D(App.Graphics);

                App.System.CreateWindow("Hello!", 1280, 720);
            }

            protected override void Render(Window window)
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

                p *= 2;

                if (!batch2.Disposed)
                {
                    batch2.Clear();
                    batch2.PushMatrix(new Vector2(p, p), Vector2.One * 0.3f, Vector2.Zero, 0f);
                    batch2.Text(font, "what's up?", Color.White * 0.9f);
                    batch2.PopMatrix();
                    batch2.Render();
                    //batch2.Dispose();
                }
            }
        }


    }
}
