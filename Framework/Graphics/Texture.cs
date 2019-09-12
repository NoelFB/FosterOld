using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Framework
{
    public abstract class Texture : GraphicsResource
    {

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract TextureFilter Filter { get; set; }
        public abstract TextureWrap WrapX { get; set; }
        public abstract TextureWrap WrapY { get; set; }

        public Texture(Graphics graphics) : base(graphics)
        {

        }

        public abstract void SetData<T>(Memory<T> buffer);
        public abstract void GetData<T>(Memory<T> buffer);

        public Bitmap AsBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            GetData(new Memory<Color>(bitmap.Pixels));
            return bitmap;
        }

        public static Texture Create(int width, int height)
        {
            return App.GetModule<Graphics>().CreateTexture(width, height);
        }
    }
}
