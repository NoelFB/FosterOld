using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Foster.Framework.Internal;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Render Target
    /// </summary>
    public class Target : IDisposable
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

        public Target(int width, int height, int attachmentCount = 1, DepthFormat depthFormat = DepthFormat.None) : this(App.Graphics, width, height, attachmentCount, depthFormat)
        {
            
        }

        public Target(Graphics graphics, int width, int height, int attachmentCount = 1, DepthFormat depthFormat = DepthFormat.None)
        {
            Width = width;
            Height = height;

            Internal = graphics.CreateTarget(width, height, attachmentCount, depthFormat);

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

        /// <summary>
        /// Disposes the internal target resources
        /// </summary>
        public void Dispose()
        {
            foreach (var attachment in Attachments)
                attachment.Dispose();

            Internal.Dispose();
        }

        public static implicit operator Texture(Target target) => target.Attachments[0];
    }
}
