using Foster.Framework;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        internal readonly GLFW_Context context;

        private string title;
        private bool visible;
        private bool lastVsync;
        private bool focused;
        private bool mouseOver;

        public override Point2 Position
        {
            get
            {
                GLFW.GetWindowPos(context.Handle, out int x, out int y);
                return new Point2(x, y);
            }

            set
            {
                GLFW.SetWindowPos(context.Handle, value.X, value.Y);
            }
        }

        public override Point2 Size
        {
            get
            {
                GLFW.GetWindowSize(context.Handle, out int w, out int h);
                return new Point2(w, h);
            }

            set
            {
                GLFW.SetWindowSize(context.Handle, value.X, value.Y);
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

        public override Vector2 ScreenMouse
        {
            get
            {
                GLFW.GetCursorPos(context.Handle, out var curX, out var curY);
                GLFW.GetWindowPos(context.Handle, out var winX, out var winY);
                return new Vector2((float)curX + winX, (float)curY + winY);
            }
        }

        public override Vector2 ContentScale
        {
            get
            {
                GLFW.GetWindowContentScale(context.Handle, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override Framework.System System { get; }

        public override Context Context => context;

        public override bool Opened => !context.Disposed;

        public override bool Focused => focused;

        public override bool MouseOver => mouseOver;

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

        public override IntPtr Pointer => context.Handle.Ptr;

        private GLFW.WindowSizeFunc windowSizeCallbackRef;
        private GLFW.WindowFocusFunc windowFocusCallbackRef;
        private GLFW.CursorEnterFunc windowCursorEnterCallbackRef;

        public GLFW_Window(GLFW_System system, GLFW_Context context, string title, bool visible)
        {
            System = system;

            this.context = context;
            this.title = title;
            this.visible = visible;

            System.SetCurrentContext(context);
            GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            GLFW.SetWindowSizeCallback(context.Handle, windowSizeCallbackRef = OnWindowResize);
            GLFW.SetWindowFocusCallback(context.Handle, windowFocusCallbackRef = OnWindowFocus);
            GLFW.SetCursorEnterCallback(context.Handle, windowCursorEnterCallbackRef = OnCursorEnter);
        }

        private void OnWindowResize(GLFW.Window window, int width, int height)
        {
            OnResize?.Invoke();
        }

        private void OnWindowFocus(GLFW.Window window, int focused)
        {
            this.focused = (focused != 0);
        }

        private void OnCursorEnter(GLFW.Window window, int entered)
        {
            mouseOver = (entered != 0);
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
