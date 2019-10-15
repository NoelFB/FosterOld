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
            private Batch2D batch;
            private Batch2D batch2;
            private SpriteFont font;
            private Context context;

            List<Vector2> Dots = new List<Vector2>();

            protected override void Startup()
            {
                batch = new Batch2D(App.Graphics);
                batch2 = new Batch2D(App.Graphics);

                var win = App.System.CreateWindow("Hello!", 1280, 720);
                win.OnClose += App.Exit;

                context = App.System.CreateContext();

                var thread = new Thread(new ThreadStart(Load));
                thread.Start();

            }

            private void Load()
            {
                App.System.SetCurrentContext(context);

                font = new SpriteFont("RobotoMono-Medium.ttf", 128, Charsets.ASCII);
                font.Charset['a'].Image.Texture.Filter = TextureFilter.Nearest;

                var b = new Batch2D();
                b.Rect(0, 0, 32, 32, Color.Red);
                b.Render();
                //b.Dispose();

                context.Dispose();
            }

            protected override void Update()
            {
                if (App.Input.State.Mouse.Pressed(MouseButtons.Left))
                    Dots.Add(App.System.Windows[0].Mouse);
            }

            protected override void Render(Window window)
            {
                App.Graphics.Clear(0x113355);

                if (font == null)
                {

                }
                else
                {
                    var p = 140f;

                    batch.Clear();

                    foreach (var dot in Dots)
                    {
                        batch.Rect(dot.X - 8, dot.Y - 8, 16, 16, Color.Red);
                    }

                    batch.PushMatrix(new Vector2(p, p), Vector2.One * 3f, Vector2.Zero, 0f);
                    batch.Text(font, "Welcome to the world wide web\n\nI'm happy to be here :)", Color.White * 1.0f);
                    batch.PopMatrix();

                    batch.PushMatrix(new Vector2(p, App.Graphics.Viewport.Height - p - font.LineHeight * 0.3f), Vector2.One * 0.3f, Vector2.Zero, 0f);
                    batch.Text(font, $"> FPS: {Time.FPS}", 0x44eeaa);
                    batch.PopMatrix();

                    batch.PushMatrix(window.Mouse, Vector2.One * 0.3f, Vector2.Zero, 0f);
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
}
