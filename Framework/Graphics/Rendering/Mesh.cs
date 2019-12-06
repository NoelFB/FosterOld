using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Foster.Framework.Internal;

namespace Foster.Framework
{
    public class Mesh : GraphicsResource
    {
        public readonly InternalMesh Internal;

        public int VertexCount { get; private set; }
        public int IndicesCount { get; private set; }
        public int InstanceCount { get; private set; }
        public int ElementCount => IndicesCount / 3;
        public VertexFormat? VertexFormat { get; private set; } = null;
        public VertexFormat? InstanceFormat { get; private set; } = null;

        private Material? material;

        public Mesh(Graphics graphics) : base(graphics)
        {
            Internal = graphics.CreateMesh();
        }

        public Mesh() : this(App.Graphics)
        {

        }

        public  Material? Material
        {
            get => material;
            set
            {
                if (material != value)
                    Internal.SetMaterial(material = value);
            }
        }

        public void SetVertices<T>(T[] vertices) where T : struct, IVertex
        {
            SetVertices(new ReadOnlySequence<T>(vertices), default(T).Format);
        }

        public void SetVertices<T>(ReadOnlyMemory<T> vertices) where T : struct, IVertex
        {
            SetVertices(new ReadOnlySequence<T>(vertices), default(T).Format);
        }

        public void SetVertices<T>(ReadOnlySequence<T> vertices) where T : struct, IVertex
        {
            SetVertices(vertices, default(T).Format);
        }

        public void SetVertices<T>(ReadOnlyMemory<T> vertices, VertexFormat format)
        {
            SetVertices(new ReadOnlySequence<T>(vertices), format);
        }

        public void SetVertices<T>(ReadOnlySequence<T> vertices, VertexFormat format)
        {
            VertexCount = (int)vertices.Length;
            VertexFormat = format ?? throw new Exception("Vertex Format cannot be null");

            Internal.UploadVertices(vertices, VertexFormat);
        }

        public void SetIndices(int[] indices)
        {
            SetIndices(new ReadOnlySequence<int>(indices));
        }

        public void SetIndices(ReadOnlyMemory<int> indices)
        {
            SetIndices(new ReadOnlySequence<int>(indices));
        }

        public void SetIndices(ReadOnlySequence<int> indices)
        {
            IndicesCount = (int)indices.Length;
            Internal.UploadIndices(indices);
        }

        public void SetInstances<T>(T[] vertices) where T : struct, IVertex
        {
            SetInstances(new ReadOnlySequence<T>(vertices), default(T).Format);
        }

        public void SetInstances<T>(ReadOnlyMemory<T> vertices) where T : struct, IVertex
        {
            SetInstances(new ReadOnlySequence<T>(vertices), default(T).Format);
        }

        public void SetInstances<T>(ReadOnlyMemory<T> vertices, VertexFormat format)
        {
            SetInstances(new ReadOnlySequence<T>(vertices), format);
        }

        public void SetInstances<T>(ReadOnlySequence<T> vertices) where T : struct, IVertex
        {
            SetInstances(vertices, default(T).Format);
        }

        public void SetInstances<T>(ReadOnlySequence<T> vertices, VertexFormat format)
        {
            InstanceCount = (int)vertices.Length;
            InstanceFormat = format ?? throw new Exception("Vertex Format cannot be null");

            Internal.UploadInstances(vertices, InstanceFormat);
        }

        public void Draw()
        {
            Internal.Draw(0, ElementCount);
        }

        public void Draw(int start, int elements)
        {
            Internal.Draw(start, elements);
        }

        public void DrawInstances()
        {
            Internal.DrawInstances(0, ElementCount, InstanceCount);
        }

        public void DrawInstances(int start, int elements, int instances)
        {
            Internal.DrawInstances(start, elements, instances);
        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                base.Dispose();
                Internal.Dispose();
            }
        }

    }
}
