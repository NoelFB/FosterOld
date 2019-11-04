using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public static class ImguiButton
    {
        public static bool ButtonBehaviour(this ImguiContext context, ImguiContext.ID id, Rect position)
        {
            var performPress = false;

            if (context.Scissor.Contains(context.Mouse) && position.Contains(context.Mouse))
                context.HotId = id;

            if (context.HotId == id && App.Input.Mouse.Pressed(MouseButtons.Left))
                context.ActiveId = id;

            if (context.ActiveId == id && App.Input.Mouse.Released(MouseButtons.Left))
            {
                if (context.HotId == id)
                    performPress = true;
                context.ActiveId = ImguiContext.ID.None;
            }

            return performPress;
        }

        public static bool Button(this ImguiContext context, string label, float width = 0f, float height = 0f)
        {
            return Button(context, label, label, width, height);
        }

        public static bool Button(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label, float width = 0f, float height = 0f)
        {
            var style = context.Style;

            if (width == ImguiContext.PreferredSize)
                width = style.Font.WidthOf(label) * style.FontScale + style.ElementPadding * 2f;
            if (height == 0f)
                height = style.FontSize + style.ElementPadding * 2;

            return Button(context, identifier, label, context.Cell(width, height));
        }

        public static bool Button(this ImguiContext context, ImguiContext.UniqueInfo identifier, string label, Rect position)
        {
            var result = false;

            if (position.Intersects(context.Scissor))
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

                context.Batch.Rect(position, color);
                context.Batch.PushMatrix(new Vector2(position.X + style.ElementPadding, position.Y + style.ElementPadding), scale, Vector2.Zero, 0f);
                context.Batch.Text(style.Font, label, Color.Black);
                context.Batch.PopMatrix();
            }

            return result;
        }
    }
}
