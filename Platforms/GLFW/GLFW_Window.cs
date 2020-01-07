using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        private readonly GLFW_System system;
        internal readonly IntPtr window;

        private readonly GLFW.WindowCloseFunc windowCloseCallbackRef;
        private readonly GLFW.WindowSizeFunc windowSizeCallbackRef;
        private readonly GLFW.WindowFocusFunc windowFocusCallbackRef;
        private readonly GLFW.CursorEnterFunc windowCursorEnterCallbackRef;

        private string title;
        private bool visible;
        private bool lastVsync;
        private bool focused;
        private bool mouseOver;
        private bool disposed;

        public override Point2 Position
        {
            get
            {
                GLFW.GetWindowPos(window, out int x, out int y);
                return new Point2(x, y);
            }

            set
            {
                GLFW.SetWindowPos(window, value.X, value.Y);
            }
        }

        public override Point2 Size
        {
            get
            {
                GLFW.GetWindowSize(window, out int w, out int h);
                return new Point2(w, h);
            }

            set
            {
                GLFW.SetWindowSize(window, value.X, value.Y);
            }
        }

        public override Point2 DrawableSize
        {
            get
            {
                GLFW.GetFramebufferSize(window, out int width, out int height);
                return new Point2(width, height);
            }
        }

        public override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(window, out var xpos, out var ypos);
                return new Vector2((float)xpos, (float)ypos);
            }
        }

        public override Vector2 ScreenMouse
        {
            get
            {
                GLFW.GetCursorPos(window, out var curX, out var curY);
                GLFW.GetWindowPos(window, out var winX, out var winY);
                return new Vector2((float)curX + winX, (float)curY + winY);
            }
        }

        public override Vector2 ContentScale
        {
            get
            {
                GLFW.GetWindowContentScale(window, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override bool Opened => !disposed;

        public override bool Focused => focused;

        public override bool MouseOver => mouseOver;

        public override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(window, title = value);
        }

        public override bool VSync { get; set; } = true;

        public override bool Bordered
        {
            get => GLFW.GetWindowAttrib(window, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(window, GLFW.WindowAttributes.Decorated, value);
        }

        public override bool Resizable
        {
            get => GLFW.GetWindowAttrib(window, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(window, GLFW.WindowAttributes.Resizable, value);
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
                    GLFW.ShowWindow(window);
                else
                    GLFW.HideWindow(window);
            }
        }

        public override IntPtr Pointer => GLFW.GetWindowUserPointer(window);

        internal GLFW_Window(GLFW_System system, IntPtr window, string title, bool visible)
        {
            this.system = system;
            this.window = window;
            this.title = title;
            this.visible = visible;

            // opengl swap interval
            if (App.Graphics is IGraphicsOpenGL)
            {
                system.SetCurrentGLContext(window);
                GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            }

            GLFW.SetWindowCloseCallback(this.window, windowCloseCallbackRef = OnWindowClose);
            GLFW.SetWindowSizeCallback(this.window, windowSizeCallbackRef = OnWindowResize);
            GLFW.SetWindowFocusCallback(this.window, windowFocusCallbackRef = OnWindowFocus);
            GLFW.SetCursorEnterCallback(this.window, windowCursorEnterCallbackRef = OnCursorEnter);
        }

        private void OnWindowClose(IntPtr window)
        {
            GLFW.SetWindowShouldClose(window, false);
            OnCloseRequested?.Invoke(this);
        }

        private void OnWindowResize(IntPtr window, int width, int height)
        {
            OnResize?.Invoke(this);
        }

        private void OnWindowFocus(IntPtr window, int focused)
        {
            this.focused = (focused != 0);
            if (this.focused)
                OnFocus?.Invoke(this);
        }

        private void OnCursorEnter(IntPtr window, int entered)
        {
            mouseOver = (entered != 0);
        }

        protected override void Present()
        {
            if (lastVsync != VSync)
            {
                if (App.Graphics is IGraphicsOpenGL)
                {
                    system.SetCurrentGLContext(window);
                    GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
                }
            }

            GLFW.SwapBuffers(window);
        }

        public override void Close()
        {
            if (!disposed)
            {
                disposed = true;
                GLFW.SetWindowShouldClose(window, true);
            }
        }
    }
}
