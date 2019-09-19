using Foster.Framework;
using System;
using System.IO;

namespace Test1
{
    internal class Program
    {
        private static Target? target;
        private static Batch2D? batch;
        private static Texture? texture;
        private static float x;

        private static void Main()
        {
            App.RegisterModule<Foster.GLFW.GLFW_System>();
            App.RegisterModule<Foster.OpenGL.GL_Graphics>();

            App.Startup("hello", 1280, 720, () =>
            {
                if (App.Graphics == null)
                {
                    throw new Exception("Expecting a Graphics Module");
                }

                target = App.Graphics.CreateTarget(500, 500);

                batch = new Batch2D(App.Graphics);

                PngFormat.Read(File.OpenRead("03_japanese.png"), out int w, out int h, out Color[] pixels, true);
                texture = App.Graphics.CreateTexture(w, h);
                texture.SetData(new Memory<Color>(pixels));

                App.Graphics.Target(target);
                App.Graphics.Clear(Color.Red);

                batch.Clear();
                Console.WriteLine("RECT");
                batch.Rect(-64, -64, 64, 64, Color.Green);
                batch.SetTexture(texture);
                Console.WriteLine("QUAD");
                batch.Quad(new Vector2(0, 0), new Vector2(600, 0), new Vector2(600, 400), new Vector2(0, 800), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), Color.White);

                Console.WriteLine("IMAGE");
                batch.Image(texture, Vector2.Zero, Vector2.One * 0.35f, Vector2.Zero, 0f, Color.White);
                batch.Render();

                App.OnUpdate += Update;
                App.OnRender += Render;
            });
        }

        private static void Update()
        {
            x += 1f;
        }

        private static void Render(Window window)
        {
            App.Graphics!.Clear(new Color(0.5f, 0.5f, 0.5f, 1.0f));

            batch!.Clear();

            batch.SetTexture(texture);
            batch.Quad(new Vector2(0, 0), new Vector2(640, 0), new Vector2(640, 480), new Vector2(0, 480), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), Color.White);

            var subtex = new Subtexture(texture, new Rect(400, 32, 200, 200));
            batch.Image(subtex, new Vector2(1000, 400), Color.White);
            batch.Image(target, new Vector2(32, 600), Color.White);

            batch.Render();
        }


    }
}
