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
        private readonly uint renderBuffer;

        internal GL_Target(GL_Graphics graphics, int width, int height, int textures, bool depthBuffer, bool stencilBuffer)
        {
            this.graphics = graphics;

            // texture (color) attachments
            for (int i = 0; i < textures; i++)
            {
                attachments.Add(new GL_Texture(graphics, width, height)
                {
                    flipVertically = true
                });
            }

            // depth buffer
            if (depthBuffer)
            {
                renderBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, renderBuffer);
                GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH24_STENCIL8, width, height);
            }

            if (stencilBuffer)
            {
                throw new NotImplementedException();
            }
        }

        ~GL_Target()
        {
            Dispose();
        }

        public void Bind()
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

                    if (renderBuffer > 0)
                        GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, renderBuffer);

                    framebuffers.Add(context, id);
                }
                else
                {
                    GL.BindFramebuffer(GLEnum.FRAMEBUFFER, id);
                }
            }
        }

        protected override void Dispose()
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
