using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiTitle
    {
        public static void Title(this Imgui imgui, string label)
        {
            imgui.Title(label, label);
        }

        public static void Title(this Imgui imgui, ImguiName identifier, string label)
        {
            var style = imgui.Style;
            var height = imgui.FontSize * style.TitleScale + style.Generic.Idle.Padding.Y * 2f;

            imgui.Title(identifier, label, imgui.Cell(Size.Fill(), Size.Explicit(height)));
        }

        public static void Title(this Imgui imgui, ImguiName identifier, string label, Rect position)
        {
            var scale = Vector2.One * imgui.FontScale * imgui.Style.TitleScale;

            imgui.Batcher.PushMatrix(new Vector2(position.X, position.Y + imgui.Style.Generic.Idle.Padding.Y), scale, Vector2.Zero, 0f);
            imgui.Batcher.Text(imgui.Font, label, imgui.Style.TitleColor);
            imgui.Batcher.PopMatrix();
            imgui.Batcher.Rect(position.X, position.Bottom - 4, position.Width, 4, imgui.Style.TitleColor);
        }
    }
}
