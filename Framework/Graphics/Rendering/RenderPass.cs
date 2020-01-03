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
        public RectInt? Scissor;

        /// <summary>
        /// Creates a Render Pass based on the given mesh and material
        /// </summary>
        public RenderPass(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
            MeshStartElement = 0;
            MeshElementCount = mesh.IndicesCount;
            MeshInstanceCount = mesh.InstanceCount;
            Scissor = null;
            BlendMode = BlendMode.Normal;
            DepthFunction = DepthFunctions.None;
            CullMode = Cull.None;
        }
    }
}
