using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{

    // TODO:
    // Value Storage needs to exist

    public class Imgui
    {

        #region Structs

        public struct ID
        {
            public readonly int Value;
            public ID(int value) => Value = value;
            public ID(Name name, ID parent) => Value = HashCode.Combine(name.Value, parent.Value);

            public override bool Equals(object? obj) => obj != null && (obj is ID id) && (this == id);
            public override int GetHashCode() => Value;
            public override string ToString() => Value.ToString();

            public static bool operator ==(ID a, ID b) => a.Value == b.Value;
            public static bool operator !=(ID a, ID b) => a.Value != b.Value;

            public static readonly ID None = new ID(0);
        }

        public struct Name
        {
            public int Value;
            public Name(int value) => Value = value;

            public static implicit operator Name(int id) => new Name(id);
            public static implicit operator Name(float id) => new Name(id.GetHashCode());
            public static implicit operator Name(string text) => new Name(text.GetHashCode());
        }

        public struct ViewportState
        {
            public ID ID;
            public Batch2d Batcher;
            public Vector2 Scale;
            public Rect Bounds;
            public Vector2 Mouse;
            public Vector2 MouseDelta;
            public bool MouseObstructed;
            public ID LastHotFrame;
            public ID NextHotFrame;
        }

        public struct FrameState
        {
            public ID ID;
            public Rect Bounds;
            public Rect Clip;
            public Rect LastCell;
            public Vector2 Scroll;

            public bool Scrollable;
            public bool Overflow;
            public BorderWeight Padding;

            public int Columns;
            public int Column;
            public float ColumnOffset;

            public int Row;
            public float RowHeight;
            public float RowOffset;

            public float InnerWidth => Bounds.Width - Padding.Width;
            public float InnerHeight => RowOffset + RowHeight + Padding.Height;

            public void NextRow(int columns, float indent, float spacing)
            {
                if (Row > 0)
                    RowOffset += RowHeight + spacing;

                RowHeight = 0;
                Row++;
                Column = 0;
                ColumnOffset = indent;
                Columns = columns;
            }

            public Rect NextCell(float width, float height, float indent, float spacing)
            {
                if (Column >= Columns)
                    NextRow(1, indent, spacing);

                if (Column != 0)
                    ColumnOffset += spacing;

                // determine cell width
                float cellWidth;
                {
                    if (width < 0)
                    {
                        // fill to the right (negative width)
                        cellWidth = InnerWidth - ColumnOffset + width;
                    }
                    else if (width == float.MaxValue)
                    {
                        // fill based on our remaining space, divided by remaining elements
                        var remaining = (Columns - Column);
                        cellWidth = (InnerWidth - (remaining - 1) * spacing - ColumnOffset) / remaining;
                    }
                    else
                    {
                        // explicit width
                        cellWidth = width;
                    }

                    // can't overflow, clamp cell width
                    if (!Overflow)
                    {
                        var max = InnerWidth - ColumnOffset;
                        cellWidth = Math.Min(max, cellWidth);
                    }

                    // smaller than zero width
                    if (cellWidth < 0)
                        cellWidth = 0;
                }

                // determine cell height
                float cellHeight;
                if (height < 0)
                    cellHeight = Bounds.Height - RowOffset - Padding.Height + height;
                else if (height == float.MaxValue)
                    cellHeight = Bounds.Height - RowOffset - Padding.Height;
                else
                    cellHeight = height;

                // position
                var position = new Rect(Bounds.X + Padding.Left + ColumnOffset - Scroll.X, Bounds.Y + Padding.Top + RowOffset - Scroll.Y, cellWidth, cellHeight);

                // setup for next cell
                ColumnOffset += cellWidth;
                RowHeight = Math.Max(RowHeight, cellHeight);
                Column++;

                LastCell = position;

                return position;
            }
        }

        public class StorageData
        {
            private Dictionary<ID, float> numbers = new Dictionary<ID, float>();
            private Dictionary<ID, bool> bools = new Dictionary<ID, bool>();
            private Dictionary<ID, ID> ids = new Dictionary<ID, ID>();

            public bool Used;

            public void SetBool(ID id, Name name, bool value) => bools[new ID(name, id)] = value;
            public bool GetBool(ID id, Name name, bool defaultValue) => bools.TryGetValue(new ID(name, id), out var v) ? v : defaultValue;

            public void SetNumber(ID id, Name name, float value) => numbers[new ID(name, id)] = value;
            public float GetNumber(ID id, Name name, float defaultValue) => numbers.TryGetValue(new ID(name, id), out var v) ? v : defaultValue;

            public void SetId(ID id, Name name, ID value) => ids[new ID(name, id)] = value;
            public ID GetId(ID id, Name name, ID defaultValue) => ids.TryGetValue(new ID(name, id), out var v) ? v : defaultValue;
        }

        #endregion

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

        public ViewportState Viewport => viewport;
        public FrameState Frame => frame;
        public Rect Clip => clipStack.Count > 0 ? clipStack.Peek() : new Rect();
        public Batch2d Batcher => viewport.Batcher;
        public Rect LastCell => frame.LastCell;
        public StorageData Storage => storageStack.Peek();

        public ID HotId = ID.None;
        public ID LastHotId = ID.None;
        public ID ActiveId = ID.None;
        public ID LastActiveId = ID.None;
        public ID ParentId => (idStack.Count > 0 ? idStack.Peek() : ID.None);

        public ID CurrentId
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

        private ID currentId;
        private bool lastActiveIdExists;

        private ViewportState viewport;
        private FrameState frame;

        private readonly Stack<FrameState> frameStack = new Stack<FrameState>();
        private readonly Stack<ID> idStack = new Stack<ID>();
        private readonly Stack<Rect> clipStack = new Stack<Rect>();
        private readonly Stack<float> indentStack = new Stack<float>();
        private readonly Stack<SpriteFont> fontStack = new Stack<SpriteFont>();
        private readonly Stack<float> fontSizeStack = new Stack<float>();
        private readonly Stack<float> spacingStack = new Stack<float>();
        private readonly Stack<StorageData> storageStack = new Stack<StorageData>();
        private readonly Dictionary<ID, StorageData> storages = new Dictionary<ID, StorageData>();

        #endregion

        #region Constructor

        public Imgui(SpriteFont font)
        {
            Style = Stylesheets.Default;
            DefaultFont = font;
            DefaultFontSize = 14;
            DefaultSpacing = 4;
        }

        #endregion

        #region State Changing

        public ID Id(Name name)
        {
            return CurrentId = new ID(name, idStack.Count > 0 ? idStack.Peek() : ID.None);
        }

        public ID PushId(ID id)
        {
            idStack.Push(id);
            return id;
        }

        public ID PushId(Name name)
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
        public void BeginStorage(Name name)
        {
            var id = new ID(name, ParentId);
            if (!storages.TryGetValue(id, out var storage))
                storage = storages[id] = new StorageData();
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
            if (frame.ID == ID.None)
                throw new Exception("You must begin a Frame before creating a Row");

            frame.NextRow(1, Indent, Spacing);
        }

        public void Row(int columns)
        {
            if (frame.ID == ID.None)
                throw new Exception("You must begin a Frame before creating a Row");

            frame.NextRow(columns, Indent, Spacing);
        }

        public Rect Remainder()
        {
            if (frame.ID == ID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(float.MaxValue, float.MaxValue, Indent, Spacing);
        }

        public Rect Cell(Vector2 size)
        {
            return Cell(size.X, size.Y);
        }

        public Rect Cell(float width, float height)
        {
            if (frame.ID == ID.None)
                throw new Exception("You must begin a Frame before creating a Cell");

            return frame.NextCell(width, height, Indent, Spacing);
        }

        public void Separator() => Cell(float.MaxValue, Spacing);
        public void Separator(float height) => Cell(float.MaxValue, height);
        public void Separator(float width, float height) => Cell(width, height);

        public void Step()
        {
            viewport = new ViewportState();
            frame = new FrameState();
            LastHotId = HotId;
            HotId = ID.None;

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
                List<ID>? removing = null;
                foreach (var kv in storages)
                    if (!kv.Value.Used)
                    {
                        if (removing == null)
                            removing = new List<ID>();
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
                if (LastActiveId == ID.None && ActiveId != ID.None)
                    lastActiveIdExists = true;

                LastActiveId = ActiveId;

                if (!lastActiveIdExists)
                    ActiveId = ID.None;

                lastActiveIdExists = false;
            }
        }

        #endregion

        #region Viewports / Frames

        public void BeginViewport(Window window, Batch2d batcher, Vector2? contentScale = null)
        {
            var scale = window.ContentScale * (contentScale ?? Vector2.One);
            var bounds = new Rect(0, 0, window.DrawableWidth / scale.X, window.DrawableHeight / scale.Y);
            var mouse = window.DrawableMouse / scale;

            BeginViewport(window.Title, batcher, bounds, mouse, scale, !window.MouseOver);
        }

        public void BeginViewport(Name name, Batch2d batcher, Rect bounds, Vector2 mouse, Vector2 scale, bool mouseObstructed = false)
        {
            if (viewport.ID != ID.None)
                throw new Exception("The previous Viewport must be ended before beginning a new one");

            viewport = new ViewportState
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
            viewport.LastHotFrame = Storage.GetId(viewport.ID, 2, ID.None);

            PushClip(viewport.Bounds);
            viewport.Batcher.PushMatrix(Matrix3x2.CreateScale(scale));
        }

        public void EndViewport()
        {
            if (viewport.ID == ID.None)
                throw new Exception("You must Begin a Viewport before ending it");
            if (frame.ID != ID.None)
                throw new Exception("The previous Group must be closed before closing the Viewport");

            Storage.SetNumber(viewport.ID, 0, viewport.Mouse.X);
            Storage.SetNumber(viewport.ID, 1, viewport.Mouse.Y);
            Storage.SetId(viewport.ID, 2, viewport.NextHotFrame);

            PopClip();
            EndStorage();

            viewport.Batcher.PopMatrix();
            viewport = new ViewportState();
        }

        public bool BeginFrame(Name info, float height)
        {
            if (frame.ID != ID.None)
                return BeginFrame(info, Cell(0, height));
            else
                return BeginFrame(info, viewport.Bounds);
        }

        public bool BeginFrame(Name name, Rect bounds, bool scrollable = true)
        {
            return BeginFrame(name, bounds, Style.Frame, scrollable);
        }

        public bool BeginFrame(Name name, Rect bounds, StyleState style, bool scrollable = true)
        {
            if (viewport.ID == ID.None)
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
                frame = new FrameState
                {
                    ID = PushId(name),
                    Bounds = bounds,
                    Clip = clip,
                    Padding = edge,
                    Scrollable = scrollable
                };

                // using constant numbers since it's fast (I presume)
                // see EndFrame for lookups
                var lastInnerHeight = Storage.GetNumber(frame.ID, 1, 0f);
                var lastScrollY = Storage.GetNumber(frame.ID, 3, 0f);

                if (frame.Clip.Contains(viewport.Mouse))
                    viewport.NextHotFrame = frame.ID;

                Box(bounds, style);

                // handle vertical scrolling
                if (frame.Scrollable)
                {
                    frame.Scroll = Vector2.Zero;
                    frame.Scroll = new Vector2(0, lastScrollY);

                    if (lastInnerHeight > frame.Bounds.Height)
                    {
                        frame.Bounds.Width -= 16;

                        frame.Scroll.Y = Calc.Clamp(frame.Scroll.Y, 0, lastInnerHeight - frame.Bounds.Height);

                        var scrollId = Id("Scroll-Y");
                        var scrollRect = VerticalScrollBar(frame.Bounds, frame.Scroll, lastInnerHeight);
                        var buttonRect = scrollRect.OverlapRect(frame.Clip);

                        if (buttonRect.Area > 0)
                        {
                            ButtonBehaviour(scrollId, buttonRect);

                            if (ActiveId == scrollId)
                            {
                                var relativeSpeed = (frame.Bounds.Height / scrollRect.Height);
                                frame.Scroll.Y = Calc.Clamp(frame.Scroll.Y + viewport.MouseDelta.Y * relativeSpeed, 0, lastInnerHeight - bounds.Height);
                                scrollRect = VerticalScrollBar(frame.Bounds, frame.Scroll, lastInnerHeight);
                            }

                            Box(scrollRect.Inflate(-4), Style.Scrollbar, scrollId);
                        }

                        frame.Clip.Width -= 16;
                    }
                    else
                    {
                        frame.Scroll = Vector2.Zero;
                    }

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
            if (frame.ID == ID.None)
                throw new Exception("You must Begin a Frame before ending it");

            Storage.SetNumber(frame.ID, 0, frame.InnerWidth);
            Storage.SetNumber(frame.ID, 1, frame.InnerHeight);
            Storage.SetNumber(frame.ID, 2, frame.Scroll.X);
            Storage.SetNumber(frame.ID, 3, frame.Scroll.Y);

            PopId();

            if (frame.Scrollable)
                PopClip();

            if (frameStack.Count > 0)
                frame = frameStack.Pop();
            else
                frame = new FrameState();
        }

        private Rect VerticalScrollBar(Rect bounds, Vector2 scroll, float innerHeight)
        {
            var barH = bounds.Height * (bounds.Height / innerHeight);
            var barY = (bounds.Height - barH) * (scroll.Y / (innerHeight - bounds.Height));

            return new Rect(bounds.Right, bounds.Y + barY, 16f, barH);
        }

        #endregion

        #region Button Behaviours

        public bool GrabbingBehaviour(ID id, Rect position)
        {
            if (HoverBehaviour(id, position))
                HotId = id;

            if (LastHotId == id && App.Input.Mouse.LeftPressed)
                ActiveId = id;

            if (ActiveId == id && App.Input.Mouse.LeftReleased)
                ActiveId = Imgui.ID.None;

            return ActiveId == id;
        }

        public bool ButtonBehaviour(ID id, Rect position)
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
                ActiveId = Imgui.ID.None;
            }

            return performPress;
        }

        public bool HoverBehaviour(ID id, Rect position)
        {
            if (viewport.MouseObstructed)
                return false;

            if (ActiveId != ID.None && ActiveId != id)
                return false;

            if (frame.ID != ID.None && viewport.LastHotFrame != ID.None && viewport.LastHotFrame != frame.ID)
                return false;

            if (App.Input.Mouse.LeftDown && !App.Input.Mouse.LeftPressed)
                return false;

            if (!Clip.Contains(viewport.Mouse) || !position.Contains(viewport.Mouse))
                return false;

            return true;
        }

        public bool HoveringOrDragging(ID id)
        {
            return LastHotId == id || LastActiveId == id;
        }

        #endregion

        #region Drawing

        public Rect Box(Rect rect, StyleElement style, ID id)
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
