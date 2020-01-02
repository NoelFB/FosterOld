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
            graphics.ClearTarget(this, flags, color, depth, stencil);
        }

        protected override void RenderInternal(ref RenderPass pass)
        {
            graphics.RenderToTarget(this, ref pass);
        }

        protected override void DisposeResources()
        {
            // ...
        }
    }
}
