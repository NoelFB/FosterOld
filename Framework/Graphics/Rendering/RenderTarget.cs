using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// An Object that can be rendered to (ex. a FrameBuffer or a Window)
    /// </summary>
    public abstract class RenderTarget
    {
        /// <summary>
        /// The Width of the Target
        /// </summary>
        public abstract int RenderWidth { get; }

        /// <summary>
        /// The Height of the Target
        /// </summary>
        public abstract int RenderHeight { get; }

        /// <summary>
        /// Whether the Target can be rendered to.
        /// </summary>
        public bool Renderable { get; internal protected set; }
    }
}
