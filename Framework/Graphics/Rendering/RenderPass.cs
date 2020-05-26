using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public enum IndexElementSize
    {
        /// <summary>
        /// 16-bit short/ushort indices.
        /// </summary>
        SixteenBits,
        /// <summary>
        /// 32-bit int/uint indices.
        /// </summary>
        ThirtyTwoBits
    }

    /// <summary>
    /// A Structure which describes a single Render Pass / Render Call
    /// </summary>
    public struct RenderPass
    {
        /// <summary>
        /// Render Target
        /// </summary>
        public RenderTarget Target;

        /// <summary>
        /// Render Viewport
        /// </summary>
        public RectInt? Viewport;

        /// <summary>
        /// Material to use
        /// </summary>
        public Material Material;

        /// <summary>
        /// Mesh to use
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// The Index to begin rendering from the Mesh
        /// </summary>
        public uint MeshIndexStart;

        /// <summary>
        /// The total number of Indices to draw from the Mesh
        /// </summary>
        public uint MeshIndexCount;

        /// <summary>
        /// The total number of Indices to draw from the Mesh
        /// </summary>
        public IndexElementSize IndexElementSize;

        /// <summary>
        /// The total number of Instances to draw from the Mesh
        /// </summary>
        public uint MeshInstanceCount;

        /// <summary>
        /// The Render State Blend Mode
        /// </summary>
        public BlendMode BlendMode;

        /// <summary>
        /// The Render State Culling Mode
        /// </summary>
        public CullMode CullMode;

        /// <summary>
        /// The Render State Depth comparison Function
        /// </summary>
        public Compare DepthFunction;

        /// <summary>
        /// The Render State Scissor Rectangle
        /// </summary>
        public RectInt? Scissor;

        /// <summary>
        /// Creates a Render Pass based on the given mesh and material
        /// </summary>
        public RenderPass(RenderTarget target, Mesh mesh, Material material)
        {
            Target = target;
            Viewport = null;
            Mesh = mesh;
            Material = material;
            MeshIndexStart = 0;
            MeshIndexCount = mesh.IndexCount;
            MeshInstanceCount = mesh.InstanceCount;
            Scissor = null;
            BlendMode = BlendMode.Normal;
            DepthFunction = Compare.None;
            CullMode = CullMode.None;
            IndexElementSize = IndexElementSize.ThirtyTwoBits;  // Default to 32 bits.
        }

        public void Render()
        {
            App.Graphics.Render(ref this);
        }

        public void Render(Graphics graphics)
        {
            graphics.Render(ref this);
        }
    }
}
