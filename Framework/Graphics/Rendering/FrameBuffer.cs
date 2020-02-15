using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D buffer that can be drawn to
    /// </summary>
    public class FrameBuffer : RenderTarget, IDisposable
    {

        public abstract class Platform
        {
            protected internal readonly List<Texture> Attachments = new List<Texture>();
            protected internal Texture? Depth;

            protected internal abstract void Dispose();
        }

        /// <summary>
        /// A reference to the internal platform implementation of the FrameBuffer
        /// </summary>
        public readonly Platform Implementation;

        /// <summary>
        /// Color Attachments
        /// </summary>
        public readonly ReadOnlyCollection<Texture> Attachments;

        /// <summary>
        /// Depth Attachment
        /// </summary>
        public Texture? Depth => Implementation.Depth;

        /// <summary>
        /// Render Target Width
        /// </summary>
        public override int RenderWidth => width;

        /// <summary>
        /// Render Target Height
        /// </summary>
        public override int RenderHeight => height;

        private readonly int width;
        private readonly int height;

        public FrameBuffer(int width, int height)
            : this(App.Graphics, width, height)
        {

        }

        public FrameBuffer(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat) 
            : this(App.Graphics, width, height, colorAttachmentFormats, depthFormat)
        {

        }

        public FrameBuffer(Graphics graphics, int width, int height) 
            : this(graphics, width, height, new[] { TextureFormat.Color }, TextureFormat.None)
        {

        }

        public FrameBuffer(Graphics graphics, int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            this.width = width;
            this.height = height;

            Implementation = graphics.CreateFrameBuffer(width, height, colorAttachmentFormats, depthFormat);
            Attachments = new ReadOnlyCollection<Texture>(Implementation.Attachments);
            Renderable = true;
        }

        public void Dispose()
        {
            Implementation.Dispose();
        }

        public static implicit operator Texture(FrameBuffer target) => target.Attachments[0];
    }
}
