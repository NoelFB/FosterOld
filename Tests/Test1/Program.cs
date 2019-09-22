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

                int size = 256;
                Color[] pixels = new Color[size * size];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = Color.Red;

                PngFormat.Write(File.OpenWrite("test.png"), size, size, pixels);
            });
        }


    }
}
