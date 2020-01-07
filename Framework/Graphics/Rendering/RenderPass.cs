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
        public int MeshIndexStart;

        /// <summary>
        /// The total number of Indices to draw from the Mesh
        /// </summary>
        public int MeshIndexCount;

        /// <summary>
        /// The total number of Instances to draw from the Mesh
        /// </summary>
        public int MeshInstanceCount;

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
        public RenderPass(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
            MeshIndexStart = 0;
            MeshIndexCount = mesh.IndexCount;
            MeshInstanceCount = mesh.InstanceCount;
            Scissor = null;
            BlendMode = BlendMode.Normal;
            DepthFunction = Compare.None;
            CullMode = CullMode.None;
        }
    }
}
