using System;
using Foster.Framework.Internal;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Foster.Framework
{
    public class Texture : Asset
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

        public Texture(Graphics graphics, int width, int height) : this(graphics, null, width, height)
        {

        }

        public Texture(int width, int height) : this(App.Graphics, width, height)
        {

        }

        public Texture(Bitmap bitmap) : this(App.Graphics, bitmap.Width, bitmap.Height)
        {
            Internal.SetColor(bitmap.Pixels);
        }

        internal Texture(Graphics graphics, InternalTexture? internalTexture, int width, int height)
        {
            Internal = internalTexture ?? graphics.CreateTexture(width, height);
            Width = width;
            Height = height;
            WrapX = TextureWrap.Clamp;
            WrapY = TextureWrap.Clamp;
            Filter = TextureFilter.Linear;
        }

        /// <summary>
        /// Creates a Bitmap with the Texture Color data
        /// </summary>
        /// <returns></returns>
        public Bitmap AsBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            Internal.GetColor(new Memory<Color>(bitmap.Pixels));
            return bitmap;
        }

        /// <summary>
        /// Sets the Texture Color data from the given buffer
        /// </summary>
        /// <param name="buffer"></param>
        public void SetColor(Memory<Color> buffer) => Internal.SetColor(buffer);

        /// <summary>
        /// Writes the Texture Color data to the given buffer
        /// </summary>
        /// <param name="buffer"></param>
        public void GetColor(Memory<Color> buffer) => Internal.GetColor(buffer);

        /// <summary>
        /// Disposes the internal Texture resources
        /// </summary>
        public override void Dispose()
        {
            Internal.Dispose();
        }
    }
}
