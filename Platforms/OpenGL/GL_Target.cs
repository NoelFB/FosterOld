using Foster.Framework;
using System;
using System.Collections.Generic;

namespace Foster.OpenGL
{
    public class GL_Target : Target
    {

        private Dictionary<Context, uint> framebuffers = new Dictionary<Context, uint>();
        private uint renderBuffer;

        public GL_Target(GL_Graphics graphics, int width, int height, int textures, bool depthBuffer, bool stencilBuffer) : base(graphics)
        {
            Width = width;
            Height = height;

            // texture (color) attachments
            for (int i = 0; i < textures; i++)
            {
                GL_Texture texture = new GL_Texture(graphics, width, height);
                texture.flipVertically = true;
                attachments.Add(texture);
            }

            // depth buffer
            if (depthBuffer)
            {
                HasDepthBuffer = true;

                renderBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, renderBuffer);
                GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH24_STENCIL8, width, height);
            }

            if (stencilBuffer)
            {
                throw new NotImplementedException();
            }
        }

        public void Use()
        {
            var context = App.System.GetCurrentContext();
            if (context != null)
            {
                // create new framebuffer if it's needed
                if (!framebuffers.TryGetValue(context, out uint id))
                {
                    id = GL.GenFramebuffer();

                    GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);

                    int i = 0;
                    foreach (GL_Texture texture in attachments)
                    {
                        GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, GLEnum.COLOR_ATTACHMENT0 + i, GLEnum.TEXTURE_2D, texture.ID, 0);
                        i++;
                    }

                    if (HasDepthBuffer)
                        GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, renderBuffer);

                    framebuffers.Add(context, id);
                }
                else
                {
                    GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);
                }
            }
        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                if (Graphics is GL_Graphics graphics)
                {
                    foreach (var kv in framebuffers)
                    {
                        var context = kv.Key;
                        var framebuffer = kv.Value;

                        if (!graphics.FrameBuffersToDelete.TryGetValue(context, out var list))
                            graphics.FrameBuffersToDelete[context] = list = new List<uint>();

                        list.Add(framebuffer);
                    }
                }
            }

            base.Dispose();
        }

    }
}
