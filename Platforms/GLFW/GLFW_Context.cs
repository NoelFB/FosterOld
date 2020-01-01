using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Context : Context
    {
        internal readonly GLFW.Window GlfwWindowPointer;

        private bool disposed;

        internal GLFW_Context(GLFW_System system, GLFW.Window window) : base(system)
        {
            if (window.Ptr == IntPtr.Zero)
                throw new Exception("Unable to create Context");

            GlfwWindowPointer = window;
        }

        public override bool Disposed => disposed;

        public override int Width
        {
            get
            {
                if (disposed)
                    return 0;

                GLFW.GetFramebufferSize(GlfwWindowPointer, out int width, out _);
                return width;
            }
        }

        public override int Height
        {
            get
            {
                if (disposed)
                    return 0;

                GLFW.GetFramebufferSize(GlfwWindowPointer, out _, out int height);
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
                GLFW.SetWindowShouldClose(GlfwWindowPointer, true);
            }
        }

        public static implicit operator IntPtr(GLFW_Context context) => context.GlfwWindowPointer.Ptr;
    }
}
