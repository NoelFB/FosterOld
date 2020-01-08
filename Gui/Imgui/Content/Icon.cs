using Foster.Framework;
using System;

namespace Foster.GuiSystem
{
    public struct Icon : IContent
    {

        public Subtexture Image;
        public bool UseContentColor;

        public Icon(Subtexture image, bool useContentColor = false)
        {
            Image = image;
            UseContentColor = useContentColor;
        }

        public void Draw(Imgui imgui, Batch2D batcher, StyleState style, Rect position)
        {
            var scale = Vector2.One * Math.Min(imgui.FontSize / Image.Width, imgui.FontSize / Image.Height);
            var pos = new Vector2(position.Center.X, position.Center.Y);
            var color = (UseContentColor ? style.ContentColor : Color.White);
            var origin = new Vector2(Image.Width, Image.Height) / 2f;

            batcher.Image(Image, pos, scale, origin, 0f, color);
        }

        public Vector2 PreferredSize(Imgui imgui)
        {
            return new Vector2(imgui.FontSize, imgui.FontSize);
        }

        public Imgui.Name UniqueInfo()
        {
            return Image.GetHashCode();
        }
    }
}
