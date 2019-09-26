using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        internal readonly GLFW_Context context;

        private string title;
        private bool visible;

        public override RectInt Bounds
        {
            get
            {
                GLFW.GetWindowPos(context.Handle, out int x, out int y);
                GLFW.GetWindowSize(context.Handle, out int w, out int h);

                return new RectInt(x, y, w, h);
            }

            set
            {
                GLFW.SetWindowPos(context.Handle, value.X, value.Y);
                GLFW.SetWindowSize(context.Handle, value.Width, value.Height);
            }
        }

        public override Vector2 PixelSize
        {
            get
            {
                IntPtr monitor = GLFW.GetWindowMonitor(context.Handle);
                GLFW.GetMonitorContentScale(monitor, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override Framework.System System { get; }

        public override Context Context => context;

        public override bool Opened => !context.Disposed;

        public override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(context.Handle, title = value);
        }

        public override bool VSync { get; set; } = true;

        public override bool Bordered
        {
            get => GLFW.GetWindowAttrib(context.Handle, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(context.Handle, GLFW.WindowAttributes.Decorated, value);
        }

        public override bool Resizable
        {
            get => GLFW.GetWindowAttrib(context.Handle, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(context.Handle, GLFW.WindowAttributes.Resizable, value);
        }

        public override bool Fullscreen
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                if (visible)
                    GLFW.ShowWindow(context.Handle);
                else
                    GLFW.HideWindow(context.Handle);
            }
        }

        public GLFW_Window(GLFW_System system, string title, int width, int height, bool visible = true)
        {
            System = system;
            this.title = title;

            GLFW_Context? shared = null;
            if (system.Contexts.Count > 0)
                shared = system.Contexts[0];

            var handle = GLFW.CreateWindow(width, height, title, IntPtr.Zero, shared ?? IntPtr.Zero);
            context = new GLFW_Context(system, handle);
            system.Contexts.Add(context);

            Visible = visible;

            system.SetCurrentContext(context);
        }

        public override void Present()
        {
            GLFW.MakeContextCurrent(context.Handle);
            GLFW.SwapInterval(VSync ? 1 : 0);
            GLFW.SwapBuffers(context.Handle);
        }

        public override void Close()
        {
            if (!Context.Disposed)
                Context.Dispose();
        }
    }
}
