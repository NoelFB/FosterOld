using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        internal readonly GLFW_Context context;

        private string title;
        private bool visible;
        private bool lastVsync;

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

        public override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(context.Handle, out var xpos, out var ypos);
                return new Vector2((float)xpos, (float)ypos);
            }
        }

        public GLFW_Window(GLFW_System system, GLFW_Context context, string title, bool visible)
        {
            System = system;

            this.context = context;
            this.title = title;
            this.visible = visible;

            System.SetCurrentContext(context);
            GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
        }

        public override void Present()
        {
            if (lastVsync != VSync)
            {
                System.SetCurrentContext(context);
                GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            }
            
            GLFW.SwapBuffers(context.Handle);
        }

        public override void Close()
        {
            if (!Context.Disposed)
                Context.Dispose();
        }
    }
}
