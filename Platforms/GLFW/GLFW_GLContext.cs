using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_GLContext : GLContext
    {
        internal readonly GLFW.Window window;
        internal bool disposed;

        internal GLFW_GLContext(GLFW.Window window)
        {
            this.window = window;
        }

        public override bool IsDisposed => disposed;

        public override void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                GLFW.SetWindowShouldClose(window, true);
            }
        }
    }
}
