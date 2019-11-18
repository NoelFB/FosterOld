using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiLabel
    {
        public static void Label(this Imgui context, string label)
        {
            context.Label(label, label);
        }

        public static void Label(this Imgui context, Imgui.UniqueInfo identifier, string label)
        {
            context.Label(identifier, label, context.Cell(context.Style.ItemHeight));
        }

        public static void Label(this Imgui context, Imgui.UniqueInfo identifier, string label, Rect position)
        {
            if (position.Intersects(context.Clip))
            {
                var scale = Vector2.One * context.Style.FontScale;

                context.Batcher.PushMatrix(new Vector2(position.X, position.Y + context.Style.ItemPadding.Y), scale, Vector2.Zero, 0f);
                context.Batcher.Text(context.Style.Font, label, Color.White);
                context.Batcher.PopMatrix();
            }
        }
    }
}
