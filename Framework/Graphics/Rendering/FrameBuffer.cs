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
            protected internal abstract void Resize(int width, int height);
            protected internal abstract void Dispose();
        }

        /// <summary>
        /// A reference to the internal platform implementation of the FrameBuffer
        /// </summary>
        public Platform Implementation { get; private set; }

        /// <summary>
        /// Texture Attachments
        /// </summary>
        public ReadOnlyCollection<Texture> Attachments { get; private set; }

        /// <summary>
        /// Render Target Width
        /// </summary>
        public override int RenderWidth => width;

        /// <summary>
        /// Render Target Height
        /// </summary>
        public override int RenderHeight => height;

        private int width;
        private int height;

        public FrameBuffer(int width, int height)
            : this(App.Graphics, width, height)
        {

        }

        public FrameBuffer(Graphics graphics, int width, int height) 
            : this(graphics, width, height, TextureFormat.Color)
        {

        }

        public FrameBuffer(Graphics graphics, int width, int height, params TextureFormat[] attachments)
        {
            this.width = width;
            this.height = height;

            if (width <= 0 || height <= 0)
                throw new Exception("FrameBuffer must have a size larger than 0");

            Implementation = graphics.CreateFrameBuffer(width, height, attachments);
            Attachments = new ReadOnlyCollection<Texture>(Implementation.Attachments);
            Renderable = true;
        }

        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("FrameBuffer must have a size larger than 0");

            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;

                Implementation.Resize(width, height);
            }
        }

        public void Dispose()
        {
            foreach (var texture in Attachments)
                texture.Dispose();

            Implementation.Dispose();
        }

        public static implicit operator Texture(FrameBuffer target) => target.Attachments[0];
    }
}
