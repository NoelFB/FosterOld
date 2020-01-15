using System;
using System.IO;

namespace Foster.Framework
{
    /// <summary>
    /// Stores a 2D Image
    /// </summary>
    public class Bitmap
    {
        /// <summary>
        /// The Pixel array of the Bitmap
        /// </summary>
        public readonly Color[] Pixels;

        /// <summary>
        /// The Width of the Bitmap, in Pixels
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The Height of the Bitmap, in Pixels
        /// </summary>
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

            throw new NotSupportedException("Stream is either an invalid or not supported image format");
        }

        public Bitmap(string path, ImageFormat format)
        {
            using var stream = File.OpenRead(path);

            if (format.IsValid(stream))
            {
                if (format.Read(stream, out Width, out Height, out Pixels))
                    return;
            }

            throw new Exception($"Stream is not a valid {format.Name} image format");
        }

        public Bitmap(string path)
        {
            using var stream = File.OpenRead(path);

            foreach (var format in ImageFormat.Formats)
            {
                if (format.IsValid(stream) && format.Read(stream, out Width, out Height, out Pixels))
                    return;
            }

            throw new NotSupportedException("Stream is either an invalid or not supported image format");
        }

        /// <summary>
        /// Premultiplies the Color data of the Bitmap
        /// </summary>
        public void Premultiply()
        {
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = Pixels[i].Premultiply();
        }

        /// <summary>
        /// Sets the contents of the bitmap to the given data
        /// </summary>
        public void SetPixels(Memory<Color> pixels)
        {
            pixels.Span.CopyTo(Pixels);
        }

        /// <summary>
        /// Sets the contents of the bitmap over the given Rect to the given data
        /// </summary>
        public void SetPixels(RectInt desintation, Memory<Color> pixels)
        {
            // TODO: perform bounds checking?

            var src = pixels.Span;
            var dst = new Span<Color>(Pixels);

            for (int y = 0; y < desintation.Height; y++)
            {
                var from = src.Slice(y * desintation.Width, desintation.Width);
                var to = dst.Slice(desintation.X + (desintation.Y + y) * Width, desintation.Width);

                from.CopyTo(to);
            }
        }

        public void SavePng(string path)
        {
            using var stream = File.Create(path);
            ImageFormat.Png.Write(stream, Width, Height, Pixels);
        }

        public void SavePng(Stream stream)
        {
            ImageFormat.Png.Write(stream, Width, Height, Pixels);
        }

        public void SaveJpg(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveJpg(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clones the Bitmap
        /// </summary>
        public Bitmap Clone()
        {
            return new Bitmap(Width, Height, new Memory<Color>(Pixels).ToArray());
        }

    }
}
