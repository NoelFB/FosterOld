using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GUI
{
    public struct ImguiFrame
    {
        public ImguiID ID;
        public int Layer;
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

        public Rect NextCell(Size width, Size height, float preferredWidth, float preferredHeight, float indent, float spacing)
        {
            if (Column >= Columns)
                NextRow(1, indent, spacing);

            if (Column != 0)
                ColumnOffset += spacing;

            // determine cell width
            float cellWidth;
            {
                if (width.Mode == Size.Modes.Preferred)
                {
                    cellWidth = preferredWidth;
                }
                else if (width.Mode == Size.Modes.Explicit)
                {
                    cellWidth = width.Min;
                }
                else
                {
                    if (width.UpTo > 0)
                    {
                        cellWidth = InnerWidth - ColumnOffset - width.UpTo;
                    }
                    else
                    {
                        var remaining = (Columns - Column);
                        cellWidth = (InnerWidth - (remaining - 1) * spacing - ColumnOffset) / remaining;
                    }

                }

                // clamp
                cellWidth = Math.Max(width.Min, Math.Min(width.Max, cellWidth));

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
            {
                if (height.Mode == Size.Modes.Preferred)
                {
                    cellHeight = preferredHeight;
                }
                else if (height.Mode == Size.Modes.Explicit)
                {
                    cellHeight = height.Min;
                }
                else
                {
                    if (height.UpTo > 0)
                    {
                        cellHeight = Bounds.Height - RowOffset - Padding.Height - height.UpTo;
                    }
                    else
                    {
                        cellHeight = Bounds.Height - RowOffset - Padding.Height;
                    }

                }

                // clamp
                cellHeight = Math.Max(height.Min, Math.Min(height.Max, cellHeight));
            }

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
}
