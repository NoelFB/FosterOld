using System;
using System.Collections.Generic;
using System.Reflection;

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
        public readonly int Size;
        public readonly bool Normalized;

        public int Offset { get; private set; }
        public int Stride { get; private set; }

        public VertexAttributeAttribute(uint location, VertexType type, int components, bool normalized = true)
        {
            Location = location;
            Components = components;
            Type = type;
            Normalized = normalized;

            Size = 1;
            if (Type == VertexType.Byte)
            {
                Size = 1;
            }
            else if (Type == VertexType.Float)
            {
                Size = 4;
            }
            else if (Type == VertexType.Int)
            {
                Size = 4;
            }
            else if (Type == VertexType.Short)
            {
                Size = 2;
            }
            else if (Type == VertexType.UnsignedByte)
            {
                Size = 1;
            }
            else if (Type == VertexType.UnsignedInt)
            {
                Size = 4;
            }
            else if (Type == VertexType.UnsignedShort)
            {
                Size = 2;
            }
        }

        public static bool TypeHasAttributes<T>()
        {
            AttributesOfType<T>(out List<VertexAttributeAttribute>? list);
            return (list != null && list.Count > 0);
        }

        public static void AttributesOfType<T>(out List<VertexAttributeAttribute>? list)
        {
            Type type = typeof(T);
            bool hasAttributes = attributesOfType.TryGetValue(type, out list);

            if (!hasAttributes)
            {
                attributesOfType.Add(type, list = new List<VertexAttributeAttribute>());

                int stride = 0;
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object[] attribs = field.GetCustomAttributes(typeof(VertexAttributeAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                    {
                        VertexAttributeAttribute attrib = (VertexAttributeAttribute)attribs[0];
                        attrib.Offset = stride;
                        stride += attrib.Components * attrib.Size;
                        list.Add(attrib);
                    }
                }

                foreach (VertexAttributeAttribute attrib in list)
                {
                    attrib.Stride = stride;
                }
            }
        }

        private static readonly Dictionary<Type, List<VertexAttributeAttribute>> attributesOfType = new Dictionary<Type, List<VertexAttributeAttribute>>();
    }
}
