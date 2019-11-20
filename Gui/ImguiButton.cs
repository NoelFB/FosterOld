using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiButton
    {
        public static bool GrabbingBehaviour(this Imgui context, Imgui.ID id, Rect position)
        {
            if (context.MouseOver(id, position))
                context.HotId = id;

            if (context.LastHotId == id && App.Input.Mouse.LeftPressed)
                context.ActiveId = id;

            if (context.ActiveId == id && App.Input.Mouse.LeftReleased)
                context.ActiveId = Imgui.ID.None;

            return context.ActiveId == id;
        }

        public static bool ButtonBehaviour(this Imgui context, Imgui.ID id, Rect position)
        {
            var performPress = false;

            if (context.MouseOver(id, position))
                context.HotId = id;

            if (context.LastHotId == id && App.Input.Mouse.LeftPressed)
                context.ActiveId = id;

            if (context.ActiveId == id && App.Input.Mouse.LeftReleased)
            {
                if (context.HotId == id)
                    performPress = true;
                context.ActiveId = Imgui.ID.None;
            }

            return performPress;
        }



        public static bool Button(this Imgui context, Imgui.UniqueInfo identifier, string label, Rect position)
        {
            return Button(context, context.Id(identifier), label, position);
        }

        public static bool Button(this Imgui context, string label, float width = 0f, float height = 0f)
        {
            return Button(context, label, label, width, height);
        }

        public static bool Button(this Imgui context, Imgui.UniqueInfo identifier, string label, float width = 0f, float height = 0f)
        {
            return Button(context, context.Id(identifier), label, width, height);
        }

        public static bool Button(this Imgui context, Imgui.ID id, string label, float width = 0f, float height = 0f)
        {
            var style = context.Style;

            if (width == Imgui.PreferredSize)
                width = style.Font.WidthOf(label) * style.FontScale + style.ItemPadding.X * 2f;
            if (height == 0f)
                height = style.FontSize + style.ItemPadding.X * 2;

            return Button(context, id, label, context.Cell(width, height));
        }

        public static bool Button(this Imgui context, Imgui.ID id, string label, Rect position)
        {
            var result = false;
            context.CurrentId = id;

            if (position.Intersects(context.Clip))
            {
                var style = context.Style;
                var scale = Vector2.One * style.FontScale;
                var element = style.ItemIdle;

                result = context.ButtonBehaviour(id, position);

                if (context.ActiveId == id)
                {
                    element = style.ItemActive;
                }
                else if (context.HotId == id)
                {
                    element = style.ItemHot;
                }

                if (context.Batcher != null)
                {
                    context.Box(position, element.BorderRadius, element.BorderWeight, element.BorderColor, element.BackgroundColor);

                    var left = position.X + style.ItemPadding.X + element.BorderWeight.Left;
                    var middle = position.Y + element.BorderWeight.Top + (position.Height - element.BorderWeight.Height) * 0.5f - style.FontSize * 0.5f;

                    context.Batcher.PushMatrix(new Vector2(left, middle), scale, Vector2.Zero, 0f);
                    context.Batcher.Text(style.Font, label, element.ContentColor);
                    context.Batcher.PopMatrix();
                }
            }

            return result;
        }
    }
}
