using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public struct RenderState
    {
        /// <summary>
        /// The Render State Blend Mode
        /// </summary>
        public BlendMode BlendMode;

        /// <summary>
        /// The Render State Culling Mode
        /// </summary>
        public Cull CullMode;

        /// <summary>
        /// The Render State Depth comparison Function
        /// </summary>
        public DepthFunctions DepthFunction;

        /// <summary>
        /// The Render State Viewport
        /// </summary>
        public RectInt Viewport;

        /// <summary>
        /// The Render State Scissor Rectangle
        /// </summary>
        public RectInt Scissor;

        /// <summary>
        /// Orthographic Matrix based on the Viewport of this Render State
        /// </summary>
        public Matrix OrthographicMatrix =>
            Matrix.CreateScale((1.0f / Viewport.Width) * 2, -(1.0f / Viewport.Height) * 2, 1f) *
            Matrix.CreateTranslation(-1.0f, 1.0f, 0f);

        public RenderState(RenderTarget target)
        {
            BlendMode = BlendMode.Normal;
            CullMode = Cull.Back;
            DepthFunction = DepthFunctions.None;
            Viewport = new RectInt(0, 0, target.Width, target.Height);
            Scissor = new RectInt(0, 0, target.Width, target.Height);
        }

        public override bool Equals(object? obj)
        {
            return (obj is RenderState renderState && this == renderState);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BlendMode, CullMode, DepthFunction, Viewport, Scissor);
        }

        public static bool operator ==(RenderState a, RenderState b)
        {
            return
                a.BlendMode == b.BlendMode &&
                a.CullMode == b.CullMode &&
                a.DepthFunction == b.DepthFunction &&
                a.Viewport == b.Viewport &&
                a.Scissor == b.Scissor;
        }

        public static bool operator !=(RenderState a, RenderState b)
        {
            return !(a == b);
        }

    }
}
