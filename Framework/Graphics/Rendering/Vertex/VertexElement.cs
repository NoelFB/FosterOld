using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public struct VertexElement
    {

        public readonly string Name;
        public readonly VertexType Type;
        public readonly int Components;
        public readonly int ComponentSizeInBytes;
        public readonly int ElementSizeInBytes;
        public readonly bool Normalized;

        public VertexElement(string name, VertexType type, int components, bool normalized = false)
        {
            Name = name;
            Type = type;
            Components = components;
            Normalized = normalized;

            ComponentSizeInBytes = type switch
            {
                VertexType.Byte => 1,
                VertexType.UnsignedByte => 1,
                VertexType.Short => 2,
                VertexType.UnsignedShort => 2,
                VertexType.Int => 4,
                VertexType.UnsignedInt => 4,
                VertexType.Float => 4,
                _ => throw new NotImplementedException(),
            };

            ElementSizeInBytes = Components * ComponentSizeInBytes;
        }

    }
}
