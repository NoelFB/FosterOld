using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiButton
    {
        public static bool ButtonBehaviour(this Imgui context, Imgui.ID id, Rect position)
        {
            var performPress = false;

            if (context.MouseOver(id, position))
                context.HotId = id;

            if (context.HotId == id && App.Input.Mouse.Pressed(MouseButtons.Left))
                context.ActiveId = id;

            if (context.ActiveId == id && App.Input.Mouse.Released(MouseButtons.Left))
            {
                if (context.HotId == id)
                    performPress = true;
                context.ActiveId = Imgui.ID.None;
            }

            return performPress;
        }

        public static bool Button(this Imgui context, string label, float width = 0f, float height = 0f)
        {
            return Button(context, label, label, width, height);
        }

        public static bool Button(this Imgui context, Imgui.UniqueInfo identifier, string label, float width = 0f, float height = 0f)
        {
            var style = context.Style;

            if (width == Imgui.PreferredSize)
                width = style.Font.WidthOf(label) * style.FontScale + style.ElementPadding * 2f;
            if (height == 0f)
                height = style.FontSize + style.ElementPadding * 2;

            return Button(context, identifier, label, context.Cell(width, height));
        }

        public static bool Button(this Imgui context, Imgui.UniqueInfo identifier, string label, Rect position)
        {
            var result = false;

            if (position.Intersects(context.ActiveClip))
            {
                var style = context.Style;
                var id = context.Id(identifier);
                var scale = Vector2.One * style.FontScale;
                var color = Color.White;

                result = context.ButtonBehaviour(id, position);

                if (context.ActiveId == id)
                {
                    color = Color.Red;
                }
                else if (context.HotId == id)
                {
                    color = Color.Yellow;
                }

                if (context.Batcher != null)
                {
                    context.Batcher.Rect(position, color);
                    context.Batcher.PushMatrix(new Vector2(position.X + style.ElementPadding, position.Y + style.ElementPadding), scale, Vector2.Zero, 0f);
                    context.Batcher.Text(style.Font, label, Color.Black);
                    context.Batcher.PopMatrix();
                }
            }

            return result;
        }
    }
}
