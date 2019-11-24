using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiButton
    {

        public static bool WideButton(this Imgui imgui, string label)
        {
            var content = new TextContent(label);
            var info = content.UniqueInfo();
            var style = imgui.Style.Item;

            var size = content.PreferredSize(imgui);
            size.X += style.Idle.Padding.X * 2;
            size.Y += style.Idle.Padding.Y * 2;

            var position = imgui.Cell(0, size.Y);

            return Button(imgui, info, content, position, style);
        }

        public static bool Button(this Imgui imgui, string label)
        {
            return Button(imgui, new TextContent(label));
        }

        public static bool Button(this Imgui imgui, string label, StyleElement style)
        {
            return Button(imgui, new TextContent(label), style);
        }

        public static bool Button(this Imgui imgui, IContent content)
        {
            return Button(imgui, content, imgui.Style.Item);
        }

        public static bool Button(this Imgui imgui, IContent content, StyleElement style)
        {
            return Button(imgui, content.UniqueInfo(), content, style);
        }

        public static bool Button(this Imgui imgui, Imgui.UniqueInfo info, IContent content)
        {
            return Button(imgui, info, content, imgui.Style.Item);
        }

        public static bool Button(this Imgui imgui, Imgui.UniqueInfo info, IContent content, StyleElement style)
        {
            var size = content.PreferredSize(imgui);
            size.X += style.Idle.Padding.X * 2;
            size.Y += style.Idle.Padding.Y * 2;

            var position = imgui.Cell(size.X, size.Y);

            return Button(imgui, info, content, position, style);
        }

        public static bool Button(this Imgui imgui, Imgui.UniqueInfo info, IContent content, Rect position, StyleElement style)
        {
            var result = false;
            var id = imgui.Id(info);
            imgui.CurrentId = id;

            if (position.Intersects(imgui.Clip))
            {
                result = imgui.ButtonBehaviour(id, position);

                var inner = imgui.Box(position, style, id);
                var styleState = style.Current(imgui.ActiveId, imgui.HotId, id);

                content.Draw(imgui, imgui.Batcher, styleState, inner);
            }

            return result;
        }
    }
}
