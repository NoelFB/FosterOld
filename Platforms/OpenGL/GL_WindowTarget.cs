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

        protected override void ClearInternal(ClearFlags flags, Color color, float depth, int stencil)
        {
            lock (Window.Context)
            {
                Window.Context.MakeCurrent();

                // update the viewport
                var meta = graphics.GetContextMeta(Window.Context);
                if (meta.LastViewport == null || meta.LastViewport.Value != Viewport)
                {
                    GL.Viewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
                    meta.LastViewport = Viewport;
                }

                // we disable the scissor for clearing
                meta.ForceScissorUpdate = true;
                GL.Disable(GLEnum.SCISSOR_TEST);

                // clear
                graphics.Clear(flags, color, depth, stencil);
            }
        }

        protected override void DisposeResources()
        {
            // ...
        }
    }
}
