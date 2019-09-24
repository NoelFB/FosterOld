using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        private readonly GLFW_System system;
        internal GLFW.Window handle;
        private string title;
        private bool opened;
        private bool visible;

        public override RectInt Bounds
        {
            get
            {
                GLFW.GetWindowPos(handle, out int x, out int y);
                GLFW.GetWindowSize(handle, out int w, out int h);

                return new RectInt(x, y, w, h);
            }

            set
            {
                GLFW.SetWindowPos(handle, value.X, value.Y);
                GLFW.SetWindowSize(handle, value.Width, value.Height);
            }
        }

        public override Point2 DrawSize
        {
            get
            {
                GLFW.GetFramebufferSize(handle, out int width, out int height);
                return new Point2(width, height);
            }
        }

        public override Vector2 PixelSize
        {
            get
            {
                IntPtr monitor = GLFW.GetWindowMonitor(handle);
                GLFW.GetMonitorContentScale(monitor, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override bool Opened => opened;

        protected override Framework.System System => system;

        public override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(handle, title = value);
        }

        public override bool VSync { get; set; } = true;

        public override bool Bordered
        {
            get => GLFW.GetWindowAttrib(handle, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(handle, GLFW.WindowAttributes.Decorated, value);
        }

        public override bool Resizable
        {
            get => GLFW.GetWindowAttrib(handle, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(handle, GLFW.WindowAttributes.Resizable, value);
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
                    GLFW.ShowWindow(handle);
                else
                    GLFW.HideWindow(handle);
            }
        }

        public GLFW_Window(GLFW_System system, string title, int width, int height, bool visible = true)
        {
            this.system = system;
            this.title = title;

            GLFW.WindowHint(GLFW.WindowHints.ScaleToMonitor, true);
            GLFW.WindowHint(GLFW.WindowHints.DoubleBuffer, true);

            var share = IntPtr.Zero;
            if (system.Windows.Count > 0 && (system.Windows[0] is GLFW_Window first))
                share = first.handle;

            handle = GLFW.CreateWindow(width, height, title, IntPtr.Zero, share);
            opened = true;
            Visible = visible;

            MakeCurrent();
        }

        protected override void MakeCurrentInternal()
        {
            if (!opened)
                throw new Exception("This Window has been Closed");

            GLFW.MakeContextCurrent(handle);
            GLFW.SwapInterval(VSync ? 1 : 0);
        }

        public override void Present()
        {
            if (!opened)
                throw new Exception("This Window has been Closed");

            GLFW.SwapBuffers(handle);
        }

        public override void Close()
        {
            if (opened)
            {
                GLFW.SetWindowShouldClose(handle, true);
                opened = false;
            }
        }
    }
}
