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
        private readonly uint depthBuffer;

        internal GL_Target(GL_Graphics graphics, int width, int height, int attachmentCount, DepthFormat depthFormat)
        {
            this.graphics = graphics;

            // texture (color) attachments
            for (int i = 0; i < attachmentCount; i++)
            {
                attachments.Add(new GL_Texture(graphics, width, height)
                {
                    flipVertically = true
                });
            }

            // depth 24
            if (depthFormat == DepthFormat.Depth24)
            {
                depthBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, depthBuffer);
                GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH_COMPONENT24, width, height);
            }
            // depth 24 stencil 8
            else if (depthFormat == DepthFormat.Depth24Stencil8)
            {
                depthBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, depthBuffer);
                GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH24_STENCIL8, width, height);
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

                    if (depthBuffer > 0)
                        GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, depthBuffer);

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
