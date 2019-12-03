using System;
using System.Buffers;
using System.Collections.Generic;

namespace Foster.Framework
{
    public abstract class Mesh : GraphicsResource
    {
        public int VertexCount { get; private set; }
        public int IndicesCount { get; private set; }
        public int InstanceCount { get; private set; }
        public int ElementCount => IndicesCount / 3;
        public VertexFormat? VertexFormat { get; private set; } = null;
        public VertexFormat? InstanceFormat { get; private set; } = null;

        protected Mesh(Graphics graphics) : base(graphics)
        {

        }

        public abstract Material? Material { get; set; }

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

            UploadVertices(vertices, VertexFormat);
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
            UploadIndices(indices);
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

            UploadInstances(vertices, InstanceFormat);
        }

        protected abstract void UploadVertices<T>(ReadOnlySequence<T> vertices, VertexFormat format);
        protected abstract void UploadInstances<T>(ReadOnlySequence<T> instances, VertexFormat format);
        protected abstract void UploadIndices(ReadOnlySequence<int> indices);

        public void Draw() => Draw(0, ElementCount);

        public abstract void Draw(int start, int elements);

        public void DrawInstances() => DrawInstances(0, ElementCount, InstanceCount);

        public abstract void DrawInstances(int start, int elements, int instances);

        public static Mesh Create()
        {
            return App.Graphics.CreateMesh();
        }
    }
}
