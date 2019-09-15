using Foster.Framework;
using System;
using System.Runtime.InteropServices;

namespace Test1
{
    class Program
    {

        static Batch2D? batch;
        static float x;

        static void Main()
        {
            App.RegisterModule<Foster.GLFW.GLFW_System>();
            App.RegisterModule<Foster.OpenGL.GL_Graphics>();

            App.Startup("hello", 1280, 720, () =>
            {
                if (App.Graphics == null)
                    throw new Exception("Expecting a Graphics Module");

                batch = new Batch2D(App.Graphics);


                App.OnUpdate += Update;
                App.OnRender += Render;
            });
        }

        static void Update()
        {
            x += 1f;


        }

        static void Render(Window window)
        {
            App.Graphics!.Clear(new Color(0.5f, 0.5f, 0.5f, 1.0f));
            var t = new System.Diagnostics.Stopwatch();
            t.Start();

            batch.Clear();
            for (int i = 0; i < 10000; i++)
            {
                batch.Rect(new Rect(x + 32 + i, 32 + i, 50, 50), Color.Lerp(Color.Green, Color.Blue, i / 10000f));

            }
            batch.Render();

            Console.WriteLine(t.ElapsedMilliseconds);
        }


    }
}
