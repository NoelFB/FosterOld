using System;

namespace Foster.Framework
{
    /// <summary>
    /// A platform Window
    /// 
    /// Note that Screen Coordinates may be different on each platform.
    /// For example, on Windows High DPI displays, this is always 1-1 with
    /// the pixel size of the Window. On MacOS Retina displays, this is
    /// usually 1-2 with the pixel size of the Window.
    /// </summary>
    public abstract class Window
    {
        /// <summary>
        /// A pointer to the underlying System Window
        /// </summary>
        public abstract IntPtr Pointer { get; }

        /// <summary>
        /// Position of the Window, in Screen coordinates
        /// </summary>
        public abstract Point2 Position { get; set; }

        /// <summary>
        /// The size of the Window, in Screen coordinates
        /// </summary>
        public abstract Point2 Size { get; set; }

        /// <summary>
        /// The X position of the Window, in Screen coordinates
        /// </summary>
        public int X
        {
            get => Position.X;
            set => Position = new Point2(value, Y);
        }

        /// <summary>
        /// The X position of the Window, in Screen coordinates
        /// </summary>
        public int Y
        {
            get => Position.Y;
            set => Position = new Point2(X, value);
        }

        /// <summary>
        /// The Width of the Window, in Screen coordinates
        /// </summary>
        public int Width
        {
            get => Size.X;
            set => Size = new Point2(value, Size.Y);
        }

        /// <summary>
        /// The Height of the Window, in Screen coordinates
        /// </summary>
        public int Height
        {
            get => Size.Y;
            set => Size = new Point2(Size.X, value);
        }

        /// <summary>
        /// The Window bounds, in Screen coordinates
        /// </summary>
        public RectInt Bounds
        {
            get
            {
                var position = Position;
                var size = Size;

                return new RectInt(position.X, position.Y, size.X, size.Y);
            }
            set
            {
                Position = value.Position;
                Size = value.Size;
            }
        }

        /// <summary>
        /// The Drawable Width of the Window, in Pixels
        /// </summary>
        public int DrawableWidth => Context.Width;

        /// <summary>
        /// The Drawable Height of the Window, in Pixels
        /// </summary>
        public int DrawableHeight => Context.Height;

        /// <summary>
        /// The drawable bounds of the Window, in Pixels
        /// </summary>
        public RectInt DrawableBounds => new RectInt(0, 0, Context.Width, Context.Height);

        /// <summary>
        /// The scale of the Drawable size compared to the Window size
        /// On Windows and Linux this is always 1.
        /// On MacOS Retina displays this is 2.
        /// </summary>
        public Vector2 DrawableScale => new Vector2(Context.Width / (float)Width, Context.Height / (float)Height);

        /// <summary>
        /// The Content Scale of the Window
        /// On High DPI displays this may be larger than 1
        /// Use this to appropriately scale UI
        /// </summary>
        public abstract Vector2 ContentScale { get; }

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
        public Action? OnResize;

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
        /// The Mouse position relative to the top-left of the Window, in Screen coordinates
        /// </summary>
        public abstract Vector2 Mouse { get; }

        /// <summary>
        /// The position of the Mouse in pixels, relative to the top-left of the Window
        /// </summary>
        public Vector2 DrawableMouse => Mouse * DrawableScale;

        /// <summary>
        /// The position of the mouse relative to the top-left of the Screen, in Screen coordinates
        /// </summary>
        public abstract Vector2 ScreenMouse { get; }

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

    }
}
