using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_RenderingContext : RenderingContext
    {
        internal readonly GLFW.Window GlfwWindowPointer;

        private bool disposed;

        internal GLFW_RenderingContext(GLFW_RenderingState state, GLFW.Window window) : base(state)
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

        protected override void DisposeContextResources()
        {
            if (!disposed)
            {
                disposed = true;
                GLFW.SetWindowShouldClose(GlfwWindowPointer, true);
            }
        }

        public static implicit operator IntPtr(GLFW_RenderingContext context) => context.GlfwWindowPointer.Ptr;
    }
}
