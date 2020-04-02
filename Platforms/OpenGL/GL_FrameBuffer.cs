using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_FrameBuffer : FrameBuffer.Platform
    {

        private readonly GL_Graphics graphics;
        private readonly Dictionary<ISystemOpenGL.Context, uint> framebuffers = new Dictionary<ISystemOpenGL.Context, uint>();

        internal GL_FrameBuffer(GL_Graphics graphics, int width, int height, TextureFormat[] attachments)
        {
            this.graphics = graphics;

            for (int i = 0; i < attachments.Length; i++)
            {
                var attachment = new Texture(graphics, width, height, attachments[i]);
                var glTexture = (GL_Texture)attachment.Implementation;
                glTexture.isRenderTexture = true;
                Attachments.Add(attachment);
            }
        }

        ~GL_FrameBuffer()
        {
            Dispose();
        }

        protected override void Resize(int width, int height)
        {
            Dispose();

            for (int i = 0; i < Attachments.Count; i++)
                Attachments[i].Resize(width, height);
        }

        public void Bind(ISystemOpenGL.Context context)
        {
            // create new framebuffer if it's needed
            // frame buffers are not shared between contexts
            if (!framebuffers.TryGetValue(context, out uint id))
            {
                id = GL.GenFramebuffer();

                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);

                // color attachments
                int i = 0;
                foreach (Texture texture in Attachments)
                {
                    if (texture.Implementation is GL_Texture glTexture)
                    {
                        if (texture.Format.IsTextureColorFormat())
                        {
                            GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.COLOR_ATTACHMENT0 + i, GLEnum.TEXTURE_2D, glTexture.ID, 0);
                            i++;
                        }
                        else
                        {
                            GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.TEXTURE_2D, glTexture.ID, 0);
                        }
                    }
                }

                framebuffers.Add(context, id);
            }
            else
            {
                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);
            }
        }

        protected override void Dispose()
        {
            if (framebuffers.Count > 0)
            {
                foreach (var kv in framebuffers)
                    graphics.GetContextMeta(kv.Key).FrameBuffersToDelete.Add(kv.Value);
                framebuffers.Clear();
            }
        }

    }
}
