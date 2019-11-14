using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Window
    {

        public abstract IntPtr PlatformPtr { get; }

        /// <summary>
        /// The X position of the Window, in Screen coordinates
        /// </summary>
        public int X
        {
            get => Bounds.X;
            set => Bounds = new RectInt(value, Y, Width, Height);
        }

        /// <summary>
        /// The X position of the Window, in Screen coordinates
        /// </summary>
        public int Y
        {
            get => Bounds.Y;
            set => Bounds = new RectInt(X, value, Width, Height);
        }

        public Point2 Position
        {
            get
            {
                var bounds = Bounds;
                return new Point2(bounds.X, bounds.Y);
            }
            set
            {
                var bounds = Bounds;
                Bounds = new RectInt(value.X, value.Y, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// The Width of the Window, in Screen coordinates
        /// </summary>
        public int Width
        {
            get => Bounds.Width;
            set => Bounds = new RectInt(X, Y, value, Height);
        }

        /// <summary>
        /// The Height of the Window, in Screen coordinates
        /// </summary>
        public int Height
        {
            get => Bounds.Height;
            set => Bounds = new RectInt(X, Y, Width, value);
        }

        /// <summary>
        /// The Drawable Width of the Window, in Pixels
        /// </summary>
        public int DrawableWidth => DrawableBounds.Width;

        /// <summary>
        /// The Drawable Height of the Window, in Pixels
        /// </summary>
        public int DrawableHeight => DrawableBounds.Height;

        /// <summary>
        /// A callback when the Window is about to close
        /// </summary>
        public Action? OnClose;

        /// <summary>
        /// A callback when the Window is redrawn
        /// </summary>
        public Action? OnRender;

        /// <summary>
        /// A callback when the Window is resized by the user
        /// </summary>
        public Action<int, int>? OnResize;

        /// <summary>
        /// The System this Window belongs to
        /// </summary>
        public abstract System System { get; }

        /// <summary>
        /// Gets or Sets the Title of this Window
        /// </summary>
        public abstract string Title { get; set; }

        /// <summary>
        /// The Rendering Context associated with this Window
        /// </summary>
        public abstract Context Context { get; }

        /// <summary>
        /// Gets if the Window is currently Open
        /// </summary>
        public abstract bool Opened { get; }

        /// <summary>
        /// Gets or Sets whether the Window has a Border
        /// </summary>
        public abstract bool Bordered { get; set; }

        /// <summary>
        /// Gets or Sets whether the Window is resizable by the user
        /// </summary>
        public abstract bool Resizable { get; set; }

        /// <summary>
        /// Gets or Sets whether the Window is in Fullscreen Mode
        /// </summary>
        public abstract bool Fullscreen { get; set; }

        /// <summary>
        /// Gets or Sets whether the Window is Visible to the user
        /// </summary>
        public abstract bool Visible { get; set; }

        /// <summary>
        /// Gets or Sets whether the Window synchronizes the vertical redraw
        /// </summary>
        public abstract bool VSync { get; set; }

        /// <summary>
        /// Whether this is the currently focused Window
        /// </summary>
        public abstract bool Focused { get; }

        /// <summary>
        /// The Window bounds, in Screen coordinates
        /// Note on High DPI displays this may not match the Drawable Bounds of the window. 
        /// DrawableBounds should be used for all drawing.
        /// </summary>
        public abstract RectInt Bounds { get; set; }

        /// <summary>
        /// Gets the Content Bounds
        /// This is the same as Bounds, but with X and Y always 0
        /// </summary>
        public RectInt ContentBounds => new RectInt(0, 0, Bounds.Width, Bounds.Height);

        /// <summary>
        /// The drawable bounds of the Window
        /// Note on High DPI displays, this may not match the Bounds of the window
        /// </summary>
        public RectInt DrawableBounds => new RectInt(0, 0, Context.Width, Context.Height);

        /// <summary>
        /// The Pixel Scale of the Window. On non-High DPI displays this should always be 1.
        /// </summary>
        public abstract Vector2 PixelScale { get; }

        /// <summary>
        /// The Mouse position relative to the top-left of the Window, in Screen coordinates
        /// </summary>
        public abstract Vector2 Mouse { get; }

        /// <summary>
        /// Whether the mouse is currently over this Window
        /// </summary>
        public abstract bool MouseOver { get; }

        /// <summary>
        /// Renders the Window. Call Present afterwards to display the rendered contents
        /// </summary>
        public void Render()
        {
            Context.MakeCurrent();
            App.Modules.BeforeRender(this);
            OnRender?.Invoke();
            App.Modules.AfterRender(this);
        }

        /// <summary>
        /// Presents the drawn contents of the Window
        /// </summary>
        public abstract void Present();

        /// <summary>
        /// Closes the Window
        /// </summary>
        public abstract void Close();

        public static Window Create(string title, int width, int height, bool visible = true)
        {
            return App.System.CreateWindow(title, width, height, visible);
        }
    }
}
