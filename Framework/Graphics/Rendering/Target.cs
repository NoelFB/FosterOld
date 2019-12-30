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
    public class Target
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

        /// <summary>
        /// Depth Attachment
        /// </summary>
        public readonly Texture? Depth;

        public Target(int width, int height) : this(width, height, new [] { TextureFormat.Color }, TextureFormat.DepthStencil)
        {

        }

        public Target(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat) : this(App.Graphics, width, height, colorAttachmentFormats, depthFormat)
        {
            
        }

        public Target(Graphics graphics, int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            Width = width;
            Height = height;

            // check attachment types
            for (int i = 0; i < colorAttachmentFormats.Length; i++)
                if (!colorAttachmentFormats[i].IsTextureFormat())
                    throw new Exception("Invalid Texture Format - Color Texture Attachments must be a Color Format");

            // create internal target
            Internal = graphics.CreateTarget(width, height, colorAttachmentFormats, depthFormat);

            // assign color attachments
            var attachments = new List<Texture>();
            for (int i = 0; i < colorAttachmentFormats.Length; i++)
                attachments.Add(new Texture(graphics, Internal.attachments[i], width, height, colorAttachmentFormats[i]));
            Attachments = new ReadOnlyCollection<Texture>(attachments);

            // assign depth buffer
            if (depthFormat != TextureFormat.None)
            {
                if (!depthFormat.IsDepthStencilFormat())
                    throw new Exception("Invalid Texture Format - Depth Texture must be Depth24 of Depth24Stencil8");

                Depth = new Texture(graphics, Internal.depth, width, height, depthFormat);
            }
        }

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
