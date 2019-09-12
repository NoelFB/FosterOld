using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    /// <summary>
    /// Vertex Attribute information
    /// This doesn't have a platform-dependent implementation which may be a problem? Or not?
    /// </summary>
    public class VertexAttributeAttribute : Attribute
    {
        public readonly uint Location;
        public readonly int Components;
        public readonly VertexType Type;
        public readonly int TypeSize;
        public readonly bool Normalized;

        public int Offset { get; private set; }
        public int Stride { get; private set; }

        public VertexAttributeAttribute(uint location, VertexType type, int components, bool normalized = true)
        {
            Location = location;
            Components = components;
            Type = type;
            Normalized = normalized;

            TypeSize = 1;
            if (Type == VertexType.Byte)
                TypeSize = 1;
            else if (Type == VertexType.Float)
                TypeSize = 4;
            else if (Type == VertexType.Int)
                TypeSize = 4;
            else if (Type == VertexType.Short)
                TypeSize = 2;
            else if (Type == VertexType.UnsignedByte)
                TypeSize = 1;
            else if (Type == VertexType.UnsignedInt)
                TypeSize = 4;
            else if (Type == VertexType.UnsignedShort)
                TypeSize = 2;
        }

        public static bool TypeHasAttributes<T>()
        {
            return (attributesOfType.TryGetValue(typeof(T), out var list) && list != null && list.Count > 0);
        }

        public static void AttributesOfType<T>(out List<VertexAttributeAttribute>? list)
        {
            var type = typeof(T);
            var hasAttributes = attributesOfType.TryGetValue(type, out list);

            if (!hasAttributes)
            {
                attributesOfType.Add(type, list = new List<VertexAttributeAttribute>());

                int stride = 0;
                foreach (var field in type.GetFields())
                {
                    var attribs = field.GetCustomAttributes(typeof(VertexAttributeAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                    {
                        var attrib = (VertexAttributeAttribute)attribs[0];
                        attrib.Offset = stride;
                        stride += attrib.Components * attrib.TypeSize;
                        list.Add(attrib);
                    }
                }

                foreach (var attrib in list)
                    attrib.Stride = stride;
            }
        }

        private static Dictionary<Type, List<VertexAttributeAttribute>> attributesOfType = new Dictionary<Type, List<VertexAttributeAttribute>>();
    }
}
