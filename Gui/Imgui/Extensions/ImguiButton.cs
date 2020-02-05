using Foster.Framework;

namespace Foster.GUI
{
    public static class ImguiButton
    {
        public static bool Button(this Imgui imgui, string label)
        {
            var content = new Text(label);
            return Button(imgui, content.UniqueInfo(), content, Size.Fill(), Size.Preferred(), imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, string label, Size width, Size height)
        {
            var content = new Text(label);
            return Button(imgui, content.UniqueInfo(), content, width, height, imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, IContent content)
        {
            return Button(imgui, content.UniqueInfo(), content, Size.Fill(), Size.Preferred(), imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, IContent content, Size width, Size height)
        {
            return Button(imgui, content.UniqueInfo(), content, width, height, imgui.Style.Generic);
        }

        public static bool Button(this Imgui imgui, ImguiName info, IContent content, Size width, Size height, StyleElement style)
        {
            var position = imgui.Cell(width, height, content, style.Idle.Padding);
            return Button(imgui, info, content, position, style);
        }

        public static bool Button(this Imgui imgui, ImguiName info, IContent content, Rect position, StyleElement style)
        {
            var result = false;
            var id = imgui.Id(info);

            if (position.Overlaps(imgui.Clip))
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
