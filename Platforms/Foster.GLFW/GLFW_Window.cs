using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        internal readonly GLFW_System system;
        internal readonly GLFW_Context context;

        private string title;
        private bool opened;
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

        public override Point2 DrawSize
        {
            get
            {
                GLFW.GetFramebufferSize(context.Handle, out int width, out int height);
                return new Point2(width, height);
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

        public override bool Opened => opened;
        protected override Framework.System System => system;
        public override Context Context => context;

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
            this.system = system;
            this.title = title;

            var handle = GLFW.CreateWindow(width, height, title, IntPtr.Zero, system?.SharedContext ?? IntPtr.Zero);
            context = new GLFW_Context(system, handle);
            opened = true;
            Visible = visible;

            system.ActiveWindow = this;
        }

        public override void Present()
        {
            if (!opened)
                throw new Exception("This Window has been Closed");

            if (system.ActiveWindow != this)
                system.ActiveWindow = this;

            GLFW.SwapInterval(VSync ? 1 : 0);
            GLFW.SwapBuffers(context.Handle);
        }

        public override void Close()
        {
            if (opened)
            {
                GLFW.SetWindowShouldClose(context.Handle, true);
                opened = false;
            }
        }
    }
}
