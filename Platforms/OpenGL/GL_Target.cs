using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Collections.Generic;

namespace Foster.OpenGL
{
    public class GL_Target : InternalTarget
    {

        private readonly GL_Graphics graphics;
        private readonly Dictionary<Context, uint> framebuffers = new Dictionary<Context, uint>();

        internal GL_Target(GL_Graphics graphics, int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            this.graphics = graphics;

            // texture (color) attachments
            for (int i = 0; i < colorAttachmentFormats.Length; i++)
            {
                attachments.Add(new GL_Texture(graphics, width, height, colorAttachmentFormats[i])
                {
                    flipVertically = true
                });
            }

            if (depthFormat != TextureFormat.None)
            {
                depth = new GL_Texture(graphics, width, height, depthFormat);
            }
        }

        ~GL_Target()
        {
            DisposeResources();
        }

        public void Bind()
        {
            var context = App.System.GetCurrentContext();
            if (context != null)
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
                    if (depth != null && depth is GL_Texture depthTexture)
                    {
                        if (depthTexture.format == TextureFormat.Depth24)
                            GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_ATTACHMENT, GLEnum.RENDERBUFFER, depthTexture.ID, 0);
                        else if (depthTexture.format == TextureFormat.Depth24Stencil8)
                            GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, depthTexture.ID, 0);
                    }

                    framebuffers.Add(context, id);
                }
                else
                {
                    GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);
                }
            }
        }

        protected override void DisposeResources()
        {
            if (framebuffers.Count > 0)
            {
                foreach (var kv in framebuffers)
                {
                    var context = kv.Key;
                    var framebuffer = kv.Value;

                    if (!graphics.FrameBuffersToDelete.TryGetValue(context, out var list))
                        graphics.FrameBuffersToDelete[context] = list = new List<uint>();

                    list.Add(framebuffer);
                }

                framebuffers.Clear();
            }
        }

    }
}
