using System;
using Foster.Framework.Internal;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Foster.Framework
{
    public class Texture
    {

        /// <summary>
        /// The internal texture object
        /// </summary>
        public readonly InternalTexture Internal;

        /// <summary>
        /// Gets the Width of the Texture
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Gets the Height of the Texture
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The Texture Data Format
        /// </summary>
        public readonly TextureFormat Format;

        /// <summary>
        /// The Texture Filter to be used while drawing
        /// </summary>
        public TextureFilter Filter
        {
            get => filter;
            set => Internal.SetFilter(filter = value);
        }

        /// <summary>
        /// The Horizontal Wrapping mode
        /// </summary>
        public TextureWrap WrapX
        {
            get => wrapX;
            set => Internal.SetWrap(wrapX = value, wrapY);
        }

        /// <summary>
        /// The Vertical Wrapping mode
        /// </summary>
        public TextureWrap WrapY
        {
            get => wrapY;
            set => Internal.SetWrap(wrapX, wrapY = value);
        }

        private TextureFilter filter;
        private TextureWrap wrapX;
        private TextureWrap wrapY;

        public Texture(Graphics graphics, int width, int height, TextureFormat format = TextureFormat.Color) : this(graphics, null, width, height, format)
        {

        }

        public Texture(int width, int height, TextureFormat format = TextureFormat.Color) : this(App.Graphics, width, height, format)
        {

        }

        public Texture(Bitmap bitmap) : this(App.Graphics, bitmap.Width, bitmap.Height)
        {
            Internal.SetData<Color>(bitmap.Pixels);
        }

        internal Texture(Graphics graphics, InternalTexture? internalTexture, int width, int height, TextureFormat format)
        {
            Internal = internalTexture ?? graphics.CreateTexture(width, height, format);
            Width = width;
            Height = height;
            Format = format;
            WrapX = TextureWrap.Clamp;
            WrapY = TextureWrap.Clamp;
            Filter = TextureFilter.Linear;
        }

        /// <summary>
        /// Creates a Bitmap with the Texture Color data
        /// </summary>
        public Bitmap AsBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            Internal.GetData<Color>(new Memory<Color>(bitmap.Pixels));
            return bitmap;
        }

        /// <summary>
        /// Sets the Texture Color data from the given buffer
        /// </summary>
        public void SetColor(Memory<Color> buffer) => Internal.SetData<Color>(buffer);

        /// <summary>
        /// Writes the Texture Color data to the given buffer
        /// </summary>
        public void GetColor(Memory<Color> buffer) => Internal.GetData<Color>(buffer);

        /// <summary>
        /// Sets the Texture data from the given buffer
        /// </summary>
        public void SetData<T>(Memory<T> buffer) => Internal.SetData<T>(buffer);

        /// <summary>
        /// Writes the Texture data to the given buffer
        /// </summary>
        public void GetData<T>(Memory<T> buffer) => Internal.GetData<T>(buffer);

        /// <summary>
        /// Disposes the internal Texture resources
        /// </summary>
        public void Dispose()
        {
            Internal.Dispose();
        }
    }
}
