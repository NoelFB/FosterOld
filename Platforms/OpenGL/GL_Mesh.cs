using Foster.Framework;
using Foster.Framework.Internal;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Foster.OpenGL
{
    public class GL_Mesh : InternalMesh
    {

        private readonly Dictionary<Context, uint> vertexArrays = new Dictionary<Context, uint>();
        private readonly Dictionary<Context, bool> bindedArrays = new Dictionary<Context, bool>();

        private readonly GL_Graphics graphics;
        private uint indexBuffer;
        private uint vertexBuffer;
        private uint instanceBuffer;
        private VertexFormat? lastVertexFormat;
        private VertexFormat? lastInstanceFormat;
        private Material? material;
        private long indexBufferSize;
        private long vertexBufferSize;
        private long instanceBufferSize;

        internal GL_Mesh(GL_Graphics graphics)
        {
            this.graphics = graphics;

            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();
        }

        ~GL_Mesh()
        {
            Dispose();
        }

        protected override void SetMaterial(Material? material)
        {
            if (this.material != material)
            {
                this.material = material;
                bindedArrays.Clear();
            }
        }

        protected override void UploadVertices<T>(ReadOnlySequence<T> vertices, VertexFormat format)
        {
            if (format != lastVertexFormat)
            {
                lastVertexFormat = format;
                bindedArrays.Clear();
            }

            UploadBuffer(vertexBuffer, GLEnum.ARRAY_BUFFER, vertices, ref vertexBufferSize);
        }

        protected override void UploadInstances<T>(ReadOnlySequence<T> instances, VertexFormat format)
        {
            if (instanceBuffer == 0)
                instanceBuffer = GL.GenBuffer();

            if (format != lastInstanceFormat)
            {
                lastInstanceFormat = format;
                bindedArrays.Clear();
            }

            // upload buffer data
            UploadBuffer(instanceBuffer, GLEnum.ARRAY_BUFFER, instances, ref instanceBufferSize);
        }

        protected override unsafe void UploadIndices(ReadOnlySequence<int> indices)
        {
            UploadBuffer(indexBuffer, GLEnum.ELEMENT_ARRAY_BUFFER, indices, ref indexBufferSize);
        }

        private unsafe void UploadBuffer<T>(uint id, GLEnum type, ReadOnlySequence<T> data, ref long currentBufferSize)
        {
            var structSize = Marshal.SizeOf<T>();

            GL.BindBuffer(type, id);

            // resize the buffer
            var neededBufferSize = data.Length * structSize;
            if (currentBufferSize < neededBufferSize)
            {
                currentBufferSize = neededBufferSize;
                GL.BufferData(type, new IntPtr(structSize * currentBufferSize), IntPtr.Zero, GLEnum.STATIC_DRAW);
            }

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

        protected override void Draw(int start, int elements)
        {
            if (material == null)
                throw new Exception("Trying to draw without a Material");

            if (material.Shader.Internal is GL_Shader shader)
                shader.Use(material);

            if (BindVertexArray())
            {
                GL.DrawElements(GLEnum.TRIANGLES, elements * 3, GLEnum.UNSIGNED_INT, new IntPtr(sizeof(int) * start * 3));
                GL.BindVertexArray(0);
            }
        }

        protected override void DrawInstances(int start, int elements, int instances)
        {
            if (lastInstanceFormat == null || indexBuffer == 0)
                throw new Exception("Instances must be assigned before being drawn");

            if (material == null)
                throw new Exception("Trying to draw without a Material");

            if (material.Shader.Internal is GL_Shader shader)
                shader.Use(material);

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
            if (context != null && material != null)
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
                    foreach (var attribute in material.Shader.Attributes.Values)
                    {
                        if (lastVertexFormat != null)
                        {
                            GL.BindBuffer(GLEnum.ARRAY_BUFFER, vertexBuffer);

                            if (TrySetupAttribPointer(attribute, lastVertexFormat, 0))
                                continue;
                        }

                        if (lastInstanceFormat != null)
                        {
                            GL.BindBuffer(GLEnum.ARRAY_BUFFER, instanceBuffer);

                            if (TrySetupAttribPointer(attribute, lastInstanceFormat, 1))
                                continue;
                        }

                        // nothing is using this so disable it
                        GL.DisableVertexAttribArray(attribute.Location);
                    }

                    // bind our index buffer
                    GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, indexBuffer);
                }

                return true;
            }

            return false;
        }

        private bool TrySetupAttribPointer(ShaderAttribute attribute, VertexFormat format, uint divisor)
        {
            if (format.TryGetElement(attribute.Name, out var element, out var ptr))
            {
                // this is kind of messy because some attributes can take up multiple slots
                // ex. a marix4x4 actually takes up 4 (size 16)
                for (int i = 0, loc = 0; i < element.Components; i += 4, loc++)
                {
                    var components = Math.Min(element.Components - i, 4);
                    var location = (uint)(attribute.Location + loc);

                    GL.EnableVertexAttribArray(location);
                    GL.VertexAttribPointer(location, components, ConvertVertexType(element.Type), element.Normalized, format.Stride, new IntPtr(ptr));
                    GL.VertexAttribDivisor(location, divisor);

                    ptr += components * element.ComponentSizeInBytes;
                }

                return true;
            }

            return false;
        }

        protected override void Dispose()
        {
            if (vertexBuffer != 0)
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

                vertexArrays.Clear();
                vertexBuffer = 0;
                indexBuffer = 0;
                instanceBuffer = 0;
            }
        }

        private static GLEnum ConvertVertexType(VertexType value)
        {
            return value switch
            {
                VertexType.Byte => GLEnum.BYTE,
                VertexType.UnsignedByte => GLEnum.UNSIGNED_BYTE,
                VertexType.Short => GLEnum.SHORT,
                VertexType.UnsignedShort => GLEnum.UNSIGNED_SHORT,
                VertexType.Int => GLEnum.INT,
                VertexType.UnsignedInt => GLEnum.UNSIGNED_INT,
                VertexType.Float => GLEnum.FLOAT,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
