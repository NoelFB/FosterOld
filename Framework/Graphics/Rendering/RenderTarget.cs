using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// An Object that can be rendered to (ex. a Target or a Window)
    /// </summary>
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
        /// Draws the given Render Pass to this Target
        /// </summary>
        public void Render(ref RenderPass pass)
        {
            if (!Drawable)
                throw new Exception("Render Target cannot currently be drawn to");

            if (pass.Mesh == null)
                throw new Exception("Mesh cannot be null when drawing");

            if (pass.Material == null)
                throw new Exception("Material cannot be null when drawing");

            if (pass.Mesh.InstanceCount > 0 && (pass.Mesh.InstanceFormat == null || (pass.Mesh.InstanceCount < pass.Mesh.InstanceCount)))
                throw new Exception("Trying to draw more Instances than exist in the Mesh");

            if (pass.Mesh.ElementCount < pass.MeshStartElement + pass.MeshElementCount)
                throw new Exception("Trying to draw more Elements than exist in the Mesh");

            RenderInternal(ref pass);
        }

        /// <summary>
        /// Draws the given Render Pass
        /// </summary>
        protected abstract void RenderInternal(ref RenderPass pass);

        /// <summary>
        /// Clears the Target
        /// </summary>
        protected abstract void ClearInternal(ClearFlags flags, Color color, float depth, int stencil);

    }
}
