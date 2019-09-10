using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Window
    {

        public int X
        {
            get => Bounds.X;
        }

        public int Y
        {
            get => Bounds.Y;
        }

        public int Width
        {
            get => Bounds.Width;
        }

        public int Height
        {
            get => Bounds.Height;
        }

        public abstract string Title { get; set; }

        public abstract bool Opened { get; }
        public abstract bool Bordered { get; set; }
        public abstract bool Resizable { get; set; }
        public abstract bool Fullscreen { get; set; }
        public abstract bool Visible { get; set; }

        public abstract RectInt Bounds { get; set; }
        public abstract Point2 DrawSize { get; }

        public abstract void MakeCurrent();
        public abstract void Present();
        public abstract void Close();

    }
}
