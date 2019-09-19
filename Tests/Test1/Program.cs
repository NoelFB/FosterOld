using Foster.Framework;
using System;
using System.IO;

namespace Test1
{
    internal class Program
    {
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

                batch = new Batch2D(App.Graphics);

                PngFormat.Read(File.OpenRead("03_japanese.png"), out int w, out int h, out Color[] pixels, true);
                PngFormat.Write(File.OpenWrite("test2.png"), w, h, pixels);

                texture = App.Graphics.CreateTexture(w, h);
                texture.SetData(new Memory<Color>(pixels));

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
            batch.PushMatrix(Matrix3x2.CreateTranslation(100, 100));
            batch.PushMatrix(Matrix3x2.CreateRotation(0.5f));
            batch.PushMatrix(Matrix3x2.CreateTranslation(100, 200));
            batch.Rect(0, 0, 200, 200, Color.Red * 0.5f);
            batch.PopMatrix();
            batch.Rect(100, 0, 200, 100, Color.White * 0.25f);
            batch.Rect(100, 0, 200, 100, Color.White * 0.25f);
            batch.Rect(100, 0, 200, 100, Color.White * 0.25f);
            batch.Rect(100, 0, 200, 100, Color.White * 0.25f);
            batch.PopMatrix();
            batch.PopMatrix();

            batch.SetTexture(texture);
            batch.Quad(new Vector2(0, 0), new Vector2(640, 0), new Vector2(640, 480), new Vector2(0, 480), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), Color.White);

            var subtex = new Subtexture(texture, new Rect(400, 32, 200, 200));
            batch.Image(subtex, new Vector2(1000, 400), Color.White);

            batch.Render();
        }


    }
}
