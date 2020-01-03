using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// An Object that can be rendered to (ex. a RenderTexture or a Window)
    /// </summary>
    public abstract class RenderTarget
    {
        /// <summary>
        /// The Width of the Render Target
        /// </summary>
        public abstract int DrawableWidth { get; }

        /// <summary>
        /// The Height of the Render Target
        /// </summary>
        public abstract int DrawableHeight { get; }

        /// <summary>
        /// The Render State Viewport
        /// </summary>
        public RectInt Viewport;

        /// <summary>
        /// Whether the Render Target can be drawn to.
        /// </summary>
        public bool Drawable { get; internal protected set; }

        /// <summary>
        /// Orthographic Matrix based on the Viewport of this Render Target
        /// </summary>
        public Matrix OrthographicMatrix
        {
            get
            {
                if (Viewport.Width <= 0 || Viewport.Height <= 0)
                    return Matrix.Identity;

                return 
                    Matrix.CreateScale((1.0f / Viewport.Width) * 2, -(1.0f / Viewport.Height) * 2, 1f) *
                    Matrix.CreateTranslation(-1.0f, 1.0f, 0f);
            }
        }
    }
}
