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
            Label(imgui, label, Sizing.Preferred(), imgui.Style.Label);
        }

        public static void Label(this Imgui imgui, string label, Sizing sizing)
        {
            Label(imgui, label, sizing, imgui.Style.Label);
        }

        public static void Label(this Imgui imgui, string label, Sizing sizing, StyleState style)
        {
            var content = new Text(label);
            var size = sizing.SizeOf(imgui, content, style.Padding);

            Label(imgui, content, imgui.Cell(size), style);
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
