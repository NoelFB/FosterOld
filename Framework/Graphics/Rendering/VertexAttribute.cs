using System;
using System.Collections.Generic;
using System.Reflection;

namespace Foster.Framework
{
    public enum VertexType
    {
        Unknown,

        Byte,
        UnsignedByte,

        Short,
        UnsignedShort,

        Int,
        UnsignedInt,

        Float
    }

    /// <summary>
    /// Vertex Attribute information
    /// This doesn't have a platform-dependent implementation which may be a problem? Or not?
    /// </summary>
    public class VertexAttributeAttribute : Attribute
    {
        public int Index;
        public string Name;
        public int ComponentCount;
        public int ComponentSize;
        public int Size => ComponentCount * ComponentSize;
        public VertexType Type;
        public bool Normalized;
        public int Pointer;
        public int Stride;

        public VertexAttributeAttribute(int index, string name, VertexType type, int components, bool normalized = false)
        {
            Index = index;
            Name = name;
            Type = type;

            ComponentCount = components;
            ComponentSize = 1;
            if (Type == VertexType.Byte)
                ComponentSize = 1;
            else if (Type == VertexType.Float)
                ComponentSize = 4;
            else if (Type == VertexType.Int)
                ComponentSize = 4;
            else if (Type == VertexType.Short)
                ComponentSize = 2;
            else if (Type == VertexType.UnsignedByte)
                ComponentSize = 1;
            else if (Type == VertexType.UnsignedInt)
                ComponentSize = 4;
            else if (Type == VertexType.UnsignedShort)
                ComponentSize = 2;

            Normalized = normalized;
        }
    }

    public static class VertexAttributes
    {
        public static bool HasType(Type type)
        {
            OfType(type, out var list);
            return (list != null && list.Count > 0);
        }

        public static bool HasType<T>()
        {
            OfType<T>(out var list);
            return (list != null && list.Count > 0);
        }

        public static void OfType(Type type, out List<VertexAttributeAttribute>? list)
        {
            bool hasAttributes = attributesOfType.TryGetValue(type, out list);

            if (!hasAttributes)
            {
                attributesOfType.Add(type, list = new List<VertexAttributeAttribute>());

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var attribs = field.GetCustomAttributes(typeof(VertexAttributeAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                        list.Add((VertexAttributeAttribute)attribs[0]);
                }

                list.Sort((a, b) => a.Index - b.Index);

                int stride = 0;
                foreach (var attribute in list)
                {
                    attribute.Pointer = stride;
                    stride += attribute.Size;
                }

                foreach (var attrib in list)
                    attrib.Stride = stride;
            }
        }

        public static void OfType<T>(out List<VertexAttributeAttribute>? list)
        {
            OfType(typeof(T), out list);
        }

        private static readonly Dictionary<Type, List<VertexAttributeAttribute>> attributesOfType = new Dictionary<Type, List<VertexAttributeAttribute>>();
    }
}
