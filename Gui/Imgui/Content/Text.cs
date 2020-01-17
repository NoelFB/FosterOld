using Foster.Framework;

namespace Foster.GuiSystem
{
    public struct Text : IContent
    {

        public string Value;

        public Text(string text)
        {
            Value = text;
        }

        public void Draw(Imgui imgui, Batch2D batcher, StyleState style, Rect position)
        {
            var scale = Vector2.One * imgui.FontScale;
            var align = new Vector2(position.X, position.Center.Y - imgui.FontSize * 0.5f);

            batcher.PushMatrix(align, scale, Vector2.Zero, 0f);
            batcher.Text(imgui.Font, Value, style.ContentColor);
            batcher.PopMatrix();
        }

        public float Width(Imgui imgui)
        {
            return imgui.Font.WidthOf(Value) * imgui.FontScale;
        }
        
        public float Height(Imgui imgui)
        {
            return imgui.FontSize;
        }

        public Imgui.Name UniqueInfo()
        {
            return Value;
        }

        public static implicit operator Text(string text) => new Text(text);

    }
}
