using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Bitmap
    {

        public readonly Color[] Pixels;
        public readonly int Width;
        public readonly int Height;

        public Bitmap(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Width and Height must be larger than 0");

            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        public void Premultiply()
        {
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = Pixels[i].Premultiply();
        }

    }
}
