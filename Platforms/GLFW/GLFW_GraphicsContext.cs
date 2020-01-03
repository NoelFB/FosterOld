using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_GraphicsContext : GraphicsContext
    {
        internal readonly GLFW.Window GlfwWindowPointer;

        private bool disposed;

        internal GLFW_GraphicsContext(GLFW_GraphicsDevice device, GLFW.Window window) : base(device)
        {
            if (window.Ptr == IntPtr.Zero)
                throw new Exception("Unable to create Context");

            GlfwWindowPointer = window;
        }

        public override bool Disposed => disposed;

        public override void Dispose()
        {
            if (!disposed)
            {
                if (GraphicsDevice.GetCurrentContext() == this)
                    GraphicsDevice.SetCurrentContext(GraphicsDevice.Contexts[0]);

                disposed = true;
                GLFW.SetWindowShouldClose(GlfwWindowPointer, true);
            }
        }

        public static implicit operator IntPtr(GLFW_GraphicsContext context) => context.GlfwWindowPointer.Ptr;
    }
}
