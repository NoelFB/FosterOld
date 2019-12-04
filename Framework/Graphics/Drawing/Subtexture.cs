namespace Foster.Framework
{
    public class Subtexture
    {

        public readonly Texture Texture;

        /// <summary>
        /// The Texture coordinates. These are set automatically based on the Source rectangle
        /// </summary>
        public readonly Vector2[] TexCoords = new Vector2[4];

        /// <summary>
        /// The draw coordinates. These are set automatically based on the Draw rectangle
        /// </summary>
        public readonly Vector2[] DrawCoords = new Vector2[4];

        /// <summary>
        /// The source rectangle to sample from the Texture
        /// </summary>
        public Rect SourceRect
        {
            get => sourceRect;
            set
            {
                if (sourceRect != value)
                {
                    sourceRect = value;

                    var tx0 = sourceRect.X / Texture.Width;
                    var ty0 = sourceRect.Y / Texture.Height;
                    var tx1 = sourceRect.Right / Texture.Width;
                    var ty1 = sourceRect.Bottom / Texture.Height;

                    TexCoords[0].X = tx0;
                    TexCoords[0].Y = ty0;
                    TexCoords[1].X = tx1;
                    TexCoords[1].Y = ty0;
                    TexCoords[2].X = tx1;
                    TexCoords[2].Y = ty1;
                    TexCoords[3].X = tx0;
                    TexCoords[3].Y = ty1;
                }
            }
        }

        /// <summary>
        /// The rectangle to draw to the screen
        /// </summary>
        public Rect DrawRect
        {
            get => drawRect;
            set
            {
                if (drawRect != value)
                {
                    drawRect = value;

                    DrawCoords[0].X = drawRect.X;
                    DrawCoords[0].Y = drawRect.Y;
                    DrawCoords[1].X = drawRect.X + drawRect.Width;
                    DrawCoords[1].Y = drawRect.Y;
                    DrawCoords[2].X = drawRect.X + drawRect.Width;
                    DrawCoords[2].Y = drawRect.Y + drawRect.Height;
                    DrawCoords[3].X = drawRect.X;
                    DrawCoords[3].Y = drawRect.Y + drawRect.Height;
                }
            }
        }

        /// <summary>
        /// The frame of the Subtexture. This is useful if you trim transparency and want to store the original size of the image
        /// For example, if the original image was (64, 64), but the trimmed version is (32, 48), the Frame may be (-16, -8, 64, 64)
        /// </summary>
        public Rect FrameRect;

        /// <summary>
        /// The Draw Width of the Subtexture
        /// </summary>
        public float Width => DrawRect.Width;

        /// <summary>
        /// The Draw Height of the Subtexture
        /// </summary>
        public float Height => DrawRect.Height;

        private Rect drawRect;
        private Rect sourceRect;

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
            SourceRect = source;
            DrawRect = new Rect(-frame.X, -frame.Y, source.Width, source.Height);
            FrameRect = frame;
        }

    }
}
