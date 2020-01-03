using System;

namespace Foster.Framework
{
    /// <summary>
    /// A platform Window.
    ///
    /// Note that not every Platform supports multiple Windows, in which case
    /// creating more than one will throw an exception. You can check whether multiple
    /// Windows is supported under Foster.Framework.System.SupportsMultipleWindows
    /// 
    /// Note that Screen Coordinates may be different on each platform.
    /// For example, on Windows High DPI displays, this is always 1-1 with
    /// the pixel size of the Window. On MacOS Retina displays, this is
    /// usually 1-2 with the pixel size of the Window.
    /// 
    /// </summary>
    public abstract class Window : RenderTarget
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
                Size = value.Size;
                Position = value.Position;
            }
        }

        /// <summary>
        /// The Drawable Size of the Window, in Pixels
        /// </summary>
        public abstract Point2 DrawableSize { get; }

        /// <summary>
        /// The Drawable Width of the Window, in Pixels
        /// </summary>
        public override int DrawableWidth => DrawableSize.X;

        /// <summary>
        /// The Drawable Height of the Window, in Pixels
        /// </summary>
        public override int DrawableHeight => DrawableSize.Y;

        /// <summary>
        /// The drawable bounds of the Window, in Pixels
        /// </summary>
        public RectInt DrawableBounds => new RectInt(0, 0, DrawableWidth, DrawableHeight);

        /// <summary>
        /// The scale of the Drawable size compared to the Window size
        /// On Windows and Linux this is always 1.
        /// On MacOS Retina displays this is 2.
        /// </summary>
        public Vector2 DrawableScale => new Vector2(DrawableWidth / (float)Width, DrawableHeight / (float)Height);

        /// <summary>
        /// The Content Scale of the Window
        /// On High DPI displays this may be larger than 1
        /// Use this to appropriately scale UI
        /// </summary>
        public abstract Vector2 ContentScale { get; }

        /// <summary>
        /// A callback when the Window is about to close
        /// </summary>
        public Action<Window>? OnClose;

        /// <summary>
        /// A callback when the Window is redrawn
        /// </summary>
        public Action<Window>? OnRender;

        /// <summary>
        /// A callback when the Window is resized by the user
        /// </summary>
        public Action<Window>? OnResize;

        /// <summary>
        /// A callback when the Window is focused
        /// </summary>
        public Action<Window>? OnFocus;

        /// <summary>
        /// The System this Window belongs to
        /// </summary>
        public readonly System System;

        /// <summary>
        /// The Rendering Context associated with this Window
        /// </summary>
        public readonly GraphicsContext Context;

        /// <summary>
        /// Gets or Sets the Title of this Window
        /// </summary>
        public abstract string Title { get; set; }

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

        protected Window(System system, GraphicsContext context)
        {
            System = system;
            Context = context;
        }

        /// <summary>
        /// Renders the Window. Call Present afterwards to display the rendered contents
        /// </summary>
        internal void Render()
        {
            // The Window Target is only allowed to be rendered to during this call
            // it greatly simplifies the various states for the Graphics Module

            Drawable = true;
            Viewport = new RectInt(0, 0, DrawableWidth, DrawableHeight);

            App.Modules.BeforeRender(this);
            OnRender?.Invoke(this);
            App.Modules.AfterRender(this);

            Drawable = false;
        }

        /// <summary>
        /// Presents the drawn contents of the Window
        /// </summary>
        protected internal abstract void Present();

        /// <summary>
        /// Closes the Window
        /// </summary>
        public abstract void Close();

    }
}
