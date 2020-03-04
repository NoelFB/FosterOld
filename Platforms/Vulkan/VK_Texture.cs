using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal class VK_Texture : Texture.Platform
    {

        internal bool isRenderTexture;

        public VK_Texture(VK_Graphics graphics, int width, int height, TextureFormat format)
        {

        }

        protected override void Init(Texture texture)
        {

        }

        protected override bool IsFrameBuffer()
        {
            return isRenderTexture;
        }

        protected override void SetFilter(TextureFilter filter)
        {

        }

        protected override void SetWrap(TextureWrap x, TextureWrap y)
        {

        }
        protected override void GetData<T>(Memory<T> buffer)
        {

        }

        protected override void SetData<T>(ReadOnlyMemory<T> buffer)
        {

        }

        protected override void Dispose()
        {

        }
    }
}
