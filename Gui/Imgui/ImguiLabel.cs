using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiLabel
    {
        public static void Label(this Imgui imgui, string label)
        {
            Label(imgui, label, imgui.Style.Generic.Idle);
        }

        public static void Label(this Imgui imgui, string label, StyleState style)
        {
            var content = new TextContent(label);
            var size = content.PreferredSize(imgui);
            var position = imgui.Cell(size.X, size.Y);

            imgui.Label(content, position, style);
        }

        public static void Label(this Imgui imgui, IContent label, Rect position, StyleState style)
        {
            if (position.Intersects(imgui.Clip))
            {
                var inner = position.Inflate(-style.Padding.X, -style.Padding.Y, -style.Padding.X, -style.Padding.Y);
                label.Draw(imgui, imgui.Batcher, style, inner);
            }
        }
    }
}
