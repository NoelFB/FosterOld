using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    public class Bitmap
    {

        public readonly Color[] Pixels;
        public readonly int Width;
        public readonly int Height;

        public Bitmap(int width, int height) 
            : this(width, height, new Color[width * height])
        {
        }

        public Bitmap(int width, int height, Color[] pixels)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Width and Height must be larger than 0");
            if (pixels.Length != width * height)
                throw new Exception("Pixels array doesn't fit the Bitmap size");

            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public void Premultiply()
        {
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = Pixels[i].Premultiply();
        }

        public void SetPixels(Memory<Color> pixels)
        {
            SetPixels(new RectInt(0, 0, Width, Height), pixels);
        }

        public void SetPixels(RectInt desintation, Memory<Color> pixels)
        {
            var src = pixels.Span;
            var dst = new Span<Color>(Pixels);

            for (int y = 0; y < desintation.Height; y++)
            {
                var from = src.Slice(y * desintation.Width, desintation.Width);
                var to = dst.Slice(desintation.X + (desintation.Y + y) * Width, desintation.Width);

                from.CopyTo(to);
            }
        }

        public Bitmap Clone()
        {
            return new Bitmap(Width, Height, new Memory<Color>(Pixels).ToArray());
        }

    }
}
