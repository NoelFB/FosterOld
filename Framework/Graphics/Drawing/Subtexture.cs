using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Foster.Framework
{
    /// <summary>
    /// A Subtexture, representing a rectangular segment of a Texture
    /// </summary>
    public class Subtexture
    {
        /// <summary>
        /// The Texture coordinates. These are set automatically based on the Source rectangle
        /// </summary>
        public readonly Vector2[] TexCoords = new Vector2[4];

        /// <summary>
        /// The draw coordinates. These are set automatically based on the Source and Frame rectangle
        /// </summary>
        public readonly Vector2[] DrawCoords = new Vector2[4];

        /// <summary>
        /// The Texture this Subtexture is... a subtexture of
        /// </summary>
        public Texture? Texture
        {
            get => texture;
            set
            {
                if (texture != value)
                {
                    texture = value;
                    UpdateCoords();
                }
            }
        }

        /// <summary>
        /// The source rectangle to sample from the Texture
        /// </summary>
        public Rect Source
        {
            get => source;
            set
            {
                source = value;
                UpdateCoords();
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
                frame = value;
                UpdateCoords();
            }
        }

        /// <summary>
        /// The Draw Width of the Subtexture
        /// </summary>
        public float Width => frame.Width;

        /// <summary>
        /// The Draw Height of the Subtexture
        /// </summary>
        public float Height => frame.Height;

        private Texture? texture;
        private Rect frame;
        private Rect source;

        public Subtexture()
        {

        }

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
            this.texture = texture;
            this.source = source;
            this.frame = frame;

            UpdateCoords();
        }

        public void Reset(Texture texture, Rect source, Rect frame)
        {
            this.texture = texture;
            this.source = source;
            this.frame = frame;

            UpdateCoords();
        }

        public (Rect Source, Rect Frame) GetClip(Rect clip)
        {
            return GetClip(clip.X, clip.Y, clip.Width, clip.Height);
        }

        public (Rect Source, Rect Frame) GetClip(float x, float y, float w, float h)
        {
            var offset = new Vector2(-frame.X, -frame.Y);
            var crop = new Rect();

            if (x < 0)
            {
                offset.X -= x;
                crop.X = source.X;
                crop.Width = Math.Max(0, Math.Min(source.Width, w + x + frame.X));
            }
            else if (x <= -frame.X)
            {
                offset.X = -frame.X - x;
                crop.X = source.X;
                crop.Width = Math.Max(0, Math.Min(source.Width, w - x + frame.X));
            }
            else
            {
                offset.X = 0;
                crop.X = source.X + (x + frame.X);
                crop.Width = Math.Max(0, Math.Min(source.Width - x - frame.X, w));
            }

            if (y < 0)
            {
                offset.Y -= y;
                crop.Y = source.Y;
                crop.Height = Math.Max(0, Math.Min(source.Height, h + y + frame.Y));
            }
            else if (y <= -frame.Y)
            {
                offset.Y = -frame.Y - y;
                crop.Y = source.Y;
                crop.Height = Math.Max(0, Math.Min(source.Height, h - y + frame.Y));
            }
            else
            {
                offset.Y = 0;
                crop.Y = source.Y + (y + frame.Y);
                crop.Height = Math.Max(0, Math.Min(source.Height - y - frame.Y, h));
            }

            return (crop, new Rect(-offset.X, -offset.Y, w, h));
        }

        public Subtexture GetClipSubtexture(Rect clip)
        {
            var (source, frame) = GetClip(clip);
            return new Subtexture(Texture!, source, frame);
        }

        private void UpdateCoords()
        {
            DrawCoords[0].X = -frame.X;
            DrawCoords[0].Y = -frame.Y;
            DrawCoords[1].X = -frame.X + source.Width;
            DrawCoords[1].Y = -frame.Y;
            DrawCoords[2].X = -frame.X + source.Width;
            DrawCoords[2].Y = -frame.Y + source.Height;
            DrawCoords[3].X = -frame.X;
            DrawCoords[3].Y = -frame.Y + source.Height;

            if (texture != null)
            {
                var tx0 = source.X / texture.Width;
                var ty0 = source.Y / texture.Height;
                var tx1 = source.MaxX / texture.Width;
                var ty1 = source.MaxY / texture.Height;

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
}
