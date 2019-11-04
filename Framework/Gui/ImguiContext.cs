using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class ImguiContext
    {
        public struct ID
        {
            public readonly int Parent;
            public readonly int Value;
            public readonly int Identifier;

            public ID(int id) : this(id, Root.Identifier) { }
            public ID(int id, ID parent) : this(id, parent.Identifier) { }
            public ID(int id, int parent)
            {
                Parent = parent;
                Value = id;
                Identifier = (Parent + Value).GetHashCode();
            }

            public override bool Equals(object? obj) => obj != null && (obj is ID id) && (this == id);
            public override int GetHashCode() => Identifier;

            public static bool operator ==(ID a, ID b) => a.Identifier == b.Identifier;
            public static bool operator !=(ID a, ID b) => a.Identifier != b.Identifier;

            public static readonly ID Root = new ID(0, 0);
            public static readonly ID None = new ID(-1, -1);
        }

        public struct UniqueInfo
        {
            public int Value;
            public UniqueInfo(int value) => Value = value;

            public static implicit operator UniqueInfo(int id) => new UniqueInfo(id);
            public static implicit operator UniqueInfo(float id) => new UniqueInfo(id.GetHashCode());
            public static implicit operator UniqueInfo(string text) => new UniqueInfo(text.GetHashCode());
        }

        private struct Group
        {
            public ID ID;
            public Rect Bounds;
            public Rect Scissor;
            public Vector2 Scroll;

            public bool Overflow;
            public float Padding;

            public int Columns;
            public int Column;
            public float ColumnOffset;

            public int Row;
            public float RowHeight;
            public float RowOffset;

            public float InnerWidth => Bounds.Width - Padding * 2f;
            public float InnerHeight => RowOffset + RowHeight + Padding * 2f;

            public Group(ID id, Vector2 scroll, Rect bounds, Rect screenBounds, float padding, bool overflow)
            {
                ID = id;
                Scroll = scroll;
                Bounds = bounds;
                Scissor = screenBounds;
                Columns = 0;
                Row = 0;
                Column = 0;
                RowHeight = 0;
                ColumnOffset = 0;
                RowOffset = 0;
                Padding = padding;
                Overflow = overflow;
            }

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
                if (width < 0)
                {
                    // fill to the right (negative width)
                    cellWidth = InnerWidth - ColumnOffset + width;
                }
                else if (width > 0)
                {
                    // explicit width
                    cellWidth = width;
                }
                else
                {
                    // fill based on our remaining space, divided by remaining elements
                    var remaining = (Columns - Column);
                    cellWidth = (InnerWidth - (remaining - 1) * spacing - ColumnOffset) / remaining;
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

                // position
                var position = new Rect(Bounds.X + Padding + ColumnOffset - Scroll.X, Bounds.Y + Padding + RowOffset - Scroll.Y, cellWidth, height);

                // setup for next cell
                ColumnOffset += cellWidth;
                RowHeight = Math.Max(RowHeight, height);
                Column++;

                return position;
            }
        }

        public struct Storage
        {
            public bool Toggled;
            public float InnerHeight;
            public Vector2 Scroll;
        }

        public struct Stylesheet
        {
            public SpriteFont Font;
            public float FontSize;
            public float Spacing;
            public float ElementPadding;
            public float WindowPadding;
            public float TitleScale;
            public float FontScale => FontSize / Font.Height;
            public float ElementHeight => FontSize + ElementPadding * 2;
        }

        public Batch2D Batch;
        public Vector2 PixelSize = Vector2.One;
        public Action<ImguiContext>? Refresh;

        public Rect Bounds;
        public Rect ScreenBounds
        {
            get => Bounds.Scale(PixelSize);
            set => Bounds = value.Scale(1f / PixelSize);
        }

        public Rect Scissor => group.Scissor;
        public Rect ScreenScissor => group.Scissor.Scale(PixelSize);

        public Vector2 Mouse;
        public Vector2 DeltaMouse;

        public Stylesheet DefaultStyle;
        public Stylesheet Style => (styles.Count > 0 ? styles.Peek() : DefaultStyle);
        public float Indent => (indents.Count > 0 ? indents.Peek() : 0f);

        public ID HotId;
        public ID ActiveId;
        public ID LastId;

        private Group group;
        private readonly Stack<Group> groups = new Stack<Group>();
        private readonly Stack<ID> ids = new Stack<ID>();
        private readonly Stack<Stylesheet> styles = new Stack<Stylesheet>();
        private readonly Stack<float> indents = new Stack<float>();
        private readonly Dictionary<ID, Storage> lastStorage = new Dictionary<ID, Storage>();
        private readonly Dictionary<ID, Storage> nextStorage = new Dictionary<ID, Storage>();

        public static float PreferredSize = float.MinValue;

        public ImguiContext(SpriteFont font)
        {
            Batch = new Batch2D();

            DefaultStyle = new Stylesheet()
            {
                Font = font,
                FontSize = 16,
                Spacing = 4,
                ElementPadding = 4,
                WindowPadding = 4,
                TitleScale = 1.25f
            };
        }

        public void Update(Vector2 mouse)
        {
            mouse /= PixelSize;
            DeltaMouse = mouse - Mouse;
            Mouse = mouse;
            HotId = ID.None;

            // reset as safety
            // these should be already empty from the previous frame
            indents.Clear();
            styles.Clear();
            ids.Clear();
            groups.Clear();
            group.Bounds = Bounds;
            group.Scissor = Bounds;

            // invoke refresh
            Batch.Clear();
            if (BeginGroup(0, 0))
            {
                Refresh?.Invoke(this);
                EndGroup();
            }

            // clear previous frame stored info with this frame's info
            lastStorage.Clear();
            foreach (var kv in nextStorage)
                lastStorage.Add(kv.Key, kv.Value);
            nextStorage.Clear();
        }

        public void Render()
        {
            Batch.Render(Matrix3x2.CreateScale(PixelSize));
        }

        public ID Id(UniqueInfo value) => LastId = new ID(value.Value, (ids.Count > 0 ? ids.Peek() : ID.Root));

        public ID PushId(ID id)
        {
            ids.Push(id);
            return id;
        }

        public ID PushId(UniqueInfo info) => PushId(Id(info));
        public void PopId() => ids.Pop();

        public void PushStyle(Stylesheet style) => styles.Push(style);
        public void PopStyle() => styles.Pop();

        public void PushIndent(float amount) => indents.Push(Indent + amount);
        public void PopIndent() => indents.Pop();

        public void Row() => group.NextRow(1, Indent, Style.Spacing);
        public void Row(int columns) => group.NextRow(columns, Indent, Style.Spacing);
        public Rect Cell(float height) => group.NextCell(0f, height, Indent, Style.Spacing);
        public Rect Cell(float width, float height) => group.NextCell(width, height, Indent, Style.Spacing);

        public void Separator() => Cell(Style.Spacing);
        public void Separator(float height) => Cell(height);
        public void Separator(float width, float height) => Cell(width, height);

        public void Store(ID id, Storage info) => nextStorage[id] = info;
        public bool Stored(ID id) => lastStorage.ContainsKey(id);
        public bool Stored(ID id, out Storage info) => lastStorage.TryGetValue(id, out info);

        /// <summary>
        /// Returns True of the Group is visible
        /// Do not call EndGroup if this is false
        /// </summary>
        public bool BeginGroup(UniqueInfo info, float height, bool fitToChildren = false)
        {
            var id = PushId(info);

            // get inner height from last frame
            var innerHeight = 0f;
            var scroll = Vector2.Zero;
            if (Stored(id, out var last))
            {
                innerHeight = last.InnerHeight;
                scroll = last.Scroll;
            }

            // determine bounds
            Rect bounds;
            Rect screen;
            if (id == ID.Root)
            {
                bounds = Bounds;
                screen = Bounds;
            }
            else
            {
                if (fitToChildren && innerHeight > 0f)
                    height = innerHeight;

                bounds = Cell(height);
                screen = group.Bounds.OverlapRect(bounds);
            }

            // only do group if we're visible
            if (screen.Area > 0)
            {
                // push last group onto stack
                if (id != ID.Root)
                    groups.Push(group);

                // draw bg
                Batch.Rect(bounds, Color.White * 0.2f);

                // handle vertical scrolling
                if (innerHeight > bounds.Height)
                {
                    bounds.Width -= 16;

                    scroll.Y = Calc.Clamp(scroll.Y, 0, innerHeight - bounds.Height);

                    var scrollId = Id("Scroll-Y");
                    var scrollRect = VerticalScrollBar(bounds, scroll, innerHeight);
                    var buttonRect = scrollRect.OverlapRect(screen);

                    if (buttonRect.Area > 0)
                    {
                        ImguiButton.ButtonBehaviour(this, scrollId, buttonRect);

                        if (ActiveId == scrollId)
                        {
                            var relativeSpeed = (bounds.Height / scrollRect.Height);
                            scroll.Y = Calc.Clamp(scroll.Y + DeltaMouse.Y * relativeSpeed, 0, innerHeight - bounds.Height);
                            scrollRect = VerticalScrollBar(bounds, scroll, innerHeight);
                        }

                        Batch.Rect(scrollRect, Color.Red);
                    }

                    screen.Width -= 16;
                }
                else
                    scroll.Y = 0;

                // create next group
                group = new Group(id, scroll, bounds, screen, Style.WindowPadding, true);

                // return true if we're visible
                Batch.SetScissor(screen.Scale(PixelSize).Int());
                return true;
            }
            // keep track of scrolling value even if we're not displayed
            else
            {
                Store(id, new Storage() { InnerHeight = innerHeight, Scroll = scroll });
                return false;
            }
        }

        private Rect VerticalScrollBar(Rect bounds, Vector2 scroll, float innerHeight)
        {
            var barH = bounds.Height * (bounds.Height / innerHeight);
            var barY = (bounds.Height - barH) * (scroll.Y / (innerHeight - bounds.Height));

            return new Rect(bounds.Right + 2f, bounds.Y + barY, 14f, barH);
        }

        public void EndGroup()
        {
            Store(group.ID, new Storage() { InnerHeight = group.InnerHeight, Scroll = group.Scroll });
            PopId();

            if (group.ID != ID.Root)
                group = groups.Pop();

            Batch.SetScissor(group.Scissor.Scale(PixelSize).Int());
        }
    }
}
