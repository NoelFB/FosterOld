using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Foster.Framework.Internal;

namespace Foster.Framework
{
    public class Target : GraphicsResource
    {

        public readonly InternalTarget Internal;

        /// <summary>
        /// Gets the Width of the Texture
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Gets the Height of the Texture
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Color Attachments
        /// </summary>
        public readonly ReadOnlyCollection<Texture> Attachments;

        public Target(int width, int height, int textures = 1, bool depthBuffer = false) : this(App.Graphics, width, height, textures, depthBuffer)
        {
            
        }

        public Target(Graphics graphics, int width, int height, int textures = 1, bool depthBuffer = false) : base(graphics)
        {
            Width = width;
            Height = height;

            Internal = graphics.CreateTarget(width, height, textures, depthBuffer);

            var attachments = new List<Texture>();
            foreach (var attachment in Internal.attachments)
                attachments.Add(new Texture(graphics, attachment, width, height));

            Attachments = new ReadOnlyCollection<Texture>(attachments);
        }

        /// <summary>
        /// Sets the Texture Color data from the given buffer
        /// </summary>
        /// <param name="buffer"></param>
        public void SetColor(int attachment, Memory<Color> buffer) => Attachments[attachment].Internal.SetColor(buffer);

        /// <summary>
        /// Writes the Texture Color data to the given buffer
        /// </summary>
        /// <param name="buffer"></param>
        public void GetColor(int attachment, Memory<Color> buffer) => Attachments[attachment].Internal.GetColor(buffer);


        public static implicit operator Texture(Target target) => target.Attachments[0];

        public override void Dispose()
        {
            if (!Disposed)
            {
                base.Dispose();

                foreach (var attachment in Attachments)
                    attachment.Dispose();

                Internal.Dispose();
            }
        }
    }
}
