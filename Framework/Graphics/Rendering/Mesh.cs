using System;
using System.Buffers;
using System.Collections.Generic;

namespace Foster.Framework
{
    public abstract class Mesh : GraphicsResource
    {
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public int InstanceCount { get; private set; }
        public int ElementCount => IndexCount / 3;
        public Type? VertexType { get; private set; }
        public Type? InstanceType { get; private set; }

        protected Mesh(Graphics graphics) : base(graphics)
        {

        }

        public abstract Material? Material { get; set; }

        public void SetVertices<T>(T[] vertices) where T : struct => SetVertices(new ReadOnlySequence<T>(vertices));
        public void SetVertices<T>(ReadOnlyMemory<T> vertices) where T : struct => SetVertices(new ReadOnlySequence<T>(vertices));

        public void SetInstances<T>(T[] instances) where T : struct => SetInstances(new ReadOnlySequence<T>(instances));
        public void SetInstances<T>(ReadOnlyMemory<T> instances) where T : struct => SetInstances(new ReadOnlySequence<T>(instances));

        public void SetIndices(int[] triangles) => SetIndices(new ReadOnlySequence<int>(triangles));
        public void SetIndices(ReadOnlyMemory<int> triangles) => SetIndices(new ReadOnlySequence<int>(triangles));

        public void SetVertices<T>(ReadOnlySequence<T> vertices) where T : struct
        {
            VertexCount = (int)vertices.Length;
            VertexType = typeof(T);

            UploadVertices<T>(vertices);
        }

        public void SetIndices(ReadOnlySequence<int> triangles)
        {
            IndexCount = (int)triangles.Length;
            UploadIndices(triangles);
        }

        public void SetInstances<T>(ReadOnlySequence<T> instances) where T : struct
        {
            InstanceCount = (int)instances.Length;
            InstanceType = typeof(T);

            UploadInstances<T>(instances);
        }

        protected abstract void UploadVertices<T>(ReadOnlySequence<T> vertices) where T : struct;
        protected abstract void UploadIndices(ReadOnlySequence<int> triangles);
        protected abstract void UploadInstances<T>(ReadOnlySequence<T> instances) where T : struct;

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
