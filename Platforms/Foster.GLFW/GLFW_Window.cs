using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GLFW
{
    public class GLFW_Window : Window
    {
        private GLFW.Window handle;
        private bool opened;

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
                return new Point2();
            }
        }

        public override bool Opened => opened;

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

        public GLFW_Window(string title, int width, int height)
        {
            handle = GLFW.CreateWindow(width, height, title, IntPtr.Zero, IntPtr.Zero);

            GLFW.SetWindowCloseCallback(handle, (handle) => Close());

            opened = true;
        }

        public override void MakeCurrent()
        {
            if (!opened)
                throw new Exception("This Window has been Closed");

            GLFW.MakeContextCurrent(handle);
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
                GLFW.DestroyWindow(handle);
                opened = false;
            }
        }
    }
}
