using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiButton
    {


        public static bool Button(this Imgui imgui, string label)
        {
            var content = new Text(label);
            return Button(imgui, content.UniqueInfo(), content, Sizing.FillX(), imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, string label, Sizing sizing)
        {
            var content = new Text(label);
            return Button(imgui, content.UniqueInfo(), content, sizing, imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, IContent content)
        {
            return Button(imgui, content.UniqueInfo(), content, Sizing.FillX(), imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, IContent content, Sizing sizing)
        {
            return Button(imgui, content.UniqueInfo(), content, sizing, imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, Imgui.UniqueInfo info, IContent content, Sizing sizing, StyleElement style)
        {
            var size = sizing.SizeOf(imgui, content, style.Idle.Padding);
            var position = imgui.Cell(size);

            return Button(imgui, info, content, position, style);
        }

        public static bool Button(this Imgui imgui, Imgui.UniqueInfo info, IContent content, Rect position, StyleElement style)
        {
            var result = false;
            var id = imgui.Id(info);

            if (position.Intersects(imgui.Clip))
            {
                result = imgui.ButtonBehaviour(id, position);

                var inner = imgui.Box(position, style, id);
                var state = style.Current(imgui.ActiveId, imgui.HotId, id);

                content.Draw(imgui, imgui.Batcher, state, inner);
            }

            return result;
        }
    }
}
