using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Framework
{
    public abstract class Texture : GraphicsResource
    {

        public readonly int Width;
        public readonly int Height;

        public abstract TextureFilter Filter { get; set; }
        public abstract TextureWrap WrapX { get; set; }
        public abstract TextureWrap WrapY { get; set; }

        public Texture(Graphics graphics) : base(graphics)
        {

        }

        public abstract void SetData<T>(Memory<T> buffer);
        public abstract void GetData<T>(Memory<T> buffer);

    }
}
