using Foster.Framework;
using System;
using System.Collections.Generic;

namespace Foster.GUI
{
    public class Imgui
    {
        #region Public Variables

        public Stylesheet Style;
        public SpriteFont DefaultFont;
        public float DefaultFontSize;
        public float DefaultSpacing;

        public SpriteFont Font => (fontStack.Count > 0 ? fontStack.Peek() : DefaultFont);
        public float FontSize => (fontSizeStack.Count > 0 ? fontSizeStack.Peek() : DefaultFontSize);
        public float FontScale => FontSize / Font.Height;
        public float Spacing => (spacingStack.Count > 0 ? spacingStack.Peek() : DefaultSpacing);
        public float Indent => (indentStack.Count > 0 ? indentStack.Peek() : 0f);
        public int Layer => (layerStack.Count > 0 ? layerStack.Peek() : 0);

        public ImguiViewport Viewport => viewport;
        public ImguiFrame Frame => frame;
        public Rect Clip => clipStack.Count > 0 ? clipStack.Peek() : new Rect();
        public Batch2D Batcher => viewport.Batcher;
        public Rect LastCell => frame.LastCell;
        public ImguiStorage Storage => storageStack.Peek();

        public ImguiID HotId = ImguiID.None;
        public ImguiID LastHotId = ImguiID.None;
        public ImguiID ActiveId = ImguiID.None;
        public ImguiID LastActiveId = ImguiID.None;
        public ImguiID ParentId => (idStack.Count > 0 ? idStack.Peek() : ImguiID.None);

        public ImguiID CurrentId
        {
            get => currentId;
            set
            {
                currentId = value;
                if (LastActiveId == currentId)
                    lastActiveIdExists = true;
            }
        }

        #endregion

        #region Private Variables

        private ImguiID currentId;
        private bool lastActiveIdExists;

        private ImguiViewport viewport;
        private ImguiFrame frame;

        private readonly Stack<ImguiFrame> frameStack = new Stack<ImguiFrame>();
        private readonly Stack<ImguiID> idStack = new Stack<ImguiID>();
        private readonly Stack<int> layerStack = new Stack<int>();
        private readonly Stack<Rect> clipStack = new Stack<Rect>();
        private readonly Stack<float> indentStack = new Stack<float>();
        private readonly Stack<SpriteFont> fontStack = new Stack<SpriteFont>();
        private readonly Stack<float> fontSizeStack = new Stack<float>();
        private readonly Stack<float> spacingStack = new Stack<float>();
        private readonly Stack<ImguiStorage> storageStack = new Stack<ImguiStorage>();
        private readonly Dictionary<ImguiID, ImguiStorage> storages = new Dictionary<ImguiID, ImguiStorage>();

        #endregion

        #region Constructor

        public Imgui(SpriteFont font)
        {
            Style = Stylesheets.Default;
            DefaultFont = font;
            DefaultFontSize = 14;
            DefaultSpacing = 1;
        }

        #endregion

        #region State Changing

        public ImguiID Id(ImguiName name)
        {
            return CurrentId = new ImguiID(name, idStack.Count > 0 ? idStack.Peek() : ImguiID.None);
        }

        public ImguiID PushId(ImguiID id)
        {
            idStack.Push(id);
            return id;
        }

        public ImguiID PushId(ImguiName name)
        {
            return PushId(Id(name));
        }

        public void PopId()
        {
            idStack.Pop();
        }

        /// <summary>
        /// Begins a new Storage Group
        /// If this Storage Group doesn't exist next frame, its data will be discarded
        /// </summary>
        public void BeginStorage(ImguiName name)
        {
            var id = new ImguiID(name, ParentId);
            if (!storages.TryGetValue(id, out var storage))
                storage = storages[id] = new ImguiStorage();
            storage.Used = true;
            storageStack.Push(storage);
        }

        /// <summary>
        /// Ends a Storage Group
        /// </summary>
        public void EndStorage()
        {
            storageStack.Pop();
        }

        public void PushIndent(float amount) => indentStack.Push(Indent + amount);
        public void PopIndent() => indentStack.Pop();

        public void PushSpacing(float spacing) => spacingStack.Push(spacing);
        public void PopSpacing() => spacingStack.Pop();

        public void PushFont(SpriteFont font) => fontStack.Push(font);
        public void PopFont() => fontStack.Pop();

        public void PushFontSize(float size) => fontSizeStack.Push(size);
        public void PopFontSize() => fontSizeStack.Pop();

        private void PushClip(Rect rect)
        {
            clipStack.Push(rect);
            Batcher.SetScissor(rect.Scale(viewport.Scale).Int());
        }

