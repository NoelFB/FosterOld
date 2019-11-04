using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiTitle
    {
        public static void Title(this ImguiContext context, string label)
        {
            context.Title(label, label);
        }

        public static void Title(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label)
        {
            var style = context.Style;
            var height = style.FontSize * style.TitleScale + style.ElementPadding * 2f;

            context.Title(identifier, label, context.Cell(height));
        }

        public static void Title(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label, Rect position)
        {
            var style = context.Style;
            var scale = Vector2.One * style.FontScale * style.TitleScale;

            context.Batch.PushMatrix(new Vector2(position.X, position.Y + style.ElementPadding), scale, Vector2.Zero, 0f);
            context.Batch.Text(style.Font, label, Color.White);
            context.Batch.PopMatrix();
            context.Batch.Rect(position.X, position.Bottom - 4, position.Width, 4, Color.White);
        }
    }
}
