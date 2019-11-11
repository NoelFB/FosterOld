using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiLabel
    {
        public static void Label(this Imgui context, string label)
        {
            context.Label(label, label);
        }

        public static void Label(this Imgui context, Imgui.UniqueInfo identifier, string label)
        {
            context.Label(identifier, label, context.Cell(context.Style.ElementHeight));
        }

        public static void Label(this Imgui context, Imgui.UniqueInfo identifier, string label, Rect position)
        {
            if (position.Intersects(context.ActiveClip))
            {
                var scale = Vector2.One * context.Style.FontScale;

                context.Batcher.PushMatrix(new Vector2(position.X, position.Y + context.Style.ElementPadding), scale, Vector2.Zero, 0f);
                context.Batcher.Text(context.Style.Font, label, Color.White);
                context.Batcher.PopMatrix();
            }
        }
    }
}
