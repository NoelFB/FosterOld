using Foster.Framework;
using System;

namespace Foster.GuiSystem
{
    public struct Text : IContent
    {

        public string Value;
        public int Start;
        public int Length;

        public Text(string text, int start = 0, int length = int.MaxValue)
        {
            Value = text;
            Start = start;
            Length = Math.Max(0, Math.Min(text.Length, length) - start);
        }

        public void Draw(Imgui imgui, Batch2D batcher, StyleState style, Rect position)
        {
            var scale = Vector2.One * imgui.FontScale;
            var align = new Vector2(position.X, position.Center.Y - imgui.FontSize * 0.5f);
            
            batcher.PushMatrix(align, scale, Vector2.Zero, 0f);
            batcher.Text(imgui.Font, Value.AsSpan(Start, Length), style.ContentColor);
            batcher.PopMatrix();
        }

        public float Width(Imgui imgui)
        {
            return imgui.Font.WidthOf(Value.AsSpan(Start, Length)) * imgui.FontScale;
        }
        
        public float Height(Imgui imgui)
        {
            return imgui.FontSize;
        }

        public ImguiName UniqueInfo()
        {
            return Value;
        }

        public static implicit operator Text(string text) => new Text(text);

    }
}
