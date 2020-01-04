using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    public class VK_Graphics : Graphics
    {

        protected override void Initialized()
        {
            Api = GraphicsApi.Vulkan;
            ApiName = "Vulkan";
        }

        protected override void Startup()
        {
            base.Startup();
        }

        public override Mesh CreateMesh()
        {
            throw new NotImplementedException();
        }

        public override RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            throw new NotImplementedException();
        }

        public override Shader CreateShader(string vertexSource, string fragmentSource)
        {
            throw new NotImplementedException();
        }

        public override Texture CreateTexture(int width, int height, TextureFormat format)
        {
            throw new NotImplementedException();
        }

        protected override void ClearInternal(RenderTarget target, ClearFlags flags, Color color, float depth, int stencil)
        {
            throw new NotImplementedException();
        }

        protected override void RenderInternal(RenderTarget target, ref RenderPass pass)
        {
            throw new NotImplementedException();
        }
    }
}
