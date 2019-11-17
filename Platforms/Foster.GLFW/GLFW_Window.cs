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

        public override RectInt Bounds
        {
            get
            {
                GLFW.GetWindowPos(context.Handle, out int x, out int y);
                GLFW.GetWindowSize(context.Handle, out int w, out int h);

                // glfwGetWindowSize returns different results depending on the platform and DPI.
                // Ex. if our Content Scale is 2, on OSX a Window created at 1280x1080 will still
                // return 1280x1080, where as on Windows and Linux this will return 2560x2160
                
                // The Foster API expects the OSX behaviour across platforms, so we must scale these
                // values based on the Content Scale

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    GLFW.GetWindowContentScale(context.Handle, out float scaleX, out float scaleY);

                    x = (int)(x / scaleX);
                    y = (int)(y / scaleY);
                    w = (int)(w / scaleX);
                    h = (int)(h / scaleY);

                }

                return new RectInt(x, y, w, h);
            }

            set
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    GLFW.GetWindowContentScale(context.Handle, out float scaleX, out float scaleY);

                    value.X = (int)(value.X * scaleX);
                    value.Y = (int)(value.Y * scaleY);
                    value.Width = (int)(value.Width * scaleX);
                    value.Height = (int)(value.Height * scaleY);
                }

                GLFW.SetWindowPos(context.Handle, value.X, value.Y);
                GLFW.SetWindowSize(context.Handle, value.Width, value.Height);
            }
        }

        public override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(context.Handle, out var xpos, out var ypos);

                xpos = Math.Floor(xpos);
                ypos = Math.Floor(ypos);

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    GLFW.GetWindowContentScale(context.Handle, out float scaleX, out float scaleY);

                    xpos = (xpos / scaleX);
                    ypos = (ypos / scaleY);
                }

                return new Vector2((float)xpos, (float)ypos);
            }
        }

        public override Vector2 PixelScale
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

        public override IntPtr PlatformPtr => context.Handle.Ptr;

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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                GLFW.GetWindowContentScale(context.Handle, out float scaleX, out float scaleY);

                width = (int)(width / scaleX);
                height = (int)(height / scaleY);
            }

            OnResize?.Invoke(width, height);
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
