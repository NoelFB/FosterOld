using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class RenderTarget : GraphicsResource
    {

        /// <summary>
        /// The Width of the Render Target
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// The Height of the Render Target
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// The Render State Viewport
        /// </summary>
        public RectInt Viewport;

        /// <summary>
        /// Whether the Render Target can be drawn to
        /// </summary>
        public bool Drawable { get; protected set; }

        /// <summary>
        /// Orthographic Matrix based on the Viewport of this Render Target
        /// </summary>
        public Matrix OrthographicMatrix =>
            Matrix.CreateScale((1.0f / Viewport.Width) * 2, -(1.0f / Viewport.Height) * 2, 1f) *
            Matrix.CreateTranslation(-1.0f, 1.0f, 0f);

        /// <summary>
        /// Clears the Color of the Target
        /// </summary>
        public void Clear(Color color) => Clear(ClearFlags.Color, color, 0, 0);

        /// <summary>
        /// Clears the Target
        /// </summary>
        public void Clear(Color color, float depth, int stencil) => Clear(ClearFlags.All, color, depth, stencil);

        /// <summary>
        /// Clears the Target
        /// </summary>
        public void Clear(ClearFlags flags, Color color, float depth, int stencil)
        {
            if (!Drawable)
                throw new Exception("Render Target cannot currently be drawn to");

            ClearInternal(flags, color, depth, stencil);
        }

        /// <summary>
        /// Clears the Target
        /// </summary>
        protected abstract void ClearInternal(ClearFlags flags, Color color, float depth, int stencil);

    }
}
