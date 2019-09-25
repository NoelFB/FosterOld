using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GLFW
{
    public class GLFW_Context : Context
    {
        internal GLFW.Window Handle;
        public bool IsDisposed;

        internal GLFW_Context(GLFW_System system, GLFW.Window window)
        {
            Handle = window;
            System = system;
        }

        protected override Framework.System System { get; }

        public override bool Disposed => IsDisposed;

        public static implicit operator IntPtr(GLFW_Context context) => context.Handle.Ptr;
    }
}
