using System;
using System.IO;

namespace Foster.Framework.Internal
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
        protected internal abstract void SetData<T>(ReadOnlyMemory<T> buffer);
        protected internal abstract void GetData<T>(Memory<T> buffer);
    }
}
