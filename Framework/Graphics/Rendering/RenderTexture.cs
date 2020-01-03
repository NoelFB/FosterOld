using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Render Target
    /// </summary>
    public abstract class RenderTexture : RenderTarget
    {
        protected readonly List<Texture> attachments = new List<Texture>();
        private readonly int width;
        private readonly int height;

        /// <summary>
        /// Color Attachments
        /// </summary>
        public readonly ReadOnlyCollection<Texture> Attachments;

        /// <summary>
        /// Depth Attachment
        /// </summary>
        public Texture? Depth { get; protected set; }

        /// <summary>
        /// Render Target Width
        /// </summary>
        public override int DrawableWidth => width;

        /// <summary>
        /// Render Target Height
        /// </summary>
        public override int DrawableHeight => height;

        public static RenderTexture Create(int width, int height)
        {
            return App.Graphics.CreateRenderTexture(width, height, new[] { TextureFormat.Color }, TextureFormat.DepthStencil);
        }

        public static RenderTexture Create(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            return App.Graphics.CreateRenderTexture(width, height, colorAttachmentFormats, depthFormat);
        }

        public static RenderTexture Create(Graphics graphics, int width, int height)
        {
            return graphics.CreateRenderTexture(width, height, new[] { TextureFormat.Color }, TextureFormat.DepthStencil);
        }

        public static RenderTexture Create(Graphics graphics, int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            return graphics.CreateRenderTexture(width, height, colorAttachmentFormats, depthFormat);
        }

        protected RenderTexture(int width, int height)
        {
            this.width = width;
            this.height = height;

            Attachments = new ReadOnlyCollection<Texture>(attachments);
            Viewport = new RectInt(0, 0, width, height);
            Drawable = true;
        }

        public static implicit operator Texture(RenderTexture target) => target.Attachments[0];
    }
}
