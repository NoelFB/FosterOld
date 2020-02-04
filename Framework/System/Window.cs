using System;

namespace Foster.Framework
{
    /// <summary>
    /// A Window.
    /// 
    /// Screen Coordinates may be different on each platform.
    /// For example, on Windows High DPI displays, this is always 1-1 with
    /// the pixel size of the Window. On MacOS Retina displays, this is
    /// usually 1-2 with the pixel size of the Window.
    /// 
    /// The Window is only able to be Rendered to during is OnRender callback. Attempting
    /// to render to the Window outside of that will throw an exception.
    /// </summary>
    public sealed class Window : RenderTarget
    {

        public abstract class Platform
        {
            public abstract IntPtr Pointer { get; }
            public abstract Point2 Position { get; set; }
            public abstract Point2 Size { get; set; }
            public abstract Point2 DrawableSize { get; }
            public abstract Vector2 ContentScale { get; }
            public abstract bool Opened { get; }

            public abstract string Title { get; set; }
            public abstract bool Bordered { get; set; }
            public abstract bool Resizable { get; set; }
            public abstract bool Fullscreen { get; set; }
            public abstract bool Visible { get; set; }
            public abstract bool VSync { get; set; }

            public abstract bool Focused { get; }
            public abstract Vector2 Mouse { get; }
            public abstract Vector2 ScreenMouse { get; }
            public abstract bool MouseOver { get; }

            public abstract void Present();
            public abstract void Close();

            public Action? OnFocus;
            public Action? OnResize;
            public Action? OnClose;
            public Action? OnCloseRequested;
        }

        public readonly Platform Implementation;

        /// <summary>
        /// A pointer to the underlying OS Window
        /// </summary>
        public IntPtr Pointer => Implementation.Pointer;

        /// <summary>
        /// Position of the Window, in Screen coordinates
        /// </summary>
        public Point2 Position
        {
            get => Implementation.Position;
            set => Implementation.Position = value;
        }

        /// <summary>
        /// The size of the Window, in Screen coordinates
        /// </summary>
        public Point2 Size
        {
            get => Implementation.Size;
            set => Implementation.Size = value;
        }

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
        public Point2 DrawableSize => Implementation.DrawableSize;

        /// <summary>
        /// The Drawable Width of the Window, in Pixels
        /// </summary>
        public override int DrawableWidth => Implementation.DrawableSize.X;

        /// <summary>
        /// The Drawable Height of the Window, in Pixels
        /// </summary>
        public override int DrawableHeight => Implementation.DrawableSize.Y;

        /// <summary>
        /// The drawable bounds of the Window, in Pixels
        /// </summary>
        public RectInt DrawableBounds => new RectInt(0, 0, Implementation.DrawableSize.X, Implementation.DrawableSize.Y);

        /// <summary>
        /// The scale of the Drawable size compared to the Window size
        /// On Windows and Linux this is always 1.
        /// On MacOS Retina displays this is 2.
        /// </summary>
        public Vector2 DrawableScale => new Vector2(Implementation.DrawableSize.X / (float)Width, Implementation.DrawableSize.Y / (float)Height);

        /// <summary>
        /// The Content Scale of the Window
        /// On High DPI displays this may be larger than 1
        /// Use this to appropriately scale UI
        /// </summary>
        public Vector2 ContentScale => Implementation.ContentScale;

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
        /// A callback when the Window is about to close
        /// </summary>
        public Action<Window>? OnClose;

        /// <summary>
        /// A callback when the Window has been requested to be closed (ex. by pressing the Close menu button).
        /// By default this calls Window.Close()
        /// </summary>
        public Action<Window>? OnCloseRequested;

        /// <summary>
        /// Gets or Sets the Title of this Window
        /// </summary>
        public string Title
        {
            get => Implementation.Title;
            set => Implementation.Title = value;
        }

        /// <summary>
        /// Gets if the Window is currently Open
        /// </summary>
        public bool Opened => Implementation.Opened;

        /// <summary>
        /// Gets or Sets whether the Window has a Border
        /// </summary>
        public bool Bordered
        {
            get => Implementation.Bordered;
            set => Implementation.Bordered = value;
        }

        /// <summary>
        /// Gets or Sets whether the Window is resizable by the user
        /// </summary>
        public bool Resizable
        {
            get => Implementation.Resizable;
            set => Implementation.Resizable = value;
        }

        /// <summary>
        /// Gets or Sets whether the Window is in Fullscreen Mode
        /// </summary>
        public bool Fullscreen
        {
            get => Implementation.Fullscreen;
            set => Implementation.Fullscreen = value;
        }

        /// <summary>
        /// Gets or Sets whether the Window is Visible to the user
        /// </summary>
        public bool Visible
        {
            get => Implementation.Visible;
            set => Implementation.Visible = value;
        }

        /// <summary>
        /// Gets or Sets whether the Window synchronizes the vertical redraw
        /// </summary>
        public bool VSync
        {
            get => Implementation.VSync;
            set => Implementation.VSync = value;
        }

        /// <summary>
        /// Whether this is the currently focused Window
        /// </summary>
        public bool Focused => Implementation.Focused;

        /// <summary>
        /// The Mouse position relative to the top-left of the Window, in Screen coordinates
        /// </summary>
        public Vector2 Mouse => Implementation.Mouse;

        /// <summary>
        /// The position of the Mouse in pixels, relative to the top-left of the Window
        /// </summary>
        public Vector2 DrawableMouse => Implementation.Mouse * DrawableScale;

        /// <summary>
        /// The position of the mouse relative to the top-left of the Screen, in Screen coordinates
        /// </summary>
        public Vector2 ScreenMouse => Implementation.ScreenMouse;

        /// <summary>
        /// Whether the mouse is currently over this Window
        /// </summary>
        public bool MouseOver => Implementation.MouseOver;

        public Window(string title, int width, int height, WindowFlags flags = WindowFlags.None) : this(App.System, title, width, height, flags)
        {

        }

        public Window(System system, string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            system.windows.Add(this);

            // create implementation object
            Implementation = system.CreateWindow(title, width, height, flags);
            Implementation.OnFocus = () => OnFocus?.Invoke(this);
            Implementation.OnResize = () => OnResize?.Invoke(this);
            Implementation.OnClose = () =>
            {
                OnClose?.Invoke(this);
                system.windows.Remove(this);
            };
            Implementation.OnCloseRequested = () => OnCloseRequested?.Invoke(this);

            // default close request to... close the window!
            OnCloseRequested = (window) => window.Close();
        }

        /// <summary>
        /// Renders the Window. Call Present afterwards to display the rendered contents
        /// </summary>
        internal void Render()
        {
            // The Window Target is only allowed to be rendered to during this call
            // it greatly simplifies the various states for the Graphics Module
            lock (this)
            {
                Drawable = true;

                App.Modules.BeforeRender(this);
                OnRender?.Invoke(this);
                App.Modules.AfterRender(this);

                Drawable = false;
            }
        }

        /// <summary>
        /// Presents the drawn contents of the Window
        /// </summary>
        internal void Present()
        {
            Implementation.Present();
        }

        /// <summary>
        /// Closes the Window
        /// </summary>
        public void Close()
        {
            Implementation.Close();
        }

    }
}
