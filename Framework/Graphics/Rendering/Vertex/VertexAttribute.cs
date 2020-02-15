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
        /// Depending on the Graphics Implementation, this may or may not be respected
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The Vertex Attribute Type
        /// Depending on the Graphics Implementation, this may or may not be respected
        /// </summary>
        public readonly VertexAttrib Attrib;

        /// <summary>
        /// The Vertex Type of the Attribute
        /// </summary>
        public readonly VertexType Type;

        /// <summary>
        /// The number of Components
        /// </summary>
        public readonly VertexComponents Components;

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

        public VertexAttribute(string name, VertexAttrib attrib, VertexType type, VertexComponents components, bool normalized = false)
        {
            Name = name;
            Attrib = attrib;
            Type = type;
            Components = components;
            Normalized = normalized;

            ComponentSize = type switch
            {
                VertexType.Byte => 1,
                VertexType.Short => 2,
                VertexType.Int => 4,
                VertexType.Float => 4,
                _ => throw new NotImplementedException(),
            };

            AttributeSize = (int)Components * ComponentSize;
        }

    }
}
