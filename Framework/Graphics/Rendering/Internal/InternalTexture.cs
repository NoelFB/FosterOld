using System;
using System.IO;

namespace Foster.Framework
{
    public abstract class InternalTexture : InternalResource
    {

        /// <summary>
        /// If the Texture should be flipped vertically when drawing
        /// For example, OpenGL Render Targets usually require this
        /// </summary>
        public abstract bool FlipVertically { get; }

        protected internal abstract void SetFilter(TextureFilter filter);
        protected internal abstract void SetWrap(TextureWrap x, TextureWrap y);
        protected internal abstract void SetColor(Memory<Color> buffer);
        protected internal abstract void GetColor(Memory<Color> buffer);
    }
}
