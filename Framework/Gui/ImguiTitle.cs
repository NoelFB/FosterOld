using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiTitle
    {
        public static void Title(this Imgui context, string label)
        {
            context.Title(label, label);
        }

        public static void Title(this Imgui context, Imgui.UniqueInfo identifier, string label)
        {
            var style = context.Style;
            var height = style.FontSize * style.TitleScale + style.ItemPadding.Y * 2f;

            context.Title(identifier, label, context.Cell(height));
        }

        public static void Title(this Imgui context, Imgui.UniqueInfo identifier, string label, Rect position)
        {
            var style = context.Style;
            var scale = Vector2.One * style.FontScale * style.TitleScale;

            context.Batcher.PushMatrix(new Vector2(position.X, position.Y + style.ItemPadding.Y), scale, Vector2.Zero, 0f);
            context.Batcher.Text(style.Font, label, Color.White);
            context.Batcher.PopMatrix();
            context.Batcher.Rect(position.X, position.Bottom - 4, position.Width, 4, Color.White);
        }
    }
}
