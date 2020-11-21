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

        public (Rect Source, Rect Frame) GetClip(in Rect clip)
        {
            (Rect Source, Rect Frame) result;

            result.Source = (clip + Source.Position + Frame.Position).OverlapRect(Source);

            result.Frame.X = MathF.Min(0, Frame.X + clip.X);
            result.Frame.Y = MathF.Min(0, Frame.Y + clip.Y);
            result.Frame.Width = clip.Width;
            result.Frame.Height = clip.Height;

            return result;
        }

        public (Rect Source, Rect Frame) GetClip(float x, float y, float w, float h)
        {
            return GetClip(new Rect(x, y, w, h));
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
                var tx1 = source.Right / texture.Width;
                var ty1 = source.Bottom / texture.Height;

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
