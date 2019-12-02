using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A Generic 3D Vertex with Position, TexCoord, Normal, and Color
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {

        [VertexAttribute(0, "vPosition", VertexType.Float, 3)]
        public Vector3 Position;

        [VertexAttribute(1, "vTex", VertexType.Float, 2)]
        public Vector2 TexCoord;

        [VertexAttribute(2, "vNormal", VertexType.Float, 3)]
        public Vector3 Normal;

        [VertexAttribute(3, "vColor", VertexType.UnsignedByte, 4, true)]
        public Color Color;

    }
}
