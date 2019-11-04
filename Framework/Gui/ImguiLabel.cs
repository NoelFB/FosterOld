using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiLabel
    {
        public static void Label(this ImguiContext context, string label)
        {
            context.Label(label, label);
        }

        public static void Label(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label)
        {
            context.Label(identifier, label, context.Cell(context.Style.ElementHeight));
        }

        public static void Label(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label, Rect position)
        {
            var scale = Vector2.One * context.Style.FontScale;

            context.Batch.PushMatrix(new Vector2(position.X, position.Y + context.Style.ElementPadding), scale, Vector2.Zero, 0f);
            context.Batch.Text(context.Style.Font, label, Color.White);
            context.Batch.PopMatrix();
        }
    }
}
