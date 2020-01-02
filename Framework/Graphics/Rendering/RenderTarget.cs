using Foster.Framework.Internal;
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
        /// The Target Render State
        /// </summary>
        public RenderState RenderState;

        /// <summary>
        /// Whether the Render Target can be drawn to
        /// </summary>
        public bool Drawable { get; protected set; }

        /// <summary>
        /// Creates a Target of the given size
        /// </summary>
        public RenderTarget()
        {
            RenderState = new RenderState(this);
        }

        /// <summary>
        /// Clears the Color of the Target
        /// </summary>
        public void Clear(Color color) => ClearTarget(ClearFlags.Color, color, 0, 0);

        /// <summary>
        /// Clears the Target
        /// </summary>
        public void Clear(Color color, float depth, int stencil) => ClearTarget(ClearFlags.All, color, depth, stencil);

        public void Clear(ClearFlags flags, Color color, float depth, int stencil)
        {
            if (!Drawable)
                throw new Exception("Render Target cannot currently be drawn to");

            ClearTarget(flags, color, depth, stencil);
        }

        /// <summary>
        /// Clears the Target
        /// </summary>
        protected abstract void ClearTarget(ClearFlags flags, Color color, float depth, int stencil);

    }
}
