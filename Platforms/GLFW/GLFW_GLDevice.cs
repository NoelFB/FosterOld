using Foster.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Foster.GLFW
{
    internal class GLFW_GLDevice : GLDevice
    {

        public readonly GLFW_System System;

        private readonly List<GLFW_GLContext> contexts = new List<GLFW_GLContext>();

        public GLFW_GLDevice(GLFW_System system)
        {
            System = system;
        }

        public override IntPtr GetProcAddress(string name)
        {
            return GLFW.GetProcAddress(name);
        }

        public override GLContext CreateContext()
        {
            // GLFW has no way to create a context without a window ...
            // so we create a Window and just hide it

            var window = System.CreateGlfwWindow("hidden-context", 128, 128, WindowFlags.Hidden);
            var context = new GLFW_GLContext(window);
            contexts.Add(context);
            return context;
        }

        public override GLContext GetWindowContext(Window window)
        {
            if (window is GLFW_Window glfwWindow)
            {
                for (int i = 0; i < contexts.Count; i++)
                    if (contexts[i].window.Ptr == glfwWindow.window.Ptr)
                        return contexts[i];
            }

            throw new Exception("Window does not have a valid Context");
        }

        public override void SetCurrentContext(GLContext? context)
        {
            if (context is GLFW_GLContext ctx && ctx != null)
                GLFW.MakeContextCurrent(ctx.window);
            else
                GLFW.MakeContextCurrent(IntPtr.Zero);
        }

        public override GLContext? GetCurrentContext()
        {
            var ptr = GLFW.GetCurrentContext();
            if (ptr != IntPtr.Zero)
            {
                for (int i = 0; i < contexts.Count; i++)
                    if (contexts[i].window.Ptr == ptr)
                        return contexts[i];
            }

            return null;
        }

        internal void Add(GLFW.Window window)
        {
            contexts.Add(new GLFW_GLContext(window));
        }

        internal void Remove(GLFW.Window window)
        {
            for (int i = 0; i < contexts.Count; i ++)
                if (contexts[i] is GLFW_GLContext context && context.window.Ptr == window.Ptr)
                {
                    contexts.RemoveAt(i);
                    break;
                }
        }

        internal void SetCurrentContext(GLFW.Window window)
        {
            for (int i = 0; i < contexts.Count; i++)
                if (contexts[i] is GLFW_GLContext context && context.window.Ptr == window.Ptr)
                {
                    SetCurrentContext(contexts[i]);
                    break;
                }
        }
    }
}
