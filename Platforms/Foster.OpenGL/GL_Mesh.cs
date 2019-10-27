using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Foster.OpenGL
{
    public class GL_Mesh<TVertex> : Mesh<TVertex> where TVertex : struct
    {

        public Dictionary<Context, uint> VertexArrays = new Dictionary<Context, uint>();

        public uint VertexBuffer;
        public uint TriangleBuffer;
        public uint InstanceBuffer;
        public Type? InstanceType;

        public GL_Mesh(GL_Graphics graphics) : base(graphics)
        {
            VertexBuffer = GL.GenBuffer();
            TriangleBuffer = GL.GenBuffer();
        }

        public override unsafe void SetVertices(Memory<TVertex> vertices)
        {
            using System.Buffers.MemoryHandle pinned = vertices.Pin();

            GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
            GL.BufferData(GLEnum.ARRAY_BUFFER, new IntPtr(Marshal.SizeOf<TVertex>() * vertices.Length), new IntPtr(pinned.Pointer), GLEnum.STATIC_DRAW);
        }

        public override unsafe void SetTriangles(Memory<int> triangles)
        {
            using System.Buffers.MemoryHandle pinned = triangles.Pin();

            GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, TriangleBuffer);
            GL.BufferData(GLEnum.ELEMENT_ARRAY_BUFFER, new IntPtr(sizeof(int) * triangles.Length), new IntPtr(pinned.Pointer), GLEnum.STATIC_DRAW);
        }

        public override unsafe void SetInstances<T>(Memory<T> instances)
        {
            // create buffer if it's missing
            Type type = typeof(T);
            if (type != InstanceType || InstanceBuffer == 0)
            {
                InstanceType = type;
                InstanceBuffer = GL.GenBuffer();

                GL.BindBuffer(GLEnum.ARRAY_BUFFER, InstanceBuffer);

                EnableAttribsOnBuffer<T>(1);
            }

            // upload buffer data
            using System.Buffers.MemoryHandle handle = instances.Pin();
            GL.BindBuffer(GLEnum.ARRAY_BUFFER, InstanceBuffer);
            GL.BufferData(GLEnum.ARRAY_BUFFER, new IntPtr(Marshal.SizeOf<T>() * instances.Length), new IntPtr(handle.Pointer), GLEnum.STATIC_DRAW);
        }

        public override void Draw(int start, int elements)
        {
            if (BindVertexArray())
            {
                // use the shader with the material parameters
                if (Material != null && Material.Shader is GL_Shader shader)
                    shader.Use(Material);

                GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
                GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, TriangleBuffer);

                GL.DrawElements(GLEnum.TRIANGLES, elements * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * start * 3));
            }
        }

        public override void DrawInstances(int start, int elements, int instances)
        {
            if (InstanceType == null)
                throw new Exception("Instances must be assigned before being drawn");

            if (BindVertexArray())
            {
                GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
                GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, TriangleBuffer);
                GL.BindBuffer(GLEnum.ARRAY_BUFFER, InstanceBuffer);

                GL.DrawElementsInstanced(GLEnum.TRIANGLES, elements * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * start * 3), instances);
            }
        }

        private bool BindVertexArray()
        {
            // Create the VAO on this context if it doesn't exist
            // VAO's are not shared between contexts so it must exist on every one we try drawing with

            var context = App.System.GetCurrentContext();
            if (context != null)
            {
                if (!VertexArrays.TryGetValue(context, out uint id))
                {
                    VertexArrays.Add(context, id = GL.GenVertexArray());
                    GL.BindVertexArray(id);
                    EnableAttribsOnBuffer<TVertex>(0);
                }
                else
                {
                    GL.BindVertexArray(id);
                }

                return true;
            }

            return false;
        }

        public override unsafe void Dispose()
        {
            if (!Disposed)
            {
                if (Graphics is GL_Graphics graphics)
                {
                    graphics.BuffersToDelete.Add(VertexBuffer);
                    graphics.BuffersToDelete.Add(TriangleBuffer);

                    if (InstanceBuffer > 0)
                        graphics.BuffersToDelete.Add(InstanceBuffer);

                    foreach (var kv in VertexArrays)
                    {
                        var context = kv.Key;
                        var vertexArray = kv.Value;

                        if (!graphics.VertexArraysToDelete.TryGetValue(context, out var list))
                            graphics.VertexArraysToDelete[context] = list = new List<uint>();

                        list.Add(vertexArray);
                    }
                }
            }

            base.Dispose();
        }

        private static void EnableAttribsOnBuffer<T>(uint divisor = 0)
        {
            VertexAttributeAttribute.AttributesOfType<T>(out System.Collections.Generic.List<VertexAttributeAttribute>? attributes);

            if (attributes == null)
                throw new Exception("Expecting Vertex Attributes");

            foreach (VertexAttributeAttribute attrib in attributes)
            {
                // this is kind of messy because some attributes can take up multiple slots
                // ex. a marix4x4 actually takes up 4 (size 16)
                IntPtr ptr = new IntPtr(attrib.Offset);
                for (int i = 0, loc = 0; i < attrib.Components; i += 4, loc++)
                {
                    int size = Math.Min(attrib.Components - i, 4);
                    uint location = (uint)(attrib.Location + loc);

                    GL.EnableVertexAttribArray(location);
                    GL.VertexAttribPointer(location, size, ToGLVertexType(attrib.Type), attrib.Normalized, attrib.Stride, ptr);
                    GL.VertexAttribDivisor(location, divisor);
                    ptr += size * attrib.Size;
                }
            }
        }

        private static GLEnum ToGLVertexType(VertexType value)
        {
            switch (value)
            {
                case VertexType.Byte: return GLEnum.BYTE;
                case VertexType.UnsignedByte: return GLEnum.UNSIGNED_BYTE;
                case VertexType.Short: return GLEnum.SHORT;
                case VertexType.UnsignedShort: return GLEnum.UNSIGNED_SHORT;
                case VertexType.Int: return GLEnum.INT;
                case VertexType.UnsignedInt: return GLEnum.UNSIGNED_INT;
                case VertexType.Float: default: return GLEnum.FLOAT;
            }
        }
    }
}
