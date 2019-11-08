using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GLFW
{
    public class GLFW_Context : Context
    {
        internal readonly GLFW.Window Handle;
        internal int ActiveThreadId = 0;

        private bool disposed;

        internal GLFW_Context(GLFW_System system, GLFW.Window window)
        {
            if (window.Ptr == IntPtr.Zero)
                throw new Exception("Unable to create Context");

            System = system;
            Handle = window;
        }

        public override Framework.System System { get; }

        public override bool Disposed => disposed;

        public override int Width
        {
            get
            {
                if (disposed)
                    return 0;

                GLFW.GetFramebufferSize(Handle, out int width, out _);
                return width;
            }
        }

        public override int Height
        {
            get
            {
                if (disposed)
                    return 0;

                GLFW.GetFramebufferSize(Handle, out _, out int height);
                return height;
            }
        }

        public override void Dispose()
        {
            if (!disposed)
            {
                if (System.GetCurrentContext() == this)
                    System.SetCurrentContext(null);

                disposed = true;
                GLFW.SetWindowShouldClose(Handle, true);
            }
        }

        public static implicit operator IntPtr(GLFW_Context context) => context.Handle.Ptr;
    }
}
