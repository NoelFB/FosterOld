using Foster.Framework;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foster.OpenGL
{
    internal class GL_Mesh : Mesh.Platform
    {

        private readonly Dictionary<ISystemOpenGL.Context, uint> vertexArrays = new Dictionary<ISystemOpenGL.Context, uint>();
        private readonly Dictionary<ISystemOpenGL.Context, bool> bindedArrays = new Dictionary<ISystemOpenGL.Context, bool>();

        private readonly GL_Graphics graphics;
        private uint indexBuffer;
        private uint vertexBuffer;
        private uint instanceBuffer;
        private VertexFormat? lastVertexFormat;
        private VertexFormat? lastInstanceFormat;
        private Material? lastMaterial;
        private long indexBufferSize;
        private long vertexBufferSize;
        private long instanceBufferSize;

        internal GL_Mesh(GL_Graphics graphics)
        {
            this.graphics = graphics;
        }

        ~GL_Mesh()
        {
            Dispose();
        }

        protected override void UploadVertices<T>(ReadOnlySequence<T> vertices, VertexFormat format)
        {
            if (format != lastVertexFormat)
            {
                lastVertexFormat = format;
                bindedArrays.Clear();
            }

            UploadBuffer(ref vertexBuffer, GLEnum.ARRAY_BUFFER, vertices, ref vertexBufferSize);
        }

        protected override void UploadInstances<T>(ReadOnlySequence<T> instances, VertexFormat format)
        {
            if (format != lastInstanceFormat)
            {
                lastInstanceFormat = format;
                bindedArrays.Clear();
            }

            // upload buffer data
            UploadBuffer(ref instanceBuffer, GLEnum.ARRAY_BUFFER, instances, ref instanceBufferSize);
        }

        protected override unsafe void UploadIndices<T>(ReadOnlySequence<T> indices)
        {
            UploadBuffer(ref indexBuffer, GLEnum.ELEMENT_ARRAY_BUFFER, indices, ref indexBufferSize);
        }

        private unsafe void UploadBuffer<T>(ref uint id, GLEnum type, ReadOnlySequence<T> data, ref long currentBufferSize)
        {
            if (graphics.MainThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                lock (graphics.BackgroundContext)
                {
                    graphics.System.SetCurrentGLContext(graphics.BackgroundContext);

                    UploadBufferData(ref id, type, data, ref currentBufferSize);
                    GL.Flush();

                    graphics.System.SetCurrentGLContext(null);
                }
            }
            else
            {
                UploadBufferData(ref id, type, data, ref currentBufferSize);
            }

            static void UploadBufferData(ref uint id, GLEnum type, ReadOnlySequence<T> data, ref long currentBufferSize)
            {
                if (id == 0)
                    id = GL.GenBuffer();

                GL.BindBuffer(type, id);

                var structSize = Marshal.SizeOf<T>();

                // resize the buffer
                var neededBufferSize = data.Length * structSize;
                if (currentBufferSize < neededBufferSize)
                {
                    currentBufferSize = neededBufferSize;
                    GL.BufferData(type, new IntPtr(structSize * currentBufferSize), IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
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
        }

        internal void Bind(ISystemOpenGL.Context context, Material material)
        {
            // make sure our VAO is up to shape on the given context
            {
                // create new array if it's needed on this context
                if (!vertexArrays.TryGetValue(context, out uint id))
                    vertexArrays.Add(context, id = GL.GenVertexArray());

                GL.BindVertexArray(id);

                // we will need to change attribute pointers probably
                if (lastMaterial != null && lastMaterial.Shader != material.Shader)
                    bindedArrays.Clear();
                lastMaterial = material;

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
            }

            static bool TrySetupAttribPointer(ShaderAttribute attribute, VertexFormat format, uint divisor)
            {
                if (format.TryGetAttribute(attribute.Name, out var element, out var ptr))
                {
                    // this is kind of messy because some attributes can take up multiple slots
                    // ex. a marix4x4 actually takes up 4 (size 16)
                    for (int i = 0, loc = 0; i < (int)element.Components; i += 4, loc++)
                    {
                        var components = Math.Min((int)element.Components - i, 4);
                        var location = (uint)(attribute.Location + loc);

                        GL.EnableVertexAttribArray(location);
                        GL.VertexAttribPointer(location, components, ConvertVertexType(element.Type), element.Normalized, format.Stride, new IntPtr(ptr));
                        GL.VertexAttribDivisor(location, divisor);

                        ptr += components * element.ComponentSize;
                    }

                    return true;
                }

                return false;
            }
        }

        private static GLEnum ConvertVertexType(VertexType value)
        {
            return value switch
            {
                VertexType.Byte => GLEnum.UNSIGNED_BYTE,
                VertexType.Short => GLEnum.SHORT,
                VertexType.Int => GLEnum.INT,
                VertexType.Float => GLEnum.FLOAT,
                _ => throw new NotImplementedException(),
            };
        }

        protected override void Dispose()
        {
            if (vertexBuffer > 0)
                graphics.BuffersToDelete.Add(vertexBuffer);

            if (indexBuffer > 0)
                graphics.BuffersToDelete.Add(indexBuffer);

            if (instanceBuffer > 0)
                graphics.BuffersToDelete.Add(instanceBuffer);

            foreach (var kv in vertexArrays)
                graphics.GetContextMeta(kv.Key).VertexArraysToDelete.Add(kv.Value);

            vertexArrays.Clear();
            bindedArrays.Clear();

            vertexBuffer = 0;
            indexBuffer = 0;
            instanceBuffer = 0;
        }
    }
}
