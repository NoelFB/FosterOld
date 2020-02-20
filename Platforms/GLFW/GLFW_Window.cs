using Foster.Framework;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

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
        private bool fullscreen;
        private RectInt storedBounds;

        protected override Point2 Position
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

        protected override Point2 Size
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

        protected override Point2 RenderSize
        {
            get
            {
                GLFW.GetFramebufferSize(pointer, out int width, out int height);
                return new Point2(width, height);
            }
        }

        protected override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(pointer, out var xpos, out var ypos);
                return new Vector2((float)xpos, (float)ypos);
            }
        }

        protected override Vector2 ScreenMouse
        {
            get
            {
                GLFW.GetCursorPos(pointer, out var curX, out var curY);
                GLFW.GetWindowPos(pointer, out var winX, out var winY);
                return new Vector2((float)curX + winX, (float)curY + winY);
            }
        }

        protected override Vector2 ContentScale
        {
            get
            {
                GLFW.GetWindowContentScale(pointer, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        protected override bool Opened => !disposed;

        protected override bool Focused => focused;

        protected override bool MouseOver => mouseOver;

        protected override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(pointer, title = value);
        }

        protected override bool VSync { get; set; } = true;

        protected override bool Bordered
        {
            get => GLFW.GetWindowAttrib(pointer, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(pointer, GLFW.WindowAttributes.Decorated, value);
        }

        protected override bool Resizable
        {
            get => GLFW.GetWindowAttrib(pointer, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(pointer, GLFW.WindowAttributes.Resizable, value);
        }

        protected override bool Fullscreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                if (fullscreen != value)
                {
                    fullscreen = value;

                    if (fullscreen)
                    {
                        var monitor = GLFW.GetWindowMonitor(pointer);
                        if (monitor == IntPtr.Zero)
                            monitor = GLFW.GetPrimaryMonitor();

                        if (monitor != IntPtr.Zero)
                        {
                            storedBounds = new RectInt(Position, Size);

                            GLFW.GetMonitorWorkarea(monitor, out var x, out var y, out var w, out var h);
                            GLFW.SetWindowMonitor(pointer, monitor, 0, 0, w, h, (int)GLFW_Enum.GLFW_DONT_CARE);
                        }
                        else
                            fullscreen = false;
                    }
                    else
                    {
                        GLFW.SetWindowMonitor(pointer, IntPtr.Zero, storedBounds.X, storedBounds.Y, storedBounds.Width, storedBounds.Height, (int)GLFW_Enum.GLFW_DONT_CARE);
                    }
                }
            }
        }

        protected override bool Visible
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

        protected override IntPtr Pointer
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return GLFW.GetWin32Window(pointer);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return GLFW.GetCocoaWindow(pointer);
                return IntPtr.Zero;
            }
        }

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

        protected override void Present()
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

        protected override void Close()
        {
            if (!disposed)
            {
                disposed = true;
                GLFW.SetWindowShouldClose(pointer, true);
            }
        }

        internal void InvokeCloseWindowCallback()
        {
            OnClose?.Invoke();
        }
    }
}
