using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Foster.GLFW
{
    public class GLFW_RenderingState : RenderingState
    {

        public GLFW_RenderingState(GLFW_System system) : base(system)
        {

        }

        public override RenderingContext CreateContext()
        {
            return CreateContextInternal("hidden-context", 128, 128, WindowFlags.Hidden);
        }

        internal GLFW_RenderingContext CreateContextInternal(string title, int width, int height, WindowFlags flags)
        {
            if (Thread.CurrentThread.ManagedThreadId != System.MainThreadId)
                throw new Exception("Creating a Context must be called from the Main Thread");

            GLFW.WindowHint(GLFW_Enum.VISIBLE, !flags.HasFlag(WindowFlags.Hidden));
            GLFW.WindowHint(GLFW_Enum.FOCUS_ON_SHOW, false);
            GLFW.WindowHint(GLFW_Enum.TRANSPARENT_FRAMEBUFFER, flags.HasFlag(WindowFlags.Transparent));
            GLFW.WindowHint(GLFW_Enum.SCALE_TO_MONITOR, flags.HasFlag(WindowFlags.ScaleToMonitor));
            GLFW.WindowHint(GLFW_Enum.SAMPLES, flags.HasFlag(WindowFlags.MultiSampling) ? 4 : 0);

            GLFW_RenderingContext? shared = null;
            if (Contexts.Count > 0)
                shared = Contexts[0] as GLFW_RenderingContext;

            // GLFW has no way to create a context without a window
            // so any background contexts also just create a hidden window

            var window = GLFW.CreateWindow(width, height, title, IntPtr.Zero, shared ?? IntPtr.Zero);
            var context = new GLFW_RenderingContext(this, window);
            contexts.Add(context);

            return context;
        }

        internal void RemoveContext(RenderingContext context)
        {
            contexts.Remove(context);
        }

        protected override void SetCurrentContextInternal(RenderingContext? context)
        {
            if (context is GLFW_RenderingContext ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.GlfwWindowPointer);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }
    }
}
