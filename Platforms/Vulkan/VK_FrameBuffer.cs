using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal class VK_FrameBuffer : FrameBuffer.Platform
    {


        public VK_FrameBuffer(VK_Graphics graphics, int width, int height, TextureFormat[] attachments)
        {
            for (int i = 0; i < attachments.Length; i ++)
            {
                var attachment = new Texture(graphics, width, height, attachments[i]);
                var texture = (VK_Texture)attachment.Implementation;
                texture.isRenderTexture = true;
                Attachments.Add(attachment);
            }
        }

        protected override void Dispose()
        {

        }
    }
}
