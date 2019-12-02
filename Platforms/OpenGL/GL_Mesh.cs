using Foster.Framework;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Foster.OpenGL
{
    public class GL_Mesh : Mesh
    {

        private Dictionary<Context, uint> vertexArrays = new Dictionary<Context, uint>();
        private Dictionary<Context, bool> bindedArrays = new Dictionary<Context, bool>();

        private readonly uint indexBuffer;
        private readonly uint vertexBuffer;
        private uint instanceBuffer;
        private Type? lastVertexType;
        private Type? lastInstanceType;

        private long vertexCount;
        private long indexCount;
        private long instanceCount;

        private Material? material;

        public GL_Mesh(GL_Graphics graphics) : base(graphics)
        {
            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();
        }

        public override Material? Material
        {
            get => material;
            set
            {
                if (material != value)
                {
                    material = value;
                    bindedArrays.Clear();
                }
            }
        }

        protected override unsafe void UploadVertices<T>(ReadOnlySequence<T> vertices)
        {
            var type = typeof(T);
            if (type != lastVertexType)
            {
                lastVertexType = type;
                bindedArrays.Clear();
            }

            UploadBuffer(vertexBuffer, GLEnum.ARRAY_BUFFER, vertices, ref vertexCount);
        }

        protected override unsafe void UploadIndices(ReadOnlySequence<int> triangles)
        {
            UploadBuffer(indexBuffer, GLEnum.ELEMENT_ARRAY_BUFFER, triangles, ref indexCount);
        }

        protected override unsafe void UploadInstances<T>(ReadOnlySequence<T> instances)
        {
            if (instanceBuffer == 0)
                instanceBuffer = GL.GenBuffer();

            Type type = typeof(T);
            if (type != lastInstanceType)
            {
                lastInstanceType = type;
                bindedArrays.Clear();
            }

            // upload buffer data
            UploadBuffer(instanceBuffer, GLEnum.ARRAY_BUFFER, instances, ref instanceCount);
        }

        private unsafe void UploadBuffer<T>(uint id, GLEnum type, ReadOnlySequence<T> data, ref long count)
        {
            var structSize = Marshal.SizeOf<T>();

            GL.BindBuffer(type, id);

            var lastCount = count;
            count = data.Length;

            // resize the buffer
            if (count > lastCount)
                GL.BufferData(type, new IntPtr(structSize * count), IntPtr.Zero, GLEnum.STATIC_DRAW);

            // upload the data
            var offset = 0;
            foreach (var memory in data)
            {
                using var pinned = memory.Pin();
                GL.BufferSubData(type, new IntPtr(structSize * offset), new IntPtr(structSize * memory.Length), new IntPtr(pinned.Pointer));
                offset += memory.Length;
            }

            GL.BindBuffer(type, 0);
        }

        public override void Draw(int start, int elements)
        {
            if (Material == null)
                throw new Exception("Trying to draw without a Material");

            if (Material.Shader is GL_Shader shader)
                shader.Use(Material);

            if (BindVertexArray())
            {
                GL.DrawElements(GLEnum.TRIANGLES, elements * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * start * 3));
                GL.BindVertexArray(0);
            }
        }

        public override void DrawInstances(int start, int elements, int instances)
        {
            if (lastInstanceType == null)
                throw new Exception("Instances must be assigned before being drawn");

            if (Material == null)
                throw new Exception("Trying to draw without a Material");

            if (Material.Shader is GL_Shader shader)
                shader.Use(Material);

            if (BindVertexArray())
            {
                GL.DrawElementsInstanced(GLEnum.TRIANGLES, elements * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * start * 3), instances);
                GL.BindVertexArray(0);
            }
        }

        private bool BindVertexArray()
        {
            // Create the VAO on this context if it doesn't exist
            // VAO's are not shared between contexts so it must exist on every one we try drawing with

            var context = App.System.GetCurrentContext();
            if (context != null && Material != null)
            {
                // create new array if it's needed
                if (!vertexArrays.TryGetValue(context, out uint id))
                    vertexArrays.Add(context, id = GL.GenVertexArray());

                GL.BindVertexArray(id);

                // rebind data if needed
                if (!bindedArrays.TryGetValue(context, out var bound) || !bound)
                {
                    bindedArrays[context] = true;

                    // bind buffers and determine what attributes are on
                    foreach (var shaderAttribute in Material.Shader.Attributes.Values)
                    {
                        if (lastVertexType != null)
                        {
                            GL.BindBuffer(GLEnum.ARRAY_BUFFER, vertexBuffer);

                            if (SetupAttributePointer(shaderAttribute, lastVertexType, 0))
                                continue;
                        }

                        if (lastInstanceType != null)
                        {
                            GL.BindBuffer(GLEnum.ARRAY_BUFFER, instanceBuffer);

                            if (SetupAttributePointer(shaderAttribute, lastInstanceType, 1))
                                continue;
                        }

                        // nothing is using this so disable it
                        GL.DisableVertexAttribArray(shaderAttribute.Location);
                    }

                    // bind our index buffer
                    GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, indexBuffer);
                }

                return true;
            }

            return false;
        }

        private bool SetupAttributePointer(ShaderAttribute shaderAttribute, Type type, uint divisor)
        {
            VertexAttributes.OfType(type, out var attributes);

            if (attributes == null)
                throw new Exception("Expecting Vertex Attributes");

            foreach (var attribute in attributes)
            {
                if (!attribute.Name.Equals(shaderAttribute.Name))
                    continue;

                // this is kind of messy because some attributes can take up multiple slots
                // ex. a marix4x4 actually takes up 4 (size 16)
                var ptr = new IntPtr(attribute.Pointer);
                for (int i = 0, loc = 0; i < attribute.ComponentCount; i += 4, loc++)
                {
                    var size = Math.Min(attribute.ComponentCount - i, 4);
                    var location = (uint)(shaderAttribute.Location + loc);

                    GL.EnableVertexAttribArray(location);
                    GL.VertexAttribPointer(location, size, ConvertVertexType(attribute.Type), attribute.Normalized, attribute.Stride, ptr);
                    GL.VertexAttribDivisor(location, divisor);

                    ptr += size * attribute.ComponentSize;
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
                    graphics.BuffersToDelete.Add(vertexBuffer);
                    graphics.BuffersToDelete.Add(indexBuffer);

                    if (instanceBuffer > 0)
                        graphics.BuffersToDelete.Add(instanceBuffer);

                    foreach (var kv in vertexArrays)
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

        private static GLEnum ConvertVertexType(VertexType value)
        {
            switch (value)
            {
                case Framework.VertexType.Byte: return GLEnum.BYTE;
                case Framework.VertexType.UnsignedByte: return GLEnum.UNSIGNED_BYTE;
                case Framework.VertexType.Short: return GLEnum.SHORT;
                case Framework.VertexType.UnsignedShort: return GLEnum.UNSIGNED_SHORT;
                case Framework.VertexType.Int: return GLEnum.INT;
                case Framework.VertexType.UnsignedInt: return GLEnum.UNSIGNED_INT;
                case Framework.VertexType.Float: default: return GLEnum.FLOAT;
            }
        }
    }
}
