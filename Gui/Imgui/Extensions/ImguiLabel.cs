using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiLabel
    {
        public static void Label(this Imgui imgui, string label)
        {
            Label(imgui, label, Size.Preferred(), Size.Preferred(), imgui.Style.Label);
        }

        public static void Label(this Imgui imgui, string label, Size width, Size height)
        {
            Label(imgui, label, width, height, imgui.Style.Label);
        }

        public static void Label(this Imgui imgui, string label, Size width, Size height, StyleState style)
        {
            var content = new Text(label);
            var position = imgui.Cell(width, height, content, style.Padding);

            Label(imgui, content, position, style);
        }

        public static void Label(this Imgui imgui, IContent label, Rect position, StyleState style)
        {
            if (position.Overlaps(imgui.Clip))
            {
                var inner = position.Inflate(-style.Padding.X, -style.Padding.Y, -style.Padding.X, -style.Padding.Y);
                label.Draw(imgui, imgui.Batcher, style, inner);
            }
        }
    }
}
