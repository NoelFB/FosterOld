using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class WindowTarget : RenderTarget
    {

        public readonly Window Window;

        public override int Width => Window?.DrawableWidth ?? 0;

        public override int Height => Window?.DrawableHeight ?? 0;

        public WindowTarget(Window window)
        {
            Window = window;
            Drawable = false;
        }

        internal void BeginRendering()
        {
            RenderState = new RenderState(this);
            Drawable = true;
        }

        internal void EndRendering()
        {
            Drawable = false;
        }
    }
}
