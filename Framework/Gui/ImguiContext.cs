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

            public Group(ID id, Rect bounds, float padding, bool overflow)
            {
                ID = id;
                Bounds = bounds;
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
                var position = new Rect(Bounds.X + Padding + ColumnOffset, Bounds.Y + Padding + RowOffset, cellWidth, height);

                // setup for next cell
                ColumnOffset += cellWidth;
                RowHeight = Math.Max(RowHeight, height);
                Column++;

                return position;
            }
        }

        private struct Storage
        {
            public bool Toggled;
            public float InnerHeight;
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
        public Rect Bounds;
        public Vector2 Mouse;
        public Action<ImguiContext>? Refresh;

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
                FontSize = 32,
                Spacing = 8,
                ElementPadding = 8,
                WindowPadding = 8,
                TitleScale = 1.25f
            };
        }

        public void Update(Vector2 mouse)
        {
            Mouse = mouse;
            HotId = ID.None;

            // reset as safety
            // these should be already empty from the previous frame
            indents.Clear();
            styles.Clear();
            ids.Clear();
            groups.Clear();

            // clear our draw data
            Batch.Clear();

            // invoke refresh
            BeginGroup(0, 0);
            Refresh?.Invoke(this);
            EndGroup();

            // clear previous frame stored info with this frame's info
            lastStorage.Clear();
            foreach (var kv in nextStorage)
                lastStorage.Add(kv.Key, kv.Value);
            nextStorage.Clear();
        }

        public void Render()
        {
            Batch.Render();
        }

        #region Layout

        public ID Id(UniqueInfo value) => LastId = new ID(value.Value, (ids.Count > 0 ? ids.Peek() : ID.Root));

        public void PushId(ID id) => ids.Push(id);
        public void PushId(UniqueInfo info) => ids.Push(Id(info));
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

        private void Store(ID id, Storage info) => nextStorage[id] = info;
        private bool Stored(ID id) => lastStorage.ContainsKey(id);
        private bool Stored(ID id, out Storage info) => lastStorage.TryGetValue(id, out info);

        public void BeginGroup(UniqueInfo info, float height, bool fitToChildren = false)
        {
            var id = Id(info);
            PushId(id);

            if (fitToChildren && Stored(id, out var last))
                height = last.InnerHeight;

            Rect bounds;
            if (id == ID.Root)
            {
                bounds = Bounds;
            }
            else
            {
                bounds = Cell(height);
                groups.Push(group);
            }

            group = new Group(id, bounds, Style.WindowPadding, true);

            Batch.Rect(bounds, Color.White * 0.2f);
        }

        public void EndGroup()
        {
            Store(group.ID, new Storage() { InnerHeight = group.InnerHeight });
            PopId();

            if (group.ID != ID.Root)
                group = groups.Pop();
        }

        #endregion

        #region Button

        private bool ButtonBehaviour(ID id, Rect position)
        {
            var performPress = false;

            if (position.Contains(Mouse))
                HotId = id;

            if (HotId == id && App.Input.Mouse.Pressed(MouseButtons.Left))
                ActiveId = id;

            if (ActiveId == id && App.Input.Mouse.Released(MouseButtons.Left))
            {
                if (HotId == id)
                    performPress = true;
                ActiveId = ID.None;
            }

            return performPress;
        }

        public bool Button(string label, float width = 0f, float height = 0f)
        {
            return Button(label, label, width, height);
        }

        public bool Button(UniqueInfo identifier, string label, float width = 0f, float height = 0f)
        {
            if (width == PreferredSize)
                width = Style.Font.WidthOf(label) * Style.FontScale + Style.ElementPadding * 2f;
            if (height == 0f)
                height = Style.FontSize + Style.ElementPadding * 2;

            return Button(identifier, label, Cell(width, height));
        }

        public bool Button(UniqueInfo identifier, string label, Rect position)
        {
            var id = Id(identifier);
            var result = ButtonBehaviour(id, position);
            var scale = Vector2.One * Style.FontScale;
            var color = Color.White;

            if (ActiveId == id)
            {
                color = Color.Red;
            }
            else if (HotId == id)
            {
                color = Color.Yellow;
            }

            Batch.Rect(position, color);
            Batch.PushMatrix(new Vector2(position.X + Style.ElementPadding, position.Y + Style.ElementPadding), scale, Vector2.Zero, 0f);
            Batch.Text(Style.Font, label, Color.Black);
            Batch.PopMatrix();

            return result;
        }

        #endregion

        #region Header

        public bool Header(string label, bool startOpen = false)
        {
            var toggle = Button(label);
            var id = LastId;
            var enabled = (Stored(id, out var info) && info.Toggled) || (!Stored(id) && startOpen);

            if (toggle)
                enabled = !enabled;

            Store(id, new Storage() { Toggled = enabled });

            if (enabled)
            {
                PushId(id);
                PushIndent(30f);
            }

            return enabled;
        }

        public void EndHeader()
        {
            PopIndent();
            PopId();
        }

        #endregion

        #region Label

        public void Label(string label)
        {
            Label(label, label);
        }

        public void Label(UniqueInfo identifier, string label)
        {
            Label(identifier, label, Cell(Style.FontSize + Style.ElementPadding * 2f));
        }

        public void Label(UniqueInfo identifier, string label, Rect position)
        {
            var scale = Vector2.One * Style.FontScale;

            Batch.PushMatrix(new Vector2(position.X, position.Y + Style.ElementPadding), scale, Vector2.Zero, 0f);
            Batch.Text(Style.Font, label, Color.White);
            Batch.PopMatrix();
        }

        #endregion

        #region Title

        public void Title(string label)
        {
            Title(label, label);
        }

        public void Title(UniqueInfo identifier, string label)
        {
            Title(identifier, label, Cell(Style.FontSize * Style.TitleScale + Style.ElementPadding * 2f));
        }

        public void Title(UniqueInfo identifier, string label, Rect position)
        {
            var scale = Vector2.One * Style.FontScale * Style.TitleScale;

            Batch.PushMatrix(new Vector2(position.X, position.Y + Style.ElementPadding), scale, Vector2.Zero, 0f);
            Batch.Text(Style.Font, label, Color.White);
            Batch.PopMatrix();
            Batch.Rect(position.X, position.Bottom - 4, position.Width, 4, Color.White);
        }

        #endregion

        #region Textbox



        #endregion
    }
}
