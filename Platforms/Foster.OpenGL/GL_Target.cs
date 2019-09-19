using Foster.Framework;

namespace Foster.OpenGL
{
    public class GL_Target : Target
    {

        public uint ID;

        public GL_Target(GL_Graphics graphics, int width, int height, int textures, bool depthBuffer) : base(graphics)
        {
            ID = GL.GenFramebuffer();
            Width = width;
            Height = height;

            GL.BindFramebuffer(GLEnum.FRAMEBUFFER, ID);

            // texture (color) attachments
            for (int i = 0; i < textures; i++)
            {
                GL_Texture color = new GL_Texture(graphics, width, height);
                color.flipVertically = true;
                attachments.Add(color);

                GL.FramebufferTexture2D(GLEnum.FRAMEBUFFER, (GLEnum.COLOR_ATTACHMENT0 + i), GLEnum.TEXTURE_2D, color.ID, 0);
            }

            // depth buffer
            if (depthBuffer)
            {
                HasDepthBuffer = true;

                uint renderBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, renderBuffer);
                GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH24_STENCIL8, width, height);
                GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, renderBuffer);
                GL.BindRenderbuffer(GLEnum.RENDERBUFFER, 0);
            }

            GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                uint targetID = ID;
                if (Graphics is GL_Graphics graphics)
                {
                    graphics.OnResourceCleanup += () => GL.DeleteFramebuffer(targetID);
                }
            }

            base.Dispose();
        }

    }
}
