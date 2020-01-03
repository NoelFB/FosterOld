using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Foster.GLFW
{
    internal class GLFW_GraphicsDevice : GraphicsDevice
    {

        public readonly new GLFW_System System;

        public GLFW_GraphicsDevice(GLFW_System system) : base(system)
        {
            System = system;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        public override GraphicsContext CreateContext()
        {
            return CreateContextInternal("hidden-context", 128, 128, WindowFlags.Hidden);
        }

        internal GLFW_GraphicsContext CreateContextInternal(string title, int width, int height, WindowFlags flags)
        {
            GLFW.WindowHint(GLFW_Enum.VISIBLE, !flags.HasFlag(WindowFlags.Hidden));
            GLFW.WindowHint(GLFW_Enum.FOCUS_ON_SHOW, false);
            GLFW.WindowHint(GLFW_Enum.TRANSPARENT_FRAMEBUFFER, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW_Enum.SCALE_TO_MONITOR, flags.HasFlag(WindowFlags.ScaleToMonitor));
            GLFW.WindowHint(GLFW_Enum.SAMPLES, flags.HasFlag(WindowFlags.MultiSampling) ? 4 : 0);

            GLFW_GraphicsContext? shared = null;
            if (Contexts.Count > 0)
                shared = Contexts[0] as GLFW_GraphicsContext;

            // GLFW has no way to create a context without a window
            // so any background contexts also just create a hidden window

            var window = GLFW.CreateWindow(width, height, title, IntPtr.Zero, shared ?? IntPtr.Zero);
            var context = new GLFW_GraphicsContext(this, window);
            contexts.Add(context);

            return context;
        }

        internal void RemoveContext(GraphicsContext context)
        {
            contexts.Remove(context);
        }

        protected override void SetCurrentContextInternal(GraphicsContext? context)
        {
            if (context is GLFW_GraphicsContext ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.GlfwWindowPointer);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }
    }
}
