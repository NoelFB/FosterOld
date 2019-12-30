using Foster.Framework;
using System;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        internal readonly GLFW_Context GlfwContext;
        internal GLFW.Window GlfwWindowPointer => GlfwContext.GlfwWindowPointer;

        private string title;
        private bool visible;
        private bool lastVsync;
        private bool focused;
        private bool mouseOver;

        public override Point2 Position
        {
            get
            {
                GLFW.GetWindowPos(GlfwWindowPointer, out int x, out int y);
                return new Point2(x, y);
            }

            set
            {
                GLFW.SetWindowPos(GlfwWindowPointer, value.X, value.Y);
            }
        }

        public override Point2 Size
        {
            get
            {
                GLFW.GetWindowSize(GlfwWindowPointer, out int w, out int h);
                return new Point2(w, h);
            }

            set
            {
                GLFW.SetWindowSize(GlfwWindowPointer, value.X, value.Y);
            }
        }

        public override Vector2 Mouse
        {
            get
            {
                GLFW.GetCursorPos(GlfwWindowPointer, out var xpos, out var ypos);
                return new Vector2((float)xpos, (float)ypos);
            }
        }

        public override Vector2 ScreenMouse
        {
            get
            {
                GLFW.GetCursorPos(GlfwWindowPointer, out var curX, out var curY);
                GLFW.GetWindowPos(GlfwWindowPointer, out var winX, out var winY);
                return new Vector2((float)curX + winX, (float)curY + winY);
            }
        }

        public override Vector2 ContentScale
        {
            get
            {
                GLFW.GetWindowContentScale(GlfwWindowPointer, out float x, out float y);
                return new Vector2(x, y);
            }
        }

        public override bool Opened => !GlfwContext.Disposed;

        public override bool Focused => focused;

        public override bool MouseOver => mouseOver;

        public override string Title
        {
            get => title;
            set => GLFW.SetWindowTitle(GlfwWindowPointer, title = value);
        }

        public override bool VSync { get; set; } = true;

        public override bool Bordered
        {
            get => GLFW.GetWindowAttrib(GlfwWindowPointer, GLFW.WindowAttributes.Decorated);
            set => GLFW.SetWindowAttrib(GlfwWindowPointer, GLFW.WindowAttributes.Decorated, value);
        }

        public override bool Resizable
        {
            get => GLFW.GetWindowAttrib(GlfwWindowPointer, GLFW.WindowAttributes.Resizable);
            set => GLFW.SetWindowAttrib(GlfwWindowPointer, GLFW.WindowAttributes.Resizable, value);
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
                    GLFW.ShowWindow(GlfwWindowPointer);
                else
                    GLFW.HideWindow(GlfwWindowPointer);
            }
        }

        public override IntPtr Pointer => GLFW.GetWindowUserPointer(GlfwWindowPointer.Ptr);

        private readonly GLFW.WindowSizeFunc windowSizeCallbackRef;
        private readonly GLFW.WindowFocusFunc windowFocusCallbackRef;
        private readonly GLFW.CursorEnterFunc windowCursorEnterCallbackRef;

        public GLFW_Window(GLFW_System system, GLFW_Context context, string title, bool visible) : base(system, context)
        {

            this.title = title;
            this.visible = visible;

            GlfwContext = context;
            System.SetCurrentContext(context);

            GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            GLFW.SetWindowSizeCallback(GlfwWindowPointer, windowSizeCallbackRef = OnWindowResize);
            GLFW.SetWindowFocusCallback(GlfwWindowPointer, windowFocusCallbackRef = OnWindowFocus);
            GLFW.SetCursorEnterCallback(GlfwWindowPointer, windowCursorEnterCallbackRef = OnCursorEnter);
        }

        private void OnWindowResize(GLFW.Window window, int width, int height)
        {
            OnResize?.Invoke();
        }

        private void OnWindowFocus(GLFW.Window window, int focused)
        {
            this.focused = (focused != 0);
            if (this.focused)
                OnFocus?.Invoke();
        }

        private void OnCursorEnter(GLFW.Window window, int entered)
        {
            mouseOver = (entered != 0);
        }

        public override void Present()
        {
            if (lastVsync != VSync)
            {
                System.SetCurrentContext(GlfwContext);
                GLFW.SwapInterval((lastVsync = VSync) ? 1 : 0);
            }

            GLFW.SwapBuffers(GlfwWindowPointer);
        }

        public override void Close()
        {
            if (!Context.Disposed)
                Context.Dispose();
        }
    }
}
