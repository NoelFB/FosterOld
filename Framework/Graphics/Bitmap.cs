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
            if (pixels.Length < width * height)
                throw new Exception("Pixels array doesn't fit the Bitmap size");

            Pixels = pixels;
            Width = width;
            Height = height;
        }

        public Bitmap(Stream stream, ImageFormat format)
        {
            if (format.IsValid(stream))
            {
                if (format.Read(stream, out Width, out Height, out Pixels))
                    return;
            }

            throw new Exception($"Stream is not a valid {format.Name} image format");
        }

        public Bitmap(Stream stream)
        {
            foreach (var format in ImageFormat.Formats)
            {
                if (format.IsValid(stream) && format.Read(stream, out Width, out Height, out Pixels))
                    return;
            }

            throw new NotImplementedException("Stream is not a valid image format or is not implemented");
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
