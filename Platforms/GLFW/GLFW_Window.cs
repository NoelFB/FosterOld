using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window.Platform
    {
        private readonly GLFW_System system;
        internal readonly IntPtr pointer;

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
                GLFW.GetWindowPos(pointer, out int x, out int y);
                return new Point2(x, y);
            }

            set
            {
                GLFW.SetWindowPos(pointer, value.X, value.Y);
            }
        }

        public override Point2 Size
        {
            get
            {
                GLFW.GetWindowSize(pointer, out int w, out int h);
                return new Point2(w, h);
            }

            set
            {
                GLFW.SetWindowSize(pointer, value.X, value.Y);
            }
        }

        public override Point2 DrawableSize
        {
            get
            {
                GLFW.GetFramebufferSize(pointer, out int width, out int height);
                return new Point2(width, height);
            }
        }

        public override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(pointer, out var xpos, out var ypos);
                return new Vector2((float)xpos, (float)ypos);
            }
        }

        public override Vector2 ScreenMouse
        {
            get
            {
                GLFW.GetCursorPos(pointer, out var curX, out var curY);
                GLFW.GetWindowPos(pointer, out var winX, out var winY);
                return new Vector2((float)curX + winX, (float)curY + winY);
            }
        }

        public override Vector2 ContentScale
        {
            get
            {
                GLFW.GetWindowContentScale(pointer, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override bool Opened => !disposed;

        public override bool Focused => focused;

        public override bool MouseOver => mouseOver;

        public override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(pointer, title = value);
        }

        public override bool VSync { get; set; } = true;

        public override bool Bordered
        {
            get => GLFW.GetWindowAttrib(pointer, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(pointer, GLFW.WindowAttributes.Decorated, value);
        }

        public override bool Resizable
        {
            get => GLFW.GetWindowAttrib(pointer, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(pointer, GLFW.WindowAttributes.Resizable, value);
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
                    GLFW.ShowWindow(pointer);
                else
                    GLFW.HideWindow(pointer);
            }
        }

        public override IntPtr Pointer => GLFW.GetWindowUserPointer(pointer);

        internal GLFW_Window(GLFW_System system, IntPtr pointer, string title, bool visible)
        {
            this.system = system;
            this.pointer = pointer;
            this.title = title;
            this.visible = visible;

            // opengl swap interval
            if (App.Graphics is IGraphicsOpenGL)
            {
                system.SetCurrentGLContext(pointer);
                GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            }

            GLFW.SetWindowCloseCallback(this.pointer, windowCloseCallbackRef = OnWindowClose);
            GLFW.SetWindowSizeCallback(this.pointer, windowSizeCallbackRef = OnWindowResize);
            GLFW.SetWindowFocusCallback(this.pointer, windowFocusCallbackRef = OnWindowFocus);
            GLFW.SetCursorEnterCallback(this.pointer, windowCursorEnterCallbackRef = OnCursorEnter);
        }

        private void OnWindowClose(IntPtr window)
        {
            GLFW.SetWindowShouldClose(window, false);
            OnCloseRequested?.Invoke();
        }

        private void OnWindowResize(IntPtr window, int width, int height)
        {
            OnResize?.Invoke();
        }

        private void OnWindowFocus(IntPtr window, int focused)
        {
            this.focused = (focused != 0);
            if (this.focused)
                OnFocus?.Invoke();
        }

        private void OnCursorEnter(IntPtr window, int entered)
        {
            mouseOver = (entered != 0);
        }

        public override void Present()
        {
            if (lastVsync != VSync)
            {
                if (App.Graphics is IGraphicsOpenGL)
                {
                    system.SetCurrentGLContext(pointer);
                    GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
                }
            }

            GLFW.SwapBuffers(pointer);
        }

        public override void Close()
        {
            if (!disposed)
            {
                disposed = true;
                GLFW.SetWindowShouldClose(pointer, true);
            }
        }
    }
}
