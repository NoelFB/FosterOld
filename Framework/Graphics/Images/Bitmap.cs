using System;
using System.Diagnostics;
using System.Drawing;
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

        public Bitmap(Stream stream)
        {
            if (Images.Read(stream, out Width, out Height, out var pixels) && pixels != null)
                Pixels = pixels;
            else
                throw new NotSupportedException("Stream is either an invalid or not supported image format");
        }

        public Bitmap(string path)
        {
            using var stream = File.OpenRead(path);

            if (Images.Read(stream, out Width, out Height, out var pixels) && pixels != null)
                Pixels = pixels;
            else
                throw new NotSupportedException("Stream is either an invalid or not supported image format");

            stream.Close();
        }

        /// <summary>
        /// Premultiplies the Color data of the Bitmap
        /// </summary>
        public void Premultiply()
        {
            unsafe
            {
                fixed (void* ptr = Pixels)
                {
                    byte* rgba = (byte*)ptr;

                    for (int i = 0, len = Pixels.Length * 4; i < len ; i += 4)
                    {
                        rgba[i + 0] = (byte)(rgba[i + 0] * rgba[i + 3] / 255);
                        rgba[i + 1] = (byte)(rgba[i + 1] * rgba[i + 3] / 255);
                        rgba[i + 2] = (byte)(rgba[i + 2] * rgba[i + 3] / 255);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the contents of the bitmap to the given data
        /// </summary>
        public void SetPixels(Memory<Color> source)
        {
            source.Span.CopyTo(Pixels);
        }

        /// <summary>
        /// Sets the contents of the bitmap over the given Rect to the given data
        /// </summary>
        public void SetPixels(RectInt destination, Memory<Color> source)
        {
            // TODO: perform bounds checking?

            var src = source.Span;
            var dst = new Span<Color>(Pixels);

            for (int y = 0; y < destination.Height; y++)
            {
                var from = src.Slice(y * destination.Width, destination.Width);
                var to = dst.Slice(destination.X + (destination.Y + y) * Width, destination.Width);

                from.CopyTo(to);
            }
        }

        public void GetPixels(Memory<Color> destination)
        {
            Pixels.CopyTo(destination);
        }

        public void GetPixels(Memory<Color> dest, Point2 destPosition, Point2 destSize, RectInt sourceRect)
        {
            var src = new Span<Color>(Pixels);
            var dst = dest.Span;

            // can't be outside of the source image
            if (sourceRect.Left < 0) sourceRect.Left = 0;
            if (sourceRect.Top < 0) sourceRect.Top = 0;
            if (sourceRect.Right > Width) sourceRect.Right = Width;
            if (sourceRect.Bottom > Height) sourceRect.Bottom = Height;

            // can't be larger than our destination
            if (sourceRect.Width > destSize.X - destPosition.X)
                sourceRect.Width = destSize.X - destPosition.X;
            if (sourceRect.Height > destSize.Y - destPosition.Y)
                sourceRect.Height = destSize.Y - destPosition.Y;

            for (int y = 0; y < sourceRect.Height; y++)
            {
                var from = src.Slice(sourceRect.X + (sourceRect.Y + y) * Width, sourceRect.Width);
                var to = dst.Slice(destPosition.X + (destPosition.Y + y) * destSize.X, sourceRect.Width);

                from.CopyTo(to);
            }
        }

        public Bitmap GetSubBitmap(RectInt source)
        {
            var bmp = new Bitmap(source.Width, source.Height);
            GetPixels(bmp.Pixels, Point2.Zero, source.Size, source);
            return bmp;
        }

        public void SavePng(string path)
        {
            using var stream = File.Create(path);
            PNG.Write(stream, Width, Height, Pixels);
        }

        public void SavePng(Stream stream)
        {
            PNG.Write(stream, Width, Height, Pixels);
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
