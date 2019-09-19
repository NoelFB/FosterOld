using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Subtexture
    {

        public Texture Texture;

        /// <summary>
        /// The Texture coordinates, stored in a clock-wise order starting from top-left
        /// </summary>
        public readonly Vector2[] TexCoords = new Vector2[4];

        /// <summary>
        /// The draw coordinates. These are set automatically based on the Width, Height, and Frame properties.
        /// </summary>
        public readonly Vector2[] DrawCoords = new Vector2[4];

        /// <summary>
        /// The width to draw at
        /// </summary>
        public float Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    UpdateDrawCoords();
                }
            }
        }

        /// <summary>
        /// The height to draw at
        /// </summary>
        public float Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    UpdateDrawCoords();
                }
            }
        }

        /// <summary>
        /// The frame of the Subtexture. This is useful if you trim transparency and want to store the original size of the image
        /// For example, if the original image was (64, 64), but the trimmed version is (32, 48), the Frame may be (-16, -8, 64, 64)
        /// </summary>
        public Rect Frame
        {
            get => frame;
            set
            {
                if (frame != value)
                {
                    frame = value;
                    UpdateDrawCoords();
                }
            }
        }

        private float width;
        private float height;
        private Rect frame;

        public Subtexture(Texture texture) 
            : this(texture, new Rect(0, 0, texture.Width, texture.Height))
        {

        }

        public Subtexture(Texture texture, Rect source) 
            : this(texture, source, new Rect(0, 0, source.Width, source.Height))
        {

        }

        public Subtexture(Texture texture, Rect source, Rect frame)
        {
            Texture = texture;
            width = source.Width;
            height = source.Height;
            this.frame = frame;

            UpdateTexCoords(source);
            UpdateDrawCoords();
        }

        /// <summary>
        /// Sets all properties of the Subtexture based on the Source Rect
        /// </summary>
        /// <param name="source"></param>
        public void SetRect(Rect source)
        {
            width = source.Width;
            height = source.Height;
            frame = new Rect(0, 0, source.Width, source.Height);

            UpdateTexCoords(source);
            UpdateDrawCoords();
        }

        /// <summary>
        /// Sets all properties of the Subtexture based on the Source Rect and Frame rect
        /// </summary>
        /// <param name="source"></param>
        public void SetRect(Rect source, Rect frame)
        {
            UpdateTexCoords(source);

            width = source.Width;
            height = source.Height;
            this.frame = frame;

            UpdateDrawCoords();
        }

        private void UpdateTexCoords(Rect source)
        {
            var tx0 = source.X / Texture.Width;
            var ty0 = source.Y / Texture.Height;
            var tx1 = source.Right / Texture.Width;
            var ty1 = source.Bottom / Texture.Height;

            TexCoords[0].X = tx0;
            TexCoords[0].Y = ty0;
            TexCoords[1].X = tx1;
            TexCoords[1].Y = ty0;
            TexCoords[2].X = tx1;
            TexCoords[2].Y = ty1;
            TexCoords[3].X = tx0;
            TexCoords[3].Y = ty1;
        }

        private void UpdateDrawCoords()
        {
            DrawCoords[0].X = -frame.X;
            DrawCoords[0].Y = -frame.Y;
            DrawCoords[1].X = -frame.X + Width;
            DrawCoords[1].Y = -frame.Y;
            DrawCoords[2].X = -frame.X + Width;
            DrawCoords[2].Y = -frame.Y + Height;
            DrawCoords[3].X = -frame.X;
            DrawCoords[3].Y = -frame.Y + Height;
        }

    }
}
