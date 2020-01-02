using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A Structure which describes a single Render Pass / Render Call
    /// </summary>
    public struct RenderPass
    {
        /// <summary>
        /// Target to Draw To
        /// </summary>
        public RenderTarget Target;

        /// <summary>
        /// Material to use
        /// </summary>
        public Material Material;

        /// <summary>
        /// Mesh to use
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// The Element to begin rendering from the Mesh
        /// </summary>
        public int MeshStartElement;

        /// <summary>
        /// The total number of Elements to draw from the Mesh
        /// </summary>
        public int MeshElementCount;

        /// <summary>
        /// The total number of instances to draw from the Mesh
        /// </summary>
        public int MeshInstanceCount;

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
        /// The Render State Scissor Rectangle
        /// </summary>
        public RectInt Scissor;

        /// <summary>
        /// Creates a Render Pass based on the given target, mesh, and material
        /// </summary>
        public RenderPass(RenderTarget target, Mesh mesh, Material material)
        {
            Target = target;
            Mesh = mesh;
            Material = material;
            MeshStartElement = 0;
            MeshElementCount = mesh.IndicesCount;
            MeshInstanceCount = mesh.InstanceCount;
            Scissor = new RectInt(0, 0, target.Width, target.Height);
            BlendMode = BlendMode.Normal;
            DepthFunction = DepthFunctions.None;
            CullMode = Cull.None;
        }

        /// <summary>
        /// Performs the Render Pass
        /// </summary>
        public void Draw()
        {
            App.Graphics.Draw(ref this);
        }

        /// <summary>
        /// Performs the Render Pass
        /// </summary>
        public void Draw(Graphics graphics)
        {
            graphics.Draw(ref this);
        }
    }
}