        private void PopClip()
        {
            clipStack.Pop();
            if (clipStack.Count > 0)
                Batcher.SetScissor(clipStack.Peek().Scale(viewport.Scale).Int());
            else
                Batcher.SetScissor(null);
        }

        public void Row()
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Row");

            frame.NextRow(1, Indent, Spacing);
        }

        public void Row(int columns)
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Row");

            frame.NextRow(columns, Indent, Spacing);
        }

        public Rect Remainder()
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(Size.Fill(), Size.Fill(), 0f, 0f, Indent, Spacing);
        }

        public Rect Cell(float width, float height)
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(Size.Explicit(width), Size.Explicit(height), 0, 0, Indent, Spacing);
        }

        public Rect Cell(Size width, Size height, float preferredWidth = 0, float preferredHeight = 0)
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(width, height, preferredWidth, preferredHeight, Indent, Spacing);
        }

        public Rect Cell(Size width, Size height, IContent content, Vector2 padding)
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(width, height, content.Width(this) + padding.X * 2, content.Height(this) + padding.Y * 2, Indent, Spacing);
        }

        public void Separator() => Cell(Size.Fill(), Size.Explicit(Spacing));
        public void Separator(float height) => Cell(Size.Fill(), Size.Explicit(height));
        public void Separator(float width, float height) => Cell(Size.Explicit(width), Size.Explicit(height));

        public void Step()
        {
            viewport = new ImguiViewport();
            frame = new ImguiFrame();
            LastHotId = HotId;
            HotId = ImguiID.None;

            // These should have been all cleared by the end of last frame
            // but we do this for safety
            indentStack.Clear();
            idStack.Clear();
            frameStack.Clear();
            clipStack.Clear();
            fontStack.Clear();
            fontSizeStack.Clear();
            spacingStack.Clear();
            storageStack.Clear();

            // destroy storage that was not used
            {
                List<ImguiID>? removing = null;
                foreach (var kv in storages)
                    if (!kv.Value.Used)
                    {
                        if (removing == null)
                            removing = new List<ImguiID>();
                        removing.Add(kv.Key);
                    }

                if (removing != null)
                {
                    foreach (var id in removing)
                        storages.Remove(id);
                }
            }

            // track active ID
            // if the active ID no longer exists, unset it
            {
                if (LastActiveId == ImguiID.None && ActiveId != ImguiID.None)
                    lastActiveIdExists = true;

                LastActiveId = ActiveId;

                if (!lastActiveIdExists)
                    ActiveId = ImguiID.None;

                lastActiveIdExists = false;
            }
        }

        #endregion

        #region Viewports / Frames

        public void BeginViewport(Window window, Batch2D batcher, Vector2? contentScale = null)
        {
            var scale = window.ContentScale * (contentScale ?? Vector2.One);
            var bounds = new Rect(0, 0, window.DrawableWidth / scale.X, window.DrawableHeight / scale.Y);

            BeginViewport(window.Title, batcher, bounds, window.DrawableMouse, scale, !window.MouseOver);
        }

        public void BeginViewport(ImguiName name, Batch2D batcher, Rect bounds, Vector2 mouse, Vector2 scale, bool mouseObstructed = false)
        {
            mouse /= scale;

            if (viewport.ID != ImguiID.None)
                throw new Exception("The previous Viewport must be ended before beginning a new one");

            viewport = new ImguiViewport
            {
                ID = Id(name),
                Bounds = bounds,
                Mouse = mouse,
                MouseObstructed = mouseObstructed,
                Scale = scale,
                Batcher = batcher
            };

            BeginStorage(0);

            viewport.MouseDelta.X = mouse.X - Storage.GetNumber(viewport.ID, 0, mouse.X);
            viewport.MouseDelta.Y = mouse.Y - Storage.GetNumber(viewport.ID, 1, mouse.Y);
            viewport.LastHotFrame = Storage.GetId(viewport.ID, 2, ImguiID.None);

            PushClip(viewport.Bounds);
            viewport.Batcher.PushMatrix(Matrix2D.CreateScale(scale));
        }

        public void EndViewport()
        {
            if (viewport.ID == ImguiID.None)
                throw new Exception("You must begin a Viewport before ending it");
            if (frame.ID != ImguiID.None)
                throw new Exception("The previous Group must be closed before closing the Viewport");

            Storage.SetNumber(viewport.ID, 0, viewport.Mouse.X);
            Storage.SetNumber(viewport.ID, 1, viewport.Mouse.Y);
            Storage.SetId(viewport.ID, 2, viewport.NextHotFrame);

            PopClip();
            EndStorage();

            viewport.Batcher.PopMatrix();
            viewport = new ImguiViewport();
        }

        public void BeginLayer(int layer)
        {
            if (viewport.ID == ImguiID.None)
                throw new Exception("You must begin a Viewport before beginning a Layer");

            Batcher.SetLayer(layer);
            PushClip(Viewport.Bounds);
            layerStack.Push(layer);
        }

        public void EndLayer()
        {
            PopClip();
            layerStack.Pop();
            Batcher.SetLayer(Layer);
        }

        public bool BeginFrame(ImguiName name, bool scrollable = true)
        {
            var bounds = viewport.Bounds;
            if (frame.ID != ImguiID.None)
                bounds = Remainder();

            return BeginFrame(name, bounds, scrollable);
        }

        public bool BeginFrame(ImguiName name, Rect bounds, bool scrollable = true)
        {
            return BeginFrame(name, bounds, Style.Frame, scrollable);
        }

        public bool BeginFrame(ImguiName name, Rect bounds, StyleState style, bool scrollable = true)
        {
            if (viewport.ID == ImguiID.None)
                throw new Exception("You must open a Viewport before beginning a Frame");

            var edge = style.BorderWeight;
            edge.Left += style.Padding.X;
            edge.Right += style.Padding.X;
            edge.Top += style.Padding.Y;
            edge.Bottom += style.Padding.Y;

            var clip = bounds.OverlapRect(clipStack.Peek());
            clip = clip.Inflate(-edge.Left, -edge.Top, -edge.Right, -edge.Bottom);

            if (clip.Area > 0)
            {
                frameStack.Push(frame);
                frame = new ImguiFrame
                {
                    ID = PushId(name),
                    Layer = Layer,
                    Bounds = bounds,
                    Clip = clip,
                    Padding = edge,
                    Scrollable = scrollable
                };

                // see EndFrame for lookups
                var innerHeight = Storage.GetNumber(frame.ID, 1, 0f);
                var scrollY = Storage.GetNumber(frame.ID, 3, 0f);

                if (frame.Clip.Contains(viewport.Mouse))
                {
                    if (viewport.NextHotFrame == ImguiID.None || viewport.NextHotFrameLayer >= Layer)
                    {
                        viewport.NextHotFrame = frame.ID;
                        viewport.NextHotFrameLayer = frame.Layer;
                    }
                }

                Box(bounds, style);

                // handle vertical scrolling
                if (frame.Scrollable)
                {
                    var verticalScroll = false;

                    frame.Scroll = Vector2.Zero;
                    frame.Scroll = new Vector2(0, scrollY);

                    if (innerHeight > frame.Bounds.Height)
                    {
                        verticalScroll = true;

                        var height = frame.Bounds.Height - frame.Padding.Height;

                        frame.Bounds.Width -= 16;
                        frame.Scroll.Y = Calc.Clamp(frame.Scroll.Y, 0, innerHeight - height);

                        var scrollRect = VerticalScrollBar(frame.Bounds, frame.Scroll, innerHeight);
                        var buttonRect = scrollRect.OverlapRect(frame.Clip);

                        if (buttonRect.Height > 8)
                        {
                            var scrollId = Id("Scroll-Y");
                            ButtonBehaviour(scrollId, buttonRect);

                            if (ActiveId == scrollId)
                            {
                                var delta = viewport.MouseDelta.Y * (innerHeight / height);
                                frame.Scroll.Y = Calc.Clamp(frame.Scroll.Y + delta, 0, innerHeight - bounds.Height);
                                scrollRect = VerticalScrollBar(frame.Bounds, frame.Scroll, innerHeight);
                            }

                            Box(scrollRect.Inflate(-4), Style.Scrollbar, scrollId);
                        }

                        frame.Clip.Width -= 16;
                    }

                    if (!verticalScroll)
                        frame.Scroll = Vector2.Zero;

                    PushClip(frame.Clip);
                }

                // behave as a button
                // this way stuff can check if it's the ActiveID
                ButtonBehaviour(frame.ID, frame.Clip);
                CurrentId = frame.ID;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void EndFrame()
        {
            if (frame.ID == ImguiID.None)
                throw new Exception("You must Begin a Frame before ending it");

            Storage.SetNumber(frame.ID, 0, frame.InnerWidth);
            Storage.SetNumber(frame.ID, 1, frame.InnerHeight);
            Storage.SetNumber(frame.ID, 2, frame.Scroll.X);
            Storage.SetNumber(frame.ID, 3, frame.Scroll.Y);

            PopId();

            if (frame.Scrollable)
                PopClip();

            if (frameStack.Count > 0)
            {
                frame = frameStack.Pop();
                Batcher.SetLayer(frame.Layer);
            }
            else
            {
                frame = new ImguiFrame();
                Batcher.SetLayer(0);
            }
        }

        private Rect VerticalScrollBar(Rect bounds, Vector2 scroll, float innerHeight)
        {
            var barH = Math.Max(32, bounds.Height * (bounds.Height / innerHeight));
            if (barH > bounds.Height)
                barH = bounds.Height;

            var barY = Calc.Clamp((bounds.Height - barH) * (scroll.Y / (innerHeight - bounds.Height)), 0, Math.Max(0, bounds.Height - barH));
            return new Rect(bounds.Right, bounds.Y + barY, 16f, barH);
        }

        #endregion

        #region Button Behaviours

        public bool GrabbingBehaviour(ImguiID id, Rect position)
        {
            if (HoverBehaviour(id, position))
                HotId = id;

            if (LastHotId == id && App.Input.Mouse.LeftPressed)
                ActiveId = id;

            if (ActiveId == id && App.Input.Mouse.LeftReleased)
                ActiveId = ImguiID.None;

            return ActiveId == id;
        }

        public bool ButtonBehaviour(ImguiID id, Rect position)
        {
            var performPress = false;

            if (HoverBehaviour(id, position))
                HotId = id;

            if (LastHotId == id && App.Input.Mouse.LeftPressed)
                ActiveId = id;

            if (ActiveId == id && App.Input.Mouse.LeftReleased)
            {
                if (HotId == id)
                    performPress = true;
                ActiveId = ImguiID.None;
            }

            return performPress;
        }

        public bool HoverBehaviour(ImguiID id, Rect position)
        {
            if (viewport.MouseObstructed)
                return false;

            if (ActiveId != ImguiID.None && ActiveId != id)
                return false;

            if (frame.ID != ImguiID.None && viewport.LastHotFrame != ImguiID.None && viewport.LastHotFrame != frame.ID)
                return false;

            if (App.Input.Mouse.LeftDown && !App.Input.Mouse.LeftPressed)
                return false;

            if (!Clip.Contains(viewport.Mouse) || !position.Contains(viewport.Mouse))
                return false;

            return true;
        }

        public bool HoveringOrDragging(ImguiID id)
        {
            return LastHotId == id || LastActiveId == id;
        }

        #endregion

        #region Drawing

        public Rect Box(Rect rect, StyleElement style, ImguiID id)
        {
            if (id == ActiveId)
                return Box(rect, style.Active);
            else if (id == HotId)
                return Box(rect, style.Hot);
            else
                return Box(rect, style.Idle);
        }

        public Rect Box(Rect rect, StyleState style)
        {
            return Box(rect, style.Padding, style.BorderRadius, style.BorderWeight, style.BorderColor, style.BackgroundColor);
        }

        public Rect Box(Rect rect, Vector2 padding, BorderRadius radius, BorderWeight borderWeight, Color border, Color background)
        {
            if (radius.Rounded)
            {
                if (borderWeight.Weighted && border.A > 0)
                {
                    var inner = rect.Inflate(-borderWeight.Left, -borderWeight.Top, -borderWeight.Right, -borderWeight.Bottom);
                    Batcher.RoundedRect(rect, radius.TopLeft, radius.TopRight, radius.BottomRight, radius.BottomLeft, border);
                    Batcher.RoundedRect(inner,
                        radius.TopLeft - (borderWeight.Top > 0 && borderWeight.Left > 0 ? 1 : 0),
                        radius.TopRight - (borderWeight.Top > 0 && borderWeight.Right > 0 ? 1 : 0),
                        radius.BottomRight - (borderWeight.Bottom > 0 && borderWeight.Right > 0 ? 1 : 0),
                        radius.BottomLeft - (borderWeight.Bottom > 0 && borderWeight.Left > 0 ? 1 : 0),
                        background);
                }
                else
                {
                    Batcher.RoundedRect(rect, radius.TopLeft, radius.TopRight, radius.BottomRight, radius.BottomLeft, background);
                }
            }
            else
            {
                if (borderWeight.Weighted && border.A > 0)
                {
                    var inner = rect.Inflate(-borderWeight.Left, -borderWeight.Top, -borderWeight.Right, -borderWeight.Bottom);
                    Batcher.Rect(rect, border);
                    Batcher.Rect(inner, background);
                }
                else
                {
                    Batcher.Rect(rect, background);
                }
            }

            return new Rect(rect.X + borderWeight.Left + padding.X, rect.Y + borderWeight.Top + padding.Y, rect.Width - borderWeight.Width - padding.X * 2, rect.Height - borderWeight.Height - padding.Y * 2);
        }

        #endregion
    }
}
