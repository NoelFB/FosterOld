using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.OpenGL
{
    internal class GL_WindowTarget : WindowTarget
    {

        private readonly GL_Graphics graphics;

        public GL_WindowTarget(GL_Graphics graphics, Window window) : base(window)
        {
            this.graphics = graphics;
        }

        protected override void ClearTarget(ClearFlags flags, Color color, float depth, int stencil)
        {
            // Window Targets can only be cleared from the main thread
            // so we don't need to do any thread / context checks ...

            lock (Window.Context)
            {
                Window.Context.MakeCurrent();
                GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
                graphics.ApplyRenderState(Window.Context, ref RenderState);
                graphics.Clear(flags, color, depth, stencil);
            }
        }

        protected override void DisposeResources()
        {
            // ...
        }
    }
}
