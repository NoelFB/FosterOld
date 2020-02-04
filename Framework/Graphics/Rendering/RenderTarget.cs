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
        /// Whether the Render Target can be drawn to.
        /// </summary>
        public bool Drawable { get; internal protected set; }
    }
}
