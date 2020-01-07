using System;

namespace Foster.Framework
{
    /// <summary>
    /// Describes a Vertex Attribute for a Shader
    /// </summary>
    public struct VertexAttribute
    {

        /// <summary>
        /// The name of the Attribute
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The Vertex Type of the Attribute
        /// </summary>
        public readonly VertexType Type;

        /// <summary>
        /// The number of Components
        /// </summary>
        public readonly int Components;

        /// <summary>
        /// The size of a single Component, in bytes
        /// </summary>
        public readonly int ComponentSize;

        /// <summary>
        /// The size of the entire Attribute, in bytes
        /// </summary>
        public readonly int AttributeSize;

        /// <summary>
        /// Whether the Attribute value should be normalized
        /// </summary>
        public readonly bool Normalized;

        public VertexAttribute(string name, VertexType type, int components, bool normalized = false)
        {
            Name = name;
            Type = type;
            Components = components;
            Normalized = normalized;

            ComponentSize = type switch
            {
                VertexType.Byte => 1,
                VertexType.UnsignedByte => 1,
                VertexType.Short => 2,
                VertexType.UnsignedShort => 2,
                VertexType.Int => 4,
                VertexType.UnsignedInt => 4,
                VertexType.Float => 4,
                _ => throw new InvalidOperationException(),
            };

            AttributeSize = Components * ComponentSize;
        }

    }
}
