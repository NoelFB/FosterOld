using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Texture used for Rendering
    /// </summary>
    public class Texture : IDisposable
    {

        /// <summary>
        /// Internal Implementation of the Texture
        /// </summary>
        public abstract class Platform
        {
            protected internal abstract void Init(Texture texture);
            protected internal abstract void Resize(int width, int height);
            protected internal abstract void SetFilter(TextureFilter filter);
            protected internal abstract void SetWrap(TextureWrap x, TextureWrap y);
            protected internal abstract void SetData<T>(ReadOnlyMemory<T> buffer);
            protected internal abstract void GetData<T>(Memory<T> buffer);
            protected internal abstract bool IsFrameBuffer();
            protected internal abstract void Dispose();
        }

        /// <summary>
        /// Default Texture Filter used for all Textures
        /// </summary>
        public static TextureFilter DefaultTextureFilter = TextureFilter.Linear;

        /// <summary>
        /// A reference to the internal platform implementation of the Texture
        /// </summary>
        public readonly Platform Implementation;

        /// <summary>
        /// The Texture Data Format
        /// </summary>
        public readonly TextureFormat Format;

        /// <summary>
        /// Gets the Width of the Texture
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the Height of the Texture
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Whether the Texture is part of a FrameBuffer
        /// </summary>
        public bool IsFrameBuffer => Implementation.IsFrameBuffer();

        /// <summary>
        /// The Size of the Texture, in bytes
        /// </summary>
        public int Size => Width * Height * (Format switch
        {
            TextureFormat.Color => 4,
            TextureFormat.Red => 1,
            TextureFormat.RG => 2,
            TextureFormat.RGB => 3,
            TextureFormat.DepthStencil => 4,
            _ => throw new Exception("Invalid Texture Format")
        });

        /// <summary>
        /// The Texture Filter to be used while drawing
        /// </summary>
        public TextureFilter Filter
        {
            get => filter;
            set => Implementation.SetFilter(filter = value);
        }

        /// <summary>
        /// The Horizontal Wrapping mode
        /// </summary>
        public TextureWrap WrapX
        {
            get => wrapX;
            set => Implementation.SetWrap(wrapX = value, wrapY);
        }

        /// <summary>
        /// The Vertical Wrapping mode
        /// </summary>
        public TextureWrap WrapY
        {
            get => wrapY;
            set => Implementation.SetWrap(wrapX, wrapY = value);
        }

        private readonly Graphics graphics;
        private TextureFilter filter = TextureFilter.Linear;
        private TextureWrap wrapX = TextureWrap.Clamp;
        private TextureWrap wrapY = TextureWrap.Clamp;

        public Texture(Graphics graphics, int width, int height, TextureFormat format = TextureFormat.Color)
        {
            if (format == TextureFormat.None)
                throw new Exception("Invalid Texture Format");

            if (width <= 0 || height <= 0)
                throw new Exception("Texture must have a size larger than 0");

            this.graphics = graphics;
            Width = width;
            Height = height;
            Format = format;

            Implementation = graphics.CreateTexture(Width, Height, Format);
            Implementation.Init(this);

            Filter = DefaultTextureFilter;
        }

        public Texture(int width, int height, TextureFormat format = TextureFormat.Color) 
            : this(App.Graphics, width, height, format)
        {

        }

        public Texture(Bitmap bitmap) 
            : this(App.Graphics, bitmap.Width, bitmap.Height, TextureFormat.Color)
        {
            Implementation.SetData<Color>(bitmap.Pixels);
        }

        public Texture(string path) 
            : this(new Bitmap(path))
        {

        }

        public Texture(Stream stream) 
            : this(new Bitmap(stream))
        {

        }

        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Texture must have a size larger than 0");

            if (Width != width || Height != height)
            {
                Width = width;
                Height = height;

                Implementation.Resize(width, height);
            }
        }

        /// <summary>
        /// Creates a Bitmap with the Texture Color data
        /// </summary>
        public Bitmap AsBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            GetData<Color>(new Memory<Color>(bitmap.Pixels));
            return bitmap;
        }

        /// <summary>
        /// Sets the Texture Color data from the given buffer
        /// </summary>
        public void SetColor(ReadOnlyMemory<Color> buffer) => SetData<Color>(buffer);

        /// <summary>
        /// Writes the Texture Color data to the given buffer
        /// </summary>
        public void GetColor(Memory<Color> buffer) => GetData<Color>(buffer);

        /// <summary>
        /// Sets the Texture data from the given buffer
        /// </summary>
        public void SetData<T>(ReadOnlyMemory<T> buffer)
        {
            if (Marshal.SizeOf<T>() * buffer.Length < Size)
                throw new Exception("Buffer is smaller than the Size of the Texture");

            Implementation.SetData(buffer);
        }

        /// <summary>
        /// Writes the Texture data to the given buffer
        /// </summary>
        public void GetData<T>(Memory<T> buffer)
        {
            if (Marshal.SizeOf<T>() * buffer.Length < Size)
                throw new Exception("Buffer is smaller than the Size of the Texture");

            Implementation.GetData(buffer);
        }

        public void SavePng(string path)
        {
            using var stream = File.OpenWrite(path);
            SavePng(stream);
        }

        public void SavePng(Stream stream)
        {
            var color = new Color[Width * Height];

            if (Format == TextureFormat.Color || Format == TextureFormat.DepthStencil)
            {
                GetData<Color>(color);
            }
            else
            {
                // TODO:
                // do this inline with a single buffer

                var buffer = new byte[Size];
                GetData<byte>(buffer);

                if (Format == TextureFormat.Red)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        color[i].R = buffer[i];
                        color[i].A = 255;
                    }
                }
                else if (Format == TextureFormat.RG)
                {
                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        color[i].R = buffer[i + 0];
                        color[i].G = buffer[i + 1];
                        color[i].A = 255;
                    }
                }
                else if (Format == TextureFormat.RGB)
                {
                    for (int i = 0; i < buffer.Length; i += 3)
                    {
                        color[i].R = buffer[i + 0];
                        color[i].G = buffer[i + 1];
                        color[i].B = buffer[i + 2];
                        color[i].A = 255;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // We may need to flip our buffer.
            // This is due to some rendering APIs drawing from the bottom left (OpenGL).
            if (IsFrameBuffer && graphics.OriginBottomLeft)
            {
                for (int y = 0; y < Height / 2; y++)
                {
                    var a = y * Width;
                    var b = (Height - y - 1) * Width;

                    for (int x = 0; x < Width; x ++, a++, b++)
                    {
                        var temp = color[a];
                        color[a] = color[b];
                        color[b] = temp;
                    }
                }
            }

            PNG.Write(stream, Width, Height, color);
        }

        public void SaveJpg(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveJpg(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Implementation.Dispose();
        }
    }
}
