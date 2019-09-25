using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Window
    {

        public int X => Bounds.X;
        public int Y => Bounds.Y;
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        protected abstract System System { get; }
        public abstract string Title { get; set; }
        public abstract Context Context { get; }

        public abstract bool Opened { get; }
        public abstract bool Bordered { get; set; }
        public abstract bool Resizable { get; set; }
        public abstract bool Fullscreen { get; set; }
        public abstract bool Visible { get; set; }
        public abstract bool VSync { get; set; }

        public abstract RectInt Bounds { get; set; }
        public abstract Point2 DrawSize { get; }
        public abstract Vector2 PixelSize { get; }

        public void SetActive()
        {
            System.ActiveWindow = this;
        }

        public abstract void Present();
        public abstract void Close();

        public static Window Create(string title, int width, int height, bool visible = true)
        {
            return App.System.CreateWindow(title, width, height, visible);
        }
    }
}
