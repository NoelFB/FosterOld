using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_Target : Target
    {

        private readonly GL_Graphics graphics;
        private readonly Dictionary<GraphicsContext, uint> framebuffers = new Dictionary<GraphicsContext, uint>();

        internal GL_Target(GL_Graphics graphics, int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat) : base(width, height)
        {
            this.graphics = graphics;

            // texture (color) attachments
            for (int i = 0; i < colorAttachmentFormats.Length; i++)
                attachments.Add(new GL_Texture(graphics, width, height, colorAttachmentFormats[i], true));

            if (depthFormat != TextureFormat.None)
                Depth = new GL_Texture(graphics, width, height, depthFormat, true);
        }

        ~GL_Target()
        {
            DisposeResources();
        }

        public void Bind(GraphicsContext context)
        {
            // create new framebuffer if it's needed
            // frame buffers are not shared between contexts
            if (!framebuffers.TryGetValue(context, out uint id))
            {
                id = GL.GenFramebuffer();

                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);

                // color attachments
                int i = 0;
                foreach (GL_Texture texture in attachments)
                {
                    GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.COLOR_ATTACHMENT0 + i, GLEnum.TEXTURE_2D, texture.ID, 0);
                    i++;
                }

                // depth stencil attachment
                if (Depth != null && Depth is GL_Texture depthTexture)
                {
                    GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, depthTexture.ID, 0);
                }

                framebuffers.Add(context, id);
            }
            else
            {
                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);
            }
        }

        protected override void ClearInternal(ClearFlags flags, Color color, float depth, int stencil)
        {
            graphics.ClearTarget(this, flags, color, depth, stencil);
        }

        protected override void RenderInternal(ref RenderPass pass)
        {
            graphics.RenderToTarget(this, ref pass);
        }

        protected override void DisposeResources()
        {
            if (framebuffers.Count > 0)
            {
                foreach (var kv in framebuffers)
                    graphics.GetContextMeta(kv.Key).FrameBuffersToDelete.Add(kv.Value);
                framebuffers.Clear();
            }

            base.DisposeResources();
        }

    }
}
