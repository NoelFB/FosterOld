using System;
using System.Buffers;

namespace Foster.Framework.Internal
{
    public abstract class InternalMesh : InternalResource
    {
        protected internal abstract void SetMaterial(Material? material);
        protected internal abstract void UploadVertices<T>(ReadOnlySequence<T> vertices, VertexFormat format);
        protected internal abstract void UploadInstances<T>(ReadOnlySequence<T> instances, VertexFormat format);
        protected internal abstract void UploadIndices(ReadOnlySequence<int> indices);
        protected internal abstract void Draw(int start, int elements);
        protected internal abstract void DrawInstances(int start, int elements, int instances);
    }
}
